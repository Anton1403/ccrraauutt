using crraut.Services;
using crraut.SignalR;
using Microsoft.AspNetCore.SignalR;
using Quartz;

namespace crraut.Quartz
{
    public class SyncJob : IJob
    {
        private readonly IServiceScopeFactory serviceScopeFactory;
        private readonly IHubContext<QuartzHub> hubContext;
        private readonly ErrorNotifier _notifier;

        private readonly int[] _pageSizes = { 1, 2, 3, 5, 10, 15, 20 };
        private static int _currentIndex = 0; // Текущий индекс в массиве pageSizes
        private static bool _sendWith48 = false; // Флаг, указывающий, что надо отправлять с 48
        private readonly int _categoryId = 48;

        public SyncJob(IServiceScopeFactory serviceScopeFactory, IHubContext<QuartzHub> hubContext, ErrorNotifier notifier) {
            this.serviceScopeFactory = serviceScopeFactory;
            this.hubContext = hubContext;
            _notifier = notifier;
        }

        public async Task Execute(IJobExecutionContext context) {
            try {
                using var scope = serviceScopeFactory.CreateScope();

                int currentPageSize = _pageSizes[_currentIndex];

                if(_sendWith48) {
                    await scope.ServiceProvider.GetService<SynchronizeService>().SynchronizeBinance(currentPageSize, _categoryId);
                    NextIteration(false);
                }
                else {
                    await scope.ServiceProvider.GetService<SynchronizeService>().SynchronizeBinance(currentPageSize, null);
                    NextIteration(true);
                }


                //await hubContext.Clients.All.SendAsync("ReceiveLastRunTime", DateTime.UtcNow);
            }
            catch(Exception e) {
                _notifier.Notify(e);
            }
        }

        private void NextIteration(bool sendWith48) {
            if(_currentIndex == _pageSizes.Length - 1) {
                _sendWith48 = sendWith48;
                _currentIndex = 0;
            }
            else {
                _currentIndex++;
            }
        }
    }
}
