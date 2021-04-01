namespace SoftJail.DataProcessor
{
    using Data;
    using Microsoft.EntityFrameworkCore.Internal;
    using Newtonsoft.Json;
    using SoftJail.Data.Models;
    using SoftJail.Data.Models.Enums;
    using SoftJail.DataProcessor.ImportDto;
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.Globalization;
    using System.IO;
    using System.Linq;
    using System.Text;
    using System.Xml.Serialization;

    public class Deserializer
    {
        private const string errorMessage = "Invalid Data";
        private const string successMessageDepartments = "Imported {0} with {1} cells";
        private const string successMessagePrisoners = "Imported {0} {1} years old";
        private const string successMessageOfficers = "Imported {0} ({1} prisoners)";

        public static string ImportDepartmentsCells(SoftJailDbContext context, string jsonString)
        {
            var departmentDTOs = JsonConvert.DeserializeObject<IEnumerable<ImportDepartmentDTO>>(jsonString);

            var sb = new StringBuilder();
            var departments = new List<Department>();

            foreach (var departmentDTO in departmentDTOs)
            {
                if (!IsValid(departmentDTO))
                {
                    sb.AppendLine(errorMessage);
                    continue;
                }

                var department = new Department()
                {
                    Name = departmentDTO.Name,
                };

                var cells = new List<Cell>();

                foreach (var currentCell in departmentDTO.Cells)
                {
                    if (!IsValid(currentCell))
                    {
                        cells = new List<Cell>();
                        break;
                    }

                    var cell = new Cell()
                    {
                        CellNumber = currentCell.CellNumber,
                        HasWindow = currentCell.HasWindow,
                        Department = department,
                    };

                    cells.Add(cell);
                }

                if (!cells.Any())
                {
                    sb.AppendLine(errorMessage);
                    continue;
                }

                department.Cells = cells;

                departments.Add(department);

                sb.AppendLine(string.Format(successMessageDepartments, department.Name, department.Cells.Count()));
            }

            context.Departments.AddRange(departments);
            context.SaveChanges();

            return sb.ToString().Trim();
        }

        public static string ImportPrisonersMails(SoftJailDbContext context, string jsonString)
        {
            var prisonersDTOs = JsonConvert.DeserializeObject<IEnumerable<ImportPrisonerDTO>>(jsonString);

            var sb = new StringBuilder();
            var prisoners = new List<Prisoner>();

            foreach (var prisonerDTO in prisonersDTOs)
            {
                if (!IsValid(prisonerDTO))
                {
                    sb.AppendLine(errorMessage);
                    continue;
                }

                var mails = new List<Mail>();

                var isNotValidMail = false;

                foreach (var currentMail in prisonerDTO.Mails)
                {
                    if (!IsValid(currentMail))
                    {
                        sb.AppendLine(errorMessage);
                        isNotValidMail = true;
                        break;
                    }

                    var mail = new Mail
                    {
                        Description = currentMail.Description,
                        Sender = currentMail.Sender,
                        Address = currentMail.Address,
                    };

                    mails.Add(mail);
                }

                if (isNotValidMail)
                {
                    continue;
                }

                var prisoner = new Prisoner()
                {
                    FullName = prisonerDTO.FullName,
                    Nickname = prisonerDTO.Nickname,
                    Age = prisonerDTO.Age,
                    IncarcerationDate = DateTime.ParseExact(prisonerDTO.IncarcerationDate, "dd/MM/yyyy", CultureInfo.InvariantCulture),
                    ReleaseDate = DateTime.ParseExact(prisonerDTO.IncarcerationDate, "dd/MM/yyyy", CultureInfo.InvariantCulture),
                    Bail = prisonerDTO.Bail,
                    CellId = prisonerDTO.CellId,
                    Mails = mails,
                };

                prisoners.Add(prisoner);

                sb.AppendLine(string.Format(successMessagePrisoners, prisoner.FullName, prisoner.Age));
            }

            context.Prisoners.AddRange(prisoners);
            context.SaveChanges();

            return sb.ToString().Trim();
        }

        public static string ImportOfficersPrisoners(SoftJailDbContext context, string xmlString)
        {
            var xmlSerializer = new XmlSerializer(typeof(ImportOfficerDTO[]), new XmlRootAttribute("Officers"));
            var officerDTOs = (ImportOfficerDTO[])xmlSerializer.Deserialize(new StringReader(xmlString));

            var sb = new StringBuilder();
            var officers = new List<Officer>();

            foreach (var officerDTO in officerDTOs)
            {
                if (!IsValid(officerDTO))
                {
                    sb.AppendLine(errorMessage);
                    continue;
                }

                if (!Enum.IsDefined(typeof(Position), officerDTO.Position))
                {
                    sb.AppendLine(errorMessage);
                    continue;
                }

                if (!Enum.IsDefined(typeof(Weapon), officerDTO.Weapon))
                {
                    sb.AppendLine(errorMessage);
                    continue;
                }

                var officer = new Officer()
                {
                    FullName = officerDTO.FullName,
                    Salary = officerDTO.Salary,
                    Position = (Position)Enum.Parse(typeof(Position), officerDTO.Position),
                    Weapon = (Weapon)Enum.Parse(typeof(Weapon), officerDTO.Weapon),
                    DepartmentId = officerDTO.DepartmentId,
                };

                foreach (var prisoner in officerDTO.Prisoners)
                {
                    officer.OfficerPrisoners.Add(new OfficerPrisoner()
                    {
                        Officer = officer,
                        PrisonerId = prisoner.Id,
                    });

                }

                officers.Add(officer);
                sb.AppendLine(string.Format(successMessageOfficers, officer.FullName, officer.OfficerPrisoners.Count));
            }

            context.Officers.AddRange(officers);
            context.SaveChanges();

            return sb.ToString().Trim();
        }

        private static bool IsValid(object obj)
        {
            var validationContext = new ValidationContext(obj);
            var validationResult = new List<ValidationResult>();

            bool isValid = Validator.TryValidateObject(obj, validationContext, validationResult, true);
            return isValid;
        }
    }
}