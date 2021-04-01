using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using ProductShop.Data;
using ProductShop.DTO;
using ProductShop.Models;

namespace ProductShop
{
    public class StartUp
    {
        static IMapper mapper;

        public static void Main(string[] args)
        {
            var db = new ProductShopContext();

            //db.Database.EnsureDeleted();
            //db.Database.EnsureCreated();

            //var usersJson = File.ReadAllText("../../../Datasets/users.json");
            //var productsJson = File.ReadAllText("../../../Datasets/products.json");
            //var categoriesJson = File.ReadAllText("../../../Datasets/categories.json");
            //var categoryProductsJson = File.ReadAllText("../../../Datasets/categories-products.json");

            //var result = ImportUsers(db, usersJson);
            //var result = ImportProducts(db, productsJson);
            //var result = ImportCategories(db, categoriesJson);
            //var result = ImportCategoryProducts(db, categoryProductsJson);

            var result = GetUsersWithProducts(db);
            Console.WriteLine(result);
        }

        //01. Import Users
        public static string ImportUsers(ProductShopContext context, string inputJson)
        {
            InitializationMapper();

            var usersDTO = JsonConvert.DeserializeObject<IEnumerable<UserInputModel>>(inputJson);

            var users = mapper.Map<IEnumerable<User>>(usersDTO);

            context.Users.AddRange(users);
            context.SaveChanges();

            return $"Successfully imported {users.Count()}";
        }

        //02. Import Products
        public static string ImportProducts(ProductShopContext context, string inputJson)
        {
            InitializationMapper();

            var productsDTO = JsonConvert.DeserializeObject<IEnumerable<ProductInputModel>>(inputJson);

            var products = mapper.Map<IEnumerable<Product>>(productsDTO);

            context.Products.AddRange(products);
            context.SaveChanges();

            return $"Successfully imported {products.Count()}";
        }

        //03. Import Categories
        public static string ImportCategories(ProductShopContext context, string inputJson)
        {
            InitializationMapper();

            var categoriesDTO = JsonConvert.DeserializeObject<IEnumerable<CategoryInputModel>>(inputJson)
                .Where(x => x.Name != null)
                .ToList();

            var categories = mapper.Map<IEnumerable<Category>>(categoriesDTO);

            context.Categories.AddRange(categories);
            context.SaveChanges();

            return $"Successfully imported {categories.Count()}";
        }

        //04. Import Categories and Products
        public static string ImportCategoryProducts(ProductShopContext context, string inputJson)
        {
            InitializationMapper();

            var categoryProductsDTO = JsonConvert.DeserializeObject<IEnumerable<CategoryProductInputModel>>(inputJson);

            var categoryProducts = mapper.Map<IEnumerable<CategoryProduct>>(categoryProductsDTO);

            context.CategoryProducts.AddRange(categoryProducts);
            context.SaveChanges();

            return $"Successfully imported {categoryProducts.Count()}";
        }

        //05. Export Products In Range
        public static string GetProductsInRange(ProductShopContext context)
        {
            var products = context.Products
                .Where(x => x.Price >= 500 && x.Price <= 1000)
                .Select(p => new
                {
                    name = p.Name,
                    price = p.Price,
                    seller = p.Seller.FirstName + " " + p.Seller.LastName
                })
                .OrderBy(x => x.price)
                .ToList();

            var productsJson = JsonConvert.SerializeObject(products, Formatting.Indented);

            return productsJson;
        }

        //06. Export Sold Products
        public static string GetSoldProducts(ProductShopContext context)
        {
            var users = context.Users
                .Where(x => x.ProductsSold.Any(p => p.BuyerId != null))
                .Select(u => new
                {
                    firstName = u.FirstName,
                    lastName = u.LastName,
                    soldProducts = u.ProductsSold.Select(ps => new
                    {
                        name = ps.Name,
                        price = ps.Price,
                        buyerFirstName = ps.Buyer.FirstName,
                        buyerLastName = ps.Buyer.LastName
                    })
                })
                .OrderBy(x => x.lastName)
                .ThenBy(x => x.firstName)
                .ToList();

            var json = JsonConvert.SerializeObject(users, Formatting.Indented);

            return json;

        }

        //07. Export Categories By Products Count
        public static string GetCategoriesByProductsCount(ProductShopContext context)
        {
            var categories = context.Categories
                .Select(x => new
                {
                    category = x.Name,
                    productsCount = x.CategoryProducts.Count(),
                    averagePrice = x.CategoryProducts.Average(y => y.Product.Price).ToString("F2"),
                    totalRevenue = x.CategoryProducts.Sum(y => y.Product.Price).ToString("F2")
                })
                .OrderByDescending(x => x.productsCount)
                .ToList();

            var json = JsonConvert.SerializeObject(categories, Formatting.Indented);

            return json;
        }

        //08. Export Users and Products
        public static string GetUsersWithProducts(ProductShopContext context)
        {
            var users = context.Users
               .AsEnumerable()
               .Where(u => u.ProductsSold.Any(b => b.BuyerId != null))
               .OrderByDescending(p => p.ProductsSold.Count(ps => ps.BuyerId != null))
               .Select(u => new UserDTO
               {
                   FirstName = u.FirstName,
                   LastName = u.LastName,
                   Age = u.Age,
                   SoldProducts = new SoldProductDTO
                   {
                       Count = u.ProductsSold.Count(p => p.BuyerId != null),
                       Products = u.ProductsSold
                           .Where(p => p.BuyerId != null)
                           .Select(p => new ProductDTO
                           {
                               Name = p.Name,
                               Price = p.Price
                           })
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

        private static void InitializationMapper()
        {
            var mapperConfig = new MapperConfiguration(mc =>
            {
                mc.AddProfile<ProductShopProfile>();
            });

            mapper = mapperConfig.CreateMapper();
        }
    }
}