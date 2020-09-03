using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Serilog;

namespace FileWatcherService1
{
    public class Worker : BackgroundService
    {
        private readonly ILogger<Worker> _logger;

        public Worker(ILogger<Worker> logger)
        {
            _logger = logger;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            var folderPath = ConfigurationManager.AppSettings["folderPath"];
            FileSystemWatcher fileWatcher = new FileSystemWatcher(folderPath);

            fileWatcher.EnableRaisingEvents = true;

            fileWatcher.Changed += FileWatcher_Changed;
            fileWatcher.Created += FileWatcher_Changed;
            fileWatcher.Deleted += FileWatcher_Changed;
            fileWatcher.Renamed += FileWatcher_Changed;

            var maxThreads = 4;

            ThreadPool.SetMaxThreads(maxThreads, maxThreads * 2);
        }
        private static void FileWatcher_Changed(object sender, FileSystemEventArgs e)
        {
            ThreadPool.QueueUserWorkItem((o) => ProcessFile(e));
        }

        private static void ProcessFile(FileSystemEventArgs e)
        {
            switch (e.ChangeType)
            {
                case WatcherChangeTypes.Changed:
                    Log.Information($"status is changed: {e.Name} in path {e.FullPath}");
                    break;
                case WatcherChangeTypes.Created:
                    Log.Information($"status is created: {e.Name} in path {e.FullPath}");
                    break;
                case WatcherChangeTypes.Deleted:
                    Log.Information($"status is deleted: {e.Name} in path {e.FullPath}");
                    break;
                case WatcherChangeTypes.Renamed:
                    Log.Information($"status is renamed: {e.Name} in path {e.FullPath}");
                    break;
            }
        }
    }
}
