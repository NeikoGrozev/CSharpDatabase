namespace VaporStore.DataProcessor
{
    using System;
    using System.Globalization;
    using System.IO;
    using System.Linq;
    using System.Text;
    using System.Xml.Serialization;
    using Data;
    using Newtonsoft.Json;
    using VaporStore.Data.Models.Enums;
    using VaporStore.DataProcessor.Dto.Export;

    public static class Serializer
    {
        public static string ExportGamesByGenres(VaporStoreDbContext context, string[] genreNames)
        {
            var genres = context.Genres
                .ToArray()
                .Where(x => genreNames.Contains(x.Name))
                .Select(g => new ExportGenreDTO()
                {
                    Id = g.Id,
                    Genre = g.Name,
                    Games = g.Games
                    .Where(x => x.Purchases.Any())
                    .Select(x => new ExportGameDTO()
                    {
                        Id = x.Id,
                        Title = x.Name,
                        Developer = x.Developer.Name,
                        Tags = string.Join(", ", x.GameTags.Select(gt => gt.Tag.Name).ToArray()),
                        Players = x.Purchases.Count(),
                    })
                    .OrderByDescending(x => x.Players)
                    .ThenBy(x => x.Id)
                    .ToArray(),
                    TotalPlayers = g.Games.Sum(x => x.Purchases.Count()),
                })
                .OrderByDescending(x => x.TotalPlayers)
                .ThenBy(x => x.Id)
                .ToList();

            var json = JsonConvert.SerializeObject(genres, Formatting.Indented);

            return json;
        }

        public static string ExportUserPurchasesByType(VaporStoreDbContext context, string storeType)
        {
            PurchaseType purchaseTypeEnum = Enum.Parse<PurchaseType>(storeType);

            var users = context.Users
                .ToArray()
                .Where(x => x.Cards.Any(y => y.Purchases.Any()))
                .Select(u => new ExportUserDTO()
                {
                    Username = u.Username,
                    Purchases = context.Purchases
                    .ToArray()
                    .Where(p => p.Card.User.FullName == u.FullName && p.Type == purchaseTypeEnum)
                    .OrderBy(p => p.Date)
                    .Select(p => new ExportPurchaseDTO()
                    {
                        Card = p.Card.Number,
                        Cvc = p.Card.Cvc,
                        Date = p.Date.ToString("yyyy-MM-dd HH:mm", CultureInfo.InvariantCulture),
                        Game = new ExportUserGameDTO()
                        {
                            Title = p.Game.Name,
                            Genre = p.Game.Genre.Name,
                            Price = p.Game.Price,
                        }
                    })
                    .ToArray(),
                    TotalSpent = context.Purchases
                    .Where(p => p.Card.User.FullName == u.FullName && p.Type == purchaseTypeEnum)
                    .Sum(x => x.Game.Price)
                })
                .Where(x => x.Purchases.Length > 0)
                .OrderByDescending(x => x.TotalSpent)
                .ThenBy(x => x.Username)
                .ToArray();

            var xmlSerializer = new XmlSerializer(typeof(ExportUserDTO[]), new XmlRootAttribute("Users"));

            var sb = new StringBuilder();
            var namespaces = new XmlSerializerNamespaces();
            namespaces.Add(string.Empty, string.Empty);

            xmlSerializer.Serialize(new StringWriter(sb), users, namespaces);

            return sb.ToString().Trim();
        }
    }
}