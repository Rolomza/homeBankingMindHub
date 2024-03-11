using HomeBankingMindHub.Models;
using HomeBankingMindHub.Models.DTOs;
using HomeBankingMindHub.Repositories;
using HomeBankingMindHub.Utils;

namespace HomeBankingMindHub.Services.Impl
{
    public class AccountService : IAccountService
    {
        private readonly IAccountRepository _accountRepository;

        public AccountService(IAccountRepository accountRepository)
        {
            _accountRepository = accountRepository;
        }

        public Account GetAccountById(long id)
        {
            return _accountRepository.FindById(id);
        }

        public Account GetAccountByNumber(string number)
        {
            return _accountRepository.FindByNumber(number);
        }

        public AccountDTO GetAccountDTOById(long id)
        {
            Account account = GetAccountById(id);
            return new AccountDTO(account);
        }

        public IEnumerable<Account> GetAllAccounts()
        {
            return _accountRepository.GetAllAccounts();
        }

        public IEnumerable<AccountDTO> GetAllAccountDTOs()
        {
            var accounts = GetAllAccounts();
            var accountDTOs = new List<AccountDTO>();
            foreach (var account in accounts)
            {
                AccountDTO accountDTO = new AccountDTO(account);
                accountDTOs.Add(accountDTO);
            }
            return accountDTOs;
        }

        public void CreateAccount(long clientId)
        {
            string newAccountNumber;
            do
            {
                newAccountNumber = RandomNumberGenerator.GenerateAccountNumber();
            } while (_accountRepository.FindByNumber(newAccountNumber) != null);

            Account newClientAccount = new Account
            {
                Number = newAccountNumber,
                CreationDate = DateTime.Now,
                Balance = 0,
                ClientId = clientId,
            };

            _accountRepository.Save(newClientAccount);
        }

        public IEnumerable<Account> GetAccountsByClientId(long clientId)
        {
            return _accountRepository.GetAccountsByClient(clientId);
        }

        public IEnumerable<AccountDTO> GetAllAccountDTOsByClientId(long clientId)
        {
            var accounts = GetAccountsByClientId(clientId);
            var accountsDTO = new List<AccountDTO>();
            foreach (var account in accounts)
            {
                var newAccountDTO = new AccountDTO(account);
                accountsDTO.Add(newAccountDTO);
            }
            return accountsDTO;
        }
    }
}
