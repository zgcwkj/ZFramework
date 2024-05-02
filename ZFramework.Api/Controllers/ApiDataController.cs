using Microsoft.AspNetCore.Mvc;
using System.Data;
using ZFramework.Api.Models;
using ZFramework.Comm;
using zgcwkj.Util;

namespace ZFramework.Api.Controllers
{
    /// <summary>
    /// API数据接口
    /// </summary>
    [Route("[controller]/[action]")]
    public class ApiDataController : BaseController
    {
        /// <summary>
        /// 获取更新信息
        /// </summary>
        /// <param name="AV">版本号</param>
        /// <returns></returns>
        [HttpGet]
        public IActionResult GetUpdateInfo(string AV)
        {
            var updateInfo = new UpdateInfoModel();
            var cmd = DbProvider.Create();
            cmd.SetCommandText("select app_path,app_name,app_version,app_desc,app_enforce from bus_appversion where app_state = 1");
            if (!AV.IsNull())
            {
                var appVersionInt = AV.Replace(".", "").ToInt();
                cmd.AppendAnd("cast(replace(app_version,'.','') as integer) > @appVersion", appVersionInt);
            }
            cmd.OrderBy("upload_time desc");
            var dataRow = cmd.QueryDataRow();
            if (dataRow.IsNull()) return Json(updateInfo);//暂无更新
            var filePath = dataRow["app_path"].ToTrim();
            var fileName = dataRow["app_name"].ToTrim();
            if (filePath.IsNull()) return Json(updateInfo);//暂无更新

            var appVersion = dataRow["app_version"].ToTrim();
            var appDesc = dataRow["app_desc"].ToTrim();
            var appEnforce = dataRow["app_enforce"].ToTrim();

            updateInfo.DownloadUrl = ConfigHelp.Get<string>("ServerIP") + updateInfo.DownloadUrl;
            updateInfo.Dialog = true;
            updateInfo.Version = appVersion;
            updateInfo.Desc = appDesc;
            updateInfo.Enforce = appEnforce.ToBool();

            return Json(updateInfo);
        }

        /// <summary>
        /// 获取程序文件
        /// </summary>
        /// <param name="AV">版本号</param>
        /// <returns></returns>
        [HttpGet]
        public IActionResult GetAppFile(string AV)
        {
            var methodResult = new MethodResult();
            methodResult.Code = -1;
            methodResult.Msg = "信息错误";

            var cmd = DbProvider.Create();
            cmd.SetCommandText("select app_path,app_name from bus_appversion where app_state = 1");
            if (!AV.IsNull())
            {
                var appVersionInt = AV.Replace(".", "").ToInt();
                cmd.AppendAnd("cast(replace(app_version,'.','') as integer) > @appVersion", appVersionInt);
            }
            cmd.OrderBy("upload_time desc");
            var dataRow = cmd.QueryDataRow();
            if (dataRow.IsNull())
            {
                methodResult.Msg = "No update";//暂无更新
                return Json(methodResult);
            }
            var filePath = dataRow["app_path"].ToTrim();
            var fileName = dataRow["app_name"].ToTrim();
            if (filePath.IsNull())
            {
                methodResult.Msg = "No update";//暂无更新
                return Json(methodResult);
            }
            //统一文件路径
            filePath = Path.Combine(GlobalConstant.GetRunPath, filePath);
            if (!System.IO.File.Exists(filePath))
            {
                methodResult.Msg = "Not found";//未找到文件
                return Json(methodResult);
            }
            var fileResult = PhysicalFile(filePath, "application/x-download");
            fileResult.FileDownloadName = fileName;
            return fileResult;
        }
    }
}
