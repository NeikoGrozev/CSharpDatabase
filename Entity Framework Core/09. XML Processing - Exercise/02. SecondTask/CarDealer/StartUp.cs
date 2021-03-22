namespace CarDealer
{
    using AutoMapper;
    using CarDealer.Data;
    using CarDealer.DTOs.Export;
    using CarDealer.DTOs.Import;
    using CarDealer.Models;
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Text;
    using System.Xml.Serialization;

    public class StartUp
    {
        static IMapper mapper;

        public static void Main(string[] args)
        {
            var db = new CarDealerContext();
            //db.Database.EnsureDeleted();
            //db.Database.EnsureCreated();

            //var suppliersXml = File.ReadAllText("../../../Datasets/suppliers.xml");
            //var partsXml = File.ReadAllText("../../../Datasets/parts.xml");
            //var carsXml = File.ReadAllText("../../../Datasets/cars.xml");
            //var customersXml = File.ReadAllText("../../../Datasets/customers.xml");
            //var salesXml = File.ReadAllText("../../../Datasets/sales.xml");


            //var result = ImportSales(db, salesXml);
            var result = GetSalesWithAppliedDiscount(db);
            Console.WriteLine(result);
        }

        //09. Import Suppliers
        public static string ImportSuppliers(CarDealerContext context, string inputXml)
        {
            InitializationMapper();

            var xmlSerializer = new XmlSerializer(typeof(ImportSupplierDTO[]), new XmlRootAttribute("Suppliers"));

            var supplierDTOs = (ImportSupplierDTO[])xmlSerializer.Deserialize(new StringReader(inputXml));

            var suppliers = mapper.Map<Supplier[]>(supplierDTOs);

            context.Suppliers.AddRange(suppliers);
            context.SaveChanges();

            return $"Successfully imported {suppliers.Length}";
        }

        //10. Import Parts
        public static string ImportParts(CarDealerContext context, string inputXml)
        {
            InitializationMapper();

            var xmlSerializer = new XmlSerializer(typeof(ImportPartDTO[]), new XmlRootAttribute("Parts"));

            var suppliersId = context.Suppliers
                .Select(x => x.Id)
                .ToList();

            var partDTOs = ((ImportPartDTO[])xmlSerializer.Deserialize(new StringReader(inputXml)))
                            .Where(x => suppliersId.Contains(x.SupplierId))
                            .ToArray();

            var parts = mapper.Map<Part[]>(partDTOs);

            context.Parts.AddRange(parts);
            context.SaveChanges();

            return $"Successfully imported {parts.Length}";
        }

        //11. Import Cars
        public static string ImportCars(CarDealerContext context, string inputXml)
        {
            var xmlSerializer = new XmlSerializer(typeof(ImportCarDTO[]), new XmlRootAttribute("Cars"));

            var carDTOs = (ImportCarDTO[])xmlSerializer.Deserialize(new StringReader(inputXml));

            var parts = context.Parts
                .Select(x => x.Id)
                .ToArray();

            var cars = carDTOs
                .Select(c => new Car
                {
                    Make = c.Make,
                    Model = c.Model,
                    TravelledDistance = c.TraveledDistance,
                    PartCars = c.Parts.Select(x => x.Id)
                    .Distinct()
                    .Intersect(parts)
                    .Select(pc => new PartCar
                    {
                        PartId = pc
                    })
                    .ToList()
                });

            context.Cars.AddRange(cars);
            context.SaveChanges();

            return $"Successfully imported {cars.Count()}";
        }

        //12. Import Customers
        public static string ImportCustomers(CarDealerContext context, string inputXml)
        {
            InitializationMapper();

            var xmlSerializer = new XmlSerializer(typeof(ImportCustomerDTO[]), new XmlRootAttribute("Customers"));

            var customerDTOs = (ImportCustomerDTO[])xmlSerializer.Deserialize(new StringReader(inputXml));

            var customers = mapper.Map<Customer[]>(customerDTOs);

            context.Customers.AddRange(customers);
            context.SaveChanges();

            return $"Successfully imported {customers.Length}";
        }

        //13. Import Sales
        public static string ImportSales(CarDealerContext context, string inputXml)
        {
            InitializationMapper();

            var xmlSerializer = new XmlSerializer(typeof(ImportSaleDTO[]), new XmlRootAttribute("Sales"));

            var carsId = context.Cars
                .Select(x => x.Id)
                .ToArray();

            var saleDTOs = ((ImportSaleDTO[])xmlSerializer.Deserialize(new StringReader(inputXml)))
                            .Where(x => carsId.Contains(x.CarId));

            var sales = mapper.Map<Sale[]>(saleDTOs);

            context.Sales.AddRange(sales);
            context.SaveChanges();

            return $"Successfully imported {sales.Length}";
        }

        //14. Export Cars With Distance
        public static string GetCarsWithDistance(CarDealerContext context)
        {
            var cars = context.Cars
                .Where(x => x.TravelledDistance > 2_000_000)
                .Select(x => new ExportCarsWithDistanceDTO
                {
                    Make = x.Make,
                    Model = x.Model,
                    TravelledDistance = x.TravelledDistance
                })
                .OrderBy(x => x.Make)
                .ThenBy(x => x.Model)
                .Take(10)
                .ToArray();

            var xmlSerializer = new XmlSerializer(typeof(ExportCarsWithDistanceDTO[]), new XmlRootAttribute("cars"));

            var sb = new StringBuilder();
            var namespaces = new XmlSerializerNamespaces();
            namespaces.Add(string.Empty, string.Empty);

            xmlSerializer.Serialize(new StringWriter(sb), cars, namespaces);

            return sb.ToString().Trim();
        }

        //15. Export Cars From Make BMW
        public static string GetCarsFromMakeBmw(CarDealerContext context)
        {
            var cars = context.Cars
                .Where(x => x.Make == "BMW")
                .Select(c => new ExportCarFromMakeBMWDTO
                {
                    Id = c.Id,
                    Model = c.Model,
                    TravelledDistance = c.TravelledDistance
                })
                .OrderBy(x => x.Model)
                .ThenByDescending(x => x.TravelledDistance)
                .ToArray();

            var xmlSerializer = new XmlSerializer(typeof(ExportCarFromMakeBMWDTO[]), new XmlRootAttribute("cars"));

            var sb = new StringBuilder();
            var namespaces = new XmlSerializerNamespaces();
            namespaces.Add(string.Empty, string.Empty);

            xmlSerializer.Serialize(new StringWriter(sb), cars, namespaces);

            return sb.ToString().Trim();
        }

        //16. Export Local Suppliers
        public static string GetLocalSuppliers(CarDealerContext context)
        {
            var suppliers = context.Suppliers
                .Where(x => x.IsImporter == false)
                .Select(s => new ExportLocalSupplierDTO
                {
                    Id = s.Id,
                    Name = s.Name,
                    PartsCount = s.Parts.Count()
                })
                .ToArray();

            var xmlSerializer = new XmlSerializer(typeof(ExportLocalSupplierDTO[]), new XmlRootAttribute("suppliers"));

            var sb = new StringBuilder();
            var namespaces = new XmlSerializerNamespaces();
            namespaces.Add(string.Empty, string.Empty);

            xmlSerializer.Serialize(new StringWriter(sb), suppliers, namespaces);

            return sb.ToString().Trim();
        }

        //17. Export Cars With Their List Of Parts
        public static string GetCarsWithTheirListOfParts(CarDealerContext context)
        {
            var cars = context.Cars
                .Select(c => new ExportCarWithPartsDTO
                {
                    Make = c.Make,
                    Model = c.Model,
                    TravelledDistance = c.TravelledDistance,
                    Parts = c.PartCars.Select(ps => new ExportPartDTO
                    {
                        Name = ps.Part.Name,
                        Price = ps.Part.Price
                    })
                    .OrderByDescending(x => x.Price)
                    .ToArray()
                })
                .OrderByDescending(x => x.TravelledDistance)
                .ThenBy(x => x.Model)
                .Take(5)
                .ToArray();

            var xmlSerializer = new XmlSerializer(typeof(ExportCarWithPartsDTO[]), new XmlRootAttribute("cars"));

            var sb = new StringBuilder();
            var namespaces = new XmlSerializerNamespaces();
            namespaces.Add(string.Empty, string.Empty);

            xmlSerializer.Serialize(new StringWriter(sb), cars, namespaces);

            return sb.ToString().Trim();
        }

        //18. Export Total Sales By Customer
        public static string GetTotalSalesByCustomer(CarDealerContext context)
        {
            var customers = context.Customers
                .Where(x => x.Sales.Count > 0)
                .Select(c => new ExportTotalSalesByCustomerDTO
                {
                    FullName = c.Name,
                    BoughtCars = c.Sales.Count(),
                    SpentMoney = c.Sales.Select(s => s.Car)
                                .SelectMany(x => x.PartCars)
                                .Sum(x => x.Part.Price)
                })
                .OrderByDescending(x => x.SpentMoney)
                .ToArray();

            var xmlSerializer = new XmlSerializer(typeof(ExportTotalSalesByCustomerDTO[]), new XmlRootAttribute("customers"));

            var sb = new StringBuilder();
            var namespaces = new XmlSerializerNamespaces();
            namespaces.Add(string.Empty, string.Empty);

            xmlSerializer.Serialize(new StringWriter(sb), customers, namespaces);

            return sb.ToString().Trim();
        }

        //19. Export Sales With Applied Discount
        public static string GetSalesWithAppliedDiscount(CarDealerContext context)
        {
            var sales = context.Sales
                .Select(x => new ExportSaleDTO
                {
                    Car = new ExportCarDTO
                    {
                        Make = x.Car.Make,
                        Model = x.Car.Model,
                        TravelledDistance = x.Car.TravelledDistance
                    },
                    Discount = x.Discount,
                    CustomerName = x.Customer.Name,
                    Price = x.Car.PartCars.Sum(pc => pc.Part.Price),
                    PriceWithDiscount = x.Car.PartCars.Sum(pc => pc.Part.Price) 
                                    - x.Car.PartCars.Sum(pc => pc.Part.Price) * x.Discount / 100M
                })
                .ToArray();

            var xmlSerializer = new XmlSerializer(typeof(ExportSaleDTO[]), new XmlRootAttribute("sales"));

            var sb = new StringBuilder();
            var namespaces = new XmlSerializerNamespaces();
            namespaces.Add(string.Empty, string.Empty);

            xmlSerializer.Serialize(new StringWriter(sb), sales, namespaces);

            return sb.ToString().Trim();
        }

        private static void InitializationMapper()
        {
            var mapperCongiguration = new MapperConfiguration(mc =>
            {
                mc.AddProfile<CarDealerProfile>();
            });

            mapper = mapperCongiguration.CreateMapper();
        }
    }
}