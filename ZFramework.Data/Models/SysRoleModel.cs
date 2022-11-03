using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ZFramework.Data.Models
{
    /// <summary>
    /// 系统角色表
    /// </summary>
    [Table("sysrole")]
    public class SysRoleModel : DefaultModel
    {
        /// <summary>
        /// 角色ID
        /// </summary>
        [Key, Column("role_id"), StringLength(32)]
        public string RoleID { get; set; }

        /// <summary>
        /// 角色名称
        /// </summary>
        [Column("role_name"), StringLength(50)]
        public string RoleName { get; set; }

        /// <summary>
        /// 角色路径
        /// </summary>
        [Column("role_path"), StringLength(500)]
        public string RolePath { get; set; }

        /// <summary>
        /// 用户ID
        /// </summary>
        [Column("user_id"), StringLength(32)]
        public string UserID { get; set; }
    }
}
