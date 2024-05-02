using System.Linq.Expressions;

namespace ZFramework.Quartz
{
    /// <summary>
    /// Quartz 配置
    /// </summary>
    public class QuartzConfig
    {
        private IWebHostEnvironment _IWebHost { get; }

        private IConfiguration _IConfig { get; }

        /// <summary>
        /// RootPath
        /// </summary>
        public string RootPath
        {
            get
            {
                var quartzConfig = "QuartzConfig";
                var rootPath = $"{Directory.GetParent(_IWebHost.ContentRootPath).FullName}/{quartzConfig}/";
                //创建文件夹
                if (!Directory.Exists(rootPath)) Directory.CreateDirectory(rootPath);
                //
                return rootPath;
            }
        }

        /// <summary>
        /// LogPath
        /// </summary>
        public string LogPath
        {
            get
            {
                var logs = "logs";
                var logPath = $"{RootPath}{logs}/";
                //创建文件夹
                if (!Directory.Exists(logPath)) Directory.CreateDirectory(logPath);
                //
                return logPath;
            }
        }

        /// <summary>
        /// TaskJobFileName
        /// </summary>
        public string TaskJobFileName { get; set; } = "TaskJob.json";

        /// <summary>
        /// Quartz 配置
        /// </summary>
        public QuartzConfig(IWebHostEnvironment webHost, IConfiguration config)
        {
            _IWebHost = webHost;
            _IConfig = config;
        }

        #region 任务

        /// <summary>
        /// 获取所有作业
        /// </summary>
        /// <returns></returns>
        public List<QuarzTask> GetJobs(Expression<Func<QuarzTask, bool>> where)
        {
            var list = this.GetAllJobs(where);
            return list;
        }

        /// <summary>
        /// 添加任务
        /// </summary>
        /// <param name="model">任务</param>
        /// <returns></returns>
        public (bool, string) AddJob(QuarzTask model)
        {
            var list = this.GetJobs(a => 1 == 1);
            if (list == null)
            {
                list = new List<QuarzTask>();
            }
            if (list.Count == 0)
            {
                model.ID = 1;
            }
            else
            {
                model.ID = list.Max(a => a.ID) + 1;
            }
            list.Add(model);
            this.WriteJobConfig(list);
            return (true, "添加任务成功");
        }

        /// <summary>
        /// 删除任务
        /// </summary>
        /// <param name="model">任务</param>
        /// <returns></returns>
        public (bool, string) Remove(QuarzTask model)
        {
            var list = this.GetJobs(a => 1 == 1);
            list.Remove(list.Find(a => a.TaskName == model.TaskName && a.GroupName == model.GroupName));
            this.WriteJobConfig(list);
            return (true, "删除任务成功");
        }

        /// <summary>
        /// 更新任务
        /// </summary>
        /// <param name="model">任务</param>
        /// <returns></returns>
        public (bool, string) Update(QuarzTask model)
        {
            var list = this.GetJobs(a => 1 == 1);
            list.Remove(list.Find(a => a.ID == model.ID));
            list.Add(model);
            this.WriteJobConfig(list);
            return (true, "更新任务成功");
        }

        /// <summary>
        /// 暂停任务
        /// </summary>
        /// <param name="model">任务</param>
        /// <returns></returns>
        public (bool, string) Pause(QuarzTask model)
        {
            var list = this.GetJobs(a => 1 == 1);
            list.ForEach(f =>
            {
                if (f.TaskName == model.TaskName && f.GroupName == model.GroupName)
                {
                    f.Status = Convert.ToInt32(JobState.停用);
                }
            });
            this.WriteJobConfig(list);
            return (true, "任务停用成功");
        }

        /// <summary>
        /// 启用任务
        /// </summary>
        /// <param name="model">任务</param>
        /// <returns></returns>
        public (bool, string) Start(QuarzTask model)
        {
            var list = this.GetJobs(a => 1 == 1);
            list.ForEach(f =>
            {
                if (f.TaskName == model.TaskName && f.GroupName == model.GroupName)
                {
                    f.Status = Convert.ToInt32(JobState.启用);
                }
            });
            this.WriteJobConfig(list);
            return (true, "启用任务成功");
        }

        /// <summary>
        /// 获取所有任务
        /// </summary>
        /// <param name="where">条件</param>
        /// <returns></returns>
        private List<QuarzTask> GetAllJobs(Expression<Func<QuarzTask, bool>> where)
        {
            var path = $"{RootPath}/{TaskJobFileName}";
            var list = new List<QuarzTask>();
            //
            if (!File.Exists(path)) return list;
            //
            var tasks = ReadFile(path);
            if (string.IsNullOrEmpty(tasks)) return new();
            //
            var quarzTasks = tasks.ToJson<List<QuarzTask>>();
            var taskList = quarzTasks.Where(where.Compile());
            var toSort = taskList.OrderByDescending(O => (O.Status, O.ChangeTime));
            return toSort.ToList();
        }

        /// <summary>
        /// 写入任务(全量)
        /// </summary>
        /// <param name="taskList">任务列表对象</param>
        private void WriteJobConfig(List<QuarzTask> taskList)
        {
            var jobs = taskList.ToJson();
            //写入配置文件
            WriteFile(RootPath, TaskJobFileName, jobs);
        }

        #endregion 任务

        #region 日志

        /// <summary>
        /// 添加日志
        /// </summary>
        /// <param name="model">日志</param>
        /// <returns></returns>
        public (bool, string) AddLog(QuarzTaskLog model)
        {
            try
            {
                this.WriteJobLogs(model);
                return (true, "日志数据保存成功");
            }
            catch (Exception)
            {
                return (false, "日志数据保存失败");
            }
        }

        /// <summary>
        /// 获取日志
        /// </summary>
        /// <param name="taskName">任务名称</param>
        /// <param name="groupName">分组名称</param>
        /// <param name="page">页码</param>
        /// <param name="pageSize">页大小</param>
        /// <returns></returns>
        public (int, List<QuarzTaskLog>) GetLogs(string taskName, string groupName, int page = 1, int pageSize = 100)
        {
            var list = this.GetJobsLog(taskName, groupName);
            int total = list.Count;
            var logList = list.Skip((page - 1) * pageSize).Take(pageSize);
            var toSort = logList.OrderByDescending(O => (O.ChangeTime));
            return (total, toSort.ToList());
        }

        /// <summary>
        /// 获取最后日志
        /// </summary>
        /// <param name="taskName">任务名称</param>
        /// <param name="groupName">分组名称</param>
        /// <returns></returns>
        public QuarzTaskLog Getlastlog(string taskName, string groupName)
        {
            var list = this.GetJobsLog(taskName, groupName);
            var data = list.OrderByDescending(a => a.BeginDate).FirstOrDefault();
            return data;
        }

        /// <summary>
        /// 读日志
        /// </summary>
        /// <param name="taskName">任务名称</param>
        /// <param name="groupName">分组名称</param>
        /// <param name="page">页码</param>
        /// <returns></returns>
        public List<QuarzTaskLog> GetJobsLog(string taskName, string groupName, int page = 1)
        {
            var path = $"{LogPath}/{groupName}/{taskName}/logs.txt";
            if (!File.Exists(path)) return new();

            var listlogs = ReadPageLine(path, page, 5000, true).ToList();
            var listtasklogs = new List<QuarzTaskLog>();
            foreach (var item in listlogs)
            {
                listtasklogs.Add(item.ToJson<QuarzTaskLog>());
            }
            return listtasklogs;
        }

        /// <summary>
        /// 读取本地txt日志内容
        /// </summary>
        /// <param name="fullPath">文件路径</param>
        /// <param name="page">页码</param>
        /// <param name="pageSize">页大小</param>
        /// <param name="seekEnd">寻找到结尾</param>
        /// <returns></returns>
        private IEnumerable<string> ReadPageLine(string fullPath, int page, int pageSize, bool seekEnd = false)
        {
            if (page <= 0) page = 1;
            //
            var lines = File.ReadLines(fullPath, Encoding.UTF8);
            if (seekEnd)
            {
                var lineCount = lines.Count();
                var linPageCount = (int)Math.Ceiling(lineCount / (pageSize * 1.00));
                //超过总页数，不处理
                if (page > linPageCount)
                {
                    page = 0;
                    pageSize = 0;
                }
                //最后一页，取最后一页剩下所有的行
                else if (page == linPageCount)
                {
                    pageSize = lineCount - (page - 1) * pageSize;
                    if (page == 1)
                    {
                        page = 0;
                    }
                    else
                    {
                        page = lines.Count() - page * pageSize;
                    }
                }
                else
                {
                    page = lines.Count() - page * pageSize;
                }
            }
            else
            {
                page = (page - 1) * pageSize;
            }
            lines = lines.Skip(page).Take(pageSize);

            using var enumerator = lines.GetEnumerator();
            var count = 1;
            while (enumerator.MoveNext() || count <= pageSize)
            {
                yield return enumerator.Current;
                count++;
            }
        }

        /// <summary>
        /// 读取任务日志
        /// </summary>
        /// <param name="taskName">任务名称</param>
        /// <param name="groupName">分组名称</param>
        /// <param name="page">页码</param>
        /// <param name="pageSize">页大小</param>
        /// <returns></returns>
        private List<QuarzTaskLog> GetJobRunLog(string taskName, string groupName, int page, int pageSize = 100)
        {
            var path = $"{LogPath}{groupName}/{taskName}";
            var list = new List<QuarzTaskLog>();

            if (!File.Exists(path)) return list;
            var logs = ReadPageLine(path, page, pageSize, true);
            foreach (var item in logs)
            {
                var arr = item?.Split('_');
                if (item == "" || arr == null || arr.Length == 0)
                    continue;
                if (arr.Length != 3)
                {
                    list.Add(new QuarzTaskLog() { Msg = item });
                    continue;
                }
                list.Add(new QuarzTaskLog()
                {
                    BeginDate = Convert.ToDateTime(arr[0]),
                    EndDate = Convert.ToDateTime(arr[1]),
                    Msg = arr[2]
                });
            }
            return list.OrderByDescending(x => x.BeginDate).ToList();
        }

        /// <summary>
        /// 写日志
        /// </summary>
        /// <param name="quarzTaskLog">日志对象</param>
        private void WriteJobLogs(QuarzTaskLog quarzTaskLog)
        {
            var content = quarzTaskLog.ToJson() + "\r\n";
            //
            WriteFile($"{LogPath}/{quarzTaskLog.GroupName}/{quarzTaskLog.TaskName}/", $"logs.txt", content, true);
        }

        #endregion 日志

        #region 内部操作

        /// <summary>
        /// 读文件
        /// </summary>
        /// <param name="path">文件路径</param>
        /// <returns></returns>
        private string ReadFile(string path)
        {
            if (!File.Exists(path)) return "";
            using var stream = new StreamReader(path);
            //读取文件
            var data = stream.ReadToEnd();
            return data;
        }

        /// <summary>
        /// 写文件
        /// </summary>
        /// <param name="path">文件路径</param>
        /// <param name="fileName">文件名称</param>
        /// <param name="content">文件内容</param>
        /// <param name="appendToLast">appendToLast</param>
        private void WriteFile(string path, string fileName, string content, bool appendToLast = false)
        {
            if (!path.EndsWith("/")) path += "/";
            //如果不存在就创建文件夹
            if (!Directory.Exists(path)) Directory.CreateDirectory(path);
            //
            using var stream = File.Open(path + fileName, FileMode.OpenOrCreate, FileAccess.Write);
            var by = Encoding.Default.GetBytes(content);
            if (appendToLast)
            {
                stream.Position = stream.Length;
            }
            else
            {
                stream.SetLength(0);
            }
            stream.Write(by, 0, by.Length);
        }

        #endregion 内部操作
    }
}
