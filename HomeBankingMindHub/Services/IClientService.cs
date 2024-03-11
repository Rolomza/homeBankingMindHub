using HomeBankingMindHub.Models;
using HomeBankingMindHub.Models.DTOs;

namespace HomeBankingMindHub.Services
{
    public interface IClientService
    {
        Client GetClientByEmail(string email);
        Client GetClientById(long id);
        IEnumerable<Client> GetAllClients();
        IEnumerable<ClientDTO> GetAllClientsDTOs();
        ClientDTO GetClientDTOByEmail(string email);
        ClientDTO GetClientDTOById(long id);
    }
}
