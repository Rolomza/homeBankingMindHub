using HomeBankingMindHub.Models;
using HomeBankingMindHub.Models.DTOs;
using System.Security.Claims;

namespace HomeBankingMindHub.Services
{
    public interface IAuthService
    {
        ClaimsIdentity AuthenticateUser(Client user);
    }
}
