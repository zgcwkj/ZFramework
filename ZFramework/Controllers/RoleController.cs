using Microsoft.AspNetCore.Mvc;
using ZFramework.Comm.Base;
using ZFramework.Comm.Filters;
using ZFramework.Comm.Models;
using zgcwkj.Util;

namespace ZFramework.Controllers
{
    /// <summary>
    /// 角色管理
    /// </summary>
    [Authorization]
    public class RoleController : BaseController
    {
        /// <summary>
        /// 角色页面
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public IActionResult Index()
        {
            return View();
        }

        /// <summary>
        /// 查询角色数据
        /// </summary>
        /// <param name="Page">页码</param>
        /// <param name="PageSize">每页数量</param>
        /// <param name="QueryLikeStr">模糊搜索内容</param>
        /// <param name="BeginDate">开始时间</param>
        /// <param name="EndDate">结束时间</param>
        [HttpPost]
        public IActionResult InquireData(int Page, int PageSize, string QueryLikeStr, string BeginDate, string EndDate)
        {
            var methodResult = new MethodResult();
            methodResult.ErrorCode = -1;
            methodResult.ErrorMessage = "信息错误";

            int pageOffset = (Page - 1) * PageSize;
            string userID = SessionHelper.Get("UserID");
            //查数据
            var cmd = DbProvider.Create();

            methodResult.ErrorCode = 0;
            methodResult.ErrorMessage = "查询完成";
            return Json(methodResult);
        }
    }
}
