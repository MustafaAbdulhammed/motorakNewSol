using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace motorak.bll.Services.Abstractions
{
    public interface IBackgroundService
    {

        Task ProcessCarImageOptimization(int carId);
        Task CheckCarAvailabilityStatus();
        Task UpdateCarPricing();
        Task CleanupExpiredCarListings();

        Task ProcessRentalReturns();
        Task SendRentalReminders();
        Task CheckOverdueRentals();

        Task SendWelcomeEmail(int customerId);
        Task ProcessInactiveCustomers();
        Task GenerateCustomerReports();

        Task ScheduleCarMaintenance();
        Task NotifyMechanicsOfUpcomingJobs();
        Task ProcessMaintenanceReminders();

        Task CleanupOldLogs();
        Task GenerateDailyReports();
        Task BackupDatabase();
        Task SendAdminNotifications();

        Task SendEmailNotification(string to, string subject, string body);
        Task SendBulkEmails(List<string> recipients, string subject, string body);
    }
}
