namespace TeisterMask.DataProcessor
{
    using System;
    using System.Globalization;
    using System.IO;
    using System.Linq;
    using System.Text;
    using System.Xml;
    using System.Xml.Linq;
    using System.Xml.Serialization;
    using Data;
    using Newtonsoft.Json;
    using TeisterMask.DataProcessor.ExportDto;
    using Formatting = Newtonsoft.Json.Formatting;

    public class Serializer
    {
        public static string ExportProjectWithTheirTasks(TeisterMaskContext context)
        {
            var projects = context.Projects
                 .Where(p => p.Tasks.Count() > 0)
                 .Select(p => new ExportProjectDTO()
                 {
                     TasksCount = p.Tasks.Count().ToString(),
                     ProjectName = p.Name,
                     HasEndDate = p.DueDate != null ? "Yes" : "No",
                     Tasks = p.Tasks.Select(t => new ExportProjectTaskDTO()
                     {
                         Name = t.Name,
                         Label = t.LabelType.ToString(),
                     })
                     .OrderBy(x => x.Name)
                     .ToArray()
                 })
                 .OrderByDescending(x => x.Tasks.Count())
                 .ThenBy(x => x.ProjectName)
                 .ToArray();

            var xmlSerializer = new XmlSerializer(typeof(ExportProjectDTO[]), new XmlRootAttribute("Projects"));

            var sb = new StringBuilder();
            var namespaces = new XmlSerializerNamespaces();
            namespaces.Add(string.Empty, string.Empty);

            xmlSerializer.Serialize(new Utf8StringWriter(sb), projects, namespaces);

            //sb = sb.Replace("utf-16", "utf-8");

            return sb.ToString().Trim();
        }

        public static string ExportMostBusiestEmployees(TeisterMaskContext context, DateTime date)
        {
            var employees = context.Employees
                .Where(x => x.EmployeesTasks.Any(et => et.Task.OpenDate >= date))
                .Select(em => new ExportEmployeeDTO()
                {
                    Username = em.Username,
                    Tasks = em.EmployeesTasks
                    .Where(x => x.Task.OpenDate >= date)
                    .OrderByDescending(x => x.Task.DueDate)
                    .ThenBy(x => x.Task.Name)
                    .Select(emp => new ExportEmployeeTaskDTO()
                    {
                        TaskName = emp.Task.Name,
                        OpenDate = emp.Task.OpenDate.ToString("d", CultureInfo.InvariantCulture),
                        DueDate = emp.Task.DueDate.ToString("d", CultureInfo.InvariantCulture),
                        ExecutionType = emp.Task.ExecutionType.ToString(),
                        LabelType = emp.Task.LabelType.ToString(),
                    })

                    .ToArray()
                })
               .OrderByDescending(x => x.Tasks.Length)
               .ThenBy(x => x.Username)
               .Take(10)
               .ToList();

            var json = JsonConvert.SerializeObject(employees, Formatting.Indented);

            return json;
        }

        public class Utf8StringWriter : StringWriter
        {
            public Utf8StringWriter(StringBuilder sb)
                : base(sb)
            {
            }
            public override Encoding Encoding => Encoding.UTF8;
        }
    }
}