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
    public class TransactionsController : ControllerBase
    {
        private readonly IAccountService _accountService;
        private readonly IClientService _clientService;
        private readonly ITransactionService _transactionService;

        public TransactionsController( 
            IAccountService accountService, 
            IClientService clientService,
            ITransactionService transactionService)
        {
            _accountService = accountService;
            _clientService = clientService;
            _transactionService = transactionService;
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
                    var fromAccount = _accountService.GetAccountByNumber(newTransfer.FromAccountNumber);
                    if (fromAccount == null)
                    {
                        return BadRequest($"La cuenta de origen {newTransfer.FromAccountNumber} no existe.");
                    }

                    // Verificar que la cuenta de origen pertenezca al cliente autenticado
                    var authenticatedClient = _clientService.GetClientByEmail(User.FindFirst("Client").Value);

                    if (!authenticatedClient.Accounts.Any(account => account.Number == fromAccount.Number))
                    {
                        return BadRequest($"La cuenta {fromAccount.Number} no le pertenece.");
                    }

                    // Verificar que exista la cuenta de destino
                    var toAccount = _accountService.GetAccountByNumber(newTransfer.ToAccountNumber);
                    if (toAccount == null)
                    {
                        return BadRequest($"La cuenta de destino {newTransfer.ToAccountNumber} no existe.");
                    }

                    // Verificar que la cuenta de origen tenga el monto disponible.
                    if (fromAccount.Balance < newTransfer.Amount)
                    {
                        return BadRequest("Saldo insuficiente para realizar la transacción.");
                    }

                    _transactionService.CreateTransaction(fromAccount, toAccount, newTransfer);

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
