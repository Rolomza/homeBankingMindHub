using HomeBankingMindHub.Models;
using HomeBankingMindHub.Models.DTOs;
using HomeBankingMindHub.Repositories;
using Microsoft.AspNetCore.Authorization;
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
        [Authorize("AdminOnly")]
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

                string email = client.Email;
                if (User.FindFirst("Admin") == null)
                {
                    var userAuthenticatedEmail = User.FindFirst("Client").Value;

                    if (userAuthenticatedEmail != email)
                    {
                        return Unauthorized();
                    }
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

        [HttpGet("current")]
        [Authorize("ClientOnly")]
        public IActionResult GetCurrent()
        {
            try
            {
                string email = User.FindFirst("Client") != null ? User.FindFirst("Client").Value : string.Empty;
                if (string.IsNullOrEmpty(email))
                {
                    return Forbid();
                }

                Client client = _clientRepository.FindByEmail(email);
                if (client == null)
                {
                    return Forbid();
                }

                var clientDTO = new ClientDTO
                {
                    Id = client.Id,
                    Email = client.Email,
                    FirstName = client.FirstName,
                    LastName = client.LastName,
                    Accounts = client.Accounts.Select(account => new AccountDTO
                    {
                        Id = account.Id,
                        Balance = account.Balance,
                        CreationDate = account.CreationDate,
                        Number = account.Number,
                    }).ToList(),
                    Credits = client.ClientLoans.Select(client => new ClientLoanDTO
                    {
                        Id = client.Id,
                        LoanId = client.LoanId,
                        Name = client.Loan.Name,
                        Amount = client.Amount,
                        Payments = int.Parse(client.Payments)
                    }).ToList(),
                    Cards = client.Cards.Select(card => new CardDTO
                    {
                        Id = card.Id,
                        CardHolder = card.CardHolder,
                        Color = card.Color.ToString(),
                        Cvv = card.Cvv,
                        FromDate = card.FromDate,
                        Number = card.Number,
                        ThruDate = card.ThruDate,
                        Type = card.Type.ToString()
                    }).ToList()
                };

                return Ok(clientDTO);
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }

        [HttpPost]
        public IActionResult Post([FromBody] ClientCreationDTO clientCreationDTO)
        {
            try
            {
                if (String.IsNullOrEmpty(clientCreationDTO.Email))
                {
                    return StatusCode(403, "Email inválido.");
                }

                if (String.IsNullOrEmpty(clientCreationDTO.Password))
                {
                    return StatusCode(403, "Password inválido.");
                }

                if (String.IsNullOrEmpty(clientCreationDTO.FirstName) || String.IsNullOrEmpty(clientCreationDTO.LastName))
                {
                    return StatusCode(403, "Datos Personales Incompletos.");
                }

                Client user = _clientRepository.FindByEmail(clientCreationDTO.Email);

                if (user != null) 
                {
                    return StatusCode(403, "Email está en uso.");
                }

                Client newClient = new Client
                {
                    Email = clientCreationDTO.Email,
                    Password = clientCreationDTO.Password,
                    FirstName = clientCreationDTO.FirstName,
                    LastName = clientCreationDTO.LastName,
                };

                _clientRepository.Save(newClient);
                return Created("", newClient);

            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }
    }
}
