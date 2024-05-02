using Microsoft.EntityFrameworkCore;
using ZFramework.Data.DefaultData;
using ZFramework.Data.Models;
using ZFramework.Data.Models.Bus;
using zgcwkj.Util;
using zgcwkj.Util.Data;

namespace ZFramework.Data
{
    /// <summary>
    /// 数据连接对象
    /// </summary>
    public class MyDbContext : DbCommon
    {
        /// <summary>
        /// 配置要使用的数据库
        /// </summary>
        /// <param name="optionsBuilder">上下文选项生成器</param>
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            //读取配置
            var dbConnect = ConfigHelp.Get("DBConnect");
            var dbTimeout = 10;
            //SQLite
            optionsBuilder.UseSqlite(dbConnect, p =>
            {
                p.CommandTimeout(dbTimeout);
            });
            //
            base.OnConfiguring(optionsBuilder);
        }

        /// <summary>
        /// 配置通过约定发现的所有模型
        /// </summary>
        /// <param name="modelBuilder">模型制作者</param>
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            //初始化用户数据
            modelBuilder.Entity<SysUserModel>().HasData(SysUserDBInitializer.GetData);
            //初始化角色数据
            modelBuilder.Entity<SysRoleModel>().HasData(SysRoleDBInitializer.GetData);
            //初始化菜单数据
            modelBuilder.Entity<SysMenuModel>().HasData(SysMenuDBInitializer.GetData);
            //初始化菜单明细数据
            modelBuilder.Entity<SysMenuDetailModel>().HasData(SysMenuDetailDBInitializer.GetData);
            //
            base.OnModelCreating(modelBuilder);
        }

        /// <summary>
        /// 系统用户表
        /// </summary>
        public DbSet<SysUserModel> SysUserModel { get; set; }

        /// <summary>
        /// 系统角色表
        /// </summary>
        public DbSet<SysRoleModel> SysRoleModel { get; set; }

        /// <summary>
        /// 系统菜单表
        /// </summary>
        public DbSet<SysMenuModel> SysMenuModel { get; set; }

        /// <summary>
        /// 系统菜单明细表
        /// </summary>
        public DbSet<SysMenuDetailModel> SysMenuDetailModel { get; set; }

        /// <summary>
        /// 版本控制数据
        /// </summary>
        public DbSet<BusAppVersionModel> BusAppVersionModel { get; set; }
    }
}
