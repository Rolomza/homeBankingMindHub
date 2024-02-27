namespace HomeBankingMindHub.Utils
{
    public static class RandomNumberGenerator
    {
        private static Random random = new Random();

        public static string GenerateAccountNumber()
        {
            int randomNumber = random.Next(0,100_000_000);
            return $"VIN-{randomNumber:D8}";
        }
    }
}
