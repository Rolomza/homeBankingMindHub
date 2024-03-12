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
        private readonly IClientRepository _clientRepository;
        private readonly IAccountRepository _accountRepository;
        private readonly ILoanRepository _loanRepository;
        private readonly IClientLoanRepository _clientLoanRepository;
        private readonly ITransactionRepository _transactionRepository;
        private readonly ILoanService _loanService;

        public LoansController(IClientRepository clientRepository, IAccountRepository accountRepository,
            ILoanRepository loanRepository, IClientLoanRepository clientLoanRepository, 
            ITransactionRepository transactionRepository, ILoanService loanService)
        {
            _clientRepository = clientRepository;
            _accountRepository = accountRepository;
            _loanRepository = loanRepository;
            _clientLoanRepository = clientLoanRepository;
            _transactionRepository = transactionRepository;
            _loanService = loanService;
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
                    var authenticatedClient = _clientRepository.FindByEmail(authenticatedClientEmail);

                    if (authenticatedClient.Id != toAccount.ClientId)
                    {
                        return StatusCode(403, "La cuenta de destino no es de tu propiedad");
                    }

                    // Cuando guardes clientLoan el monto debes multiplicarlo por el 20%.
                    var amountPlusInterest = loanApplicationDTO.Amount * 1.20;

                    var newClientLoan = new ClientLoan
                    {
                        Amount = amountPlusInterest,
                        Payments = loanApplicationDTO.Payments,
                        ClientId = toAccount.ClientId,
                        LoanId = loanApplicationDTO.LoanId,
                    };

                    _clientLoanRepository.Save(newClientLoan);

                    // Se debe crear una transacción “CREDIT” asociada a la cuenta de destino
                    // con la descripción concatenando el nombre del préstamo y la frase “loan approved”
                    // Guardar la transaccción.
                    var newAccountTransaction = new Models.Transaction
                    {
                        AccountId = toAccount.Id,
                        Amount = loanApplicationDTO.Amount,
                        Date = DateTime.Now,
                        Description = $"{loan.Name} loan approved.",
                        Type = TransactionType.CREDIT,
                    };

                    _transactionRepository.Save(newAccountTransaction);

                    // Actualizar el Balance de la cuenta sumando el monto del préstamo.
                    toAccount.Balance += loanApplicationDTO.Amount;
                    // Guardar (Actualizar) la cuenta.
                    _accountRepository.Save(toAccount);

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
