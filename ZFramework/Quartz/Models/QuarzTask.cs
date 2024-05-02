using System.ComponentModel;

namespace ZFramework.Quartz
{
    /// <summary>
    /// 任务
    /// </summary>
    [Description("任务表")]
    public class QuarzTask : BaseModel
    {
        /// <summary>
        /// 任务名
        /// </summary>
        [Description("任务名")]
        public string TaskName { get; set; }

        /// <summary>
        /// 分组名
        /// </summary>
        [Description("分组名")]
        public string GroupName { get; set; }

        /// <summary>
        /// 间隔时间
        /// </summary>
        [Description("间隔时间")]
        public string Interval { get; set; }

        /// <summary>
        /// 调用的API地址
        /// </summary>
        [Description("调用的API地址")]
        public string ApiUrl { get; set; }

        /// <summary>
        /// 任务描述
        /// </summary>
        [Description("任务描述")]
        public string Describe { get; set; }

        /// <summary>
        /// 最近一次运行时间
        /// </summary>
        [Description("最近一次运行时间")]
        public DateTime? LastRunTime { get; set; }

        /// <summary>
        /// 运行状态
        /// </summary>
        [Description("运行状态")]
        public int Status { get; set; }

        /// <summary>
        /// 运行状态描述
        /// </summary>
        [Description("运行状态描述")]
        public string StatusStr
        {
            get
            {
                return Enum.GetName(typeof(JobState), Status);
            }
        }

        /// <summary>
        /// 请求类型
        /// </summary>
        [Description("请求类型")]
        public string ApiRequestType { get; set; }

        /// <summary>
        /// 请求头Key
        /// </summary>
        [Description("请求头Key")]
        public string ApiAuthKey { get; set; }

        /// <summary>
        /// 请求头Value
        /// </summary>
        [Description("请求头Value")]
        public string ApiAuthValue { get; set; }

        /// <summary>
        /// API参数
        /// </summary>
        [Description("API参数")]
        public string ApiParameter { get; set; }
    }
}
