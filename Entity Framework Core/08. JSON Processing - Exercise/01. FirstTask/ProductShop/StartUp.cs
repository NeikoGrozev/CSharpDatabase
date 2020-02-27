namespace ProductShop
{
    using Newtonsoft.Json;
    using ProductShop.Data;
    using ProductShop.DTO;
    using ProductShop.Models;
    using System;
    using System.Linq;

    public class StartUp
    {
        public static void Main()
        {
            using (var db = new ProductShopContext())
            {
                //db.Database.EnsureCreated();

                //var input = File.ReadAllText("./../../../Datasets/users.json");
                //var input = File.ReadAllText("./../../../Datasets/products.json");
                //var input = File.ReadAllText("./../../../Datasets/categories.json");
                //var input = File.ReadAllText("./../../../Datasets/categories-products.json");

                var result = GetUsersWithProducts(db);

                Console.WriteLine(result);
            }
        }

        public static string ImportUsers(ProductShopContext context, string inputJson)
        {
            var users = JsonConvert.DeserializeObject<User[]>(inputJson)
                .ToList();

            context.Users.AddRange(users);
            context.SaveChanges();

            return $"Successfully imported {users.Count}";
        }

        public static string ImportProducts(ProductShopContext context, string inputJson)
        {
            var products = JsonConvert.DeserializeObject<Product[]>(inputJson)
                .ToList();

            context.Products.AddRange(products);
            context.SaveChanges();

            return $"Successfully imported {products.Count}";
        }

        public static string ImportCategories(ProductShopContext context, string inputJson)
        {
            var categories = JsonConvert.DeserializeObject<Category[]>(inputJson)
                .Where(x => x.Name != null)
                .ToList();

            context.Categories.AddRange(categories);
            context.SaveChanges();

            return $"Successfully imported {categories.Count}";
        }

        public static string ImportCategoryProducts(ProductShopContext context, string inputJson)
        {
            var categoryProducts = JsonConvert.DeserializeObject<CategoryProduct[]>(inputJson)
                .ToList();

            context.CategoryProducts.AddRange(categoryProducts);
            context.SaveChanges();

            return $"Successfully imported {categoryProducts.Count}";
        }

        public static string GetProductsInRange(ProductShopContext context)
        {
            var products = context.Products
                .Where(x => x.Price >= 500 && x.Price <= 1000)
                .OrderBy(x => x.Price)
                .Select(x => new
                {
                    name = x.Name,
                    price = x.Price,
                    seller = $"{x.Seller.FirstName} {x.Seller.LastName}"
                })
                .ToList();

            var json = JsonConvert.SerializeObject(products, Formatting.Indented);

            return json;
        }

        public static string GetSoldProducts(ProductShopContext context)
        {
            var users = context.Users
                .Where(x => x.ProductsSold.Any(y => y.Buyer != null))
                .OrderBy(x => x.LastName)
                .ThenBy(x => x.FirstName)
                .Select(x => new
                {
                    firstName = x.FirstName,
                    lastName = x.LastName,
                    soldProducts = x.ProductsSold
                    .Select(y => new
                    {
                        name = y.Name,
                        price = y.Price,
                        buyerFirstName = y.Buyer.FirstName,
                        buyerLastName = y.Buyer.LastName,
                    }).ToList()
                })
                .ToList();

            var json = JsonConvert.SerializeObject(users, Formatting.Indented);

            return json;
        }

        public static string GetCategoriesByProductsCount(ProductShopContext context)
        {
            var categories = context.Categories
                .OrderByDescending(c => c.CategoryProducts.Count)
                .Select(x => new
                {
                    category = x.Name,
                    productsCount = x.CategoryProducts.Count(),
                    averagePrice = x.CategoryProducts
                        .Average(y => y.Product.Price)
                        .ToString("F2"),
                    totalRevenue = x.CategoryProducts
                        .Sum(y => y.Product.Price)
                        .ToString("F2")
                })
                .ToList();

            var json = JsonConvert.SerializeObject(categories, Formatting.Indented);

            return json;
        }

        public static string GetUsersWithProducts(ProductShopContext context)
        {            
            var users = context.Users
            .Where(u => u.ProductsSold.Any(b => b.BuyerId != null))
            .OrderByDescending(p => p.ProductsSold.Count(ps => ps.BuyerId != null))
            .Select(u => new UsersDTO
            {
                FirstName = u.FirstName,
                LastName = u.LastName,
                Age = u.Age,
                SoldProducts = new SoldProductsDTO
                {
                    Count = u.ProductsSold.Count(p => p.BuyerId != null),
                    Products = u.ProductsSold
                        .Where(p => p.BuyerId != null)
                        .Select(p => new ProductsDTO
                        {
                            Name = p.Name,
                            Price = p.Price
                        })
                        .ToList()
                }
            })
            .ToList();

            var result = new UsersAndProductsDTO()
            {
                UsersCount = users.Count(),
                Users = users
            };

            var json = JsonConvert.SerializeObject(result, Formatting.Indented,
                                                new JsonSerializerSettings()
                                                {
                                                    NullValueHandling = NullValueHandling.Ignore
                                                });

            return json;
        }
    }
}