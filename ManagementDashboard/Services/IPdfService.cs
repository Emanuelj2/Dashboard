using ManagementDashboard.Models;

namespace ManagementDashboard.Services
{
    public interface IPdfService
    {
        byte[] GenerateUserPdfReport(List<User> users);
    }
}
