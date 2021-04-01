namespace SoftUni
{
    using Data;
    using SoftUni.Models;
    using System;
    using System.Globalization;
    using System.Linq;
    using System.Runtime.InteropServices.ComTypes;
    using System.Text;

    public class StartUp
    {
        public static void Main()
        {
            using (SoftUniContext context = new SoftUniContext())
            {
                var result = RemoveTown(context);

                Console.WriteLine(result);
            }
        }

        //Prolem 3. Employees Full Information
        public static string GetEmployeesFullInformation(SoftUniContext context)
        {
            var employees = context.Employees
                .Select(e => new
                {
                    EmployeeId = e.EmployeeId,
                    FirstName = e.FirstName,
                    LastName = e.LastName,
                    MiddleName = e.MiddleName,
                    JobTitle = e.JobTitle,
                    Salary = e.Salary
                })
                .OrderBy(e => e.EmployeeId)
                .ToList();

            StringBuilder sb = new StringBuilder();

            foreach (var e in employees)
            {
                sb.AppendLine($"{e.FirstName} {e.LastName} {e.MiddleName} {e.JobTitle} {e.Salary:F2}");
            }

            return sb.ToString().TrimEnd();
        }

        //Problem 4. Employees with Salary Over 50 000
        public static string GetEmployeesWithSalaryOver50000(SoftUniContext context)
        {
            var employees = context.Employees
                .Select(e => new
                {
                    e.FirstName,
                    e.Salary
                })
                .Where(e => e.Salary > 50000)
                .OrderBy(e => e.FirstName)
                .ToList();

            StringBuilder sb = new StringBuilder();

            foreach (var e in employees)
            {
                sb.AppendLine($"{e.FirstName} - {e.Salary:F2}");
            }

            return sb.ToString().TrimEnd();
        }

        //Problem 5. Employees from Research and Development
        public static string GetEmployeesFromResearchAndDevelopment(SoftUniContext context)
        {
            var employees = context.Employees
                .Select(e => new
                {
                    FirstName = e.FirstName,
                    LastName = e.LastName,
                    DepartmentName = e.Department.Name,
                    Salary = e.Salary
                })
                .Where(e => e.DepartmentName == "Research and Development")
                .OrderBy(e => e.Salary)
                .ThenByDescending(e => e.FirstName)
                .ToList();

            StringBuilder sb = new StringBuilder();

            foreach (var e in employees)
            {
                sb.AppendLine($"{e.FirstName} {e.LastName} from {e.DepartmentName} - ${e.Salary:F2}");
            }

            return sb.ToString().TrimEnd();
        }

        //Problem 6. Adding a New Address and Updating Employee
        public static string AddNewAddressToEmployee(SoftUniContext context)
        {
            Address address = new Address
            {
                AddressText = "Vitoshka 15",
                TownId = 4
            };

            var nakov = context.Employees
                .FirstOrDefault(f => f.LastName == "Nakov");
            nakov.Address = address;

            context.SaveChanges();

            var addresses = context.Employees
                .OrderByDescending(e => e.AddressId)
                .Select(e => e.Address.AddressText)
                .Take(10)
                .ToList();

            StringBuilder sb = new StringBuilder();

            foreach (var a in addresses)
            {
                sb.AppendLine($"{a}");
            }

            return sb.ToString().TrimEnd();
        }

        //Problem 7. Employees and Projects
        public static string GetEmployeesInPeriod(SoftUniContext context)
        {
            var employees = context.Employees
                .Where(e => e.EmployeesProjects
                        .Any(p => p.Project.StartDate.Year >= 2001 &&
                        p.Project.StartDate.Year <= 2003))
                .Select(e => new
                {
                    EmployeeName = e.FirstName + " " + e.LastName,
                    ManagerName = e.Manager.FirstName + " " + e.Manager.LastName,
                    Projects = e.EmployeesProjects
                    .Select(p => new
                    {
                        ProjectName = p.Project.Name,
                        StartDate = p.Project.StartDate,
                        EndDate = p.Project.EndDate
                    })
                    .ToList()
                })
                .Take(10)
                .ToList();

            StringBuilder sb = new StringBuilder();

            foreach (var e in employees)
            {
                sb.AppendLine($"{e.EmployeeName} - Manager: {e.ManagerName}");

                foreach (var project in e.Projects)
                {
                    var startDate = project.StartDate
                        .ToString("M/d/yyyy h:mm:ss tt", CultureInfo.InvariantCulture);

                    var endDate = project.EndDate == null ?
                        "not finished" :
                        project.EndDate.Value.ToString("M/d/yyyy h:mm:ss tt", CultureInfo.InvariantCulture);

                    sb.AppendLine($"--{project.ProjectName} - {startDate} - {endDate}");
                }
            }

            return sb.ToString().TrimEnd();
        }

        //Problem 8. Addresses by Town
        public static string GetAddressesByTown(SoftUniContext context)
        {
            var addresses = context.Addresses
                .Select(a => new
                {
                    AddresText = a.AddressText,
                    TownName = a.Town.Name,
                    Employees = a.Employees.Count
                })
                .OrderByDescending(a => a.Employees)
                .ThenBy(a => a.TownName)
                .ThenBy(a => a.AddresText)
                .Take(10)
                .ToList();

            StringBuilder sb = new StringBuilder();

            foreach (var a in addresses)
            {
                sb.AppendLine($"{a.AddresText}, {a.TownName} - {a.Employees} employees");
            }

            return sb.ToString().TrimEnd();
        }

        //Problem 9. Employee 147
        public static string GetEmployee147(SoftUniContext context)
        {
            var employeeWithId147 = context.Employees
                .Select(e => new
                {
                    Id = e.EmployeeId,
                    Name = e.FirstName + " " + e.LastName,
                    JobTitle = e.JobTitle,
                    Projects = e.EmployeesProjects
                    .Select(p => new
                    {
                        ProjectName = p.Project.Name
                    })
                    .OrderBy(n => n.ProjectName)
                    .ToList()
                })
                .Where(e => e.Id == 147)
                .ToList();

            var test = context.Employees
                .Where(e => e.EmployeeId == 147)
                .ToList();

            StringBuilder sb = new StringBuilder();

            foreach (var e in employeeWithId147)
            {
                sb.AppendLine($"{e.Name} - {e.JobTitle}");

                foreach (var p in e.Projects)
                {
                    sb.AppendLine($"{p.ProjectName}");
                }
            }

            return sb.ToString().TrimEnd();
        }

        //Porblem 10. Departments with More Than 5 Employees
        public static string GetDepartmentsWithMoreThan5Employees(SoftUniContext context)
        {
            var departments = context.Departments
                .Where(d => d.Employees.Count() > 5)
                .OrderBy(d => d.Employees.Count())
                .ThenBy(d => d.Name)
                .Select(d => new
                {
                    DepartmentName = d.Name,
                    ManagerName = d.Manager.FirstName + " " + d.Manager.LastName,
                    EmployeesName = d.Employees
                            .Select(e => new
                            {
                                EmpFirstNAme = e.FirstName,
                                EmpLastName = e.LastName,
                                JobTitle = e.JobTitle
                            })
                            .OrderBy(x => x.EmpFirstNAme)
                            .ThenBy(x => x.EmpLastName)
                })
                .ToList();

            StringBuilder sb = new StringBuilder();

            foreach (var d in departments)
            {
                sb.AppendLine($"{d.DepartmentName} – {d.ManagerName}");

                foreach (var e in d.EmployeesName)
                {
                    sb.AppendLine($"{e.EmpFirstNAme} {e.EmpLastName} - {e.JobTitle}");
                }
            }

            return sb.ToString().TrimEnd();
        }

        //Problem 11. Find Latest 10 Projects
        public static string GetLatestProjects(SoftUniContext context)
        {
            var projects = context.Projects
                .OrderByDescending(p => p.StartDate)
                .Take(10)
                .OrderBy(p => p.Name)
                .Select(p => new
                {
                    ProjectName = p.Name,
                    Description = p.Description,
                    StartDate = p.StartDate
                })
                .ToList();

            StringBuilder sb = new StringBuilder();

            foreach (var p in projects)
            {
                sb.AppendLine($"{p.ProjectName}");
                sb.AppendLine($"{p.Description}");
                sb.AppendLine($"{p.StartDate.ToString("M/d/yyyy h:mm:ss tt", CultureInfo.InvariantCulture)}");
            }

            return sb.ToString().TrimEnd();
        }

        //Problem 12. Increase Salaries
        public static string IncreaseSalaries(SoftUniContext context)
        {
            var employees = context.Employees
                .Where(e => e.Department.Name == "Engineering" ||
                        e.Department.Name == "Tool Design" ||
                        e.Department.Name == "Marketing" ||
                        e.Department.Name == "Information Services")
                .Select(e => new
                {
                    EmpFirstName = e.FirstName,
                    EmpLastName = e.LastName,
                    Salary = e.Salary * 1.12m
                })
                .OrderBy(e => e.EmpFirstName)
                .ThenBy(e => e.EmpLastName)
                .ToList();

            StringBuilder sb = new StringBuilder();

            foreach (var e in employees)
            {
                sb.AppendLine($"{e.EmpFirstName} {e.EmpLastName} (${e.Salary:F2})");
            }

            return sb.ToString().TrimEnd();
        }

        //Problem 13. Find Employees by First Name Starting With "Sa"
        public static string GetEmployeesByFirstNameStartingWithSa(SoftUniContext context)
        {
            var employees = context.Employees
                .Where(e => e.FirstName.StartsWith("Sa"))
                .Select(e => new
                {
                    EmpFirstName = e.FirstName,
                    EmpLastName = e.LastName,
                    JobTitle = e.JobTitle,
                    Salary = e.Salary
                })
                .OrderBy(e => e.EmpFirstName)
                .ThenBy(e => e.EmpLastName)
                .ToList();

            StringBuilder sb = new StringBuilder();

            foreach (var e in employees)
            {
                sb.AppendLine($"{e.EmpFirstName} {e.EmpLastName} - {e.JobTitle} - (${e.Salary:F2})");
            }

            return sb.ToString().TrimEnd();
        }

        //Problem 14. Delete Project by Id
        public static string DeleteProjectById(SoftUniContext context)
        {
            var project = context.EmployeesProjects
                .Where(p => p.ProjectId == 2)
                .ToList();

            foreach (var p in project)
            {
                context.EmployeesProjects.Remove(p);
            }

            context.SaveChanges();

            var projectNew = context.Projects.Find(2);
            context.Projects.Remove(projectNew);
            context.SaveChanges();

            var selectProject = context.Projects
                .Select(e => e.Name)
                .Take(10)
                .ToList();

            StringBuilder sb = new StringBuilder();

            foreach (var p in selectProject)
            {
                sb.AppendLine($"{p}");
            }

            return sb.ToString().TrimEnd();
        }

        //Problem 15. Remove Town
        public static string RemoveTown(SoftUniContext context)
        {
            var employees = context.Employees
                .Where(e => e.Address.Town.Name == "SEattle")
                .ToList();
                        
            foreach (var e in employees)
            {
                e.AddressId = null;
                context.SaveChanges();
            }
                        
            var addresses = context.Addresses
                .Where(a => a.Town.Name == "Seattle")
                .ToList();

            var towns = context.Towns
                .Where(t => t.Name == "Seattle")
                .ToList();

            int count = addresses.Count();

            foreach (var a in addresses)
            {
                context.Addresses.Remove(a);
                context.SaveChanges();
            }

            foreach (var t in towns)
            {
                context.Towns.Remove(t);
                context.SaveChanges();
            }

            return $"{count} addresses in Seattle were deleted";
        }
    }
}
