using Application.Interfaces;
using Domain.Entities;
using Microsoft.Extensions.DependencyInjection;
using Quartz;


namespace Infrastructure.Jobs
{
    [DisallowConcurrentExecution]
    public class OverdueCheckJob(IServiceScopeFactory scopeFactory) : IJob
    {
        public async Task Execute(IJobExecutionContext context)
        {
            using var scope = scopeFactory.CreateScope();
            var unitOfWork = scope.ServiceProvider.GetRequiredService<IUnitOfWork>();

            // Find records that are past due but still marked as 'Borrowed'
            var overdueRecords = await unitOfWork.Borrows.GetActiveOverdueRecordsAsync();

            if (!overdueRecords.Any()) return;

            foreach (var record in overdueRecords)
            {
                record.Status = BorrowStatus.Overdue;
                await unitOfWork.Borrows.UpdateAsync(record);
            }

            // Persist changes
            await unitOfWork.CommitAsync();
        }
    }
}
