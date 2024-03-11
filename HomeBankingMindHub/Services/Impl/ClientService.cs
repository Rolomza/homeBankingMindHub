using HomeBankingMindHub.Models;
using HomeBankingMindHub.Models.DTOs;
using HomeBankingMindHub.Repositories;

namespace HomeBankingMindHub.Services.Impl
{
    public class ClientService : IClientService
    {
        private readonly IClientRepository _clientRepository;
        private readonly IAccountService _accountService;

        public ClientService(IClientRepository clientRepository, IAccountService accountService)
        {
            _clientRepository = clientRepository;
            _accountService = accountService;
        }

        public Client GetClientById(long id)
        {
            return _clientRepository.FindById(id);
        }

        public Client GetClientByEmail(string email)
        {
            return _clientRepository.FindByEmail(email);
        }

        public IEnumerable<Client> GetAllClients() 
        {

            return _clientRepository.GetAllClients();
        }

        public ClientDTO GetClientDTOById(long id)
        {
            Client client = GetClientById(id);
            if (client == null)
            {
                return null;
            }
            return new ClientDTO(client);
        }

        public ClientDTO GetClientDTOByEmail(string email)
        {
            Client client = GetClientByEmail(email);
            if (client == null)
            {
                return null;
            }
            return new ClientDTO(client);
        }

        public IEnumerable<ClientDTO> GetAllClientsDTOs()
        {
            var clients = GetAllClients();
            var clientsDTO = new List<ClientDTO>();
            foreach (Client client in clients)
            {
                var newClientDTO = new ClientDTO(client);
                clientsDTO.Add(newClientDTO);
            }

            return clientsDTO;
        }

        public void CreateClientWithAccount(ClientCreationDTO clientCreationDTO)
        {
            Client newClient = new Client
            {
                Email = clientCreationDTO.Email,
                Password = clientCreationDTO.Password,
                FirstName = clientCreationDTO.FirstName,
                LastName = clientCreationDTO.LastName,
            };

            _clientRepository.Save(newClient);

            Client clientAtDB = _clientRepository.FindByEmail(clientCreationDTO.Email);

            _accountService.CreateAccount(clientAtDB.Id);

        }
    }
}
