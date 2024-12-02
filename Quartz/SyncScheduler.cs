using Quartz;
using Quartz.Impl;

namespace crraut.Quartz
{
    public class SyncScheduler
    {
        public static async void Start(IServiceProvider serviceProvider) {
            var scheduler = await StdSchedulerFactory.GetDefaultScheduler();
            scheduler.JobFactory = serviceProvider.GetService<JobFactory>();
            await scheduler.Start();
            var now = DateTimeOffset.Now;
            var job = JobBuilder.Create<SyncJob>()
                .WithIdentity("SyncJob", "Synchronization")
                .Build();

            var trigger = TriggerBuilder.Create()
                .WithIdentity("SynchronizationTrigger", "Synchronization")
                .WithCronSchedule("0/2 * * * * ?")
                .Build();

            await scheduler.ScheduleJob(job, trigger);
        }
    }
}
