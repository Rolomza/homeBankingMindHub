using System.Text;

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

        public static string GenerateCardNumber()
        {
            StringBuilder cardNumber = new StringBuilder();
            for (int i = 0; i < 4; i++)
            {
                string field = random.Next(0, 10_000).ToString("D4");
                cardNumber.Append(field);

                if (i < 3)
                {
                    cardNumber.Append('-');
                }
            }
            return cardNumber.ToString();
        }

        public static int GenerateCvvNumber()
        {
            int randomCvv = random.Next(100, 1000);
            return randomCvv;
        }
    }
}
