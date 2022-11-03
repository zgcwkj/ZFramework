using ZFramework.Data.Models;
using zgcwkj.Util;

namespace ZFramework.Data.DefaultData
{
    /// <summary>
    /// 菜单数据初始化
    /// </summary>
    public class SysMenuDBInitializer
    {
        /// <summary>
        /// 获取数据
        /// </summary>
        public static List<SysMenuModel> GetData
        {
            get
            {
                var lists = new List<SysMenuModel>();

                lists.Add(new SysMenuModel
                {
                    MenuID = 1,
                    ParentID = 0,
                    Title = "系统设置",
                    Icon = "fa fa-gear",
                    Link = "",
                    Sort = 999,
                    IsDelete = 0,
                    CreateTime = "2020-01-01".ToDate(),
                    CreatorID = "",
                });

                lists.Add(new SysMenuModel
                {
                    MenuID = 2,
                    ParentID = 1,
                    Title = "用户管理",
                    Icon = "fa fa-user",
                    Link = "/User",
                    Sort = 1,
                    IsDelete = 0,
                    CreateTime = "2020-01-01".ToDate(),
                    CreatorID = "",
                });

                lists.Add(new SysMenuModel
                {
                    MenuID = 3,
                    ParentID = 1,
                    Title = "角色管理",
                    Icon = "fa fa-users",
                    Link = "/Role",
                    Sort = 2,
                    IsDelete = 0,
                    CreateTime = "2020-01-01".ToDate(),
                    CreatorID = "",
                });

                lists.Add(new SysMenuModel
                {
                    MenuID = 4,
                    ParentID = 1,
                    Title = "系统日志",
                    Icon = "fa fa-leaf",
                    Link = "/Log",
                    Sort = 3,
                    IsDelete = 0,
                    CreateTime = "2020-01-01".ToDate(),
                    CreatorID = "",
                });

                lists.Add(new SysMenuModel
                {
                    MenuID = 5,
                    ParentID = 1,
                    Title = "系统信息",
                    Icon = "fa fa-server",
                    Link = "/Server",
                    Sort = 3,
                    IsDelete = 0,
                    CreateTime = "2020-01-01".ToDate(),
                    CreatorID = "",
                });

                lists.Add(new SysMenuModel
                {
                    MenuID = 6,
                    ParentID = 0,
                    Title = "功能管理",
                    Icon = "fa fa-leaf",
                    Link = "",
                    Sort = 998,
                    IsDelete = 0,
                    CreateTime = "2020-01-01".ToDate(),
                    CreatorID = "",
                });

                lists.Add(new SysMenuModel
                {
                    MenuID = 7,
                    ParentID = 6,
                    Title = "版本控制",
                    Icon = "fa fa-file-code-o",
                    Link = "/AppVersion/AppVersion",
                    Sort = 3,
                    IsDelete = 0,
                    CreateTime = "2020-01-01".ToDate(),
                    CreatorID = "",
                });

                return lists;
            }
        }
    }
}
