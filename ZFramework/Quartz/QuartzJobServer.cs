using Quartz;
using Quartz.Impl;
using zgcwkj.Util;

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
        /// <param name="services"></param>
        public static void AddQuartzJob(this IServiceCollection services)
        {
            //添加服务
            services.AddQuartz(config =>
            {
                config.UseMicrosoftDependencyInjectionJobFactory();

                //==>
                if (GlobalConstant.IsDevelopment)
                {
                    Console.WriteLine($"不启用任务服务，测试环境");
                    return;
                }
                else
                {
                    Console.WriteLine($"启用任务服务");
                }
                //==>

                //删除文件任务开关
                if (ConfigHelp.Get<bool>("DelFileServiceConfig:Switch")) StartDelFileAsync(config);
            });
            //托管服务
            services.AddQuartzServer(options =>
            {
                //关闭时我们希望作业能够优雅地完成
                options.WaitForJobsToComplete = true;
            });
        }

        /// <summary>
        /// 启动删除文件任务
        /// </summary>
        /// <returns></returns>
        public static void StartDelFileAsync(IServiceCollectionQuartzConfigurator config)
        {
            //任务信息
            var jobName = typeof(DelFileQuartzJob).Name;
            var jobKey = new JobKey(jobName);
            //运行表达式
            var runTime = ConfigHelp.Get("DelFileServiceConfig:RunTime");
            //创建作业
            config.AddJob<DelFileQuartzJob>(opts => opts.WithIdentity(jobKey));
            //添加调度
            config.AddTrigger(opts => opts.ForJob(jobKey).WithIdentity(jobName).WithCronSchedule(runTime));
        }
    }
}
