using HomeBankingMindHub.Models;
using HomeBankingMindHub.Models.DTOs;
using HomeBankingMindHub.Repositories;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
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
                    // Que el usuario este autentificado
                    // Verificar que el préstamo exista
                    // Que el monto NO sea 0 y que no sobrepase el máximo autorizado.
                    // Que los payments no lleguen vacíos.
                    // Que exista la cuenta de destino.
                    // Que la cuenta de destino pertenezca al Cliente autentificado.
                    // Cuando guardes clientLoan el monto debes multiplicarlo por el 20%.
                    // Guardar la transaccción.
                    // Actualizar el Balance de la cuenta sumando el monto del préstamo.
                    // Guardar la cuenta.
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
