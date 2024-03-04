namespace HomeBankingMindHub.Models.DTOs
{
    public class LoanApplicationDTO
    {
        long LoanId { get; set; }
        double Amount { get; set; }
        string Payments { get; set; }
        string ToAccountNumber { get; set; }
    }
}
