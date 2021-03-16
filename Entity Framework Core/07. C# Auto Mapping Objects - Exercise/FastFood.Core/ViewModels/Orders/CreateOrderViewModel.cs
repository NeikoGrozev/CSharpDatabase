namespace FastFood.Core.ViewModels.Orders
{
    using System.Collections.Generic;

    public class CreateOrderViewModel
    {
        public List<ItemsForOrdersAllViewModel> Items { get; set; }

        public List<EmployeesForOrdersAllViewModel> Employees { get; set; }
    }
}
