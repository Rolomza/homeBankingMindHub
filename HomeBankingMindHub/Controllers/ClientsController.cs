using HomeBankingMindHub.Models;
using HomeBankingMindHub.Models.DTOs;
using HomeBankingMindHub.Models.Enums;
using HomeBankingMindHub.Repositories;
using HomeBankingMindHub.Utils;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.VisualStudio.Web.CodeGenerators.Mvc.Templates.BlazorIdentity.Pages.Manage;
using System.Drawing;

namespace HomeBankingMindHub.Controllers
{

    [ApiController]
    [Route("api/[controller]")]
    public class ClientsController : ControllerBase
    {
        private readonly IClientRepository _clientRepository;
        private readonly IAccountRepository _accountRepository;
        private readonly ICardRepository _cardRepository;

        public ClientsController(IClientRepository clientRepository, IAccountRepository accountRepository,
            ICardRepository cardRepository)
        {
            _clientRepository = clientRepository;
            _accountRepository = accountRepository;
            _cardRepository = cardRepository;
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

                var newClientAtDB = _clientRepository.FindByEmail(newClient.Email);

                string newAccountNumber;
                do
                {
                    newAccountNumber = RandomNumberGenerator.GenerateAccountNumber();
                } while (_accountRepository.FindByNumber(newAccountNumber) != null);

                Account newClientAccount = new Account
                {
                    Number = newAccountNumber,
                    CreationDate = DateTime.Now,
                    Balance = 0,
                    ClientId = newClientAtDB.Id,
                };

                _accountRepository.Save(newClientAccount);

                return Created("", newClient);
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }

        [HttpPost("current/accounts")]
        [Authorize("ClientOnly")]
        public IActionResult Post()
        {
            try
            {
                string email = User.FindFirst("Client") != null ? User.FindFirst("Client").Value : string.Empty;
                if (string.IsNullOrEmpty(email))
                {
                    return Forbid();
                }

                var client = _clientRepository.FindByEmail(email);
                
                if (client.Accounts.ToList().Count == 3)
                {
                    return StatusCode(403, "Has Alcanzado la cantidad maxima de cuentas por cliente (3).");
                } 
                else
                {
                    string newAccountNumber;
                    do
                    {
                        newAccountNumber = RandomNumberGenerator.GenerateAccountNumber();
                    } while (_accountRepository.FindByNumber(newAccountNumber) != null);

                    Account newAccount = new Account
                    {
                        ClientId = client.Id,
                        CreationDate = DateTime.Now,
                        Balance = 0,
                        Number = newAccountNumber,
                    };

                    _accountRepository.Save(newAccount);
                }

                return StatusCode(201, "Cuenta Creada satisfactoriamente.");
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }

        [HttpPost("current/cards")]
        [Authorize("ClientOnly")]
        public IActionResult Post([FromBody] CardCreationDTO cardCreationDTO)
        {
            try
            {
                string email = User.FindFirst("Client") != null ? User.FindFirst("Client").Value : string.Empty;
                if (string.IsNullOrEmpty(email))
                {
                    return Forbid();
                }

                var client = _clientRepository.FindByEmail(email);
                var cardCreationDTOType = (CardType)Enum.Parse(typeof(CardType), cardCreationDTO.Type);
                var cardCreationDTOColor = (CardColor)Enum.Parse(typeof(CardColor), cardCreationDTO.Color);

                if (client.Cards.Count(card => card.Type == cardCreationDTOType) == 3)
                {
                    return StatusCode(403, $"Cantidad máxima de Tarjetas tipo {cardCreationDTO.Type} por cliente (3).");
                } 
                else
                {
                    if (client.Cards.Where(card => card.Type == cardCreationDTOType).Any(card => card.Color == cardCreationDTOColor))
                    {
                        return StatusCode(403, $"Ya posee tarjeta tipo {cardCreationDTO.Type} de color {cardCreationDTO.Color}");
                    }
                }

                string newCardNumber;
                do
                {
                    newCardNumber = RandomNumberGenerator.GenerateCardNumber();
                } while (_cardRepository.FindByNumber(newCardNumber) != null);

                Card newCard = new Card
                {
                    ClientId = client.Id,
                    CardHolder = client.FirstName + " " + client.LastName,
                    Type = cardCreationDTOType,
                    Color = cardCreationDTOColor,
                    Number = newCardNumber,
                    Cvv = RandomNumberGenerator.GenerateCvvNumber(),
                    FromDate = DateTime.Now,
                    ThruDate = DateTime.Now.AddYears(5),
                };

                _cardRepository.Save(newCard);

                return StatusCode(201, $"Tarjeta Creada satisfactoriamente.");
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }

        [HttpGet("current/accounts")]
        [Authorize("ClientOnly")]
        public IActionResult GetCurrentAccounts()
        {
            try
            {
                string clientEmail = User.FindFirst("Client") != null ? User.FindFirst("Client").Value : string.Empty;
                if (string.IsNullOrEmpty(clientEmail))
                {
                    return Forbid();
                }

                var client = _clientRepository.FindByEmail(clientEmail);
                var accounts = _accountRepository.GetAccountsByClient(client.Id);
                var accountsDTO = new List<AccountDTO>();

                foreach (Account account in accounts)
                {
                    var newAccountDTO = new AccountDTO
                    {
                        Id = account.Id,
                        Number = account.Number,
                        CreationDate = account.CreationDate,
                        Balance = account.Balance,
                        Transactions = account.Transactions.Select(transaction =>
                            new TransactionDTO
                            {
                                Id = transaction.Id,
                                Type = transaction.Type.ToString(),
                                Amount = transaction.Amount,
                                Description = transaction.Description,
                                Date = transaction.Date,
                            }).ToList()
                    };

                    accountsDTO.Add(newAccountDTO);
                }

                return Ok(accountsDTO);
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }

        [HttpGet("current/cards")]
        [Authorize("ClientOnly")]
        public IActionResult GetCurrentCards()
        {
            try
            {
                string clientEmail = User.FindFirst("Client") != null ? User.FindFirst("Client").Value : string.Empty;
                if (string.IsNullOrEmpty(clientEmail))
                {
                    return Forbid();
                }

                var client = _clientRepository.FindByEmail(clientEmail);
                var cards = _cardRepository.GetCardsByClient(client.Id);
                var cardsDTO = new List<CardDTO>();

                foreach (Card card in cards)
                {
                    var newCardDTO = new CardDTO
                    {
                        Id = card.Id,
                        CardHolder = card.CardHolder,
                        Color = card.Color.ToString(),
                        Cvv = card.Cvv,
                        FromDate = card.FromDate,
                        Number = card.Number,
                        ThruDate = card.ThruDate,
                        Type = card.Type.ToString()
                    };

                    cardsDTO.Add(newCardDTO);
                }

                return Ok(cardsDTO);
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }
    }
}
