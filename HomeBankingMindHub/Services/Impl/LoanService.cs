using HomeBankingMindHub.Models;
using HomeBankingMindHub.Models.DTOs;
using HomeBankingMindHub.Repositories;

namespace HomeBankingMindHub.Services.Impl
{
    public class LoanService : ILoanService
    {
        private readonly ILoanRepository _loanRepository;

        public LoanService(ILoanRepository loanRepository)
        {
            _loanRepository = loanRepository;
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

    }
}
