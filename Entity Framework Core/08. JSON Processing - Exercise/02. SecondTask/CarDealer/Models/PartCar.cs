using Castle.Components.DictionaryAdapter;

namespace CarDealer.Models
{
    public class PartCar
    {
        [Key("PartId")]
        public int PartId { get; set; }
        public Part Part { get; set; }

        [Key("CarId")]
        public int CarId { get; set; }
        public Car Car { get; set; }
    }
}
