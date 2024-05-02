using Quartz;
using Quartz.AspNetCore;
using Quartz.Impl;

namespace ZFramework.Quartz
{
    /// <summary>
    /// 定时任务
    /// </summary>
    public static class QuartzJobServer
    {
        /// <summary>
        /// 添加任务服务
        /// </summary>
        public static void AddQuartz(this IServiceCollection services)
        {
            //添加服务
            services.AddQuartz(config =>
            {
                //使用 Microsoft 依赖注入作业工厂
                config.UseMicrosoftDependencyInjectionJobFactory();
            });
            //托管服务
            services.AddQuartzServer(options =>
            {
                //关闭时我们希望作业能够优雅地完成
                options.WaitForJobsToComplete = true;
            });
            //
            services.AddSingleton<ISchedulerFactory, StdSchedulerFactory>();
            //网络请求主键
            services.AddHttpClient();
            services.AddScoped<QuartzJobHttp>();
            //依赖注入
            services.AddScoped<QuartzConfig>();
            services.AddScoped<QuartzHandle>();
        }

        /// <summary>
        /// 启用任务服务
        /// </summary>
        public static void AddQuartz(this IApplicationBuilder builder)
        {
            var services = builder.ApplicationServices;
            var scope = services.CreateScope();
            var quartzHandle = scope.ServiceProvider.GetRequiredService<QuartzHandle>();
            //初始化服务
            Task.Run(async () =>
            {
                await quartzHandle.InitJobs();
            });
            //var initTask = quartzHandle.InitJobs();
            //initTask.GetAwaiter().GetResult();
        }
    }
}
