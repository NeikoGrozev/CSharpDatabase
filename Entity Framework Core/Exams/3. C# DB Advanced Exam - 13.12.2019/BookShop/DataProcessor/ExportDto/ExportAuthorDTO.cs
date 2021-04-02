namespace BookShop.DataProcessor.ExportDto
{
    using Newtonsoft.Json;
    using System.Collections.Generic;

    public class ExportAuthorDTO
    {
        [JsonProperty("AuthorName")]
        public string AuthorName { get; set; }

        [JsonProperty("Books")]
        public ICollection<ExportAuthorBookDTO> Books { get; set; }
    }
}
