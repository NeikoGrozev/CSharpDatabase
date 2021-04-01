namespace BookShop
{
    using BookShop.Models;
    using BookShop.Models.Enums;
    using Data;
    using Initializer;
    using System;
    using System.Collections.Generic;
    using System.Globalization;
    using System.Linq;
    using System.Text;

    public class StartUp
    {
        public static void Main()
        {
            using (var db = new BookShopContext())
            {
                //DbInitializer.ResetDatabase(db);

                //string input = Console.ReadLine();
                //int input = int.Parse(Console.ReadLine());

                int result = RemoveBooks(db);

                Console.WriteLine(result);
            }
        }

        public static string GetBooksByAgeRestriction(BookShopContext context, string command)
        {
            var books = context.Books
                .Where(x => x.AgeRestriction.ToString().ToLower() == command.ToLower())
                .OrderBy(x => x.Title)
                .Select(b => new
                {
                    b.Title
                }).ToList();

            StringBuilder sb = new StringBuilder();

            foreach (var book in books)
            {
                sb.AppendLine(book.Title);
            }

            return sb.ToString().TrimEnd();
        }

        public static string GetGoldenBooks(BookShopContext context)
        {
            var books = context.Books
                .Where(x => x.EditionType.ToString() == "Gold" && x.Copies < 5000)
                .OrderBy(x => x.BookId)
                .Select(b => new
                {
                    b.Title
                }).ToList();

            StringBuilder sb = new StringBuilder();

            foreach (var book in books)
            {
                sb.AppendLine(book.Title);
            }

            return sb.ToString().TrimEnd();
        }

        public static string GetBooksByPrice(BookShopContext context)
        {
            var books = context.Books
                .Where(x => x.Price > 40)
                .OrderByDescending(x => x.Price)
                .Select(x => new
                {
                    x.Title,
                    x.Price,
                }).ToList();

            StringBuilder sb = new StringBuilder();

            foreach (var book in books)
            {
                sb.AppendLine($"{book.Title} - ${book.Price:F2}");
            }

            return sb.ToString().TrimEnd();
        }

        public static string GetBooksNotReleasedIn(BookShopContext context, int year)
        {
            var books = context.Books
                .Where(x => x.ReleaseDate.Value.Year != year)
                .OrderBy(x => x.BookId)
                .Select(x => x.Title)
                .ToList();

            StringBuilder sb = new StringBuilder();

            foreach (var book in books)
            {
                sb.AppendLine(book);
            }

            return sb.ToString().TrimEnd();
        }

        public static string GetBooksByCategory(BookShopContext context, string input)
        {
            List<string> categories = input
                .Split(" ", StringSplitOptions.RemoveEmptyEntries)
                .Select(x => x.ToLower())
                .ToList();

            var books = new List<string>();

            foreach (var item in categories)
            {
                var currentbooks = context.Books
                     .Where(x => x.BookCategories.Any(c => c.Category.Name.ToLower() == item))
                     .Select(x => x.Title)
                     .ToList();

                books.AddRange(currentbooks);
            }

            books = books.OrderBy(x => x).ToList();

            StringBuilder sb = new StringBuilder();

            foreach (var book in books)
            {
                sb.AppendLine(book);
            }

            return sb.ToString().TrimEnd();
        }

        public static string GetBooksReleasedBefore(BookShopContext context, string date)
        {
            var currentDate = DateTime.ParseExact(date, "dd-MM-yyyy", System.Globalization.CultureInfo.InvariantCulture);

            var books = context.Books
                .Where(x => x.ReleaseDate < currentDate)
                .OrderByDescending(x => x.ReleaseDate)
                .Select(x => new
                {
                    Title = x.Title,
                    EditionType = Enum.Parse<EditionType>(x.EditionType.ToString()),
                    Price = x.Price,
                }).ToList();

            StringBuilder sb = new StringBuilder();

            foreach (var book in books)
            {
                sb.AppendLine($"{book.Title} - {book.EditionType} - ${book.Price:F2}");
            }

            return sb.ToString().TrimEnd();
        }

        public static string GetAuthorNamesEndingIn(BookShopContext context, string input)
        {
            var authors = context.Authors
                .Where(x => x.FirstName.EndsWith(input))
                .OrderBy(x => x.FirstName)
                .ThenBy(x => x.LastName)
                .Select(x => new
                {
                    FirstName = x.FirstName,
                    LastName = x.LastName,
                }).ToList();

            StringBuilder sb = new StringBuilder();

            foreach (var autor in authors)
            {
                sb.AppendLine($"{autor.FirstName} {autor.LastName}");
            }

            return sb.ToString().TrimEnd();
        }

        public static string GetBookTitlesContaining(BookShopContext context, string input)
        {
            var books = context.Books
                .Where(x => x.Title.ToLower().Contains(input.ToLower()))
                .Select(x => x.Title)
                .OrderBy(x => x)
                .ToList();

            StringBuilder sb = new StringBuilder();

            foreach (var book in books)
            {
                sb.AppendLine(book);
            }

            return sb.ToString().TrimEnd();
        }

        public static string GetBooksByAuthor(BookShopContext context, string input)
        {
            var booksAndAuthors = context.Books
                .Where(x => x.Author.LastName.ToLower().StartsWith(input.ToLower()))
                .OrderBy(x => x.BookId)
                .Select(x => new
                {
                    Title = x.Title,
                    Author = $"{x.Author.FirstName} {x.Author.LastName}",
                })
                .ToList();

            StringBuilder sb = new StringBuilder();

            foreach (var item in booksAndAuthors)
            {
                sb.AppendLine($"{item.Title.ToString()} ({item.Author})");
            }

            return sb.ToString().TrimEnd();
        }

        public static int CountBooks(BookShopContext context, int lengthCheck)
        {
            var numbersOfBbooks = context.Books
                .Where(x => x.Title.Length > lengthCheck)
                .Count();

            return numbersOfBbooks;
        }

        public static string CountCopiesByAuthor(BookShopContext context)
        {
            var authors = context.Authors
                .Select(x => new
                {
                    Name = $"{x.FirstName} {x.LastName}",
                    Copies = x.Books.Sum(b => b.Copies),
                })
                .OrderByDescending(x => x.Copies)
                .ToList();

            StringBuilder sb = new StringBuilder();

            foreach (var author in authors)
            {
                sb.AppendLine($"{author.Name} - {author.Copies}");
            }

            return sb.ToString().TrimEnd();
        }

        public static string GetTotalProfitByCategory(BookShopContext context)
        {
            var categorys = context.Categories
                .Select(x => new
                {
                    Name = x.Name,
                    Sum = x.CategoryBooks.Sum(b => b.Book.Price * b.Book.Copies),
                })
                .OrderByDescending(x => x.Sum)
                .ThenBy(x => x.Name)
                .ToList();

            StringBuilder sb = new StringBuilder();

            foreach (var item in categorys)
            {
                sb.AppendLine($"{item.Name} ${item.Sum:F2}");
            }

            return sb.ToString().TrimEnd();
        }

        public static string GetMostRecentBooks(BookShopContext context)
        {
            var categorys = context.Categories
                .OrderBy(x => x.Name)
                .Select(x => new
                {
                    Name = x.Name,
                    Book = x.CategoryBooks
                    .OrderByDescending(cb => cb.Book.ReleaseDate)
                    .Take(3)
                    .Select(cb => new
                    {
                        BookName = cb.Book.Title,
                        BookDate = cb.Book.ReleaseDate,
                    })
                })
                .ToList();

            StringBuilder sb = new StringBuilder();

            foreach (var c in categorys)
            {
                sb.AppendLine($"--{c.Name}");

                foreach (var b in c.Book)
                {
                    sb.AppendLine($"{b.BookName} ({b.BookDate.Value.Year})");
                }
            }

            return sb.ToString().TrimEnd();
        }

        public static void IncreasePrices(BookShopContext context)
        {
            var books = context.Books
                .Where(x => x.ReleaseDate.Value.Year < 2010)
                .ToList();

            foreach (var book in books)
            {
                book.Price += 5;
            }

            context.SaveChanges();
        }

        public static int RemoveBooks(BookShopContext context)
        {
            var numbersOfBooks = context.Books
                .Where(x => x.Copies < 4200)
                .Count();

            var books = context.Books
                .Where(x => x.Copies < 4200)
                .ToList();

            foreach (var book in books)
            {
                context.Books.Remove(book);
            }

            context.SaveChanges();

            return numbersOfBooks;
        }
    }
}
