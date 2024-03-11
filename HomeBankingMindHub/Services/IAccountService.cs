using HomeBankingMindHub.Models.DTOs;
using HomeBankingMindHub.Models;

namespace HomeBankingMindHub.Services
{
    public interface IAccountService
    {
        IEnumerable<Account> GetAccountsByClientId(long clientId);
        IEnumerable<AccountDTO> GetAllAccountDTOsByClientId(long clientId);
        void CreateAccount(long clientId);
    }
}
