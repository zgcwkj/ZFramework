namespace ZFramework.Quartz
{
    /// <summary>
    /// 基础对象
    /// </summary>
    public class BaseModel
    {
        /// <summary>
        /// 主键ID
        /// </summary>
        public int ID { get; set; }

        /// <summary>
        /// 数据插入时间
        /// </summary>
        public DateTime? TimeFlag { get; set; } = DateTime.Now;

        /// <summary>
        /// 数据修改时间
        /// </summary>
        public DateTime? ChangeTime { get; set; } = DateTime.Now;
    }
}
