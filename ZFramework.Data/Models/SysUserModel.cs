using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ZFramework.Data.Models
{
    /// <summary>
    /// 系统用户表
    /// </summary>
    [Table("sysuser")]
    public class SysUserModel : DefaultModel
    {
        /// <summary>
        /// 用户ID
        /// </summary>
        [Key, Column("user_id"), StringLength(32)]
        public string UserID { get; set; }

        /// <summary>
        /// 用户名称
        /// </summary>
        [Column("user_name"), StringLength(32)]
        public string UserName { get; set; }

        /// <summary>
        /// 账号
        /// </summary>
        [Column("accounts"), StringLength(32)]
        public string Accounts { get; set; }

        /// <summary>
        /// 密码
        /// </summary>
        [Column("password"), StringLength(32)]
        public string Password { get; set; }

        /// <summary>
        /// 获取RP数据
        /// </summary>
        [Column("load_rp")]
        public int LoadRP { get; set; }

        /// <summary>
        /// 获取数据时是否连表
        /// </summary>
        [Column("link_table")]
        public int LinkTable { get; set; }
    }
}
