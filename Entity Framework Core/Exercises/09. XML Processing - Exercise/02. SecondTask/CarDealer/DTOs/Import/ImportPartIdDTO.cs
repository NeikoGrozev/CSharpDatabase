namespace CarDealer.DTOs.Import
{
    using System.Xml;
    using System.Xml.Serialization;

    [XmlType("partId")]
    public class ImportPartIdDTO
    {
        [XmlAttribute("id")]
        public int Id { get; set; }
    }
}
