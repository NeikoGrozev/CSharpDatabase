namespace ProductShop
{
    using AutoMapper;
    using ProductShop.Data;
    using ProductShop.Dtos.Export;
    using ProductShop.Dtos.Import;
    using ProductShop.Models;
    using System;
    using System.IO;
    using System.Linq;
    using System.Text;
    using System.Xml.Serialization;

    public class StartUp
    {
        static IMapper mapper;

        public static void Main(string[] args)
        {
            var db = new ProductShopContext();
            //db.Database.EnsureDeleted();
            //db.Database.EnsureCreated();

            //var usersXml = File.ReadAllText("../../../Datasets/users.xml");
            //var productsXml = File.ReadAllText("../../../Datasets/products.xml");
            //var categoryXml = File.ReadAllText("../../../Datasets/categories.xml");
            //var categoryProductXml = File.ReadAllText("../../..//Datasets/categories-products.xml");

            //var result = ImportCategoryProducts(db, categoryProductXml);
            var result = GetUsersWithProducts(db);
            Console.WriteLine(result);
        }

        //01. Import Users
        public static string ImportUsers(ProductShopContext context, string inputXml)
        {
            InitializationMapper();

            var xmlSerializer = new XmlSerializer(typeof(ImportUserDTO[]), new XmlRootAttribute("Users"));
            var userDTOs = (ImportUserDTO[])xmlSerializer.Deserialize(new StringReader(inputXml));

            var users = mapper.Map<User[]>(userDTOs);

            context.Users.AddRange(users);
            context.SaveChanges();

            return $"Successfully imported {users.Length}";
        }

        //02. Import Products
        public static string ImportProducts(ProductShopContext context, string inputXml)
        {
            InitializationMapper();

            var xmlSerializer = new XmlSerializer(typeof(ImportProductDTO[]), new XmlRootAttribute("Products"));
            var productDTOs = (ImportProductDTO[])xmlSerializer.Deserialize(new StringReader(inputXml));

            var products = mapper.Map<Product[]>(productDTOs);

            context.Products.AddRange(products);
            context.SaveChanges();

            return $"Successfully imported {products.Length}";
        }

        //03. Import Categories
        public static string ImportCategories(ProductShopContext context, string inputXml)
        {
            InitializationMapper();

            var xmlSerializer = new XmlSerializer(typeof(ImportCategoryDTO[]), new XmlRootAttribute("Categories"));
            var categoryDTOs = (ImportCategoryDTO[])xmlSerializer.Deserialize(new StringReader(inputXml));

            var categories = mapper.Map<Category[]>(categoryDTOs.Where(x => x.Name != null))
                            .Distinct();

            context.Categories.AddRange(categories);
            context.SaveChanges();

            return $"Successfully imported {categories.Count()}";
        }

        //04. Import Categories and Products
        public static string ImportCategoryProducts(ProductShopContext context, string inputXml)
        {
            InitializationMapper();

            var xmlSerializer = new XmlSerializer(typeof(ImportCategoryProductDTO[]),
                                                    new XmlRootAttribute("CategoryProducts"));

            var categoryProductDTOs = (ImportCategoryProductDTO[])xmlSerializer.Deserialize(new StringReader(inputXml));

            var categoryProducts = mapper.Map<CategoryProduct[]>(categoryProductDTOs);

            context.CategoryProducts.AddRange(categoryProducts);
            context.SaveChanges();

            return $"Successfully imported {categoryProducts.Length}";
        }

        //05. Export Products In Range
        public static string GetProductsInRange(ProductShopContext context)
        {
            var products = context.Products
                .Where(x => x.Price >= 500 && x.Price <= 1000)
                .OrderBy(x => x.Price)
                .Select(x => new ExportProductInRangeDTO()
                {
                    Name = x.Name,
                    Price = x.Price,
                    Buyer = x.Buyer.FirstName + " " + x.Buyer.LastName
                })
                .Take(10)
                .ToArray();

            StringBuilder sb = new StringBuilder();
            var namespaces = new XmlSerializerNamespaces();
            namespaces.Add(String.Empty, String.Empty);

            var xmlSerializer = new XmlSerializer(typeof(ExportProductInRangeDTO[]), new XmlRootAttribute("Products"));

            xmlSerializer.Serialize(new StringWriter(sb), products, namespaces);

            return sb.ToString().Trim();
        }

        //06. Export Sold Products
        public static string GetSoldProducts(ProductShopContext context)
        {
            var users = context.Users
                .Where(x => x.ProductsSold.Count() > 0)
                .OrderBy(x => x.LastName)
                .ThenBy(x => x.FirstName)
                .Take(5)
                .Select(x => new ExportUserSoldProductDTO()
                {
                    FirstName = x.FirstName,
                    LastName = x.LastName,
                    SoldProducts = x.ProductsSold
                        .Select(ps => new ExportSoldProductDTO()
                        {
                            Name = ps.Name,
                            Price = ps.Price
                        })
                        .ToArray()
                })
                .ToArray();

            var xmlSelializer = new XmlSerializer(typeof(ExportUserSoldProductDTO[]), new XmlRootAttribute("Users"));

            var sb = new StringBuilder();
            var namespaces = new XmlSerializerNamespaces();
            namespaces.Add(string.Empty, string.Empty);

            xmlSelializer.Serialize(new StringWriter(sb), users, namespaces);

            return sb.ToString().Trim();
        }

        //07. Export Categories By Products Count
        public static string GetCategoriesByProductsCount(ProductShopContext context)
        {
            var categoties = context.Categories
                .Select(x => new ExportCategoryByProductCountDTO()
                {
                    Name = x.Name,
                    Count = x.CategoryProducts.Count(),
                    AveragePrice = x.CategoryProducts.Average(cp => cp.Product.Price),
                    TotalRevenue = x.CategoryProducts.Sum(cp => cp.Product.Price)
                })
                .OrderByDescending(x => x.Count)
                .ThenBy(x => x.TotalRevenue)
                .ToArray();

            var xmlSerializer = new XmlSerializer(typeof(ExportCategoryByProductCountDTO[]), new XmlRootAttribute("Categories"));

            var sb = new StringBuilder();
            var namespaces = new XmlSerializerNamespaces();
            namespaces.Add(string.Empty, string.Empty);

            xmlSerializer.Serialize(new StringWriter(sb), categoties, namespaces);

            return sb.ToString().Trim();
        }

        //08. Export Users and Products
        public static string GetUsersWithProducts(ProductShopContext context)
        {
            var users = new ExportUsersDTO()
            {
                Count = context.Users.Count(u => u.ProductsSold.Any(x => x.Buyer != null)),
                Users = context.Users
                .ToArray()
                    .Where(x => x.ProductsSold.Count() > 0)
                    .OrderByDescending(x => x.ProductsSold.Count())
                    .Take(10)
                    .Select(u => new ExportUserDTO()
                    {
                        FirstName = u.FirstName,
                        LastName = u.LastName,
                        Age = u.Age,
                        SoldProducts = new ExportSoldProductsDTO()
                        {
                            Count = u.ProductsSold.Count(),
                            Products = u.ProductsSold
                            .Select(ps => new ExportProductDTO()
                            {
                                Name = ps.Name,
                                Price = ps.Price
                            })
                            .OrderByDescending(x => x.Price)
                            .ToArray()
                        }
                    })
                    .ToArray()
            };

            var xmlSerializer = new XmlSerializer(typeof(ExportUsersDTO), new XmlRootAttribute("Users"));

            var sb = new StringBuilder();
            var namespaces = new XmlSerializerNamespaces();
            namespaces.Add(string.Empty, string.Empty);

            xmlSerializer.Serialize(new StringWriter(sb), users, namespaces);

            return sb.ToString().Trim();
        }

        private static void InitializationMapper()
        {
            var mapperConfiguration = new MapperConfiguration(mc =>
            {
                mc.AddProfile<ProductShopProfile>();
            });

            mapper = mapperConfiguration.CreateMapper();
        }
    }
}