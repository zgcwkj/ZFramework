using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.EntityFrameworkCore;
using System.Net;
using System.Text.RegularExpressions;
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
            //请求数据
            var httpContext = context.HttpContext;
            var request = httpContext.Request;
            var toUserIP = httpContext.Connection.RemoteIpAddress.ToStr();//获取访问者IP
            if (toUserIP == "127.0.0.1")
            {
                //反代理时获取真实IP
                var RealIP = request.Headers["X-Real-IP"].ToStr();
                if (!RealIP.IsNull()) toUserIP = RealIP;
            }
            var toMethod = request.Method;//获取访问类型
            var toUserAgent = request.Headers.UserAgent;//获取访问者信息
            var parameterData = GetParameter(request);//获取请求参数
            var toController = context.ActionDescriptor as dynamic;//控制器对象
            var toClass = toController.ControllerName;//获取访问控制器
            var toAction = toController.ActionName;//获取访问动作
            var toPath = request.Path.Value;//获取访问地址
            //放行请求
            if (LetGO(toPath)) await next();
            //读取 Session
            this.UserID = SessionHelper.Get("UserID").ToStr();
            this.RolePath = SessionHelper.Get("RolePath").ToStr();
            if (UserID.IsNull() || RolePath.IsNull())
            {
                string message = WebUtility.UrlEncode("验证过期");
                context.Result = new RedirectResult($"/Admin/Index?Data={message}");
                return;
            }
            this.UserID = Convert.ToString(UserID);
            this.RolePath = Convert.ToString(RolePath);

            var resultContext = await next();
        }

        /// <summary>
        /// 放行请求
        /// </summary>
        /// <param name="openPath">路径</param>
        /// <returns></returns>
        internal bool LetGO(string openPath)
        {
            return openPath switch
            {
                "/Admin/Login" => true,
                _ => false,
            };
        }

        /// <summary>
        /// 获取参数
        /// </summary>
        /// <param name="request">请求对象</param>
        /// <returns></returns>
        internal string GetParameter(HttpRequest request)
        {
            //参数数据
            var dic = new Dictionary<string, string>();
            //GET 参数
            try
            {
                foreach (var query in request.Query)
                {
                    dic.Add(query.Key, query.Value);
                }
            }
            catch { }
            //POST 参数
            try
            {
                foreach (var form in request.Form)
                {
                    dic.Add(form.Key, form.Value);
                }
            }
            catch { }
            //参数解码
            return Regex.Unescape(dic.ToJson());
        }
    }
}
