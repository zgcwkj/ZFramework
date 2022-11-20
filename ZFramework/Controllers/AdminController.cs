using Microsoft.AspNetCore.Mvc;
using System.Data;
using ZFramework.Comm.Base;
using ZFramework.Comm.Common;
using ZFramework.Comm.Filters;
using ZFramework.Comm.Models;
using zgcwkj.Util;
using zgcwkj.Util.Common;

namespace ZFramework.Controllers
{
    /// <summary>
    /// Admin控制器
    /// </summary>
    public class AdminController : BaseController
    {
        /// <summary>
        /// 登录页面
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public IActionResult Index()
        {
            //重定向到Admin
            if (!SessionHelper.Get("UserID").IsNull())
            {
                return RedirectToAction("Admin");
            }
            //自动登录帐号
            if (CookieHelper.Get("Remember").IsNotNull())
            {
                var accounts = CookieHelper.Get("Accounts");
                var password = CookieHelper.Get("Password");
                var remember = CookieHelper.Get("Remember");
                remember = remember == "checked" ? "true" : "false";
                var validateCode = "zgcwkjToken";//验证码
                SessionHelper.Set("ValidateCode", validateCode);
                var result = Login(remember, accounts, password, validateCode) as dynamic;
                if ((result.Value as MethodResult)?.ErrorCode == 0)
                {
                    return RedirectToAction("Admin");
                }
            }
            //填充前端数据
            try
            {
                ViewData["DateTime"] = Math.Truncate(DateTime.Now.ToDateByUnix());//时间戳
                ViewData["Accounts"] = CookieHelper.Get("Accounts");
                ViewData["Password"] = CookieHelper.Get("Password");
                ViewData["Remember"] = CookieHelper.Get("Remember");
                ViewData["ServerName"] = ConfigHelp.Get("ServerName").ToStr();
            }
            catch { }

            return View();
        }

        /// <summary>
        /// 获取验证码
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public IActionResult ValidateImage()
        {
            //var vCode = new ValidateCode(6);
            //SessionHelper.Set("ValidateCode", vCode.GetValidate());
            var tuple = CaptchaCode.GetImgData();
            SessionHelper.Set("ValidateCode", tuple.Item1);
            return File(tuple.Item2, "image/jpg");
        }

        /// <summary>
        /// 登录验证
        /// </summary>
        /// <param name="rememberMe">是否记住用户</param>
        /// <param name="accounts">用户帐号</param>
        /// <param name="password">用户密码</param>
        /// <param name="captcha">验证码</param>
        /// <returns></returns>
        [HttpPost]
        public IActionResult Login(string rememberMe, string accounts, string password, string captcha)
        {
            var methodResult = new MethodResult();
            methodResult.ErrorCode = -1;
            methodResult.ErrorMessage = "信息错误";

            var cmd = DbProvider.Create();
            //防止页面的打开方式是不正确
            if (!SessionHelper.Get("ValidateCode").IsNull())
            {
#if DEBUG
                captcha = SessionHelper.Get("ValidateCode").ToStr();
#endif
                if (!rememberMe.IsNull() && !accounts.IsNull() && !password.IsNull() && !captcha.IsNull())
                {
                    string validateCode = SessionHelper.Get("ValidateCode").ToStr();
                    if (validateCode.ToLower() == captcha.ToLower())
                    {
                        cmd.SetCommandText(@"
select su.user_id,su.accounts,su.password,su.user_name,
sr.role_id,sr.role_path
from sysuser su
INNER JOIN sysrole sr on su.user_id = sr.user_id
where su.is_delete = 0 and sr.is_delete = 0 and su.accounts = @accounts", accounts);
                        DataRow drUser = cmd.QueryDataRow();

                        if (!drUser.IsNull())
                        {
                            string passwords = password;
                            //将密码进行MD5加密
                            passwords = MD5Tool.GetMd5(passwords);
                            if (drUser["password"].ToStr() == passwords)
                            {
                                methodResult.ErrorCode = 0;
                                methodResult.ErrorMessage = "登录成功，马上跳转";

                                //重要的数据存储到Session中
                                string remember = "checked";//记住密码
                                try
                                {
                                    if (!rememberMe.ToBool())
                                    {
                                        accounts = "";
                                        password = "";
                                        remember = "";
                                    }
                                }
                                catch { }

                                SessionHelper.Set("UserID", drUser["user_id"].ToStr());
                                SessionHelper.Set("RoleID", drUser["role_id"].ToStr());
                                SessionHelper.Set("RolePath", drUser["role_path"].ToStr());
                                SessionHelper.Set("UserName", drUser["user_name"].ToStr());
                                CookieHelper.Set("Accounts", accounts);
                                CookieHelper.Set("Password", password);
                                CookieHelper.Set("Remember", remember);
                            }
                            else methodResult.ErrorMessage = "您输入的密码错误";
                        }
                        else methodResult.ErrorMessage = "您输入的帐号错误";
                    }
                    else methodResult.ErrorMessage = "您输入的验证码错误";
                }
                else methodResult.ErrorMessage = "请输入您的信息";
            }
            return Json(methodResult);
        }

        /// <summary>
        /// 后台面板
        /// </summary>
        /// <returns></returns>
        [Authorization]
        [HttpGet]
        public IActionResult Admin()
        {
            //用户名称
            ViewData["UserName"] = SessionHelper.Get("UserName");
            ViewData["ServerName"] = ConfigHelp.Get("ServerName").ToStr();

            return View();
        }

        /// <summary>
        /// 后台菜单
        /// </summary>
        /// <returns></returns>
        [Authorization]
        [HttpPost]
        public IActionResult Menu()
        {
            string roleID = SessionHelper.Get("RoleID");
            string rolePath = SessionHelper.Get("RolePath");

            var cmd = DbProvider.Create();
            //SQL查询拥有的菜单
            cmd.SetCommandText(@"
select sm.menu_id,sm.parent_id,sm.title,sm.icon,sm.link,sm.sort
from sysmenu sm
INNER JOIN sysmenu_detail smd on sm.menu_id = smd.menu_id
where sm.is_delete = 0 and smd.is_delete = 0 and smd.role_id = @roleID", roleID);
            cmd.OrderBy("sm.Sort");
            DataTable dataTable = cmd.QueryDataTable();
            return Json(dataTable.ToList());
        }

        /// <summary>
        /// 更改网站服务状态
        /// </summary>
        /// <returns></returns>
        [Authorization]
        [AcceptVerbs("GET", "POST")]
        public IActionResult UpWebState()
        {
            var methodResult = new MethodResult();
            methodResult.ErrorCode = -1;
            methodResult.ErrorMessage = "信息错误";

            methodResult.ErrorMessage = "更改失败";
            string userID = SessionHelper.Get("UserID");
            if (userID == "admin")
            {
                var stopWebService = CacheAccess.Get<bool>("StopWebService");
                CacheAccess.Set<bool>("StopWebService", !stopWebService);
                methodResult.ErrorCode = 0;
                methodResult.ErrorMessage = "已更改系统运行状态";
            }

            return Json(methodResult);
        }

        /// <summary>
        /// 修改个人信息
        /// </summary>
        /// <param name="Name">用户名称</param>
        /// <param name="Password">旧密码</param>
        /// <param name="toPassword">新密码</param>
        /// <returns></returns>
        [Authorization]
        [HttpPost]
        public IActionResult UpdateUser(string Name, string Password, string toPassword)
        {
            var methodResult = new MethodResult();
            methodResult.ErrorCode = -1;
            methodResult.ErrorMessage = "信息错误";

            if (Name == null || Password == null || toPassword == null)
            {
                return Json(methodResult);
            }

            //把密码加密一遍
            Password = MD5Tool.GetMd5(Password);
            toPassword = MD5Tool.GetMd5(toPassword);

            var cmd = DbProvider.Create();
            string userID = SessionHelper.Get("UserID");
            cmd.SetCommandText(@"
update sysuser set user_name = @userName,password = @toPassword
where is_delete = 0 and user_id = @userID and password = @password", Name, toPassword, userID, Password);
            int updCount = cmd.UpdateData();

            if (updCount > 0)
            {
                SessionHelper.Clear();
                methodResult.ErrorCode = 0;
                methodResult.ErrorMessage = "修改成功，请您重新登录";
            }
            else
            {
                methodResult.ErrorCode = -1;
                methodResult.ErrorMessage = "修改失败，请您重新尝试";
            }
            return Json(methodResult);
        }

        /// <summary>
        /// 退出登录
        /// </summary>
        /// <returns></returns>
        [Authorization]
        [HttpPost]
        public IActionResult ExitUser()
        {
            var methodResult = new MethodResult();
            methodResult.ErrorCode = -1;
            methodResult.ErrorMessage = "信息错误";

            try
            {
                SessionHelper.Clear();
                CookieHelper.Set("Remember", "");
                methodResult.ErrorCode = 0;
                methodResult.ErrorMessage = "成功退出，请等待";
            }
            catch
            {
                methodResult.ErrorCode = -1;
                methodResult.ErrorMessage = "退出失败，请重试";
            }

            return Json(methodResult);
        }
    }
}
