using Microsoft.AspNetCore.Mvc;

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
        /// <param name="page">页码</param>
        /// <param name="pageSize">每页数量</param>
        /// <param name="queryLikeStr">模糊搜索内容</param>
        /// <param name="startDate">开始时间</param>
        /// <param name="endDate">结束时间</param>
        [HttpPost]
        public IActionResult InquireData(int page, int pageSize, string queryLikeStr, string startDate, string endDate)
        {
            var methodResult = new MethodResult();
            methodResult.Code = -1;
            methodResult.Msg = "信息错误";

            var pageOffset = (page - 1) * pageSize;
            var userID = SessionHelper.Get("UserID");
            //查数据
            var cmd = DbProvider.Create();

            methodResult.Code = 0;
            methodResult.Msg = "查询完成";
            return Json(methodResult);
        }
    }
}
