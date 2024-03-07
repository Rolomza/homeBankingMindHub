using HomeBankingMindHub.Models.DTOs;

namespace HomeBankingMindHub.Services
{
    public interface IClientService
    {
        ClientDTO getClientDTOByEmail(string email);
    }
}
