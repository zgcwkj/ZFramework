namespace ZFramework.Comm
{
    /// <summary>
    /// 方法结果
    /// </summary>
    public class MethodResult
    {
        /// <summary>
        /// 错误代码
        /// </summary>
        public int? Code { get; set; } = null;

        /// <summary>
        /// 错误消息
        /// </summary>
        public string Msg { get; set; } = null;

        /// <summary>
        /// 数据
        /// </summary>
        public object Data { get; set; } = null;

        /// <summary>
        /// 数据总数量
        /// </summary>
        public int? Total { get; set; } = null;
    }
}
