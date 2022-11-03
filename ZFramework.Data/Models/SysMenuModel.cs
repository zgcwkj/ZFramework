using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ZFramework.Data.Models
{
    /// <summary>
    /// 系统菜单表
    /// </summary>
    [Table("sysmenu")]
    public class SysMenuModel : DefaultModel
    {
        /// <summary>
        /// 菜单ID
        /// </summary>
        [Key, Column("menu_id"), DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int MenuID { get; set; }

        /// <summary>
        /// 父级菜单ID
        /// </summary>
        [Column("parent_id")]
        public int ParentID { get; set; }

        /// <summary>
        /// 标题
        /// </summary>
        [Column("title"), StringLength(32)]
        public string Title { get; set; }

        /// <summary>
        /// 图标
        /// </summary>
        [Column("icon"), StringLength(32)]
        public string Icon { get; set; }

        /// <summary>
        /// 连接
        /// </summary>
        [Column("link"), StringLength(100)]
        public string Link { get; set; }

        /// <summary>
        /// 排序
        /// </summary>
        [Column("sort")]
        public int Sort { get; set; }
    }
}
