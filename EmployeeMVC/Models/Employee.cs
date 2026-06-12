using System;
using System.ComponentModel.DataAnnotations;

namespace EmployeeMVC.Models
{
    public class Employee
    {
        public int EmployeeCode { get; set; }

        [Required(ErrorMessage = "First Name is required")]
        [StringLength(50, ErrorMessage = "Maximum 50 characters allowed")]
        [RegularExpression(@"^[A-Za-z ]+$",
            ErrorMessage = "First Name can contain only letters")]
        public string FirstName { get; set; }

        [Required(ErrorMessage = "Last Name is required")]
        [StringLength(50, ErrorMessage = "Maximum 50 characters allowed")]
        [RegularExpression(@"^[A-Za-z ]+$",
            ErrorMessage = "Last Name can contain only letters")]
        public string LastName { get; set; }

        [Required(ErrorMessage = "City is required")]
        [StringLength(50, ErrorMessage = "Maximum 50 characters allowed")]
        [RegularExpression(@"^[A-Za-z ]+$",
            ErrorMessage = "City can contain only letters")]
        public string City { get; set; }

        [Required(ErrorMessage = "Please select a Gender")]
        public string Gender { get; set; }

        [Required(ErrorMessage = "Designation is required")]
        [StringLength(100, ErrorMessage = "Maximum 100 characters allowed")]
        public string Designation { get; set; }

        [Required(ErrorMessage = "Salary is required")]
        [Range(10000, 10000000,
    ErrorMessage = "Salary must be between ₹10,000 and ₹1,00,00,000")]
        public decimal Salary { get; set; }

        [Required(ErrorMessage = "Birth Date is required")]
        [DataType(DataType.Date)]
        [Display(Name = "Birth Date")]
        public DateTime BirthDate { get; set; }
    }
}