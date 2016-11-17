using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Humanizer;
using Microsoft.Azure.Batch;
using Microsoft.Azure.Batch.Common;
using Serilog;
using Serilog.Events;

namespace Conreign.LoadTest.Supervisor.Utility
{
    public static class AzureBatchExtensions
    {
        private static readonly ILogger Logger = Log.Logger.ForContext(typeof(AzureBatchExtensions));

        private const string ApplicationId = "conreign-loadtest";
        private const string WindowsServer = "4";
        private const int MaxTasksPerNode = 2;

        public static CloudJob CreateLoadTestJob(this BatchClient client, LoadTestSupervisorOptions options)
        {
            if (client == null)
            {
                throw new ArgumentNullException(nameof(client));
            }
            if (options == null)
            {
                throw new ArgumentNullException(nameof(options));
            }
            var jobId = $"loadtest-{options.Name}";
            var poolInfo = new PoolInformation();
            if (options.UseAutoPool)
            {
                var poolSpecification = CreatePoolSpecification(options, jobId);
                poolInfo.AutoPoolSpecification = new AutoPoolSpecification
                {
                    AutoPoolIdPrefix = "loadtest",
                    PoolLifetimeOption = PoolLifetimeOption.Job,
                    PoolSpecification = poolSpecification
                };
            }
            else
            {
                poolInfo.PoolId = options.PoolId;
            }

            var job = client.JobOperations.CreateJob(jobId, poolInfo);
            job.DisplayName = $"Run load test ({jobId})";
            job.Constraints = new JobConstraints(maxWallClockTime: options.JobTimeout);
            return job;
        }

        public static async Task<CloudPool> GetOrCreateLoadTestPool(this BatchClient client, LoadTestSupervisorOptions options)
        {
            var spec = CreatePoolSpecification(options);
            CloudPool pool;
            try
            {
                pool = await client.PoolOperations.GetPoolAsync(options.PoolId);
                pool.ApplicationPackageReferences = spec.ApplicationPackageReferences;
                await pool.CommitChangesAsync();
            }
            catch (BatchException ex)
            {
                if (ex.RequestInformation.BatchError.Code != "PoolNotFound")
                {
                    throw;
                }
                Logger.Information("Existing pool {PoolId} was not found. Creating new pool.", options.PoolId);
                pool = client.PoolOperations.CreatePool(options.PoolId,
                    spec.VirtualMachineSize,
                    spec.CloudServiceConfiguration,
                    spec.TargetDedicated);
                pool.MaxTasksPerComputeNode = MaxTasksPerNode;
                pool.ApplicationPackageReferences = spec.ApplicationPackageReferences;
                await pool.CommitAsync();
            }
            if (pool.TargetDedicated == spec.TargetDedicated)
            {
                return pool;
            }
            Logger.Information("Resizing pool to {NodesCount} nodes.", spec.TargetDedicated);
            // ReSharper disable once PossibleInvalidOperationException
            await client.PoolOperations.ResizePoolAsync(pool.Id, spec.TargetDedicated.Value);
            return pool;
        }

        public static CloudTask CreateLoadTestTask(this LoadTestSupervisorOptions options, int index)
        {
            var maxExecutionTime = options.InstanceOptions.Timeout + 10.Minutes();
            var taskId = $"loadtest__{options.Name}__{index}";
            var instanceOptions = options.InstanceOptions;
            var builder = new StringBuilder();
            builder.Append($"cmd /c %AZ_BATCH_APP_PACKAGE_CONREIGN-LOADTEST#{options.ApplicationVersion}%\\Conreign.LoadTest.exe");
            var arguments = new List<CommandLineArgument>
            {
                new CommandLineArgument(nameof(instanceOptions.InstanceId), taskId),
                new CommandLineArgument(nameof(instanceOptions.ConnectionUri), instanceOptions.ConnectionUri),
                new CommandLineArgument(nameof(instanceOptions.ElasticSearchUri), instanceOptions.ElasticSearchUri),
                new CommandLineArgument(
                    $"{nameof(instanceOptions.BotOptions)}:{nameof(instanceOptions.BotOptions.RoomsCount)}",
                    instanceOptions.BotOptions.RoomsCount),
                new CommandLineArgument(
                    $"{nameof(instanceOptions.BotOptions)}:{nameof(instanceOptions.BotOptions.BotsPerRoomCount)}",
                    instanceOptions.BotOptions.BotsPerRoomCount),
                new CommandLineArgument(
                    $"{nameof(instanceOptions.BotOptions)}:{nameof(instanceOptions.BotOptions.RoomPrefix)}",
                    $"{taskId}:"),
                new CommandLineArgument(nameof(instanceOptions.MinimumConsoleLogLevel), LogEventLevel.Warning),
                new CommandLineArgument(nameof(instanceOptions.LogFileName), Path.Combine("%AZ_BATCH_TASK_DIR%", "log.json")),
                new CommandLineArgument(nameof(instanceOptions.Timeout), instanceOptions.Timeout)
            };
            var args = arguments.Where(x => x.Value != null).ToList();
            builder.Append($" {string.Join(" ", args)}");
            var cmd = builder.ToString();
            var task = new CloudTask(taskId, cmd);
            var app = new ApplicationPackageReference
            {
                ApplicationId = ApplicationId,
                Version = options.ApplicationVersion
            };
            task.Constraints = new TaskConstraints(maxWallClockTime: maxExecutionTime);
            task.ApplicationPackageReferences = new List<ApplicationPackageReference> {app};
            task.DisplayName = $"Execute load test ({taskId})";
            return task;
        }

        public static async Task<string> GetLatestApplicationVersion(this BatchClient client)
        {
            var summary = await client.ApplicationOperations.GetApplicationSummaryAsync(ApplicationId);
            return summary.Versions
                .Select(v => new Version(v))
                .OrderByDescending(x => x)
                .FirstOrDefault()?.ToString();
        }

        private static PoolSpecification CreatePoolSpecification(LoadTestSupervisorOptions options, string jobId = null)
        {
            var applicationPackage = new ApplicationPackageReference
            {
                ApplicationId = ApplicationId,
                Version = options.ApplicationVersion
            };
            return new PoolSpecification
            {
                TargetDedicated = (int)Math.Ceiling((double)options.Instances / MaxTasksPerNode),
                MaxTasksPerComputeNode = MaxTasksPerNode,
                DisplayName = jobId == null ? "Load test pool" : $"Load test pool ({jobId})",
                ApplicationPackageReferences = new List<ApplicationPackageReference> { applicationPackage },
                CloudServiceConfiguration = new CloudServiceConfiguration(osFamily: WindowsServer),
                VirtualMachineSize = options.MachineSize
            };
        }
    }


}