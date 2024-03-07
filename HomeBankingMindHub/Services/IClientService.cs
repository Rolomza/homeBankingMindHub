using HomeBankingMindHub.Models;
using HomeBankingMindHub.Models.DTOs;

namespace HomeBankingMindHub.Services
{
    public interface IClientService
    {
        Client getClientByEmail(string email);
        ClientDTO getClientDTOByEmail(string email);
    }
}
