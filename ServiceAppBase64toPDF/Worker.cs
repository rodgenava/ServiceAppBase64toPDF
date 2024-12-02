using ServiceAppBase64toPDF.Application;

namespace ServiceAppBase64toPDF
{
    public class Worker : BackgroundService
    {
        private readonly ILogger<Worker> _logger;

        private readonly IServiceScopeFactory _scopeFactory;
        private readonly IScannedSIService _scannedSIService;
        public Worker(ILogger<Worker> logger, IServiceScopeFactory scopeFactory, IScannedSIService scannedSIService)
        {
            _logger = logger;
            _scopeFactory = scopeFactory;
            _scannedSIService = scannedSIService;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                bool idone = await _scannedSIService.GetScannedSIList();
                _logger.LogInformation("Worker running at: {time}", DateTimeOffset.Now);
                await Task.Delay(1000, stoppingToken);
            }
        }
    }
}
