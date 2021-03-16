namespace CarDealer
{
    using AutoMapper;
    using CarDealer.DTO;
    using CarDealer.Models;

    public class CarDealerProfile : Profile
    {
        public CarDealerProfile()
        {
            this.CreateMap<SupplierDTO, Supplier>();
            this.CreateMap<PartDTO, Part>();
            this.CreateMap<CustomerDTO, Customer>();
            this.CreateMap<SaleDTO, Sale>();
        }
    }
}
