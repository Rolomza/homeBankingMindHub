using HomeBankingMindHub.Models;
using HomeBankingMindHub.Models.DTOs;

namespace HomeBankingMindHub.Services
{
    public interface ILoanService
    {
        IEnumerable<Loan> GetAllLoans();
        IEnumerable<LoanDTO> GetAllLoanDTOs(); 
    }
}
