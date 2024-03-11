﻿using HomeBankingMindHub.Models;
using HomeBankingMindHub.Models.DTOs;
using HomeBankingMindHub.Models.Enums;
using HomeBankingMindHub.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace HomeBankingMindHub.Controllers
{

    [ApiController]
    [Route("api/[controller]")]
    public class ClientsController : ControllerBase
    {
        private readonly IClientService _clientService;
        private readonly IAccountService _accountService;
        private readonly ICardService _cardService;

        public ClientsController(
            IClientService clientService, 
            IAccountService accountService,
            ICardService cardService)
        {
            _clientService = clientService;
            _accountService = accountService;
            _cardService = cardService;
        }

        [HttpGet]
        [Authorize("AdminOnly")]
        public IActionResult Get()
        {
            try
            {
                var clients = _clientService.GetAllClientsDTOs();
                
                return Ok(clients);
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
                ClientDTO client = _clientService.GetClientDTOById(id);
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

                return Ok(client);
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

                ClientDTO client = _clientService.GetClientDTOByEmail(email);

                if (client == null)
                {
                    return Forbid();
                }

                return Ok(client);
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

                Client user = _clientService.GetClientByEmail(clientCreationDTO.Email);

                if (user != null)
                {
                    return StatusCode(403, "Email está en uso.");
                }

                _clientService.CreateClientWithAccount(clientCreationDTO);
                ClientDTO createdClient = _clientService.GetClientDTOByEmail(clientCreationDTO.Email);

                return Created("", createdClient);
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

                var client = _clientService.GetClientByEmail(email);
                
                if (client.Accounts.ToList().Count == 3)
                {
                    return StatusCode(403, "Has Alcanzado la cantidad maxima de cuentas por cliente (3).");
                } 
                else
                {
                    _accountService.CreateAccount(client.Id);
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

                var client = _clientService.GetClientByEmail(email);
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

                _cardService.CreateCard(client, cardCreationDTOType, cardCreationDTOColor);

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

                var client = _clientService.GetClientByEmail(clientEmail);
                var accounts = _accountService.GetAllAccountDTOsByClientId(client.Id);
                
                return Ok(accounts);
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

                var client = _clientService.GetClientByEmail(clientEmail);
                var cardDTOs = _cardService.GetCardDTOsByClientId(client.Id);
                
                return Ok(cardDTOs);
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }
    }
}
