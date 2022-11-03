using ZFramework.Data.Models;
using zgcwkj.Util;
using zgcwkj.Util.Common;

namespace ZFramework.Data.DefaultData
{
    /// <summary>
    /// 菜单数据初始化
    /// </summary>
    public class SysMenuDetailDBInitializer
    {
        /// <summary>
        /// 获取数据
        /// </summary>
        public static List<SysMenuDetailModel> GetData
        {
            get
            {
                var lists = new List<SysMenuDetailModel>();

                lists.Add(new SysMenuDetailModel
                {
                    MdetailID = MD5Tool.GetMd5("1"),
                    MenuID = 1,
                    RoleID = "admin",
                    IsDelete = 0,
                    CreateTime = "2020-01-01".ToDate(),
                    CreatorID = "",
                });

                lists.Add(new SysMenuDetailModel
                {
                    MdetailID = MD5Tool.GetMd5("2"),
                    MenuID = 2,
                    RoleID = "admin",
                    IsDelete = 0,
                    CreateTime = "2020-01-01".ToDate(),
                    CreatorID = "",
                });

                lists.Add(new SysMenuDetailModel
                {
                    MdetailID = MD5Tool.GetMd5("3"),
                    MenuID = 3,
                    RoleID = "admin",
                    IsDelete = 0,
                    CreateTime = "2020-01-01".ToDate(),
                    CreatorID = "",
                });

                lists.Add(new SysMenuDetailModel
                {
                    MdetailID = MD5Tool.GetMd5("4"),
                    MenuID = 4,
                    RoleID = "admin",
                    IsDelete = 0,
                    CreateTime = "2020-01-01".ToDate(),
                    CreatorID = "",
                });

                lists.Add(new SysMenuDetailModel
                {
                    MdetailID = MD5Tool.GetMd5("5"),
                    MenuID = 5,
                    RoleID = "admin",
                    IsDelete = 0,
                    CreateTime = "2020-01-01".ToDate(),
                    CreatorID = "",
                });

                lists.Add(new SysMenuDetailModel
                {
                    MdetailID = MD5Tool.GetMd5("6"),
                    MenuID = 6,
                    RoleID = "admin",
                    IsDelete = 0,
                    CreateTime = "2020-01-01".ToDate(),
                    CreatorID = "",
                });

                lists.Add(new SysMenuDetailModel
                {
                    MdetailID = MD5Tool.GetMd5("7"),
                    MenuID = 7,
                    RoleID = "admin",
                    IsDelete = 0,
                    CreateTime = "2020-01-01".ToDate(),
                    CreatorID = "",
                });

                return lists;
            }
        }
    }
}
