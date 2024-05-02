namespace ZFramework.Quartz
{
    /// <summary>
    /// 任务状态
    /// </summary>
    public enum JobState
    {
        /// <summary>
        /// 新增
        /// </summary>
        新增 = 1,

        /// <summary>
        /// 删除
        /// </summary>
        删除 = 2,

        /// <summary>
        /// 修改
        /// </summary>
        修改 = 3,

        /// <summary>
        /// 停用
        /// </summary>
        停用 = 4,

        /// <summary>
        /// 停止
        /// </summary>
        停止 = 5,

        /// <summary>
        /// 启用
        /// </summary>
        启用 = 6,

        /// <summary>
        /// 立即执行
        /// </summary>
        立即执行 = 7
    }
}
