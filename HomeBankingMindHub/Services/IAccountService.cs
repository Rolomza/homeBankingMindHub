using HomeBankingMindHub.Models.DTOs;
using HomeBankingMindHub.Models;

namespace HomeBankingMindHub.Services
{
    public interface IAccountService
    {
        IEnumerable<Account> GetAllAccounts();
        IEnumerable<AccountDTO> GetAllAccountDTOs();
        Account GetAccountById(long id);
        AccountDTO GetAccountDTOById(long id);
        IEnumerable<Account> GetAccountsByClientId(long clientId);
        IEnumerable<AccountDTO> GetAllAccountDTOsByClientId(long clientId);
        void CreateAccount(long clientId);
    }
}
