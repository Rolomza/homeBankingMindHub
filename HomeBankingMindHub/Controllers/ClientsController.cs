using HomeBankingMindHub.Models;
using HomeBankingMindHub.Models.DTOs;
using HomeBankingMindHub.Repositories;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace HomeBankingMindHub.Controllers
{

    [ApiController]
    [Route("api/[controller]")]
    public class ClientsController : ControllerBase
    {
        private readonly IClientRepository _clientRepository;

        public ClientsController(IClientRepository clientRepository)
        {
            _clientRepository = clientRepository;
        }

        [HttpGet]
        public IActionResult Get()
        {
            try
            {
                var clients = _clientRepository.GetAllClients();
                var clientsDTO = new List<ClientDTO>();

                foreach (Client client in clients)
                {
                    var newClientDTO = new ClientDTO
                    {
                        Id = client.Id,
                        Email = client.Email,
                        FirstName = client.FirstName,
                        LastName = client.LastName,
                        Accounts = client.Accounts.Select(account =>
                            new AccountDTO
                            {
                                Id = account.Id,
                                Balance = account.Balance,
                                CreationDate = account.CreationDate,
                                Number = account.Number
                            }).ToList(),
                        Credits = client.ClientLoans.Select(client =>
                            new ClientLoanDTO
                            {
                                Id = client.Id,
                                LoanId = client.LoanId,
                                Name = client.Loan.Name,
                                Amount = client.Amount,
                                Payments = int.Parse(client.Payments)
                            }).ToList(),
                        Cards = client.Cards.Select(client =>
                            new CardDTO
                            {
                                Id = client.Id,
                                CardHolder = client.CardHolder,
                                Color = client.Color.ToString(),
                                Cvv = client.Cvv,
                                FromDate = client.FromDate,
                                Number = client.Number,
                                ThruDate = client.ThruDate,
                                Type = client.Type.ToString()
                            }).ToList()
                    };

                    clientsDTO.Add(newClientDTO);
                }

                return Ok(clientsDTO);
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }

        [HttpGet("{id}")]
        public IActionResult Get(int id)
        {
            try
            {
                var client = _clientRepository.FindById(id);
                if (client == null)
                {
                    return Forbid();
                }

                var clientDTO = new ClientDTO
                {
                    Id = client.Id,
                    FirstName = client.FirstName,
                    LastName = client.LastName,
                    Email = client.Email,
                    Accounts = client.Accounts.Select(account => new AccountDTO
                    {
                        Id = account.Id,
                        Balance = account.Balance,
                        CreationDate = account.CreationDate,
                        Number = account.Number
                    }).ToList(),
                    Credits = client.ClientLoans.Select(client =>
                        new ClientLoanDTO
                        {
                            Id = client.Id,
                            LoanId = client.LoanId,
                            Name = client.Loan.Name,
                            Amount = client.Amount,
                            Payments = int.Parse(client.Payments)
                        }).ToList(),
                    Cards = client.Cards.Select(client =>
                        new CardDTO
                        {
                            Id = client.Id,
                            CardHolder = client.CardHolder,
                            Color = client.Color.ToString(),
                            Cvv = client.Cvv,
                            FromDate = client.FromDate,
                            Number = client.Number,
                            ThruDate = client.ThruDate,
                            Type = client.Type.ToString()
                        }).ToList()
                };

                return Ok(clientDTO);
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }
    }
}
