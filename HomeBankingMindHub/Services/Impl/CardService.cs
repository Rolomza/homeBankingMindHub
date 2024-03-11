using HomeBankingMindHub.Models;
using HomeBankingMindHub.Models.DTOs;
using HomeBankingMindHub.Models.Enums;
using HomeBankingMindHub.Repositories;
using HomeBankingMindHub.Utils;

namespace HomeBankingMindHub.Services.Impl
{
    public class CardService : ICardService
    {
        private readonly ICardRepository _cardRepository;

        public CardService(ICardRepository cardRepository)
        {
            _cardRepository = cardRepository;
        }

        public IEnumerable<Card> GetCardsByClientId(long clientId) 
        {
            return _cardRepository.GetCardsByClient(clientId);
        }

        public IEnumerable<CardDTO> GetCardDTOsByClientId(long clientId)
        {
            var cards = _cardRepository.GetCardsByClient(clientId);
            var cardDTOs = new List<CardDTO>();
            foreach (var card in cards)
            {
                CardDTO cardDTO = new CardDTO(card);
                cardDTOs.Add(cardDTO);
            }
            return cardDTOs;
        }

        public void CreateCard(Client client, CardType cardType, CardColor cardColor)
        {
            string newCardNumber;
            do
            {
                newCardNumber = RandomNumberGenerator.GenerateCardNumber();
            } while (_cardRepository.FindByNumber(newCardNumber) != null);

            Card newCard = new Card
            {
                ClientId = client.Id,
                CardHolder = client.FirstName + " " + client.LastName,
                Type = cardType,
                Color = cardColor,
                Number = newCardNumber,
                Cvv = RandomNumberGenerator.GenerateCvvNumber(),
                FromDate = DateTime.Now,
                ThruDate = DateTime.Now.AddYears(5),
            };

            _cardRepository.Save(newCard);
        }
    }
}
