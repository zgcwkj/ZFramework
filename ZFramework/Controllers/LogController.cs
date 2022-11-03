using Microsoft.AspNetCore.Mvc;
using ZFramework.Comm.Base;
using ZFramework.Comm.Filters;
using ZFramework.Comm.Models;
using zgcwkj.Util;

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

            methodResult.ErrorCode = 0;
            methodResult.ErrorMessage = "查询完成";
            return Json(methodResult);
        }

        /// <summary>
        /// 新增错误数据
        /// </summary>
        /// <param name="FilePath">路径</param>
        /// <param name="ControllerName">控制器名称</param>
        /// <returns></returns>
        public IActionResult InsertLogData(string FilePath, string ControllerName)
        {
            var methodResult = new MethodResult();
            methodResult.ErrorCode = -1;
            methodResult.ErrorMessage = "信息错误";

            methodResult.ErrorCode = 0;
            methodResult.ErrorMessage = "信息已记录";
            return Json(methodResult);
        }
    }
}
