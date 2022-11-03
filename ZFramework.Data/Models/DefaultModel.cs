using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using zgcwkj.Util;

namespace ZFramework.Data.Models
{
    /// <summary>
    /// 默认模型
    /// </summary>
    public class DefaultModel : DbModel
    {
        /// <summary>
        /// 是否删除
        /// </summary>
        [Column("is_delete")]
        public int IsDelete { get; set; }

        /// <summary>
        /// 创建时间
        /// </summary>
        [Column("create_time")]
        public DateTime CreateTime { get; set; }

        /// <summary>
        /// 创建人
        /// </summary>
        [Column("creator_id"), StringLength(32)]
        public string CreatorID { get; set; }
    }
}
