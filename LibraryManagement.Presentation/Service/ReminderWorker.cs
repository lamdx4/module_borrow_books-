using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.DependencyInjection;
using LibraryManagement.Application.Interfaces;

namespace LibraryManagement.Presentation.Service;

public class ReminderWorker : BackgroundService
{
    private readonly ILogger<ReminderWorker> _logger;
    private readonly IServiceProvider _serviceProvider;

    public ReminderWorker(ILogger<ReminderWorker> logger, IServiceProvider serviceProvider)
    {
        _logger = logger;
        _serviceProvider = serviceProvider;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        _logger.LogInformation("Reminder Worker started.");

        while (!stoppingToken.IsCancellationRequested)
        {
            _logger.LogInformation("Reminder Worker running at: {time}", DateTimeOffset.Now);

            try
            {
                // Create a scope to resolve scoped services like repositories if needed 
                // (though in this case they might be singletons, scope is standard practice)
                using var scope = _serviceProvider.CreateScope();
                var giaoDichRepo = scope.ServiceProvider.GetRequiredService<IGiaoDichMuonTraRepository>();
                var docGiaRepo = scope.ServiceProvider.GetRequiredService<IDocGiaRepository>();
                var cuonSachRepo = scope.ServiceProvider.GetRequiredService<ICuonSachRepository>();

                var activeTransactions = await giaoDichRepo.GetAllActiveTransactionsAsync();

                foreach (var trans in activeTransactions)
                {
                    var daysUntilDue = (trans.NgayDenHan - DateTime.Now).TotalDays;

                    // Send near due notification (1 to 2 days)
                    if (daysUntilDue > 0 && daysUntilDue <= 2)
                    {
                        var docGia = await docGiaRepo.GetByMaTheAsync(trans.MaThe);
                        var cuonSach = await cuonSachRepo.GetByMaVachAsync(trans.MaVachRFID);
                        if (docGia != null && cuonSach != null)
                        {
                            _logger.LogInformation($"[NOTIFICATION-NEAR-DUE] To: {docGia.Email} - Sách '{cuonSach.DauSach?.TenSach ?? cuonSach.ISBN}' sắp đến hạn trả vào ngày {trans.NgayDenHan:dd/MM/yyyy}. Vui lòng trả đúng hạn.");
                        }
                    }
                    // Send overdue notification
                    else if (daysUntilDue < 0)
                    {
                        var docGia = await docGiaRepo.GetByMaTheAsync(trans.MaThe);
                        var cuonSach = await cuonSachRepo.GetByMaVachAsync(trans.MaVachRFID);
                        if (docGia != null && cuonSach != null)
                        {
                            int overdueDays = (int)Math.Abs(daysUntilDue);
                            _logger.LogWarning($"[NOTIFICATION-OVERDUE] To: {docGia.Email} - Sách '{cuonSach.DauSach?.TenSach ?? cuonSach.ISBN}' đã quá hạn trả {overdueDays} ngày. Vui lòng trả sớm để tránh bị phạt.");
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred executing Reminder Worker.");
            }

            // Simulate running every 24 hours (for testing, we can set this lower, e.g., 1 minute)
            // _logger.LogInformation("Reminder Worker sleeping for 24 hours.");
            // await Task.Delay(TimeSpan.FromHours(24), stoppingToken);
            
            _logger.LogInformation("Reminder Worker sleeping for 1 minute (for testing).");
            await Task.Delay(TimeSpan.FromMinutes(1), stoppingToken);
        }
    }
}
