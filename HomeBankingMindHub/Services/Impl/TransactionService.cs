using HomeBankingMindHub.Models;
using HomeBankingMindHub.Models.DTOs;
using HomeBankingMindHub.Models.Enums;
using HomeBankingMindHub.Repositories;
using Microsoft.Identity.Client;

namespace HomeBankingMindHub.Services.Impl
{
    public class TransactionService : ITransactionService
    {
        private readonly ITransactionRepository _transactionRepository;
        private readonly IAccountRepository _accountRepository;

        public TransactionService(ITransactionRepository transactionRepository, IAccountRepository accountRepository)
        {
            _transactionRepository = transactionRepository;
            _accountRepository = accountRepository;
        }
        public void CreateTransaction(Account fromAccount, Account toAccount, TransferDTO transferDTO)
        {
            Transaction debitTransaction = new Transaction
            {
                AccountId = fromAccount.Id,
                Amount = -transferDTO.Amount,
                Date = DateTime.Now,
                Description = transferDTO.Description,
                Type = TransactionType.DEBIT,
            };

            Transaction creditTransaction = new Transaction
            {
                AccountId = toAccount.Id,
                Amount = transferDTO.Amount,
                Date = DateTime.Now,
                Description = transferDTO.Description,
                Type = TransactionType.CREDIT
            };

            _transactionRepository.Save(debitTransaction);
            _transactionRepository.Save(creditTransaction);

            fromAccount.Balance -= transferDTO.Amount;
            toAccount.Balance += transferDTO.Amount;

            _accountRepository.Save(fromAccount);
            _accountRepository.Save(toAccount);
        }
    }
}
