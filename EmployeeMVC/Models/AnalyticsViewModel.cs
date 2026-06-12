namespace EmployeeMVC.Models
{
    public class AnalyticsViewModel
    {
        public int TotalEmployees { get; set; }

        public int TotalCities { get; set; }

        public string NewestEmployee { get; set; }

        public string OldestEmployee { get; set; }

        public decimal AverageSalary { get; set; }

        public decimal TotalPayroll { get; set; }
    }
}