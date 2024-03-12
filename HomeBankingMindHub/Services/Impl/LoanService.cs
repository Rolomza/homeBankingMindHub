using HomeBankingMindHub.Models;
using HomeBankingMindHub.Models.DTOs;
using HomeBankingMindHub.Models.Enums;
using HomeBankingMindHub.Repositories;

namespace HomeBankingMindHub.Services.Impl
{
    public class LoanService : ILoanService
    {
        private readonly ILoanRepository _loanRepository;
        private readonly IClientLoanRepository _clientLoanRepository;
        private readonly ITransactionService _transactionService;

        public LoanService(
            ILoanRepository loanRepository, 
            IClientLoanRepository clientLoanRepository,
            ITransactionService transactionService)
        {
            _loanRepository = loanRepository;
            _clientLoanRepository = clientLoanRepository;
            _transactionService = transactionService;
        }

        public IEnumerable<Loan> GetAllLoans()
        {
            return _loanRepository.GetAll();
        }

        public IEnumerable<LoanDTO> GetAllLoanDTOs()
        {
            var loans = GetAllLoans();
            var loanDTOS = new List<LoanDTO>();
            foreach (var loan in loans)
            {
                var newLoanDTO = new LoanDTO(loan);
                loanDTOS.Add(newLoanDTO);
            }
            return loanDTOS;
        }

        public Loan GetLoanById(long id)
        {
            return _loanRepository.FindById(id);
        }

        public bool IsValidPayment(LoanApplicationDTO loanApplicationDTO, string[] availablePayments)
        {
            bool validPayment = false;

            foreach (var item in availablePayments)
            {
                if (item == loanApplicationDTO.Payments)
                {
                    validPayment = true;
                }
            }

            return validPayment;
        }

        public void AssingLoanToAccount(LoanApplicationDTO loanApplicationDTO, Account destinationAccount)
        {
            var amountPlusInterest = loanApplicationDTO.Amount * 1.20;

            var newClientLoan = new ClientLoan
            {
                Amount = amountPlusInterest,
                Payments = loanApplicationDTO.Payments,
                ClientId = destinationAccount.ClientId,
                LoanId = loanApplicationDTO.LoanId,
            };

            _clientLoanRepository.Save(newClientLoan);

            string loanName = GetLoanById(loanApplicationDTO.LoanId).Name;

            _transactionService.CreateLoanTransaction(destinationAccount, loanApplicationDTO, loanName);
        }
    }
}
