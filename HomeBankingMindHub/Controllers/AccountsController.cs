using HomeBankingMindHub.Models;
using HomeBankingMindHub.Models.DTOs;
using HomeBankingMindHub.Repositories;
using HomeBankingMindHub.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.VisualStudio.Web.CodeGenerators.Mvc.Templates.BlazorIdentity.Pages.Manage;

namespace HomeBankingMindHub.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AccountsController : ControllerBase
    {
        private readonly IAccountRepository _accountRepository;
        private readonly IAccountService _accountService;

        public AccountsController(IAccountRepository accountRepository, IAccountService accountService)
        {
            _accountRepository = accountRepository;
            _accountService = accountService;
        }

        [HttpGet]
        [Authorize("AdminOnly")]
        public IActionResult Get()
        {
            try
            {
                var accountDTOs = _accountService.GetAllAccountDTOs();
                return Ok(accountDTOs);
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
                var account = _accountRepository.FindById(id);

                if (account == null)
                {
                    return Forbid();
                }

                string email = account.Client.Email;
                if (User.FindFirst("Admin") == null)
                {
                    var userAuthenticatedEmail = User.FindFirst("Client").Value;

                    if (userAuthenticatedEmail != email)
                    {
                        return Unauthorized();
                    }
                }

                var accountDTO = new AccountDTO
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

                return Ok(accountDTO);
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }
    }
}
