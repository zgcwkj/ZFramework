using Microsoft.AspNetCore.Mvc;
using ZFramework.Comm.Base;
using ZFramework.Comm.Filters;

namespace ZFramework.Controllers
{
    /// <summary>
    /// 服务器信息控制器
    /// </summary>
    [Authorization]
    public class ServerController : BaseController
    {
        /// <summary>
        /// 服务器信息
        /// </summary>
        /// <returns></returns>
        public IActionResult Index()
        {
            return View();
        }
    }
}
