using HomeBankingMindHub.Models;
using HomeBankingMindHub.Models.DTOs;
using HomeBankingMindHub.Models.Enums;
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
    public class TransactionsController : ControllerBase
    {
        private readonly IClientRepository _clientRepository;
        private readonly IAccountRepository _accountRepository;
        private readonly ITransactionRepository _transactionRepository;

        public TransactionsController(IClientRepository clientRepository, IAccountRepository accountRepository,
            ITransactionRepository transactionRepository)
        {
            _clientRepository = clientRepository;
            _accountRepository = accountRepository;
            _transactionRepository = transactionRepository;
        }

        [HttpPost]
        [Authorize("ClientOnly")]
        public IActionResult MakeTransaction([FromBody] TransferDTO newTransfer)
        {
            using(var scope = new TransactionScope())
            {
                try
                {
                    // Verificar que los parámetros no estén vacíos
                    if (newTransfer.FromAccountNumber.IsNullOrEmpty() ||
                        newTransfer.FromAccountNumber.IsNullOrEmpty() ||
                        newTransfer.Description.IsNullOrEmpty() ||
                        newTransfer.Amount <= 0)
                    {
                        return BadRequest("Campos de transacción no válidos. Complete todos los campos requeridos.");
                    }

                    // Verificar que los números de cuenta no sean iguales
                    if (newTransfer.FromAccountNumber.Equals(newTransfer.ToAccountNumber))
                    {
                        return BadRequest("Transacción no válida. No puedes transferir a la misma cuenta.");
                    }

                    // Verificar que exista la cuenta de origen
                    var fromAccount = _accountRepository.FindByNumber(newTransfer.FromAccountNumber);
                    if (fromAccount == null)
                    {
                        return BadRequest($"La cuenta de origen {newTransfer.FromAccountNumber} no existe.");
                    }

                    // Verificar que la cuenta de origen pertenezca al cliente autenticado
                    var authenticatedClient = _clientRepository.FindByEmail(User.FindFirst("Client").Value);

                    if (!authenticatedClient.Accounts.Any(account => account.Number == fromAccount.Number))
                    {
                        return BadRequest($"La cuenta {fromAccount.Number} no le pertenece.");
                    }

                    // Verificar que exista la cuenta de destino
                    var toAccount = _accountRepository.FindByNumber(newTransfer.ToAccountNumber);
                    if (toAccount == null)
                    {
                        return BadRequest($"La cuenta de destino {newTransfer.ToAccountNumber} no existe.");
                    }

                    // Verificar que la cuenta de origen tenga el monto disponible.
                    if (fromAccount.Balance < newTransfer.Amount)
                    {
                        return BadRequest("Saldo insuficiente para realizar la transacción.");
                    }

                    // Crear dos transacciones, una tipo “DEBIT” asociada a la cuenta de origen y 
                    // la otra con el tipo “CREDIT” asociada a la cuenta de destino.
                    var debitTransaction = new Models.Transaction
                    {
                        AccountId = fromAccount.Id,
                        Amount = -newTransfer.Amount,
                        Date = DateTime.Now,
                        Description = newTransfer.Description,
                        Type = TransactionType.DEBIT
                    };
                    //var toAccountClient = _clientRepository.FindById(ToAccount.ClientId);
                    var creditTransaction = new Models.Transaction
                    {
                        AccountId = toAccount.Id,
                        Amount = newTransfer.Amount,
                        Date = DateTime.Now,
                        Description = newTransfer.Description,
                        Type = TransactionType.CREDIT
                    };

                    _transactionRepository.Save(debitTransaction);
                    _transactionRepository.Save(creditTransaction);

                    // A la cuenta de origen se le restará el monto indicado en la petición
                    // y a la cuenta de destino se le sumará el mismo monto.
                    fromAccount.Balance -= newTransfer.Amount;
                    toAccount.Balance += newTransfer.Amount;

                    _accountRepository.Save(fromAccount);
                    _accountRepository.Save(toAccount);

                    scope.Complete();
                    return Ok(newTransfer);
                }
                catch (Exception ex)
                {
                    return StatusCode(500, ex.Message);
                }
            }
            
        }
        
    }
}
