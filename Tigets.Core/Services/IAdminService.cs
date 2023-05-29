using Tigets.Core.Models;

namespace Tigets.Core.Services
{
    public interface IAdminService
    {
        Task Login(string username, string password);
        Task<Admin> GetProfileData(string username);
        Task Logout();
        string GetAppInfo();
    }
}