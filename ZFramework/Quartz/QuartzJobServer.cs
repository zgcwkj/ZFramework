using Quartz;
using Quartz.Impl;
using zgcwkj.Util;

namespace ZFramework.Quartz
{
    /// <summary>
    /// 定时任务
    /// </summary>
    public class QuartzJobServer
    {
        /// <summary>
        /// 启动
        /// </summary>
        /// <returns></returns>
        public static bool Start()
        {
            //启动定时服务
            try
            {
                //启动线程
                Task.Run(async () =>
                {
                    await StartAsync();
                });
                Console.WriteLine($"任务服务启动成功");
                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"任务服务启动失败（{ex.Message}）");
            }
            return false;
        }

        /// <summary>
        /// 启动线程
        /// </summary>
        /// <returns></returns>
        private static async Task StartAsync()
        {
            //标准调度器工厂
            var schedulerFactory = new StdSchedulerFactory();
            //调度程序
            var scheduler = await schedulerFactory.GetScheduler();
            //启动
            await scheduler.Start();

            //==>

            //删除文件任务开关
            if (ConfigHelp.Get<bool>("DelFileServiceConfig:Switch"))
            {
                await QuartzStartDelFileAsync(scheduler);
            }
        }

        /// <summary>
        /// 启动删除文件任务
        /// </summary>
        /// <returns></returns>
        public static async Task QuartzStartDelFileAsync(IScheduler scheduler)
        {
            //创建作业和触发器
            var jobDetail = JobBuilder.Create<DelFileQuartzJob>().Build();
            //触发器生成器
            string runTime = ConfigHelp.Get("DelFileServiceConfig:RunTime");
            var itrigger = TriggerBuilder.Create().WithCronSchedule(runTime);
            //触发器生成器创建
            var trigger = itrigger.Build();
            //添加调度
            await scheduler.ScheduleJob(jobDetail, trigger);
        }

        /// <summary>
        /// 尝试访问数据库
        /// </summary>
        /// <returns></returns>
        public static bool TryLinkDataBase()
        {
            try
            {
                //检查数据库的链接状态
                using var dbAccess = DbProvider.Create();
                dbAccess.SetCommandText("select 0");
                var valueData = dbAccess.QueryScalar().ToTrim();

                Console.WriteLine($"数据库正常，执行本次操作");

                return true;
            }
            catch (Exception ex)
            {
                string msg = ex.Message;

                Console.WriteLine($"数据库异常，放弃本次执行");

                return false;
            }
        }
    }
}
