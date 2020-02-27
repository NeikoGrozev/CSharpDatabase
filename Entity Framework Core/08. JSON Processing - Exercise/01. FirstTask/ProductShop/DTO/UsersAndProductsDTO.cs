namespace ProductShop.DTO
{
    using Newtonsoft.Json;
    using System.Collections.Generic;

    public class UsersAndProductsDTO
    {
        [JsonProperty("usersCount")]
        public int UsersCount { get; set; }

        [JsonProperty("users")]
        public ICollection<UsersDTO> Users { get; set; }
    }
}
