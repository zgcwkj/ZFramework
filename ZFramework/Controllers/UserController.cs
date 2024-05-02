using Microsoft.AspNetCore.Mvc;
using System.Data;
using zgcwkj.Util.Common;

namespace ZFramework.Controllers
{
    /// <summary>
    /// 用户控制器
    /// </summary>
    [Authorization]
    public class UserController : BaseController
    {
        /// <summary>
        /// 数据库连接对象
        /// </summary>
        private MyDbContext MyDb { get; }

        /// <summary>
        /// 用户控制器
        /// </summary>
        public UserController(MyDbContext myDbContext)
        {
            this.MyDb = myDbContext;
        }

        /// <summary>
        /// 用户管理
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public IActionResult Index()
        {
            return View();
        }

        /// <summary>
        /// 查询用户数据
        /// </summary>
        /// <param name="page">页码</param>
        /// <param name="pageSize">每页数量</param>
        /// <param name="queryLikeStr">模糊搜索内容</param>
        /// <param name="startDate">开始时间</param>
        /// <param name="endDate">结束时间</param>
        [HttpPost]
        public IActionResult InquireUserData(int page, int pageSize, string queryLikeStr, string startDate, string endDate)
        {
            var methodResult = new MethodResult();
            methodResult.Code = -1;
            methodResult.Msg = "信息错误";
            //
            var pageOffset = (page - 1) * pageSize;
            var userID = SessionHelper.Get("UserID");
            //Linq
            var linqData = from su in MyDb.SysUserModel
                           join sr in (
                               from sr1 in MyDb.SysRoleModel
                               from sr2 in MyDb.SysRoleModel
                               join su1 in MyDb.SysUserModel on sr2.UserID equals su1.UserID
                               where su1.IsDelete == 0 && su1.UserID == userID
                               where sr1.RolePath.Contains(sr2.RolePath)
                               select new { sr1.UserID }
                           ) on su.UserID equals sr.UserID
                           where su.IsDelete == 0// && su.UserID != userID
                           select su;
            //条件
            if (queryLikeStr.IsNotNull()) linqData = linqData.Where(T => T.UserName.Contains(queryLikeStr) || T.Accounts.Contains(queryLikeStr));
            if (startDate.IsNotNull())
            {
                var toBeginDate = startDate.ToDate();
                linqData = linqData.Where(T => T.CreateTime >= toBeginDate);
            }
            if (endDate.IsNotNull())
            {
                var toEndDate = endDate.ToDate();
                linqData = linqData.Where(T => T.CreateTime <= toEndDate);
            }
            //格式化
            methodResult.Data = linqData.OrderByDescending(T => T.CreateTime).Select(T => new
            {
                user_id = T.UserID,
                accounts = T.Accounts,
                user_name = T.UserName,
                create_time = T.CreateTime.ToString("yyyy-MM-dd HH:mm:ss"),
            }).Skip(pageOffset).Take(pageSize).ToList();
            methodResult.Total = linqData.Count();
            //
            methodResult.Code = 0;
            methodResult.Msg = "查询完成";
            return Json(methodResult);
        }

        /// <summary>
        /// 新增用户数据
        /// </summary>
        /// <param name="accounts">用户帐号</param>
        /// <param name="userName">用户名称</param>
        /// <param name="password">用户密码</param>
        /// <returns></returns>
        [HttpPost]
        public IActionResult InsertUserData(string accounts, string userName, string password)
        {
            var methodResult = new MethodResult();
            methodResult.Code = -1;
            methodResult.Msg = "信息错误";

            if (accounts.IsNull())
            {
                methodResult.Msg = "用户帐号不能为空";
                return Json(methodResult);
            }
            if (userName.IsNull())
            {
                methodResult.Msg = "用户名称不能为空";
                return Json(methodResult);
            }
            if (password.IsNull())
            {
                methodResult.Msg = "用户密码不能为空";
                return Json(methodResult);
            }

            //把密码加密一遍
            password = MD5Tool.GetMd5(password);

            var cmd = DbProvider.Create(MyDb);
            //防止添加同样帐号的用户
            cmd.Clear();
            cmd.SetCommandText(@"select count(0) from sysuser where is_delete = 0 and accounts = @accounts", accounts);
            int userCount = cmd.QueryRowCount();
            if (userCount > 0)
            {
                methodResult.Msg = "存在同样帐号的用户";
                return Json(methodResult);
            }
            //添加用户
            var userID = SessionHelper.Get("UserID");
            var newUserID = GlobalConstant.GuidMd5;
            cmd.Clear();
            cmd.SetCommandText(@"
insert into sysuser(user_id,accounts,user_name,password,load_rp,is_delete,create_time,creator_id)
values(@userID,@accounts,@userName,@password,0,0,now(),@creatorID)", newUserID, accounts, userName, password, userID);
            int insUserCount = cmd.UpdateData();
            //权限表创建对应关系
            var rolePath = SessionHelper.Get("RolePath");
            var newRoleID = GlobalConstant.GuidMd5;
            cmd.Clear();
            cmd.SetCommandText(@"
insert into sysrole(role_id,role_path,user_id,is_delete,create_time,creator_id)
values(@roleID,@rolePath,@userID,0,now(),@creatorID)", newRoleID, $"{rolePath}_{newUserID}", newUserID, userID);
            int insRoleCount = cmd.UpdateData();
            //创建菜单明细
            cmd.Clear();
            cmd.SetCommandText($@"
insert into sysmenu_detail (mdetail_id,menu_id,role_id,is_delete,create_time,creator_id) values ('{GlobalConstant.GuidMd5}','1',@roleID,'0',now(),@creatorID);
insert into sysmenu_detail (mdetail_id,menu_id,role_id,is_delete,create_time,creator_id) values ('{GlobalConstant.GuidMd5}','2',@roleID,'0',now(),@creatorID);

insert into sysmenu_detail (mdetail_id,menu_id,role_id,is_delete,create_time,creator_id) values ('{GlobalConstant.GuidMd5}','7',@roleID,'0',now(),@creatorID);
insert into sysmenu_detail (mdetail_id,menu_id,role_id,is_delete,create_time,creator_id) values ('{GlobalConstant.GuidMd5}','8',@roleID,'0',now(),@creatorID)
", newRoleID, userID);
            var insMenuCount = cmd.UpdateData();
            if (insUserCount > 0 && insRoleCount > 0 && insMenuCount > 0)
            {
                methodResult.Code = 0;
                methodResult.Msg = "新增成功";
            }
            else
            {
                methodResult.Msg = "新增失败";
            }
            return Json(methodResult);
        }

        /// <summary>
        /// 修改用户数据
        /// </summary>
        /// <param name="accounts">用户帐号</param>
        /// <param name="userName">用户名称</param>
        /// <param name="password">用户密码</param>
        /// <param name="userID">用户ID</param>
        /// <returns></returns>
        [HttpPost]
        public IActionResult UpdateUserData(string accounts, string userName, string password, string userID)
        {
            var methodResult = new MethodResult();
            methodResult.Code = -1;
            methodResult.Msg = "信息错误";

            if (accounts.IsNull())
            {
                methodResult.Msg = "用户帐号不能为空";
                return Json(methodResult);
            }
            if (userName.IsNull())
            {
                methodResult.Msg = "用户名称不能为空";
                return Json(methodResult);
            }
            if (userID.IsNull())
            {
                return Json(methodResult);
            }

            var cmd = DbProvider.Create(MyDb);
            //防止修改成，除了本身的其它同样帐号的用户
            cmd.Clear();
            cmd.SetCommandText(@"select user_id from sysuser where accounts = @accounts", accounts);
            var userDataRow = cmd.QueryDataRow();
            if (!userDataRow.IsNull())
            {
                var userIDTemp = userDataRow["user_id"].ToStr();
                if (userID != userIDTemp)
                {
                    methodResult.Msg = "存在同样帐号的用户";
                    return Json(methodResult);
                }
            }
            //看看改不改密码
            if (password.IsNull())
            {
                cmd.Clear();
                cmd.SetCommandText(@"
update sysuser
set accounts = @accounts,user_name = @userName
where user_id = @userID", accounts, userName, userID);
            }
            else
            {
                password = MD5Tool.GetMd5(password);
                cmd.Clear();
                cmd.SetCommandText(@"
update sysuser
set accounts = @accounts,password = @password,user_name = @userName
where user_id = @userID", accounts, password, userName, userID);
            }
            var updateCount = cmd.UpdateData();

            if (updateCount > 0)
            {
                methodResult.Code = 0;
                methodResult.Msg = "修改成功";
            }
            else
            {
                methodResult.Msg = "修改失败";
            }
            return Json(methodResult);
        }

        /// <summary>
        /// 删除用户数据
        /// </summary>
        /// <param name="ids">ID集合</param>
        /// <returns></returns>
        [HttpPost]
        public IActionResult DeleteUserData(string ids)
        {
            var methodResult = new MethodResult();
            methodResult.Code = -1;
            methodResult.Msg = "删除失败";

            if (ids.IsNull())
            {
                return Json(methodResult);
            }

            var iDs = ids.Split(',');
            var delLinq = MyDb.SysUserModel.Where(w => iDs.Contains(w.UserID));
            if (delLinq.Any())
            {
                delLinq.ToList().ForEach(f =>
                {
                    f.IsDelete = 1;
                });
            }
            var delCount = MyDb.SaveChanges();
            if (delCount > 0)
            {
                methodResult.Code = 0;
                methodResult.Msg = "删除成功";
            }

            return Json(methodResult);
        }
    }
}
