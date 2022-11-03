using System.Text.Json.Serialization;

namespace ZFramework.Api.Models
{
    /// <summary>
    /// 更新信息模型
    /// </summary>
    public class UpdateInfoModel
    {
        /// <summary>
        /// 下载地址
        /// </summary>
        [JsonPropertyName("download_url")]
        public string DownloadUrl { get; set; } = "/ApiData/GetAppFile";

        /// <summary>
        /// 版本号
        /// </summary>
        [JsonPropertyName("version")]
        public string Version { get; set; } = "";

        /// <summary>
        /// 弹窗
        /// </summary>
        [JsonPropertyName("dialog")]
        public bool Dialog { get; set; } = false;

        /// <summary>
        /// 更新描述
        /// </summary>
        [JsonPropertyName("msg")]
        public string Desc { get; set; } = "";

        /// <summary>
        /// 强制更新
        /// </summary>
        [JsonPropertyName("force")]
        public bool Enforce { get; set; } = false;
    }
}
