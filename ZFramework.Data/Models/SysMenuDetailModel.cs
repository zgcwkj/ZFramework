using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ZFramework.Data.Models
{
    /// <summary>
    /// 系统菜单明细表
    /// </summary>
    [Table("sysmenu_detail")]
    public class SysMenuDetailModel : DefaultModel
    {
        /// <summary>
        /// 菜单明细ID
        /// </summary>
        [Key, Column("mdetail_id")]
        public string MdetailID { get; set; }

        /// <summary>
        /// 菜单ID
        /// </summary>
        [Column("menu_id")]
        public int MenuID { get; set; }

        /// <summary>
        /// 角色ID
        /// </summary>
        [Column("role_id"), StringLength(32)]
        public string RoleID { get; set; }
    }
}
