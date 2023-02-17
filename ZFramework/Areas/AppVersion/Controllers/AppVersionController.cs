using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Data;
using ZFramework.Comm.Base;
using ZFramework.Comm.Filters;
using ZFramework.Comm.Models;
using ZFramework.Data;
using ZFramework.Data.Models.Bus;
using zgcwkj.Util;

namespace ZFramework.Areas.ChartData.Controllers
{
    /// <summary>
    /// 版本控制控制器
    /// </summary>
    [Authorization]
    [Area("AppVersion")]
    public class AppVersionController : BaseController
    {
        /// <summary>
        /// 数据库上下文
        /// </summary>
        private MyDbContext MyDb { get; }

        /// <summary>
        /// 对象实例时
        /// </summary>
        public AppVersionController(MyDbContext myDb)
        {
            this.MyDb = myDb;
        }

        /// <summary>
        /// 版本控制页面
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public IActionResult Index()
        {
            return View();
        }

        /// <summary>
        /// 查询数据
        /// </summary>
        /// <param name="Page">页码</param>
        /// <param name="PageSize">每页数量</param>
        /// <param name="QueryLikeStr">模糊搜索内容</param>
        /// <param name="BeginDate">开始时间</param>
        /// <param name="EndDate">结束时间</param>
        /// <returns></returns>
        [HttpPost]
        public IActionResult InquireData(int Page, int PageSize, string QueryLikeStr, string BeginDate, string EndDate)
        {
            var methodResult = new MethodResult();
            methodResult.ErrorCode = -1;
            methodResult.ErrorMessage = "信息错误";

            var pageOffset = (Page - 1) * PageSize;
            var rolePath = SessionHelper.Get("RolePath").ToStr();
            //Linq
            var linqData = from bav in this.MyDb.BusAppVersionModel select bav;
            //条件
            if (QueryLikeStr.IsNotNull()) linqData = linqData.Where(T => T.AppName.Contains(QueryLikeStr) || T.AppVersion.Contains(QueryLikeStr));
            if (BeginDate.IsNotNull())
            {
                var toBeginDate = BeginDate.ToDate();
                linqData = linqData.Where(T => T.CreateTime >= toBeginDate);
            }
            if (EndDate.IsNotNull())
            {
                var toEndDate = EndDate.ToDate();
                linqData = linqData.Where(T => T.CreateTime <= toEndDate);
            }
            //数据
            methodResult.Data = linqData.OrderByDescending(T => T.CreateTime).Select(T => new
            {
                app_id = T.AppID,
                app_name = T.AppName,
                app_desc = T.AppDesc,
                app_path = T.AppPath,
                app_version = T.AppVersion,
                app_enforce = T.AppEnforce,
                app_state = T.AppState,
                upload_time = T.UploadTime.ToString("yyyy-MM-dd HH:mm:ss"),
                create_time = T.CreateTime.ToString("yyyy-MM-dd HH:mm:ss"),
            }).Skip(pageOffset).Take(PageSize).ToList();
            methodResult.DataCount = linqData.Count();

            methodResult.ErrorCode = 0;
            methodResult.ErrorMessage = "查询完成";
            return Json(methodResult);
        }

        /// <summary>
        /// 上传文件
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        public IActionResult UploadFile(IFormFile file)
        {
            var methodResult = new MethodResult();
            methodResult.ErrorCode = -1;
            methodResult.ErrorMessage = "信息错误";

            //获取文件扩展名
            var Fileexc = Path.GetExtension(file.FileName);
            //保存路径
            var filePath = Path.Combine(GlobalConstant.GetRunPath, "Resource/Temp");
            //防止文件夹没有
            if (!Directory.Exists(filePath)) Directory.CreateDirectory(filePath);
            //文件名称
            var fileName = $"{DateTime.Now:yyyyMMddHHmmss}_{file.FileName}";
            //文件路径
            filePath = Path.Combine(filePath, fileName);
            //存储文件
            using (var stream = file.OpenReadStream())
            {
                //准备文件流
                var bytes = new byte[stream.Length];
                stream.Read(bytes, 0, bytes.Length);
                //准备存储文件
                using var fileStream = new FileStream(filePath, FileMode.Create);
                fileStream.Write(bytes, 0, bytes.Length);
            }

            //查询最新的版本号
            var appStateToFind = new List<int> { 0, 1 };
            var linqData = this.MyDb.BusAppVersionModel.Where(T => appStateToFind.Contains(T.AppState)).OrderByDescending(T => T.UploadTime).FirstOrDefault();
            if (linqData == null) linqData = new BusAppVersionModel();
            var appVersion = linqData.AppVersion;
            var appVersionInt = appVersion.Replace(".", "").ToInt();

            methodResult.Data = new
            {
                FilePath = filePath,
                FileName = file.FileName,
                AppDesc = "应用有更新了",
                AppEnforce = false,
                Version = (appVersionInt + 1).ToTrim(),
            };
            methodResult.ErrorCode = 0;
            methodResult.ErrorMessage = "";
            return Json(methodResult);
        }

        /// <summary>
        /// 新增数据
        /// </summary>
        /// <param name="FilePath">文件路径</param>
        /// <param name="FileName">文件名称</param>
        /// <param name="AppDesc">应用描述</param>
        /// <param name="AppEnforce">强制更新</param>
        /// <param name="Version">版本号</param>
        /// <returns></returns>
        [HttpPost]
        public IActionResult InsertData(string FilePath, string FileName, string AppDesc, int AppEnforce, string Version)
        {
            var methodResult = new MethodResult();
            methodResult.ErrorCode = -1;
            methodResult.ErrorMessage = "信息错误";

            if (FilePath.IsNull()) return Json(methodResult);
            if (FileName.IsNull()) return Json(methodResult);
            if (Version.IsNull()) return Json(methodResult);

            var rolePath = SessionHelper.Get("RolePath").ToStr();
            //目标位置
            var targetPath = Path.Combine(GlobalConstant.GetRunPath, "Resource/AppVersion");
            //防止文件夹没有
            if (!Directory.Exists(targetPath)) Directory.CreateDirectory(targetPath);
            var okPath = Path.Combine(targetPath, $"{DateTime.Now:yyyyMMddHHmmss}_{FileName}");
            //移动文件
            System.IO.File.Copy(FilePath, okPath);
            //统一文件路径
            okPath = okPath.Replace(GlobalConstant.GetRunPath, "").Replace("\\", "/")[1..];
            //新增数据
            var appVersionModel = new BusAppVersionModel();
            appVersionModel.AppID = GlobalConstant.GuidMd5;
            appVersionModel.AppName = FileName;
            appVersionModel.AppDesc = AppDesc;
            appVersionModel.AppPath = okPath;
            appVersionModel.AppVersion = Version;
            appVersionModel.AppEnforce = AppEnforce;
            appVersionModel.AppState = 1;
            appVersionModel.RolePath = rolePath;
            appVersionModel.UploadTime = DateTime.Now;
            this.MyDb.Add(appVersionModel);
            this.MyDb.SaveChanges();

            methodResult.ErrorCode = 0;
            methodResult.ErrorMessage = "新增完成";
            return Json(methodResult);
        }

        /// <summary>
        /// 更新状态
        /// </summary>
        /// <param name="IDS">应用版本ID</param>
        /// <param name="State">状态</param>
        /// <returns></returns>
        [HttpPost]
        public IActionResult UpdateState(string IDS, string State)
        {
            var methodResult = new MethodResult();
            methodResult.ErrorCode = -1;
            methodResult.ErrorMessage = "信息错误";

            if (IDS.IsNull()) return Json(methodResult);
            if (State.IsNull()) return Json(methodResult);
            //批量修改
            var appIDs = IDS.Split(',');
            var data = this.MyDb.BusAppVersionModel.Where(W => appIDs.Contains(W.AppID));
            data.ForEachAsync(F =>
            {
                F.AppState= State.ToInt();
            });
            this.MyDb.SaveChanges();
            //结果描述文字
            var message = State == "0" ? "停用" : "启用";
            
            methodResult.ErrorCode = 0;
            methodResult.ErrorMessage = $"{message}完成";
            return Json(methodResult);
        }
    }
}
