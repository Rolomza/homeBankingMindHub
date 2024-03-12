using HomeBankingMindHub.Models;
using HomeBankingMindHub.Models.DTOs;
using HomeBankingMindHub.Models.Enums;
using HomeBankingMindHub.Repositories;
using HomeBankingMindHub.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.Transactions;

namespace HomeBankingMindHub.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class LoansController : ControllerBase
    {
        private readonly ILoanService _loanService;
        private readonly IAccountService _accountService;
        private readonly IClientService _clientService;

        public LoansController(
            ILoanService loanService, 
            IAccountService accountService, 
            IClientService clientService)
        {
            _loanService = loanService;
            _accountService = accountService;
            _clientService = clientService;
        }

        [HttpGet]
        [Authorize("ClientOnly")]
        public IActionResult Get()
        {
            try
            {
                var loanDTOs = _loanService.GetAllLoanDTOs();
                return Ok(loanDTOs);
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }

        [HttpPost]
        [Authorize("ClientOnly")]
        public IActionResult Post([FromBody] LoanApplicationDTO loanApplicationDTO)
        {
            using(var scope = new TransactionScope())
            {
                try
                {
                    // Verificar que el préstamo exista
                    var loan = _loanService.GetLoanById(loanApplicationDTO.LoanId);
                    if (loan == null)
                    {
                        return StatusCode(403, "El tipo de prestamo solicitado no existe. Revisar LoanId");
                    }

                    // Que el monto NO sea 0 y que no sobrepase el máximo autorizado.
                    if (loanApplicationDTO.Amount <= 0 || loanApplicationDTO.Amount > loan.MaxAmount)
                    {
                        return StatusCode(403, "El monto solicitado no es válido para este tipo de préstamo.");
                    }

                    // Que los payments no lleguen vacíos.
                    if (loanApplicationDTO.Payments.IsNullOrEmpty())
                    {
                        return StatusCode(403, "Payments vacio.");
                    }

                    // Verifica que la cantidad de cuotas se encuentre entre las disponibles del préstamo
                    string[] availablePayments = loan.Payments.Split(',');
                    bool validPayment = _loanService.IsValidPayment(loanApplicationDTO, availablePayments);

                    if (!validPayment)
                    {
                        return StatusCode(403, $"Cantidad de cuotas no válida para este préstamo. \nOpciones válidas: {string.Join(",", availablePayments)}");
                    }

                    // Que exista la cuenta de destino.
                    var toAccount = _accountService.GetAccountByNumber(loanApplicationDTO.ToAccountNumber);
                    if (toAccount == null)
                    {
                        return StatusCode(403, "Cuenta de destino inexistente.");
                    }

                    // Que la cuenta de destino pertenezca al Cliente autentificado.
                    var authenticatedClientEmail = User.FindFirst("Client").Value;
                    var authenticatedClient = _clientService.GetClientByEmail(authenticatedClientEmail);

                    if (authenticatedClient.Id != toAccount.ClientId)
                    {
                        return StatusCode(403, "La cuenta de destino no es de tu propiedad");
                    }

                    _loanService.AssingLoanToAccount(loanApplicationDTO, toAccount);

                    scope.Complete();
                    return StatusCode(201, "Created");
                }
                catch (Exception ex)
                {
                    return StatusCode(500, ex.Message);
                }
            }
        }
    }
}
