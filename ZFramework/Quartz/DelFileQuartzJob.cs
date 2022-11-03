using Quartz;
using zgcwkj.Util;

namespace ZFramework.Quartz
{
    /// <summary>
    /// 删除文件服务
    /// </summary>
    public class DelFileQuartzJob : IJob
    {
        /// <summary>
        /// 执行事件
        /// </summary>
        /// <param name="context"></param>
        /// <returns></returns>
        public Task Execute(IJobExecutionContext context)
        {
            return Task.Factory.StartNew(() =>
            {
                var filePaths = ConfigHelp.Get<string[]>("DelFileServiceConfig:FilePaths");
                foreach (var filePath in filePaths)
                {
                    if (filePath.IsNull()) continue;
                    var fpath = Path.Combine(Directory.GetCurrentDirectory(), filePath);
                    DeleteFolder(fpath);
                }
            });
        }

        /// <summary>
        /// 递归删除文件夹目录及文件
        /// </summary>
        /// <param name="dir"></param>
        /// <returns></returns>
        public static void DeleteFolder(string dir)
        {
            //如果存在这个文件夹删除之
            if (Directory.Exists(dir))
            {
                var files = Directory.GetFileSystemEntries(dir);
                foreach (string d in files)
                {
                    if (File.Exists(d))
                    {
                        //直接删除其中的文件
                        File.Delete(d);
                    }
                    else
                    {
                        //递归删除子文件夹
                        DeleteFolder(d);
                    }
                }
                //删除已空文件夹
                Directory.Delete(dir, true);
            }
        }
    }
}
