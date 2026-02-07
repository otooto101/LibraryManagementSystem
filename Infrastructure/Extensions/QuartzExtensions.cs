using Infrastructure.Jobs;
using Microsoft.Extensions.DependencyInjection;
using Quartz;


namespace Infrastructure.Extensions
{
    public static class QuartzExtensions
    {
        public static IServiceCollection AddBackgroundJobs(this IServiceCollection services)
        {
            var jobKey = new JobKey("OverdueCheck", "Maintenance");

            services.AddQuartz(q =>
            {
                q.AddJob<OverdueCheckJob>(opts => opts.WithIdentity(jobKey));

                q.AddTrigger(opts => opts
                    .ForJob(jobKey)
                    .WithIdentity("MidnightTrigger", "Maintenance") 
                    .WithCronSchedule("0 0 0 * * ?")); // runs everyday 12 pm 
            });

            services.AddQuartzHostedService(q => q.WaitForJobsToComplete = true);

            return services;
        }
    }
}
