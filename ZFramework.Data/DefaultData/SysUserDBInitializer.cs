using ZFramework.Data.Models;
using zgcwkj.Util;
using zgcwkj.Util.Common;

namespace ZFramework.Data.DefaultData
{
    /// <summary>
    /// 用户数据初始化
    /// </summary>
    public class SysUserDBInitializer
    {
        /// <summary>
        /// 获取数据
        /// </summary>
        public static List<SysUserModel> GetData
        {
            get
            {
                var lists = new List<SysUserModel>();

                lists.Add(new SysUserModel
                {
                    UserID = "admin",
                    UserName = "admin",
                    Accounts = "admin",
                    Password = MD5Tool.GetMd5("admin"),
                    LoadRP = 1,
                    LinkTable = 0,
                    IsDelete = 0,
                    CreateTime = "2020-01-01".ToDate(),
                    CreatorID = "",
                });

                return lists;
            }
        }
    }
}
