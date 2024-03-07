using HomeBankingMindHub.Models;
using HomeBankingMindHub.Models.DTOs;
using HomeBankingMindHub.Repositories;

namespace HomeBankingMindHub.Services.Impl
{
    public class ClientService : IClientService
    {
        private readonly IClientRepository _clientRepository;

        public ClientService(IClientRepository clientRepository)
        {
            _clientRepository = clientRepository;
        }

        public Client getClientByEmail(string email)
        {
            return _clientRepository.FindByEmail(email);
        }

        public ClientDTO getClientDTOByEmail(string email)
        {
            Client client = getClientByEmail(email);
            if (client == null)
            {
                return null;
            }
            return new ClientDTO(client);
        }
    }
}
