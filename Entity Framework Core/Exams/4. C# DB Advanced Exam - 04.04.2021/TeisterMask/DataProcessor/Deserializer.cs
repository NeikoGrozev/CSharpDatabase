namespace TeisterMask.DataProcessor
{
    using System;
    using System.Collections.Generic;

    using System.ComponentModel.DataAnnotations;
    using System.Globalization;
    using System.IO;
    using System.Linq;
    using System.Text;
    using System.Xml.Serialization;
    using Data;
    using Newtonsoft.Json;
    using TeisterMask.Data.Models;
    using TeisterMask.Data.Models.Enums;
    using TeisterMask.DataProcessor.ImportDto;
    using ValidationContext = System.ComponentModel.DataAnnotations.ValidationContext;

    public class Deserializer
    {
        private const string ErrorMessage = "Invalid data!";

        private const string SuccessfullyImportedProject
            = "Successfully imported project - {0} with {1} tasks.";

        private const string SuccessfullyImportedEmployee
            = "Successfully imported employee - {0} with {1} tasks.";

        public static string ImportProjects(TeisterMaskContext context, string xmlString)
        {
            var xmlSerializer = new XmlSerializer(typeof(ImportProjectDTO[]), new XmlRootAttribute("Projects"));

            var projectDTOs = (ImportProjectDTO[])xmlSerializer.Deserialize(new StringReader(xmlString));

            var sb = new StringBuilder();
            var projects = new List<Project>();

            foreach (var projectDTO in projectDTOs)
            {
                if (!IsValid(projectDTO))
                {
                    sb.AppendLine(ErrorMessage);
                    continue;
                }

                DateTime openDateProject;
                var isOpenDate = DateTime.TryParseExact(projectDTO.OpenDate, "dd/MM/yyyy", CultureInfo.InvariantCulture, DateTimeStyles.None, out openDateProject);

                if (!isOpenDate)
                {
                    sb.AppendLine(ErrorMessage);
                    continue;
                }

                DateTime? dueDateProject;
                bool isDueDate = false;

                if (!string.IsNullOrEmpty(projectDTO.DueDate))
                {
                    DateTime dueDate;
                    isDueDate = DateTime.TryParseExact(projectDTO.DueDate, "dd/MM/yyyy", CultureInfo.InvariantCulture, DateTimeStyles.None, out dueDate);

                    if (!isDueDate)
                    {
                        sb.AppendLine(ErrorMessage);
                        continue;
                    }

                    dueDateProject = dueDate;
                }
                else
                {
                    dueDateProject = null;
                }

                var project = new Project()
                {
                    Name = projectDTO.Name,
                    OpenDate = openDateProject,
                    DueDate = dueDateProject,
                };

                foreach (var taskDTO in projectDTO.Tasks)
                {
                    if (!IsValid(taskDTO))
                    {
                        sb.AppendLine(ErrorMessage);
                        continue;
                    }

                    DateTime openDateTask;
                    var isOpenDateTask = DateTime.TryParseExact(taskDTO.OpenDate, "dd/MM/yyyy", CultureInfo.InvariantCulture, DateTimeStyles.None, out openDateTask);

                    DateTime dueDateTask;
                    var isDueDateTask = DateTime.TryParseExact(taskDTO.DueDate, "dd/MM/yyyy", CultureInfo.InvariantCulture, DateTimeStyles.None, out dueDateTask);

                    if (!isOpenDateTask)
                    {
                        sb.AppendLine(ErrorMessage);
                        continue;
                    }

                    if (openDateProject > openDateTask)
                    {
                        sb.AppendLine(ErrorMessage);
                        continue;
                    }

                    if (!isDueDateTask)
                    {
                        sb.AppendLine(ErrorMessage);
                        continue;
                    }

                    if (project.DueDate.HasValue)
                    {
                        if (dueDateProject < dueDateTask)
                        {
                            sb.AppendLine(ErrorMessage);
                            continue;
                        }
                    }

                    var task = new Task()
                    {
                        Name = taskDTO.Name,
                        OpenDate = openDateTask,
                        DueDate = dueDateTask,
                        ExecutionType = (ExecutionType)Enum.Parse(typeof(ExecutionType), taskDTO.ExecutionType),
                        LabelType = (LabelType)Enum.Parse(typeof(LabelType), taskDTO.LabelType)
                    };

                    project.Tasks.Add(task);
                }

                projects.Add(project);
                sb.AppendLine(string.Format(SuccessfullyImportedProject, project.Name, project.Tasks.Count));
            }

            context.Projects.AddRange(projects);
            context.SaveChanges();

            return sb.ToString().Trim();
        }

        public static string ImportEmployees(TeisterMaskContext context, string jsonString)
        {
            var employeeDTOs = JsonConvert.DeserializeObject<IEnumerable<ImportEmployeeDTO>>(jsonString);

            var sb = new StringBuilder();
            var employees = new List<Employee>();

            foreach (var employeeDTO in employeeDTOs)
            {
                if (!IsValid(employeeDTO))
                {
                    sb.AppendLine(ErrorMessage);
                    continue;
                }

                var employee = new Employee()
                {
                    Username = employeeDTO.Username,
                    Email = employeeDTO.Email,
                    Phone = employeeDTO.Phone,
                };

                foreach (var taskDTO in employeeDTO.Tasks.Distinct())
                {
                    var currentTask = context.Tasks.FirstOrDefault(x => x.Id == taskDTO);

                    if (currentTask == null)
                    {
                        sb.AppendLine(ErrorMessage);
                        continue;
                    }

                    var task = new EmployeeTask()
                    {
                        Employee = employee,
                        Task = currentTask,
                    };

                    employee.EmployeesTasks.Add(task);
                }

                employees.Add(employee);
                sb.AppendLine(string.Format(SuccessfullyImportedEmployee, employee.Username, employee.EmployeesTasks.Count));
            }

            context.Employees.AddRange(employees);
            context.SaveChanges();

            return sb.ToString().Trim();
        }

        private static bool IsValid(object dto)
        {
            var validationContext = new ValidationContext(dto);
            var validationResult = new List<ValidationResult>();

            return Validator.TryValidateObject(dto, validationContext, validationResult, true);
        }
    }
}