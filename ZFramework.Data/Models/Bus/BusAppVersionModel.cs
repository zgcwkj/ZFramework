using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using zgcwkj.Util;

namespace ZFramework.Data.Models.Bus
{
    /// <summary>
    /// 版本控制数据
    /// </summary>
    [Table("bus_appversion")]
    public class BusAppVersionModel : DbModel
    {
        /// <summary>
        /// 主键
        /// </summary>
        [Key, Column("app_id"), StringLength(32)]
        public string AppID { get; set; } = "";

        /// <summary>
        /// 名称
        /// </summary>
        [Column("app_name"), StringLength(15)]
        public string AppName { get; set; } = "";

        /// <summary>
        /// 说明
        /// </summary>
        [Column("app_desc")]
        public string AppDesc { get; set; } = "";

        /// <summary>
        /// 路径
        /// </summary>
        [Column("app_path")]
        public string AppPath { get; set; } = "";

        /// <summary>
        /// 版本
        /// </summary>
        [Column("app_version")]
        public string AppVersion { get; set; } = "";

        /// <summary>
        /// 强制更新（1强制，0不强制）
        /// </summary>
        [Column("app_enforce")]
        public int AppEnforce { get; set; } = 0;

        /// <summary>
        /// 状态（1启用，0停用）
        /// </summary>
        [Column("app_state")]
        public int AppState { get; set; } = -1;

        /// <summary>
        /// 上传时间
        /// </summary>
        [Column("upload_time")]
        public DateTime UploadTime { get; set; }

        /// <summary>
        /// 角色路径
        /// </summary>
        [Column("role_path"), StringLength(500)]
        public string RolePath { get; set; } = "";

        /// <summary>
        /// 是否删除
        /// </summary>
        [Column("is_delete")]
        public int IsDelete { get; set; } = 0;

        /// <summary>
        /// 创建时间
        /// </summary>
        [Column("create_time")]
        public DateTime CreateTime { get; set; } = DateTime.Now;
    }
}
