using Quartz;
using Quartz.Impl.Matchers;
using Quartz.Impl.Triggers;
using Quartz.Spi;

namespace ZFramework.Quartz
{
    /// <summary>
    /// Quartz 功能操作
    /// </summary>
    public class QuartzHandle
    {
        private ISchedulerFactory _ISchedulerFactory { get; }
        private IJobFactory _IJobFactory { get; }
        private ILogger<QuartzHandle> _ILogger { get; }
        private QuartzConfig _QuartzConfig { get; }

        /// <summary>
        /// Quartz 功能操作
        /// </summary>
        public QuartzHandle(
            ISchedulerFactory schedulerFactory,
            IJobFactory jobFactory,
            ILogger<QuartzHandle> logger,
            QuartzConfig quartzConfig)
        {
            this._ISchedulerFactory = schedulerFactory;
            this._IJobFactory = jobFactory;
            this._ILogger = logger;
            this._QuartzConfig = quartzConfig;
        }

        /// <summary>
        /// 初始化任务
        /// </summary>
        /// <returns></returns>
        public async Task InitJobs()
        {
            var jobs = _QuartzConfig.GetJobs(a => 1 == 1);
            var scheduler = await _ISchedulerFactory.GetScheduler();
            foreach (var item in jobs)
            {
                try
                {
                    var job = JobBuilder.Create<QuartzJobHttp>()
                        .WithIdentity(item.TaskName, item.GroupName)
                        .Build();
                    var trigger = TriggerBuilder.Create()
                       .WithIdentity(item.TaskName, item.GroupName)
                       .WithDescription(item.Describe)
                       .WithCronSchedule(item.Interval)
                       .Build();

                    if (_IJobFactory != null)
                    {
                        scheduler.JobFactory = _IJobFactory;
                    }

                    if (item.Status == (int)JobState.启用)
                    {
                        await scheduler.ScheduleJob(job, trigger);
                        _QuartzConfig.AddLog(new()
                        {
                            TaskName = item.TaskName,
                            GroupName = item.GroupName,
                            BeginDate = DateTime.Now,
                            Msg = $"任务初始化启动成功:{item.StatusStr}({item.Status})"
                        });
                    }
                    else
                    {
                        await scheduler.ScheduleJob(job, trigger);
                        await Pause(item.ID);
                        _ILogger.LogInformation($"任务初始化,未启动,状态为:{item.StatusStr}({item.Status})");
                    }
                }
                catch (Exception ex)
                {
                    _QuartzConfig.AddLog(new()
                    {
                        TaskName = item.TaskName,
                        GroupName = item.GroupName,
                        Msg = $"任务初始化未启动,出现异常,异常信息{ex.Message}"
                    });
                    continue;
                }
                await scheduler.Start();
            }
        }

        /// <summary>
        /// 获取所有的作业
        /// </summary>
        /// <returns></returns>
        public async Task<List<QuarzTask>> GetJobs()
        {
            try
            {
                var _scheduler = await _ISchedulerFactory.GetScheduler();
                var groups = await _scheduler.GetJobGroupNames();
                var list = _QuartzConfig.GetJobs(a => 1 == 1);
                foreach (var groupName in groups)
                {
                    foreach (var jobKey in await _scheduler.GetJobKeys(GroupMatcher<JobKey>.GroupEquals(groupName)))
                    {
                        var taskOption = list.Where(x => x.GroupName == jobKey.Group && x.TaskName == jobKey.Name).FirstOrDefault();
                        if (taskOption == null) continue;

                        var triggers = await _scheduler.GetTriggersOfJob(jobKey);
                        foreach (ITrigger trigger in triggers)
                        {
                            var dateTimeOffset = trigger.GetPreviousFireTimeUtc();
                            if (dateTimeOffset != null)
                            {
                                taskOption.LastRunTime = Convert.ToDateTime(dateTimeOffset.ToString());
                            }
                            else
                            {
                                var runlog = _QuartzConfig.Getlastlog(taskOption.TaskName, taskOption.GroupName);
                                if (runlog != null)
                                {
                                    taskOption.LastRunTime = runlog.BeginDate;
                                }
                            }
                        }
                    }
                }
                return list;
            }
            catch (Exception ex)
            {
                if (_ILogger != null)
                {
                    _ILogger.LogWarning("获取作业异常：" + ex.Message + ex.StackTrace);
                }
            }
            return new();
        }

        /// <summary>
        /// 添加作业
        /// </summary>
        /// <param name="taskOptions"></param>
        /// <returns></returns>
        public async Task<(bool, string)> AddJob(QuarzTask taskOptions)
        {
            try
            {
                var validExpression = IsValidExpression(taskOptions.Interval);
                if (!validExpression.Item1) return validExpression;

                var model = _QuartzConfig.GetJobs(a => a.TaskName == taskOptions.TaskName && a.GroupName == taskOptions.GroupName).FirstOrDefault();
                if (model != null) return (false, $"任务已存在,添加失败");

                var isaddsql = _QuartzConfig.AddJob(taskOptions);
                var job = JobBuilder.Create<QuartzJobHttp>()
                    .WithIdentity(taskOptions.TaskName, taskOptions.GroupName)
                    .Build();
                var trigger = TriggerBuilder.Create()
                   .WithIdentity(taskOptions.TaskName, taskOptions.GroupName)
                   .WithDescription(taskOptions.Describe)
                   .WithCronSchedule(taskOptions.Interval)
                   .Build();

                var scheduler = await _ISchedulerFactory.GetScheduler();

                if (_IJobFactory != null)
                {
                    scheduler.JobFactory = _IJobFactory;
                }

                //启用才加入Schedule中,如果加入在停用而定时任务执行过快,会导致卡死
                if (taskOptions.Status == (int)JobState.启用)
                {
                    await scheduler.ScheduleJob(job, trigger);
                    await scheduler.Start();
                }
                else
                {
                    await Pause(taskOptions.ID);
                    _QuartzConfig.AddLog(new QuarzTaskLog()
                    {
                        TaskName = taskOptions.TaskName,
                        GroupName = taskOptions.GroupName,
                        Msg = $"任务新建,未启动,状态为:{taskOptions.StatusStr}({taskOptions.Status})"
                    });
                }
                return (isaddsql.Item1, "任务添加成功");
            }
            catch (Exception ex)
            {
                return (false, ex.Message);
                //return (false, "任务添加失败");
            }
        }

        /// <summary>
        /// 更新作业
        /// </summary>
        /// <param name="taskOptions"></param>
        /// <returns></returns>
        public async Task<(bool, string)> Update(QuarzTask taskOptions)
        {
            var isjob = await IsQuartzJob(taskOptions.TaskName, taskOptions.GroupName);
            var taskmodle = _QuartzConfig.GetJobs(a => a.ID == taskOptions.ID).FirstOrDefault();
            var message = "";
            if (isjob.Item1) //如果Quartz存在就更新
            {
                try
                {
                    var scheduler = await _ISchedulerFactory.GetScheduler();
                    var jobKeys = scheduler.GetJobKeys(GroupMatcher<JobKey>.GroupEquals(taskOptions.GroupName)).Result.ToList();
                    var jobKey = jobKeys.Where(s => scheduler.GetTriggersOfJob(s).Result.Any(x => (x as CronTriggerImpl).Name == taskOptions.TaskName)).FirstOrDefault();
                    var triggers = await scheduler.GetTriggersOfJob(jobKey);
                    var triggerold = triggers?.Where(x => (x as CronTriggerImpl).Name == taskOptions.TaskName).FirstOrDefault();
                    await scheduler.PauseTrigger(triggerold.Key);
                    await scheduler.UnscheduleJob(triggerold.Key);// 移除触发器
                    await scheduler.DeleteJob(triggerold.JobKey);
                    var job = JobBuilder.Create<QuartzJobHttp>()
                        .WithIdentity(taskOptions.TaskName, taskOptions.GroupName)
                        .Build();
                    var triggernew = TriggerBuilder.Create()
                       .WithIdentity(taskOptions.TaskName, taskOptions.GroupName)
                       .StartNow()
                       .WithDescription(taskOptions.Describe)
                       .WithCronSchedule(taskOptions.Interval)
                       .Build();

                    if (_IJobFactory != null)
                    {
                        scheduler.JobFactory = _IJobFactory;
                    }
                    await scheduler.ScheduleJob(job, triggernew);
                    if (taskOptions.Status == (int)JobState.启用)
                    {
                        await scheduler.Start();
                    }
                    else
                    {
                        await scheduler.PauseTrigger(triggernew.Key);
                        _QuartzConfig.AddLog(new QuarzTaskLog()
                        {
                            TaskName = taskOptions.TaskName,
                            GroupName = taskOptions.GroupName,
                            Msg = $"任务新建,未启动,状态为:{taskOptions.StatusStr}({taskOptions.Status})"
                        });
                    }
                    message += "quarz已更新,";
                }
                catch (Exception ex)
                {
                    message += ex.Message;
                }
            }
            if (taskmodle != null)
            {
                isjob = _QuartzConfig.Update(taskOptions);
                message += isjob.Item2;
            }
            return (isjob.Item1, message);
        }

        /// <summary>
        /// 移除作业
        /// </summary>
        /// <param name="taskOptions"></param>
        /// <returns></returns>
        public async Task<(bool, string)> Remove(QuarzTask taskOptions)
        {
            var isjob = await IsQuartzJob(taskOptions.TaskName, taskOptions.GroupName);
            var taskmodle = _QuartzConfig.GetJobs(a => a.TaskName == taskOptions.TaskName && a.GroupName == taskOptions.GroupName).FirstOrDefault();
            var message = "";
            if (isjob.Item1)
            {
                try
                {
                    var scheduler = await _ISchedulerFactory.GetScheduler();
                    var jobKeys = scheduler.GetJobKeys(GroupMatcher<JobKey>.GroupEquals(taskOptions.GroupName)).Result.ToList();
                    var jobKey = jobKeys.Where(s => scheduler.GetTriggersOfJob(s).Result.Any(x => (x as CronTriggerImpl).Name == taskOptions.TaskName)).FirstOrDefault();
                    var triggers = await scheduler.GetTriggersOfJob(jobKey);
                    var trigger = triggers?.Where(x => (x as CronTriggerImpl).Name == taskOptions.TaskName).FirstOrDefault();
                    await scheduler.PauseTrigger(trigger.Key);
                    await scheduler.UnscheduleJob(trigger.Key);// 移除触发器
                    await scheduler.DeleteJob(trigger.JobKey);
                }
                catch (Exception ex)
                {
                    message += ex.Message;
                }
            }
            if (taskmodle != null)
            {
                isjob = _QuartzConfig.Remove(taskmodle);
            }
            message += isjob.Item2;
            return (isjob.Item1, message);
        }

        /// <summary>
        /// 立即执行一次作业
        /// </summary>
        /// <param name="id">ID</param>
        /// <returns></returns>
        public async Task<(bool, string)> Run(int id)
        {
            try
            {
                var allJob = await GetJobs();
                var getJob = allJob.Where(w => w.ID == id);
                if (getJob.Any())
                {
                    var taskOptions = getJob.First();
                    var isjob = await IsQuartzJob(taskOptions.TaskName, taskOptions.GroupName);
                    if (isjob.Item1)
                    {
                        //taskmodle.Status = (int)JobState.立即执行;
                        var scheduler = await _ISchedulerFactory.GetScheduler();
                        var jobKeys = (await scheduler.GetJobKeys(GroupMatcher<JobKey>.GroupEquals(taskOptions.GroupName))).ToList();
                        var jobKey = jobKeys.Where(s => scheduler.GetTriggersOfJob(s).Result.Any(x => (x as CronTriggerImpl).Name == taskOptions.TaskName)).FirstOrDefault();
                        var triggers = await scheduler.GetTriggersOfJob(jobKey);
                        var trigger = triggers?.Where(x => (x as CronTriggerImpl).Name == taskOptions.TaskName).FirstOrDefault();
                        await scheduler.TriggerJob(jobKey);
                        return (true, $"{taskOptions.TaskName}立即执行任务成功");
                    }
                    else
                    {
                        return isjob;
                    }
                }
            }
            catch (Exception ex)
            {
                return (false, ex.Message);
            }
            return (false, "执行失败");
        }

        /// <summary>
        /// 启动作业
        /// </summary>
        /// <param name="id">ID</param>
        /// <returns></returns>
        public async Task<(bool, string)> Start(int id)
        {
            try
            {
                var allJob = await GetJobs();
                var getJob = allJob.Where(w => w.ID == id);
                if (getJob.Any())
                {
                    var taskOptions = getJob.First();
                    taskOptions.Status = (int)JobState.启用;
                    var isjob = await IsQuartzJob(taskOptions.TaskName, taskOptions.GroupName);
                    var scheduler = await _ISchedulerFactory.GetScheduler();
                    if (!isjob.Item1) //如果不存在则加入
                    {
                        var job = JobBuilder.Create<QuartzJobHttp>()
                            .WithIdentity(taskOptions.TaskName, taskOptions.GroupName)
                            .Build();
                        var trigger = TriggerBuilder.Create()
                           .WithIdentity(taskOptions.TaskName, taskOptions.GroupName)
                           .WithDescription(taskOptions.Describe)
                           .WithCronSchedule(taskOptions.Interval)
                           .Build();
                        if (_IJobFactory != null)
                        {
                            scheduler.JobFactory = _IJobFactory;
                        }
                        await scheduler.ScheduleJob(job, trigger);
                        await scheduler.Start();
                    }
                    else //存在则直接启动
                    {
                        var jobKeys = scheduler.GetJobKeys(GroupMatcher<JobKey>.GroupEquals(taskOptions.GroupName)).Result.ToList();
                        var jobKey = jobKeys.Where(s => scheduler.GetTriggersOfJob(s).Result.Any(x => (x as CronTriggerImpl).Name == taskOptions.TaskName)).FirstOrDefault();
                        var triggers = await scheduler.GetTriggersOfJob(jobKey);
                        var trigger = triggers?.Where(x => (x as CronTriggerImpl).Name == taskOptions.TaskName).FirstOrDefault();
                        await scheduler.ResumeTrigger(trigger.Key);
                    }
                    var date = _QuartzConfig.Update(taskOptions);
                    return date;
                }
            }
            catch (Exception ex)
            {
                return (false, ex.Message);
            }
            return (false, "启动失败");
        }

        /// <summary>
        /// 停用作业
        /// </summary>
        /// <param name="id">ID</param>
        /// <returns></returns>
        public async Task<(bool, string)> Pause(int id)
        {
            try
            {
                var allJob = await GetJobs();
                var getJob = allJob.Where(w => w.ID == id);
                if (getJob.Any())
                {
                    var taskOptions = getJob.First();
                    var isjob = await IsQuartzJob(taskOptions.TaskName, taskOptions.GroupName);
                    if (isjob.Item1)
                    {
                        var scheduler = await _ISchedulerFactory.GetScheduler();
                        var jobKeys = scheduler.GetJobKeys(GroupMatcher<JobKey>.GroupEquals(taskOptions.GroupName)).Result.ToList();
                        var jobKey = jobKeys.Where(s => scheduler.GetTriggersOfJob(s).Result.Any(x => (x as CronTriggerImpl).Name == taskOptions.TaskName)).FirstOrDefault();
                        var triggers = await scheduler.GetTriggersOfJob(jobKey);
                        var trigger = triggers?.Where(x => (x as CronTriggerImpl).Name == taskOptions.TaskName).FirstOrDefault();
                        await scheduler.PauseTrigger(trigger.Key);
                        isjob.Item2 += "Quartz已停用";
                    }
                    if (taskOptions != null)
                    {
                        taskOptions.Status = (int)JobState.停用;
                        var date = _QuartzConfig.Update(taskOptions);
                        isjob.Item1 = date.Item1;
                        isjob.Item2 += date.Item2;
                    }

                    return isjob;
                }
            }
            catch (Exception ex)
            {
                return (false, ex.Message);
            }
            return (false, "停用失败");
        }

        /// <summary>
        /// 判断是否存在此任务
        /// </summary>
        /// <param name="taskName"></param>
        /// <param name="groupName"></param>
        /// <returns></returns>
        public async Task<(bool, string)> IsQuartzJob(string taskName, string groupName)
        {
            try
            {
                var errorMsg = "";
                var scheduler = await _ISchedulerFactory.GetScheduler();
                var jobKeys = (await scheduler.GetJobKeys(GroupMatcher<JobKey>.GroupEquals(groupName))).ToList();
                if (jobKeys == null || jobKeys.Count() == 0)
                {
                    errorMsg = $"未找到分组[{groupName}]";
                    return (false, errorMsg);
                }
                var jobKey = jobKeys.Where(s => scheduler.GetTriggersOfJob(s).Result.Any(x => (x as CronTriggerImpl).Name == taskName)).FirstOrDefault();
                if (jobKey == null)
                {
                    errorMsg = $"未找到任务{taskName}]";
                    return (false, errorMsg);
                }
                var triggers = await scheduler.GetTriggersOfJob(jobKey);
                var trigger = triggers?.Where(x => (x as CronTriggerImpl).Name == taskName).FirstOrDefault();

                if (trigger == null)
                {
                    errorMsg = $"未找到触发器[{taskName}]";
                    return (false, errorMsg);
                }

                return (true, errorMsg);
            }
            catch (Exception ex)
            {
                return (false, ex.Message);
            }
        }

        /// <summary>
        /// 检查表达式
        /// </summary>
        private (bool, string) IsValidExpression(string cronExpression)
        {
            try
            {
                var trigger = new CronTriggerImpl();
                trigger.CronExpressionString = cronExpression;
                var date = trigger.ComputeFirstFireTimeUtc(null);
                var iscron = date != null;
                return (iscron, date == null ? $"请确认表达式{cronExpression}是否正确!" : "");
            }
            catch (Exception ex)
            {
                var mag = ex.Message;
                return (false, $"请确认表达式{cronExpression}是否正确");
            }
        }
    }
}
