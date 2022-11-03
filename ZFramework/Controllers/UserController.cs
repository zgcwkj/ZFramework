using Microsoft.AspNetCore.Mvc;
using System.Data;
using ZFramework.Comm.Base;
using ZFramework.Comm.Filters;
using ZFramework.Comm.Models;
using zgcwkj.Util;
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
        /// <param name="Page">页码</param>
        /// <param name="PageSize">每页数量</param>
        /// <param name="QueryLikeStr">模糊搜索内容</param>
        /// <param name="BeginDate">开始时间</param>
        /// <param name="EndDate">结束时间</param>
        [HttpPost]
        public IActionResult InquireUserData(int Page, int PageSize, string QueryLikeStr, string BeginDate, string EndDate)
        {
            var methodResult = new MethodResult();
            methodResult.ErrorCode = -1;
            methodResult.ErrorMessage = "信息错误";

            int pageOffset = (Page - 1) * PageSize;
            string userID = SessionHelper.Get("UserID");
            //查数据
            var cmd = DbProvider.Create();
            cmd.Clear();
            cmd.SetCommandText(@"
select su.user_id,su.accounts,su.user_name,su.create_time
from sysuser su
INNER JOIN (
	select sr.user_id
	from sysrole sr
	INNER JOIN (
		select sr.role_path
		from sysuser su
		INNER JOIN sysrole sr on su.user_id = sr.user_id
		where su.is_delete = 0 and su.user_id = @userID) usr on sr.role_path like concat('%',usr.role_path,'%')
) sr on su.user_id = sr.user_id
where su.is_delete = 0 and su.user_id <> @userID", userID);
            cmd.AppendAnd("su.user_name like concat('%',@queryLikeStr,'%')", QueryLikeStr);
            cmd.OrderBy(@"create_time desc");
            cmd.SetEndSql($"limit {pageOffset},{PageSize}");
            DataTable dataTable = cmd.QueryDataTable();
            methodResult.Data = dataTable.ToList();

            //查总数
            cmd.Clear();
            cmd.SetCommandText(@"
select count(0)
from sysuser su
INNER JOIN (
	select sr.user_id
	from sysrole sr
	INNER JOIN (
		select sr.role_path
		from sysuser su
		INNER JOIN sysrole sr on su.user_id = sr.user_id
		where su.is_delete = 0 and su.user_id = @userID) usr on sr.role_path like concat('%',usr.role_path,'%')
) sr on su.user_id = sr.user_id
where su.is_delete = 0 and su.user_id <> @userID", userID);
            methodResult.DataCount = cmd.QueryRowCount();

            methodResult.ErrorCode = 0;
            methodResult.ErrorMessage = "查询完成";
            return Json(methodResult);
        }

        /// <summary>
        /// 新增用户数据
        /// </summary>
        /// <param name="Accounts">用户账号</param>
        /// <param name="UserName">用户名称</param>
        /// <param name="Password">用户密码</param>
        /// <returns></returns>
        [HttpPost]
        public IActionResult InsertUserData(string Accounts, string UserName, string Password)
        {
            var methodResult = new MethodResult();
            methodResult.ErrorCode = -1;
            methodResult.ErrorMessage = "信息错误";

            if (Accounts.IsNull())
            {
                methodResult.ErrorMessage = "用户账号不能为空";
                return Json(methodResult);
            }
            if (UserName.IsNull())
            {
                methodResult.ErrorMessage = "用户名称不能为空";
                return Json(methodResult);
            }
            if (Password.IsNull())
            {
                methodResult.ErrorMessage = "用户密码不能为空";
                return Json(methodResult);
            }

            //把密码加密一遍
            Password = MD5Tool.GetMd5(Password);

            var cmd = DbProvider.Create();
            //防止添加同样账号的用户
            cmd.Clear();
            cmd.SetCommandText(@"select count(0) from sysuser where is_delete = 0 and accounts = @accounts", Accounts);
            int userCount = cmd.QueryRowCount();
            if (userCount > 0)
            {
                methodResult.ErrorMessage = "存在同样账号的用户";
                return Json(methodResult);
            }
            //添加用户
            string userID = SessionHelper.Get("UserID");
            string newUserID = GlobalConstant.GuidMd5;
            cmd.Clear();
            cmd.SetCommandText(@"
insert into sysuser(user_id,accounts,user_name,password,load_rp,is_delete,create_time,creator_id)
values(@userID,@accounts,@userName,@password,0,0,now(),@creatorID)", newUserID, Accounts, UserName, Password, userID);
            int insUserCount = cmd.UpdateData();
            //权限表创建对应关系
            string rolePath = SessionHelper.Get("RolePath");
            string newRoleID = GlobalConstant.GuidMd5;
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

insert into sysmenu_detail (mdetail_id,menu_id,role_id,is_delete,create_time,creator_id) values ('{GlobalConstant.GuidMd5}','6',@roleID,'0',now(),@creatorID);
insert into sysmenu_detail (mdetail_id,menu_id,role_id,is_delete,create_time,creator_id) values ('{GlobalConstant.GuidMd5}','7',@roleID,'0',now(),@creatorID)
", newRoleID, userID);
            int insMenuCount = cmd.UpdateData();
            if (insUserCount > 0 && insRoleCount > 0 && insMenuCount > 0)
            {
                methodResult.ErrorCode = 0;
                methodResult.ErrorMessage = "新增成功";
            }
            else
            {
                methodResult.ErrorMessage = "新增失败";
            }
            return Json(methodResult);
        }

        /// <summary>
        /// 修改用户数据
        /// </summary>
        /// <param name="Accounts">用户账号</param>
        /// <param name="UserName">用户名称</param>
        /// <param name="Password">用户密码</param>
        /// <param name="UserID">用户ID</param>
        /// <returns></returns>
        [HttpPost]
        public IActionResult UpdateUserData(string Accounts, string UserName, string Password, string UserID)
        {
            var methodResult = new MethodResult();
            methodResult.ErrorCode = -1;
            methodResult.ErrorMessage = "信息错误";

            if (Accounts.IsNull())
            {
                methodResult.ErrorMessage = "用户账号不能为空";
                return Json(methodResult);
            }
            if (UserName.IsNull())
            {
                methodResult.ErrorMessage = "用户名称不能为空";
                return Json(methodResult);
            }
            if (UserID.IsNull())
            {
                return Json(methodResult);
            }

            var cmd = DbProvider.Create();
            //防止修改成，除了本身的其它同样账号的用户
            cmd.Clear();
            cmd.SetCommandText(@"select user_id from sysuser where accounts = @accounts", Accounts);
            DataRow userDataRow = cmd.QueryDataRow();
            if (!userDataRow.IsNull())
            {
                string userID = userDataRow["user_id"].ToStr();
                if (userID != UserID)
                {
                    methodResult.ErrorMessage = "存在同样账号的用户";
                    return Json(methodResult);
                }
            }
            //看看改不改密码
            if (Password.IsNull())
            {
                cmd.Clear();
                cmd.SetCommandText(@"
update sysuser
set accounts = @accounts,user_name = @userName
where user_id = @userID", Accounts, UserName, UserID);
            }
            else
            {
                Password = MD5Tool.GetMd5(Password);
                cmd.Clear();
                cmd.SetCommandText(@"
update sysuser
set accounts = @accounts,password = @password,user_name = @userName
where user_id = @userID", Accounts, Password, UserName, UserID);
            }
            int updateCount = cmd.UpdateData();

            if (updateCount > 0)
            {
                methodResult.ErrorCode = 0;
                methodResult.ErrorMessage = "修改成功";
            }
            else
            {
                methodResult.ErrorMessage = "修改失败";
            }
            return Json(methodResult);
        }

        /// <summary>
        /// 删除用户数据
        /// </summary>
        /// <param name="IDS">ID集合</param>
        /// <returns></returns>
        [HttpPost]
        public IActionResult DeleteUserData(string IDS)
        {
            var methodResult = new MethodResult();
            methodResult.ErrorCode = -1;
            methodResult.ErrorMessage = "信息错误";

            if (IDS.IsNull())
            {
                return Json(methodResult);
            }

            int delectCount = 0;//统计删除的数量
            string[] IDs = IDS.Split(',');

            var cmd = DbProvider.Create();
            foreach (var id in IDs)
            {
                cmd.Clear();
                cmd.SetCommandText(@"update sysuser set is_delete = 1 where user_id = @userID", id);
                delectCount += cmd.UpdateData();
            }

            if (delectCount > 0)
            {
                methodResult.ErrorCode = 0;
                methodResult.ErrorMessage = "删除成功";
            }
            else
            {
                methodResult.ErrorMessage = "删除失败";
            }

            return Json(methodResult);
        }
    }
}
