namespace VaporStore.DataProcessor.Dto.Export
{
    using Newtonsoft.Json;
    using System.Collections.Generic;

    public class ExportGenreDTO
    {
        [JsonProperty("Id")]
        public int Id { get; set; }

        [JsonProperty("Genre")]
        public string Genre { get; set; }

        [JsonProperty("Games")]
        public ExportGameDTO[] Games { get; set; }

        [JsonProperty("TotalPlayers")]
        public int TotalPlayers { get; set; }
    }
}
