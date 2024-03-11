using HomeBankingMindHub.Models;
using HomeBankingMindHub.Models.DTOs;
using HomeBankingMindHub.Models.Enums;

namespace HomeBankingMindHub.Services
{
    public interface ITransactionService
    {
        void CreateTransaction(Account fromAccount, Account toAccount, TransferDTO transferDTO);
    }
}
