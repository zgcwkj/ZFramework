using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ZFramework.Data.Models.Bus;

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
        /// <param name="page">页码</param>
        /// <param name="pageSize">每页数量</param>
        /// <param name="queryLikeStr">模糊搜索内容</param>
        /// <param name="startDate">开始时间</param>
        /// <param name="endDate">结束时间</param>
        /// <returns></returns>
        [HttpPost]
        public IActionResult InquireData(int page, int pageSize, string queryLikeStr, string startDate, string endDate)
        {
            var methodResult = new MethodResult();
            methodResult.Code = -1;
            methodResult.Msg = "信息错误";
            //
            var pageOffset = (page - 1) * pageSize;
            var rolePath = SessionHelper.Get("RolePath").ToStr();
            //Linq
            var linqData = from bav in this.MyDb.BusAppVersionModel select bav;
            //条件
            if (queryLikeStr.IsNotNull()) linqData = linqData.Where(T => T.AppName.Contains(queryLikeStr) || T.AppVersion.Contains(queryLikeStr));
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
            }).Skip(pageOffset).Take(pageSize).ToList();
            methodResult.Total = linqData.Count();
            //
            methodResult.Code = 0;
            methodResult.Msg = "查询完成";
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
            methodResult.Code = -1;
            methodResult.Msg = "信息错误";
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
            //
            methodResult.Data = new
            {
                FilePath = filePath,
                FileName = file.FileName,
                AppDesc = "应用有更新了",
                AppEnforce = false,
                Version = (appVersionInt + 1).ToTrim(),
            };
            methodResult.Code = 0;
            methodResult.Msg = "";
            return Json(methodResult);
        }

        /// <summary>
        /// 新增数据
        /// </summary>
        /// <param name="filePath">文件路径</param>
        /// <param name="fileName">文件名称</param>
        /// <param name="appDesc">应用描述</param>
        /// <param name="appEnforce">强制更新</param>
        /// <param name="version">版本号</param>
        /// <returns></returns>
        [HttpPost]
        public IActionResult InsertData(string filePath, string fileName, string appDesc, int appEnforce, string version)
        {
            var methodResult = new MethodResult();
            methodResult.Code = -1;
            methodResult.Msg = "信息错误";
            if (filePath.IsNull()) return Json(methodResult);
            if (fileName.IsNull()) return Json(methodResult);
            if (version.IsNull()) return Json(methodResult);
            var rolePath = SessionHelper.Get("RolePath").ToStr();
            //目标位置
            var targetPath = Path.Combine(GlobalConstant.GetRunPath, "Resource/AppVersion");
            //防止文件夹没有
            if (!Directory.Exists(targetPath)) Directory.CreateDirectory(targetPath);
            var okPath = Path.Combine(targetPath, $"{DateTime.Now:yyyyMMddHHmmss}_{fileName}");
            //移动文件
            System.IO.File.Copy(filePath, okPath);
            //统一文件路径
            okPath = okPath.Replace(GlobalConstant.GetRunPath, "").Replace("\\", "/")[1..];
            //新增数据
            var appVersionModel = new BusAppVersionModel();
            appVersionModel.AppID = GlobalConstant.GuidMd5;
            appVersionModel.AppName = fileName;
            appVersionModel.AppDesc = appDesc;
            appVersionModel.AppPath = okPath;
            appVersionModel.AppVersion = version;
            appVersionModel.AppEnforce = appEnforce;
            appVersionModel.AppState = 1;
            appVersionModel.RolePath = rolePath;
            appVersionModel.UploadTime = DateTime.Now;
            this.MyDb.Add(appVersionModel);
            this.MyDb.SaveChanges();
            //
            methodResult.Code = 0;
            methodResult.Msg = "新增完成";
            return Json(methodResult);
        }

        /// <summary>
        /// 更新状态
        /// </summary>
        /// <param name="ids">应用版本ID</param>
        /// <param name="state">状态</param>
        /// <returns></returns>
        [HttpPost]
        public IActionResult UpdateState(string ids, string state)
        {
            var methodResult = new MethodResult();
            methodResult.Code = -1;
            methodResult.Msg = "信息错误";
            if (ids.IsNull()) return Json(methodResult);
            if (state.IsNull()) return Json(methodResult);
            //批量修改
            var appIDs = ids.Split(',');
            var data = this.MyDb.BusAppVersionModel.Where(W => appIDs.Contains(W.AppID));
            data.ForEachAsync(F =>
            {
                F.AppState = state.ToInt();
            });
            this.MyDb.SaveChanges();
            //结果描述文字
            var message = state == "0" ? "停用" : "启用";
            //
            methodResult.Code = 0;
            methodResult.Msg = $"{message}完成";
            return Json(methodResult);
        }
    }
}
