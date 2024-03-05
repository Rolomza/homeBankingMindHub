using HomeBankingMindHub.Models;
using HomeBankingMindHub.Models.DTOs;
using HomeBankingMindHub.Repositories;
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
        private readonly IClientRepository _clientRepository;
        private readonly IAccountRepository _accountRepository;
        private readonly ILoanRepository _loanRepository;
        private readonly IClientLoanRepository _clientLoanRepository;
        private readonly ITransactionRepository _transactionRepository;

        public LoansController(IClientRepository clientRepository, IAccountRepository accountRepository,
            ILoanRepository loanRepository, IClientLoanRepository clientLoanRepository, 
            ITransactionRepository transactionRepository)
        {
            _clientRepository = clientRepository;
            _accountRepository = accountRepository;
            _loanRepository = loanRepository;
            _clientLoanRepository = clientLoanRepository;
            _transactionRepository = transactionRepository;
        }

        [HttpGet]
        [Authorize("ClientOnly")]
        public IActionResult Get()
        {
            try
            {
                var loans = _loanRepository.GetAll();
                var loansDTO = new List<LoanDTO>();

                foreach (Loan loan in loans)
                {
                    var newLoanDTO = new LoanDTO
                    {
                        Id = loan.Id,
                        Name = loan.Name,
                        MaxAmount = loan.MaxAmount,
                        Payments = loan.Payments,
                    };

                    loansDTO.Add(newLoanDTO);
                }

                return Ok(loansDTO);
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
                    var loan = _loanRepository.FindById(loanApplicationDTO.LoanId);
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
                    bool validPayment = false;

                    foreach (var item in availablePayments)
                    {
                        if (item == loanApplicationDTO.Payments)
                        {
                            validPayment = true;
                        }
                    }

                    if (!validPayment)
                    {
                        return StatusCode(403, $"Cantidad de cuotas no válida para este préstamo. \nOpciones válidas: {string.Join(",", availablePayments)}");
                    }

                    // Que exista la cuenta de destino.
                    var toAccount = _accountRepository.FindByNumber(loanApplicationDTO.ToAccountNumber);
                    if (toAccount == null)
                    {
                        return StatusCode(403, "Cuenta de destino inexistente.");
                    }

                    // Que la cuenta de destino pertenezca al Cliente autentificado.
                    var authenticatedClientEmail = User.FindFirst("Client").Value;
                    var client = _clientRepository.FindByEmail(authenticatedClientEmail);

                    if (client.Id != toAccount.ClientId)
                    {
                        return StatusCode(403, "La cuenta de destino no es de tu propiedad");
                    }

                    // Cuando guardes clientLoan el monto debes multiplicarlo por el 20%.

                    // Guardar la transaccción.

                    // Actualizar el Balance de la cuenta sumando el monto del préstamo.

                    // Guardar la cuenta.
                    scope.Complete();
                    return Ok();
                }
                catch (Exception ex)
                {
                    return StatusCode(500, ex.Message);
                }
            }
        }
    }
}
