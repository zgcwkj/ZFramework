using Microsoft.AspNetCore.Mvc;
using System.Data;
using ZFramework.Comm.Base;
using ZFramework.Comm.Common;
using ZFramework.Comm.Filters;
using ZFramework.Comm.Models;
using ZFramework.Data;
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
        /// 数据库连接对象
        /// </summary>
        private MyDbContext MyDb { get; }

        /// <summary>
        /// Admin控制器
        /// </summary>
        public AdminController(MyDbContext myDbContext)
        {
            this.MyDb = myDbContext;
        }

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
                if (!remember.ToBool()) return View();//取消向下执行
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
            var tuple = CaptchaCodeSK.GetImgData();
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

            //防止页面的打开方式是不正确
            if (!SessionHelper.Get("ValidateCode").IsNull())
            {
#if DEBUG
                captcha = SessionHelper.Get("ValidateCode").ToStr();
#endif
                if (!rememberMe.IsNull() && !accounts.IsNull() && !password.IsNull() && !captcha.IsNull())
                {
                    var validateCode = SessionHelper.Get("ValidateCode").ToStr();
                    if (validateCode.ToLower() == captcha.ToLower())
                    {
                        var linq = from su in MyDb.SysUserModel
                                   join sr in MyDb.SysRoleModel on su.UserID equals sr.UserID
                                   where su.IsDelete == 0 && sr.IsDelete == 0 && su.Accounts == accounts
                                   select new
                                   {
                                       user_id = su.UserID,
                                       accounts = su.Accounts,
                                       password = su.Password,
                                       user_name = su.UserName,
                                       role_id = sr.RoleID,
                                       role_path = sr.RolePath
                                   };
                        var linqData = linq.FirstOrDefault();
                        if (linqData != null)
                        {
                            var passwords = password;
                            //将密码进行MD5加密
                            passwords = MD5Tool.GetMd5(passwords);
                            if (linqData.password == passwords)
                            {
                                methodResult.ErrorCode = 0;
                                methodResult.ErrorMessage = "登录成功，马上跳转";

                                //重要的数据存储到Session中
                                var remember = "checked";//记住密码
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

                                SessionHelper.Set("UserID", linqData.user_id);
                                SessionHelper.Set("RoleID", linqData.role_id);
                                SessionHelper.Set("RolePath", linqData.role_path);
                                SessionHelper.Set("UserName", linqData.user_name);
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
            var roleID = SessionHelper.Get("RoleID");
            var rolePath = SessionHelper.Get("RolePath");

            var linq = from sm in MyDb.SysMenuModel
                       join smd in MyDb.SysMenuDetailModel on sm.MenuID equals smd.MenuID
                       where sm.IsDelete == 0 && smd.IsDelete == 0 && smd.RoleID == roleID
                       orderby sm.Sort
                       select new
                       {
                           menu_id = sm.MenuID,
                           parent_id = sm.ParentID,
                           title = sm.Title,
                           icon = sm.Icon,
                           link = sm.Link,
                           sort = sm.Sort
                       };
            var linqData = linq.ToList();
            return Json(linqData);
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

            var updCount = 0;
            var userID = SessionHelper.Get("UserID");
            //把密码加密一遍
            Password = MD5Tool.GetMd5(Password);
            toPassword = MD5Tool.GetMd5(toPassword);

            var linqData = MyDb.SysUserModel.Where(W => W.IsDelete == 0 && W.UserID == userID && W.Password == Password).FirstOrDefault();
            if (linqData != null)
            {
                linqData.UserName = Name;
                linqData.Password = toPassword;
                MyDb.SysUserModel.UpdateRange(linqData);
                updCount = MyDb.SaveChanges();
            }

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
