namespace ProductShop.DTO
{
    using Newtonsoft.Json;
    using System.Collections.Generic;

    public class SoldProductsDTO
    {
        [JsonProperty("count")]
        public int Count { get; set; }

        [JsonProperty("products")]
        public ICollection<ProductsDTO> Products { get; set; }
    }
}
