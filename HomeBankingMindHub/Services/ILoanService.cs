using HomeBankingMindHub.Models;
using HomeBankingMindHub.Models.DTOs;

namespace HomeBankingMindHub.Services
{
    public interface ILoanService
    {
        IEnumerable<Loan> GetAllLoans();
        IEnumerable<LoanDTO> GetAllLoanDTOs();
        Loan GetLoanById(long id);
        bool IsValidPayment(LoanApplicationDTO applicationDTO, string[] availablePayments);
        void AssingLoanToAccount(LoanApplicationDTO loanApplicationDTO, Account destinationAccount);
    }
}
