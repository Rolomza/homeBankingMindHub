using HomeBankingMindHub.Models;
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
    }
}
