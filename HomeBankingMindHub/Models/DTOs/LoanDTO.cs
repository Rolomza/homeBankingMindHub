namespace HomeBankingMindHub.Models.DTOs
{
    public class LoanDTO
    {
        long Id { get; set; }
        string Name { get; set; }
        double MaxAmount { get; set; }
        string Payments { get; set; }
    }
}
