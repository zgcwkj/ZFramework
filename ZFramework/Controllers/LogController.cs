using Microsoft.AspNetCore.Mvc;

namespace ZFramework.Controllers
{
    /// <summary>
    /// 日志管理
    /// </summary>
    [Authorization]
    public class LogController : BaseController
    {
        /// <summary>
        /// 日志页面
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public IActionResult Index()
        {
            var logPath = $"{GlobalConstant.GetRunPath}/Log/";
            var dir = Directory.CreateDirectory(logPath);
            var files = dir.GetFiles();
            var logContent = "暂无日志";
            if (files.Length > 0)
            {
                var newTime = DateTime.MinValue;
                foreach (var file in files)
                {
                    if (file.Extension == ".txt" && file.CreationTime > newTime)
                    {
                        newTime = file.CreationTime;
                        logContent = "日志文件太大，请联系管理员分析";
                        if (file.Length / 1000 <= 1000)
                        {
                            logContent = System.IO.File.ReadAllText(file.FullName);
                        }
                    }
                }
            }

            ViewData["LogContent"] = logContent;

            return View();
        }

        /// <summary>
        /// 查询日志数据
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

            methodResult.Code = 0;
            methodResult.Msg = "查询完成";
            return Json(methodResult);
        }

        /// <summary>
        /// 新增错误数据
        /// </summary>
        /// <param name="filePath">路径</param>
        /// <param name="controllerName">控制器名称</param>
        /// <returns></returns>
        public IActionResult InsertLogData(string filePath, string controllerName)
        {
            var methodResult = new MethodResult();
            methodResult.Code = -1;
            methodResult.Msg = "信息错误";

            methodResult.Code = 0;
            methodResult.Msg = "错误已记录";
            return Json(methodResult);
        }
    }
}
