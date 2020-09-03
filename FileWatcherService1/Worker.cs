using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

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
            while (!stoppingToken.IsCancellationRequested)
            {
                FileSystemWatcher fileWatcher = new FileSystemWatcher(@"C:\WorkerServiceExample");

                //Enable events
                fileWatcher.EnableRaisingEvents = true;

                //Add event watcher
                fileWatcher.Changed += FileWatcher_Changed;
                fileWatcher.Created += FileWatcher_Changed;
                fileWatcher.Deleted += FileWatcher_Changed;
                fileWatcher.Renamed += FileWatcher_Changed;

                var maxThreads = 4;

                // Times to as most machines have double the logic processers as cores
                ThreadPool.SetMaxThreads(maxThreads, maxThreads * 2);

                //_logger.LogInformation("Worker running at: {time}", DateTimeOffset.Now);
                // await Task.Delay(1000, stoppingToken);
            }
        }
        private static void FileWatcher_Changed(object sender, FileSystemEventArgs e)
        {
            ThreadPool.QueueUserWorkItem((o) => ProcessFile(e));
        }

        //This method processes your file, you can do your sync here
        private static void ProcessFile(FileSystemEventArgs e)
        {
            // Based on the eventtype you do your operation
            switch (e.ChangeType)
            {
                case WatcherChangeTypes.Changed:
                    Console.WriteLine($"File is changed: {e.Name}");
                    Log.Information($"File is changed: {e.Name}");
                    break;
                case WatcherChangeTypes.Created:
                    Console.WriteLine($"File is created: {e.Name}");
                    Log.Information($"File is created: {e.Name}");
                    break;
                case WatcherChangeTypes.Deleted:
                    Console.WriteLine($"File is deleted: {e.Name}");
                    Log.Information($"File is deleted: {e.Name}");
                    break;
                case WatcherChangeTypes.Renamed:
                    Console.WriteLine($"File is renamed: {e.Name}");
                    Log.Information($"File is renamed: {e.Name}");
                    break;
            }
        }
    }
}
