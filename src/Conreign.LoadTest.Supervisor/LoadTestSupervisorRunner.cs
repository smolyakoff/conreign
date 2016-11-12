using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using Conreign.LoadTest.Supervisor.Utility;
using Microsoft.Azure.Batch;
using Microsoft.Azure.Batch.Auth;
using Microsoft.Azure.Batch.Common;
using Serilog;

namespace Conreign.LoadTest.Supervisor
{
    public static class LoadTestSupervisorRunner
    {
        public static Task Run(string[] args)
        {
            var options = LoadTestSupervisorOptions.Parse(args);
            return Run(options);
        }

        public static async Task Run(LoadTestSupervisorOptions options)
        {
            if (options == null)
            {
                throw new ArgumentNullException(nameof(options));
            }
            ServicePointManager.DefaultConnectionLimit = options.Instances * 4;
            Log.Logger = new LoggerConfiguration()
                .WriteTo.LiterateConsole()
                .Enrich.FromLogContext()
                .CreateLogger();
            var logger = Log.Logger;
            try
            {
                logger.Information("Initialized with the following configuration: {@Configuration}", options);
                var creds = new BatchSharedKeyCredentials(options.BatchUrl, options.BatchAccountName,
                    options.BatchAccountKey);
                using (var client = await BatchClient.OpenAsync(creds))
                {
                    logger.Information("Connected to Azure Batch service.");
                    if (!options.UseAutoPool)
                    {
                        await client.GetOrCreateLoadTestPool(options);
                    }
                    var job = client.CreateLoadTestJob(options);
                    await job.CommitAsync();
                    logger.Information("Created job: {JobId}.", job.Id);
                    var tasks = Enumerable.Range(0, options.Instances)
                        .Select(options.CreateLoadTestTask)
                        .ToList();
                    await client.JobOperations.AddTaskAsync(job.Id, tasks);
                    logger.Information("Added {TasksCount} tasks.", tasks.Count);
                    job = client.JobOperations.GetJob(job.Id, new ODATADetailLevel(selectClause: "id"));
                    job.OnAllTasksComplete = OnAllTasksComplete.TerminateJob;
                    await job.CommitChangesAsync();
                    var monitor = client.Utilities.CreateTaskStateMonitor();
                    tasks = await client.JobOperations.ListTasks(job.Id, new ODATADetailLevel(selectClause: "id")).ToListAsync();
                    var cts = new CancellationTokenSource();
                    cts.CancelAfter(options.Timeout);
                    await monitor.WhenAll(tasks, TaskState.Running, cts.Token);
                    logger.Information("All tasks are {TaskState}.", TaskState.Running);
                    var watchCts = new CancellationTokenSource();
                    WatchStatistics(logger, tasks, options.TaskStatisticsFetchInterval, watchCts.Token);
                    await monitor.WhenAll(tasks, TaskState.Completed, cts.Token);
                    watchCts.Cancel();
                    logger.Information("All tasks are {TaskState}.", TaskState.Completed);
                    // ReSharper disable MethodSupportsCancellation
                    var downloadTasks = new List<Task>();
                    var jobOutputDirectory = Path.Combine(options.OutputDirectory, job.Id);
                    if (!Directory.Exists(jobOutputDirectory))
                    {
                        Directory.CreateDirectory(jobOutputDirectory);
                    }
                    logger.Information("Created job output directory: {JobDirectoryPath}.", jobOutputDirectory);
                    foreach (var task in tasks)
                    {
                        await task.RefreshAsync(new ODATADetailLevel(selectClause: "id,executionInfo"));
                        if (task.ExecutionInformation.SchedulingError != null)
                        {
                            logger.Error("Task {TaskId} completed with scheduling error: {@SchedulingError}.", task.Id, task.ExecutionInformation.SchedulingError);
                            continue;
                        }
                        if (task.ExecutionInformation.ExitCode != 0)
                        {
                            logger.Error("Task {TaskId} exited with non-zero code.", task.Id);
                        }
                        var files = await task.ListNodeFiles(false).ToListAsync();
                        var logFile = files.FirstOrDefault(x => x.Name == options.InstanceOptions.LogFileName);
                        if (logFile == null)
                        {
                            continue;
                        }
                        var downloadTask = Task.Run((async () =>
                        {
                            var filePath = Path.Combine(jobOutputDirectory, $"{task.Id}-log.json");
                            using (var fs = new FileStream(filePath, FileMode.Create))
                            {
                                await logFile.CopyToStreamAsync(fs);
                            }
                            logger.Information("Downloaded log file from task [{TaskId}] to {TaskLogFilePath}.", task.Id, filePath);
                        }));
                        downloadTasks.Add(downloadTask);
                    }
                    await Task.WhenAll(downloadTasks);
                    logger.Information("Load test complete.");
                }
            }
            catch (Exception ex)
            {
                logger.Fatal(ex, "Load test failed: {ErrorMessage}", ex.Message);
                throw;
            }
            finally
            {
                Log.CloseAndFlush();
            }
        }

        private static async void WatchStatistics(ILogger logger, List<CloudTask> tasks, TimeSpan interval, CancellationToken cancellationToken)
        {
            await Task.Delay(TimeSpan.FromSeconds(1));
            while (!cancellationToken.IsCancellationRequested)
            {
                try
                {
                    foreach (var task in tasks)
                    {
                        await task.RefreshAsync(new ODATADetailLevel(selectClause: "id,stats"), null, cancellationToken);
                        logger.Information("Task {TaskId} statistics: {@Statistics}", task.Id, task.Statistics);
                    }
                    await Task.Delay(interval, cancellationToken);
                }
                catch (TaskCanceledException)
                {
                    // Do nothing
                }
            }
        }
    }
}