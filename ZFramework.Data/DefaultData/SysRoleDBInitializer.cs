using ZFramework.Data.Models;
using zgcwkj.Util;

namespace ZFramework.Data.DefaultData
{
    /// <summary>
    /// 用户数据初始化
    /// </summary>
    public class SysRoleDBInitializer
    {
        /// <summary>
        /// 获取数据
        /// </summary>
        public static List<SysRoleModel> GetData
        {
            get
            {
                var lists = new List<SysRoleModel>();

                lists.Add(new SysRoleModel
                {
                    RoleID = "admin",
                    RoleName = "超级用户",
                    RolePath = "admin",
                    UserID = "admin",
                    IsDelete = 0,
                    CreateTime = "2020-01-01".ToDate(),
                    CreatorID = "",
                });

                return lists;
            }
        }
    }
}
