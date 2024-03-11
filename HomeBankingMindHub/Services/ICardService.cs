using HomeBankingMindHub.Models;
using HomeBankingMindHub.Models.Enums;

namespace HomeBankingMindHub.Services
{
    public interface ICardService
    {
        void CreateCard(Client client, CardType cardType, CardColor cardColor);
    }
}
