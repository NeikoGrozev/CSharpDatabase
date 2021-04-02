namespace BookShop.DataProcessor
{
    using System;
    using System.Globalization;
    using System.IO;
    using System.Linq;
    using System.Text;
    using System.Xml;
    using System.Xml.Serialization;
    using BookShop.Data.Models.Enums;
    using BookShop.DataProcessor.ExportDto;
    using Data;
    using Newtonsoft.Json;
    using Formatting = Newtonsoft.Json.Formatting;

    public class Serializer
    {
        public static string ExportMostCraziestAuthors(BookShopContext context)
        {
            var author = context.Authors
                 .Select(a => new ExportAuthorDTO()
                 {
                     AuthorName = a.FirstName + " " + a.LastName,
                     Books = a.AuthorsBooks
                     .OrderByDescending(x => x.Book.Price)
                     .Select(ab => new ExportAuthorBookDTO()
                     {
                         BookName = ab.Book.Name,
                         BookPrice = ab.Book.Price.ToString("F2"),
                     })
                     .ToList()
                 })
                 .ToList()
                 .OrderByDescending(x => x.Books.Count())
                 .ThenBy(x => x.AuthorName);

            var json = JsonConvert.SerializeObject(author, Formatting.Indented);

            return json;        
        }

        public static string ExportOldestBooks(BookShopContext context, DateTime date)
        {
          

            var books = context.Books
                .Where(x => x.PublishedOn < date && x.Genre == Genre.Science)
                .ToArray()
                .OrderByDescending(x => x.Pages)
                .ThenByDescending(x => x.PublishedOn)
                .Take(10)
                .Select(b => new ExportBookDTO()
                {
                    Pages = b.Pages,
                    Name = b.Name,
                    Date = b.PublishedOn.ToString("d", CultureInfo.InvariantCulture),
                })
                .ToArray();

            var xmlSerializer = new XmlSerializer(typeof(ExportBookDTO[]), new XmlRootAttribute("Books"));

            var sb = new StringBuilder();
            var namespaces = new XmlSerializerNamespaces();
            namespaces.Add(string.Empty, string.Empty);

            xmlSerializer.Serialize(new StringWriter(sb), books, namespaces);

            return sb.ToString().Trim();
        }
    }
}