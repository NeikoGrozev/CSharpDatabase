namespace BookShop.DataProcessor.ExportDto
{
    using Newtonsoft.Json;

    public class ExportAuthorBookDTO
    {
        [JsonProperty("BookName")]
        public string BookName { get; set; }

        [JsonProperty("BookPrice")]
        public string BookPrice { get; set; }
    }
}
