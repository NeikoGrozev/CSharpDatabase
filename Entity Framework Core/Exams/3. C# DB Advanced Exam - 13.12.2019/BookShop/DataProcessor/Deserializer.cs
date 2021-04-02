namespace BookShop.DataProcessor
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.Globalization;
    using System.IO;
    using System.Linq;
    using System.Text;
    using System.Xml.Serialization;
    using BookShop.Data.Models;
    using BookShop.Data.Models.Enums;
    using BookShop.DataProcessor.ImportDto;
    using Data;
    using Newtonsoft.Json;
    using ValidationContext = System.ComponentModel.DataAnnotations.ValidationContext;

    public class Deserializer
    {
        private const string ErrorMessage = "Invalid data!";

        private const string SuccessfullyImportedBook
            = "Successfully imported book {0} for {1:F2}.";

        private const string SuccessfullyImportedAuthor
            = "Successfully imported author - {0} with {1} books.";

        public static string ImportBooks(BookShopContext context, string xmlString)
        {
            var xmlSerializer = new XmlSerializer(typeof(ImportBookDTO[]), new XmlRootAttribute("Books"));

            var bookDTOs = (ImportBookDTO[])xmlSerializer.Deserialize(new StringReader(xmlString));

            var sb = new StringBuilder();
            var books = new List<Book>();

            foreach (var bookDTO in bookDTOs)
            {
                if (!IsValid(bookDTO))
                {
                    sb.AppendLine(ErrorMessage);
                    continue;
                }

                Genre genreType;
                var isGenreType = Enum.TryParse<Genre>(bookDTO.Genre.ToString(), out genreType);

                if (!isGenreType)
                {
                    sb.AppendLine(ErrorMessage);
                    continue;
                }

                var book = new Book()
                {
                    Name = bookDTO.Name,
                    Genre = genreType,
                    Price = bookDTO.Price,
                    Pages = bookDTO.Pages,
                    PublishedOn = DateTime.ParseExact(bookDTO.PublishedOn, "MM/dd/yyyy", CultureInfo.InvariantCulture)
                };

                books.Add(book);
                sb.AppendLine(string.Format(SuccessfullyImportedBook, book.Name, book.Price));
            }

            context.Books.AddRange(books);
            context.SaveChanges();

            return sb.ToString().Trim();
        }

        public static string ImportAuthors(BookShopContext context, string jsonString)
        {
            var authorDTOs = JsonConvert.DeserializeObject<ImportAuthorDTO[]>(jsonString);

            var sb = new StringBuilder();
            var books = context.Books.ToList();
            var authors = new List<Author>();

            foreach (var authorDTO in authorDTOs)
            {
                if (!IsValid(authorDTO))
                {
                    sb.AppendLine(ErrorMessage);
                    continue;
                }

                if(authors.Any(x => x.Email == authorDTO.Email))
                {
                    sb.AppendLine(ErrorMessage);
                    continue;
                }

                if (authorDTO.Books.Length == 0)
                {
                    continue;
                }

                var author = new Author()
                {
                    FirstName = authorDTO.FirstName,
                    LastName = authorDTO.LastName,
                    Email = authorDTO.Email,
                    Phone = authorDTO.Phone,
                };

                foreach (var currentBook in authorDTO.Books)
                {
                    var book = books.FirstOrDefault(x => x.Id == currentBook.Id);

                    if (book == null)
                    {
                        continue;
                    }

                    var authorBook = new AuthorBook()
                    {
                        Author = author,
                        Book = book,
                    };

                    author.AuthorsBooks.Add(authorBook);
                }

                if(author.AuthorsBooks.Count() == 0)
                {
                    sb.AppendLine(ErrorMessage);
                    continue;
                }

                authors.Add(author);
                var authorFullName = author.FirstName + " " + author.LastName;
                sb.AppendLine(String.Format(SuccessfullyImportedAuthor, authorFullName, author.AuthorsBooks.Count));
            }

            context.Authors.AddRange(authors);
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