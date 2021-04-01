namespace CarDealer
{
    using AutoMapper;
    using CarDealer.DTOs.Import;
    using CarDealer.Models;

    public class CarDealerProfile : Profile
    {
        public CarDealerProfile()
        {
            this.CreateMap<ImportSupplierDTO, Supplier>();
            this.CreateMap<ImportPartDTO, Part>();
            this.CreateMap<ImportCustomerDTO, Customer>();
            this.CreateMap<ImportSaleDTO, Sale>();
        }
    }
}
