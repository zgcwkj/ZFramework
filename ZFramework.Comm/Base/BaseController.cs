using Microsoft.AspNetCore.Mvc;

namespace ZFramework.Comm
{
    /// <summary>
    /// 根控制器
    /// </summary>
    //[Route("[controller]/[action]")]
    public class BaseController : Controller
    {
        /// <summary>
        /// Error
        /// </summary>
        /// <param name="message">消息内容</param>
        /// <returns></returns>
        public IActionResult Error(string message)
        {
            var methodResult = new MethodResult();
            methodResult.Code = -1;
            methodResult.Msg = message;
            return Json(methodResult);
        }
    }
}
