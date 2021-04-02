namespace VaporStore.DataProcessor
{
    using Data;
    using Microsoft.EntityFrameworkCore.Internal;
    using Newtonsoft.Json;
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.Globalization;
    using System.IO;
    using System.Linq;
    using System.Text;
    using System.Xml.Serialization;
    using VaporStore.Data.Models;
    using VaporStore.Data.Models.Enums;
    using VaporStore.DataProcessor.Dto.Import;

    public static class Deserializer
    {
        private const string errorMessage = "Invalid Data";
        private const string successMessageImportGame = "Added {0} ({1}) with {2} tags";
        private const string successsMessageImportUser = "Imported {0} with {1} cards";
        private const string successsMessageImportPurchase = "Imported {0} for {1}";

        public static string ImportGames(VaporStoreDbContext context, string jsonString)
        {
            var gameDTOs = JsonConvert.DeserializeObject<IEnumerable<ImportGameDTO>>(jsonString);

            var sb = new StringBuilder();
            var games = new List<Game>();
            var developers = new List<Developer>();
            var genres = new List<Genre>();
            var tags = new List<Tag>();

            foreach (var gameDTO in gameDTOs)
            {
                if (!IsValid(gameDTO))
                {
                    sb.AppendLine(errorMessage);
                    continue;
                }

                if(gameDTO.Tags.Length == 0)
                {
                    sb.AppendLine(errorMessage);
                    continue;
                }

                var game = new Game()
                {
                    Name = gameDTO.Name,
                    Price = gameDTO.Price,
                    ReleaseDate = DateTime.ParseExact(gameDTO.ReleaseDate, "yyyy-MM-dd", CultureInfo.InvariantCulture),
                };

                var currentDeveloper = developers.FirstOrDefault(x => x.Name == gameDTO.Developer);

                if (currentDeveloper == null)
                {
                    currentDeveloper = new Developer()
                    {
                        Name = gameDTO.Developer
                    };

                    developers.Add(currentDeveloper);
                }

                game.Developer = currentDeveloper;

                var currentGenre = genres.FirstOrDefault(x => x.Name == gameDTO.Genre);

                if (currentGenre == null)
                {
                    currentGenre = new Genre()
                    {
                        Name = gameDTO.Genre
                    };

                    genres.Add(currentGenre);
                }

                game.Genre = currentGenre;

                foreach (var tagDTO in gameDTO.Tags)
                {
                    if (string.IsNullOrEmpty(tagDTO))
                    {
                        sb.AppendLine(errorMessage);
                        continue;
                    }

                    var currentTag = tags.FirstOrDefault(x => x.Name == tagDTO);

                    if (currentTag == null)
                    {
                        currentTag = new Tag()
                        {
                            Name = tagDTO,
                        };

                        tags.Add(currentTag);
                    }

                    game.GameTags.Add(new GameTag()
                    {
                        Game = game,
                        Tag = currentTag
                    });
                }

                if (!game.GameTags.Any())
                {
                    continue;
                }

                games.Add(game);
                sb.AppendLine(string.Format(successMessageImportGame, game.Name, game.Genre.Name, game.GameTags.Count));
            }

            context.Games.AddRange(games);
            context.SaveChanges();

            return sb.ToString().Trim();
        }

        public static string ImportUsers(VaporStoreDbContext context, string jsonString)
        {
            var usersDTO = JsonConvert.DeserializeObject<IEnumerable<ImportUserDTO>>(jsonString);

            var sb = new StringBuilder();
            var users = new List<User>();

            foreach (var userDTO in usersDTO)
            {
                if (!IsValid(userDTO))
                {
                    sb.AppendLine(errorMessage);
                    continue;
                }

                var user = new User()
                {
                    Username = userDTO.Username,
                    FullName = userDTO.FullName,
                    Email = userDTO.Email,
                    Age = userDTO.Age
                };

                foreach (var cardDTO in userDTO.Cards)
                {
                    if (!IsValid(cardDTO))
                    {
                        sb.AppendLine(errorMessage);
                        continue;
                    }

                    CardType cardType;
                    var isValidEnum = Enum.TryParse(cardDTO.Type, out cardType);

                    if (!isValidEnum)
                    {
                        sb.AppendLine(errorMessage);
                        continue;
                    }

                    var card = new Card()
                    {
                        Number = cardDTO.Number,
                        Cvc = cardDTO.CVC,
                        Type = cardType,
                    };

                    user.Cards.Add(card);
                }

                users.Add(user);
                sb.AppendLine(string.Format(successsMessageImportUser, user.Username, user.Cards.Count));
            }

            context.Users.AddRange(users);
            context.SaveChanges();

            return sb.ToString().Trim();
        }

        public static string ImportPurchases(VaporStoreDbContext context, string xmlString)
        {
            var xmlSerializer = new XmlSerializer(typeof(ImportPurchaseDTO[]), new XmlRootAttribute("Purchases"));

            var purchaseDTOs = (ImportPurchaseDTO[])xmlSerializer.Deserialize(new StringReader(xmlString));

            var sb = new StringBuilder();
            var purchases = new List<Purchase>();

            foreach (var purchaseDTO in purchaseDTOs)
            {
                if (!IsValid(purchaseDTO))
                {
                    sb.AppendLine(errorMessage);
                    continue;
                }

                PurchaseType purchaseType;
                var isPurchaseType = Enum.TryParse(purchaseDTO.Type, out purchaseType);

                if (!isPurchaseType)
                {
                    sb.AppendLine(errorMessage);
                    continue;
                }                              

                var currentCard = context.Cards.FirstOrDefault(x => x.Number == purchaseDTO.CardNumber);
                var currentGame = context.Games.FirstOrDefault(x => x.Name == purchaseDTO.GameName);

                if (currentCard == null || currentGame == null)
                {
                    sb.AppendLine(errorMessage);
                    continue;
                }

                var purchase = new Purchase()
                {
                    Type = purchaseType,
                    ProductKey = purchaseDTO.ProductKey,
                    Date = DateTime.ParseExact(purchaseDTO.Date, "dd/MM/yyyy HH:mm", CultureInfo.InvariantCulture),
                    Game = currentGame,
                    Card = currentCard,
                };

                purchases.Add(purchase);
                sb.AppendLine(string.Format(successsMessageImportPurchase, purchase.Game.Name, purchase.Card.User.Username));
            }

            context.Purchases.AddRange(purchases);
            context.SaveChanges();

            return sb.ToString().Trim();
        }

        private static bool IsValid(object dto)
        {
            var validationContext = new ValidationContext(dto);
            var validationResult = new List<ValidationResult>();

            return Validator.TryValidateObject(dto, validationContext, validationResult, true);
        }
    }
}
