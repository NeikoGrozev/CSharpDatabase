namespace SoftJail.DataProcessor
{

    using Data;
    using Newtonsoft.Json;
    using SoftJail.DataProcessor.ExportDto;
    using System;
    using System.IO;
    using System.Linq;
    using System.Text;
    using System.Xml.Serialization;

    public class Serializer
    {
        public static string ExportPrisonersByCells(SoftJailDbContext context, int[] ids)
        {
            var prisoners = context.Prisoners
                .ToList()
                .Where(x => ids.Contains(x.Id))
                .Select(p => new ExportPrisonerDTO()
                {
                    Id = p.Id,
                    Name = p.FullName,
                    CellNumber = p.Cell.CellNumber,
                    Officers = p.PrisonerOfficers.Select(po => new ExportOfficerDTO()
                    {
                        OfficerName = po.Officer.FullName,
                        Department = po.Officer.Department.Name,
                    })
                    .OrderBy(x => x.OfficerName),
                    TotalOfficerSalary = p.PrisonerOfficers.Sum(po => po.Officer.Salary),
                })
                .OrderBy(x => x.Name)
                .ThenBy(x => x.Id)
                .ToList();

            var json = JsonConvert.SerializeObject(prisoners, Formatting.Indented);

            return json;
        }

        public static string ExportPrisonersInbox(SoftJailDbContext context, string prisonersNames)
        {
            var names = prisonersNames.Split(",");

            var prisoners = context.Prisoners
                .ToArray()
                 .Where(x => names.Contains(x.FullName))
                 .Select(p => new ExportPrisonerInboxDTO()
                 {
                     Id = p.Id,
                     Name = p.FullName,
                     IncarcerationDate = p.IncarcerationDate.ToString("yyyy-MM-dd"),
                     EncryptedMessages = p.Mails.Select(m => new ExportMessageDTO()
                     {
                         Description = ReverseMessage(m.Description)
                     })
                     .ToArray()
                 })
                 .OrderBy(x => x.Name)
                 .ThenBy(x => x.Id)
                 .ToArray();

            var xmlSerializer = new XmlSerializer(typeof(ExportPrisonerInboxDTO[]), new XmlRootAttribute("Prisoners"));

            var sb = new StringBuilder();
            var namespaces = new XmlSerializerNamespaces();
            namespaces.Add(string.Empty, string.Empty);

            xmlSerializer.Serialize(new StringWriter(sb), prisoners, namespaces);

            return sb.ToString().Trim();
        }

        private static string ReverseMessage(string message)
        {
            var chars = message.ToCharArray();
            Array.Reverse(chars);

            return new string(chars);
        }
    }
}