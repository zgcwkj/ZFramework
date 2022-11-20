using Microsoft.AspNetCore.Mvc;
using ZFramework.Comm.Base;
using ZFramework.Comm.Filters;
using zgcwkj.Util;

namespace ZFramework.Controllers
{
    /// <summary>
    /// 帮助控制器
    /// </summary>
    public class HelpController : BaseController
    {
        /// <summary>
        /// Help/Index
        /// </summary>
        /// <returns></returns>
        [Authorization]
        [HttpGet]
        public IActionResult Index()
        {
            #region 通知消息

            var mesInfo = "请登录帐号";
            var userID = SessionHelper.Get("UserID");
            if (!userID.IsNull())
            {
                var mesInfoCache = "";
                if (!mesInfoCache.IsNull()) mesInfo = mesInfoCache;
                else mesInfo = "暂无消息";
            }
            ViewData["MesInfo"] = mesInfo;

            #endregion 通知消息

            return View();
        }

        /// <summary>
        /// Help/Error
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public IActionResult Error()
        {
            return View();
        }

        /// <summary>
        /// Help/Close
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public IActionResult Close()
        {
            return View();
        }
    }
}
