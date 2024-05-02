using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using System.Runtime.InteropServices;
using ZFramework.Comm;

namespace ZFramework.Api.Controllers
{
    /// <summary>
    /// 系统数据控制器
    /// </summary>
    [Authorization]
    [Route("[controller]/[action]")]
    public class SystemDataController : Controller
    {
        /// <summary>
        /// 获取系统名称
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        public string GetOSName()
        {
            return Environment.MachineName;
        }

        /// <summary>
        /// 获取系统描述
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        public string GetOSDescription()
        {
            return RuntimeInformation.OSDescription;
        }

        /// <summary>
        /// 获取系统类型
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        public string GetOSArchitecture()
        {
            return RuntimeInformation.OSArchitecture.ToStr();
        }

        /// <summary>
        /// 获取启动时间
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        public string GetStartTime()
        {
            return Process.GetCurrentProcess().StartTime.ToStr("yyyy-MM-dd HH:mm:ss");
        }

        /// <summary>
        /// 获取系统类型
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        public string GetFrameworkDescription()
        {
            return RuntimeInformation.FrameworkDescription;
        }
    }
}
