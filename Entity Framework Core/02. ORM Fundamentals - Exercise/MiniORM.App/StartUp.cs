namespace MiniORM.App
{
    using MiniORM.App.Data;
    using MiniORM.App.Data.Entities;
    using System.Linq;

    public class StartUp
    {
        public static void Main()
        {
            string connectionString = "Server=NEIKO\\SQLEXPRESS;" +
                                      "Database=MiniORM;" +
                                      "Integrated Security=true";

            var context = new SoftUniDbContextClass(connectionString);

            context.Employees.Add(new Employee
            {
                FirstName = "Gosho",
                LastName = "Inserted",
                DepartmentId = context.Departments.First().Id,
                IsEmloyed = true
            });

            Employee employee = context.Employees.Last();
            employee.FirstName = "Modified";

            context.SaveChanges();
        }
    }
}
