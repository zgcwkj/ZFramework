using Microsoft.AspNetCore.Mvc;
using ZFramework.Quartz;

namespace Labor.EduPlatForm.RestApi.Areas.Quartz.Controllers
{
    /// <summary>
    /// 任务管理
    /// </summary>
    public class QuartzController : BaseController
    {
        /// <summary>
        /// Quartz 功能
        /// </summary>
        private QuartzHandle _QuartzHandle { get; }

        /// <summary>
        /// Quartz 配置
        /// </summary>
        private QuartzConfig _QuartzConfig { get; }

        /// <summary>
        /// 任务管理
        /// </summary>
        public QuartzController(QuartzHandle quartzHandle, QuartzConfig quartzConfig)
        {
            this._QuartzHandle = quartzHandle;
            this._QuartzConfig = quartzConfig;
        }

        /// <summary>
        /// 任务管理页
        /// </summary>
        /// <returns></returns>
        public IActionResult Index()
        {
            return View();
        }

        /// <summary>
        /// 获取任务列表
        /// </summary>
        /// <returns></returns>
        public async Task<IActionResult> InquireData()
        {
            var methodResult = new MethodResult();
            methodResult.Code = -1;
            methodResult.Msg = "信息错误";

            var jobs = await _QuartzHandle.GetJobs();
            methodResult.Data = jobs;
            methodResult.Code = 0;
            methodResult.Msg = "查询完成";

            return Json(methodResult);
        }

        /// <summary>
        /// 新建任务
        /// </summary>
        /// <returns></returns>
        public async Task<IActionResult> InsertData(QuarzTask quarzTask)
        {
            var methodResult = new MethodResult();
            methodResult.Code = -1;
            methodResult.Msg = "新建失败";
            //
            quarzTask.Status = Convert.ToInt32(JobState.停用);
            var data = await _QuartzHandle.AddJob(quarzTask);
            if (data.Item1) methodResult.Code = 0;
            methodResult.Msg = data.Item2;
            return Json(methodResult);
        }

        /// <summary>
        /// 修改任务
        /// </summary>
        /// <returns></returns>
        public async Task<IActionResult> UpdateData(QuarzTask quarzTask)
        {
            var methodResult = new MethodResult();
            methodResult.Code = -1;
            methodResult.Msg = "修改失败";
            //
            var data = await _QuartzHandle.Update(quarzTask);
            if (data.Item1) methodResult.Code = 0;
            methodResult.Msg = data.Item2;
            return Json(methodResult);
        }

        /// <summary>
        /// 删除任务
        /// </summary>
        /// <returns></returns>
        public async Task<IActionResult> DeleteData(string ids)
        {
            var methodResult = new MethodResult();
            methodResult.Code = -1;
            methodResult.Msg = "删除失败";
            //
            var allJob = await _QuartzHandle.GetJobs();
            var getJob = allJob.Where(w => ids.Split(',').Contains(w.ID.ToStr()));
            if (getJob.Any())
            {
                foreach (var item in getJob.ToList())
                {
                    var data = await _QuartzHandle.Remove(item);
                    if (data.Item1) methodResult.Code = 0;
                    methodResult.Msg = data.Item2;
                }
            }
            return Json(methodResult);
        }

        /// <summary>
        /// 立即执行任务
        /// </summary>
        /// <returns></returns>
        public async Task<IActionResult> Run(string ids)
        {
            var methodResult = new MethodResult();
            methodResult.Code = -1;
            methodResult.Msg = "执行失败";
            //
            foreach (var id in ids.Split(','))
            {
                var data = await _QuartzHandle.Run(id.ToInt());
                if (data.Item1) methodResult.Code = 0;
                methodResult.Msg = data.Item2;
            }
            return Json(methodResult);
        }

        /// <summary>
        /// 开启任务
        /// </summary>
        /// <returns></returns>
        public async Task<IActionResult> Start(string ids)
        {
            var methodResult = new MethodResult();
            methodResult.Code = -1;
            methodResult.Msg = "开启失败";
            //
            foreach (var id in ids.Split(','))
            {
                var data = await _QuartzHandle.Start(id.ToInt());
                if (data.Item1) methodResult.Code = 0;
                methodResult.Msg = data.Item2;
            }
            return Json(methodResult);
        }

        /// <summary>
        /// 暂停任务
        /// </summary>
        /// <returns></returns>
        public async Task<IActionResult> Pause(string ids)
        {
            var methodResult = new MethodResult();
            methodResult.Code = -1;
            methodResult.Msg = "暂停失败";
            //
            foreach (var id in ids.Split(','))
            {
                var data = await _QuartzHandle.Pause(id.ToInt());
                if (data.Item1) methodResult.Code = 0;
                methodResult.Msg = data.Item2;
            }
            return Json(methodResult);
        }

        /// <summary>
        /// 获取任务执行记录
        /// </summary>
        /// <param name="taskName">任务名称</param>
        /// <param name="groupName">分组名称</param>
        /// <param name="page">页码</param>
        /// <param name="pageSize">页大小</param>
        /// <returns></returns>
        public IActionResult GetLog(string taskName, string groupName, int page = 1, int pageSize = 10)
        {
            var data = _QuartzConfig.GetLogs(taskName, groupName, page, pageSize);
            var linqData = new
            {
                total = data.Item1,
                data = data.Item2,
            };
            //
            return Json(linqData);
        }
    }
}
