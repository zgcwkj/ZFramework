using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using System.Net;
using zgcwkj.Util;

namespace ZFramework.Comm.Filters
{
    /// <summary>
    /// 权限检测特性类
    /// </summary>
    public class Authorization : ActionFilterAttribute
    {
        /// <summary>
        /// 权限字符串
        /// </summary>
        public string Authorize { get; set; }

        /// <summary>
        /// 用户ID
        /// </summary>
        private string UserID { get; set; }

        /// <summary>
        /// 角色路径
        /// </summary>
        private string RolePath { get; set; }

        /// <summary>
        /// 权限检测特性类
        /// </summary>
        public Authorization()
        { }

        /// <summary>
        /// 权限检测特性类
        /// </summary>
        /// <param name="authorize">授权</param>
        public Authorization(string authorize)
        {
            this.Authorize = authorize;
        }

        /// <summary>
        /// 动作执行异步
        /// </summary>
        /// <param name="context">动作执行上下文</param>
        /// <param name="next">动作执行委托</param>
        /// <returns></returns>
        public override async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
        {
            //验证并读取Session数据
            var UserID = SessionHelper.Get("UserID");
            var RolePath = SessionHelper.Get("RolePath");
            if (UserID == null || RolePath == null)
            {
                string message = WebUtility.UrlEncode("验证过期");
                context.Result = new RedirectResult($"/Admin/Index?Data={message}");
                return;
            }
            this.UserID = Convert.ToString(UserID);
            this.RolePath = Convert.ToString(RolePath);

            var resultContext = await next();
        }
    }
}
