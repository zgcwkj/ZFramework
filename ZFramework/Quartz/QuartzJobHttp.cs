using Quartz;
using Quartz.Impl;
using Quartz.Impl.Triggers;

namespace ZFramework.Quartz
{
    /// <summary>
    /// Quartz 任务网络请求
    /// </summary>
    public class QuartzJobHttp : IJob
    {
        /// <summary>
        /// ILogger
        /// </summary>
        private ILogger<QuartzJobHttp> _ILogger { get; }

        /// <summary>
        /// IHttpClientFactory
        /// </summary>
        private IHttpClientFactory _IHttpClient { get; }

        /// <summary>
        /// QuartzConfig
        /// </summary>
        private QuartzConfig _QuartzConfig { get; }

        /// <summary>
        /// Quartz 任务网络请求
        /// </summary>
        public QuartzJobHttp(
            ILogger<QuartzJobHttp> logger,
            IHttpClientFactory httpClientFactory,
            QuartzConfig quartzConfig)
        {
            this._ILogger = logger;
            this._IHttpClient = httpClientFactory;
            this._QuartzConfig = quartzConfig;
        }

        public async Task Execute(IJobExecutionContext context)
        {
            var dateTime = DateTime.Now;
            var httpMessage = "";
            var trigger = (context as JobExecutionContextImpl).Trigger as AbstractTrigger;

            var taskOptions = _QuartzConfig.GetJobs(a => a.TaskName == trigger.Name && a.GroupName == trigger.Group).FirstOrDefault();
            if (taskOptions == null)
            {
                taskOptions = _QuartzConfig.GetJobs(a => a.TaskName == trigger.JobName && a.GroupName == trigger.JobGroup).FirstOrDefault();
            }
            if (taskOptions == null)
            {
                _ILogger.LogError($"组别:{trigger.Group},名称:{trigger.Name},的作业未找到,可能已被移除");
                // FileHelper.WriteFile(FileQuartz.LogPath + trigger.Group, $"{trigger.Name}.txt", "未到找作业或可能被移除", true);
                return;
            }
            _ILogger.LogInformation($"组别:{trigger.Group},名称:{trigger.Name},的作业开始执行,时间:{DateTime.Now.ToString("yyyy-MM-dd HH:mm:sss")}");
            Console.WriteLine($"作业[{taskOptions.TaskName}]开始:{DateTime.Now.ToString("yyyy-MM-dd HH:mm:sss")}");
            var tab_Quarz_Tasklog = new QuarzTaskLog() { TaskName = taskOptions.TaskName, GroupName = taskOptions.GroupName, BeginDate = DateTime.Now };
            if (string.IsNullOrEmpty(taskOptions.ApiUrl) || taskOptions.ApiUrl == "/")
            {
                _ILogger.LogError($"组别:{trigger.Group},名称:{trigger.Name},参数非法或者异常!,时间:{DateTime.Now.ToString("yyyy-MM-dd HH:mm:sss")}");
                //FileHelper.WriteFile(FileQuartz.LogPath + trigger.Group, $"{trigger.Name}.txt", $"{ DateTime.Now.ToString("yyyy-MM-dd HH:mm:sss")}未配置url,", true);
                return;
            }

            try
            {
                var header = new Dictionary<string, string>();
                if (!string.IsNullOrEmpty(taskOptions.ApiAuthKey)
                    && !string.IsNullOrEmpty(taskOptions.ApiAuthValue))
                {
                    header.Add(taskOptions.ApiAuthKey.Trim(), taskOptions.ApiAuthValue.Trim());
                }

                httpMessage = await HttpSendAsync(
                    _IHttpClient,
                    taskOptions.ApiRequestType?.ToLower() == "get" ? HttpMethod.Get : HttpMethod.Post,
                    taskOptions.ApiUrl,
                    taskOptions.ApiParameter ?? "",
                    header);
            }
            catch (Exception ex)
            {
                httpMessage = ex.Message;
            }

            try
            {
                //string logContent = $"{(string.IsNullOrEmpty(httpMessage) ? "OK" : httpMessage)}\r\n";
                tab_Quarz_Tasklog.EndDate = DateTime.Now;
                tab_Quarz_Tasklog.Msg = httpMessage;
                _QuartzConfig.AddLog(tab_Quarz_Tasklog);
            }
            catch (Exception)
            {
            }
            Console.WriteLine(trigger.FullName + " " + DateTime.Now.ToString("yyyy-MM-dd HH:mm:sss") + " " + httpMessage);
            return;
        }

        public async Task<string> HttpSendAsync(IHttpClientFactory httpClientFactory, HttpMethod method, string url, string parmet, Dictionary<string, string> headers = null)
        {
            var client = httpClientFactory.CreateClient();
            var postContent = new StringContent(parmet, Encoding.UTF8, "application/json");
            var request = new HttpRequestMessage(method, url)
            {
                Content = postContent
            };
            if (headers != null)
            {
                foreach (var header in headers)
                {
                    request.Headers.Add(header.Key, header.Value);
                }
            }
            try
            {
                var httpResponseMessage = await client.SendAsync(request);
                //
                var result = await httpResponseMessage.Content.ReadAsStringAsync();
                return result;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return ex.Message;
            }
        }
    }
}
