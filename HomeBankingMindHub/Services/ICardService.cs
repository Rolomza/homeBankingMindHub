using HomeBankingMindHub.Models;
using HomeBankingMindHub.Models.DTOs;
using HomeBankingMindHub.Models.Enums;

namespace HomeBankingMindHub.Services
{
    public interface ICardService
    {
        IEnumerable<Card> GetCardsByClientId(long clientId);
        IEnumerable<CardDTO> GetCardDTOsByClientId(long clientId);
        void CreateCard(Client client, CardType cardType, CardColor cardColor);
    }
}
