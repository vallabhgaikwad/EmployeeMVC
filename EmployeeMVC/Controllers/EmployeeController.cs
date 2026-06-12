using System;
using EmployeeMVC.Models;
using System.Collections.Generic;
using System.Configuration;
using System.Data.SqlClient;
using System.Web.Mvc;

using ClosedXML.Excel;
using System.IO;
using System.Data;

namespace EmployeeMVC.Controllers
{
    public class EmployeeController : Controller
    {
        protected override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            if (Session["Username"] == null)
            {
                filterContext.Result =
                    RedirectToAction(
                        "Login",
                        "Account");
            }

            base.OnActionExecuting(filterContext);
        }

        string cs = ConfigurationManager
            .ConnectionStrings["EmployeeDBConnection"]
            .ConnectionString;
        public ActionResult Dashboard()
        {
            DashboardViewModel model =
                new DashboardViewModel();

            using (SqlConnection con = new SqlConnection(cs))
            {
                con.Open();

                SqlCommand employeeCmd =
                    new SqlCommand(
                        "SELECT COUNT(*) FROM Employee",
                        con);

                model.TotalEmployees =
                    (int)employeeCmd.ExecuteScalar();

                SqlCommand cityCmd =
                    new SqlCommand(
                        "SELECT COUNT(DISTINCT City) FROM Employee",
                        con);

                model.TotalCities =
                    (int)cityCmd.ExecuteScalar();
            }

            return View(model);
        }

        public ActionResult Index(
    string search,
    string gender,
    string designation)
        {
            List<Employee> employees =
                new List<Employee>();

            using (SqlConnection con =
                new SqlConnection(cs))
            {
                con.Open();

                string query =
                    "SELECT * FROM Employee WHERE 1=1";

                SqlCommand cmd =
                    new SqlCommand();

                cmd.Connection = con;

                if (!string.IsNullOrEmpty(search))
                {
                    query +=
                        @" AND (
                FirstName LIKE @search
                OR LastName LIKE @search
                OR City LIKE @search)";

                    cmd.Parameters.AddWithValue(
                        "@search",
                        "%" + search + "%");
                }

                if (!string.IsNullOrEmpty(gender))
                {
                    query +=
                        " AND Gender=@gender";

                    cmd.Parameters.AddWithValue(
                        "@gender",
                        gender);
                }

                if (!string.IsNullOrEmpty(designation))
                {
                    query +=
                        " AND Designation=@designation";

                    cmd.Parameters.AddWithValue(
                        "@designation",
                        designation);
                }

                cmd.CommandText = query;

                SqlDataReader dr =
                    cmd.ExecuteReader();

                while (dr.Read())
                {
                    employees.Add(
                        new Employee
                        {
                            EmployeeCode =
                                (int)dr["EmployeeCode"],

                            FirstName =
                                dr["FirstName"].ToString(),

                            LastName =
                                dr["LastName"].ToString(),

                            Gender =
                                dr["Gender"].ToString(),

                            Designation =
                                dr["Designation"].ToString(),

                            Salary =
                                dr["Salary"] == DBNull.Value
                                ? 0
                                : Convert.ToDecimal(
                                    dr["Salary"]),

                            City =
                                dr["City"].ToString(),

                            BirthDate =
                                (DateTime)dr["BirthDate"]
                        });
                }
            }

            return View(employees);
        }
        public ActionResult Edit(int id)

        {
            Employee employee = new Employee();

            using (SqlConnection con = new SqlConnection(cs))
            {
                con.Open();

                SqlCommand cmd =
                    new SqlCommand(
                        "SELECT * FROM Employee WHERE EmployeeCode=@id",
                        con);

                cmd.Parameters.AddWithValue("@id", id);

                SqlDataReader dr = cmd.ExecuteReader();

                if (dr.Read())
                {
                    employee.EmployeeCode = (int)dr["EmployeeCode"];
                    employee.FirstName = dr["FirstName"].ToString();
                    employee.LastName = dr["LastName"].ToString();

                    employee.Gender = dr["Gender"].ToString();
                    employee.Designation = dr["Designation"].ToString();

                    employee.Salary =
                        dr["Salary"] == DBNull.Value
                        ? 0
                        : Convert.ToDecimal(dr["Salary"]);

                    employee.City = dr["City"].ToString();
                    employee.BirthDate = (DateTime)dr["BirthDate"];
                }
            }

            return View(employee);
        }
        [HttpPost]
        public ActionResult Edit(Employee employee)
        {
            if (!ModelState.IsValid)
            {
                return View(employee);
            }

            if (employee.BirthDate > DateTime.Now)
            {
                ModelState.AddModelError(
                    "BirthDate",
                    "Birth Date cannot be in the future.");

                return View(employee);
            }

            if (employee.BirthDate > DateTime.Now.AddYears(-18))
            {
                ModelState.AddModelError(
                    "BirthDate",
                    "Employee must be at least 18 years old.");

                return View(employee);
            }

            using (SqlConnection con = new SqlConnection(cs))
            {
                con.Open();

                SqlCommand cmd = new SqlCommand(
 @"UPDATE Employee
SET FirstName=@FirstName,
LastName=@LastName,
Gender=@Gender,
Designation=@Designation,
Salary=@Salary,
City=@City,
BirthDate=@BirthDate
WHERE EmployeeCode=@EmployeeCode",
 con);

                cmd.Parameters.AddWithValue("@EmployeeCode", employee.EmployeeCode);
                cmd.Parameters.AddWithValue("@FirstName", employee.FirstName);
                cmd.Parameters.AddWithValue("@LastName", employee.LastName);
                cmd.Parameters.AddWithValue("@Gender", employee.Gender);
                cmd.Parameters.AddWithValue("@Designation", employee.Designation);
                cmd.Parameters.AddWithValue("@Salary", employee.Salary);
                cmd.Parameters.AddWithValue("@City", employee.City);
                cmd.Parameters.AddWithValue("@BirthDate", employee.BirthDate);


                cmd.ExecuteNonQuery();
            }

            TempData["Success"] =
    "Employee updated successfully.";

            return RedirectToAction("Index");
        }

        public ActionResult Create()
        {
            return View();
        }

        [HttpPost]
        public ActionResult Create(Employee employee)
        {
            if (!ModelState.IsValid)
            {
                return View(employee);
            }

            if (employee.BirthDate > DateTime.Now)
            {
                ModelState.AddModelError(
                    "BirthDate",
                    "Birth Date cannot be in the future.");

                return View(employee);
            }

            if (employee.BirthDate > DateTime.Now.AddYears(-18))
            {
                ModelState.AddModelError(
                    "BirthDate",
                    "Employee must be at least 18 years old.");

                return View(employee);
            }

            using (SqlConnection con = new SqlConnection(cs))
            {
                con.Open();

                SqlCommand cmd = new SqlCommand(
    @"INSERT INTO Employee
      (
        FirstName,
        LastName,
        Gender,
        Designation,
        Salary,
        City,
        BirthDate
      )
      VALUES
      (
        @FirstName,
        @LastName,
        @Gender,
        @Designation,
        @Salary,
        @City,
        @BirthDate
      )",
    con);

                cmd.Parameters.AddWithValue("@FirstName", employee.FirstName);
                cmd.Parameters.AddWithValue("@LastName", employee.LastName);
                cmd.Parameters.AddWithValue("@City", employee.City);
                cmd.Parameters.AddWithValue("@BirthDate", employee.BirthDate);
                cmd.Parameters.AddWithValue("@Gender", employee.Gender);

                cmd.Parameters.AddWithValue("@Designation", employee.Designation);

                cmd.Parameters.AddWithValue("@Salary", employee.Salary);
                cmd.ExecuteNonQuery();
            }

            TempData["Success"] =
    "Employee created successfully.";

            return RedirectToAction("Index");
        }

        public ActionResult ExportEmployees()
        {
            DataTable dt = new DataTable("Employees");

            dt.Columns.Add("Emp ID");
            dt.Columns.Add("First Name");
            dt.Columns.Add("Last Name");
            dt.Columns.Add("Gender");
            dt.Columns.Add("Designation");
            dt.Columns.Add("Salary");
            dt.Columns.Add("City");
            dt.Columns.Add("Birth Date");

            using (SqlConnection con = new SqlConnection(cs))
            {
                con.Open();

                SqlCommand cmd =
                    new SqlCommand(
                        "SELECT * FROM Employee",
                        con);

                SqlDataReader dr =
                    cmd.ExecuteReader();

                while (dr.Read())
                {
                    dt.Rows.Add(
                        "EMP-" + Convert.ToInt32(dr["EmployeeCode"]).ToString("D3"),
                        dr["FirstName"],
                        dr["LastName"],
                        dr["Gender"],
                        dr["Designation"],
                        dr["Salary"],
                        dr["City"],
                        Convert.ToDateTime(dr["BirthDate"])
                            .ToString("dd-MM-yyyy")
                    );
                }
            }

            using (XLWorkbook wb = new XLWorkbook())
            {
                wb.Worksheets.Add(dt, "Employees");

                using (MemoryStream stream =
                    new MemoryStream())
                {
                    wb.SaveAs(stream);

                    return File(
                        stream.ToArray(),
                        "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
                        "Employees.xlsx");
                }
            }
        }

        public ActionResult DownloadTemplate()
        {
            using (XLWorkbook wb = new XLWorkbook())
            {
                var ws = wb.Worksheets.Add("Template");

                ws.Cell(1, 1).Value = "FirstName";
                ws.Cell(1, 2).Value = "LastName";
                ws.Cell(1, 3).Value = "Gender";
                ws.Cell(1, 4).Value = "Designation";
                ws.Cell(1, 5).Value = "Salary";
                ws.Cell(1, 6).Value = "City";
                ws.Cell(1, 7).Value = "BirthDate";

                ws.Cell(2, 1).Value = "Rahul";
                ws.Cell(2, 2).Value = "Sharma";
                ws.Cell(2, 3).Value = "Male";
                ws.Cell(2, 4).Value = "Software Engineer";
                ws.Cell(2, 5).Value = 75000;
                ws.Cell(2, 6).Value = "Mumbai";
                ws.Cell(2, 7).Value = "15-08-1998";

                ws.Columns().AdjustToContents();

                using (MemoryStream stream = new MemoryStream())
                {
                    wb.SaveAs(stream);

                    return File(
                        stream.ToArray(),
                        "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
                        "EmployeeTemplate.xlsx");
                }
            }
        }

        public ActionResult Analytics()
        {
            AnalyticsViewModel model =
                new AnalyticsViewModel();

            using (SqlConnection con = new SqlConnection(cs))
            {
                con.Open();

                model.TotalEmployees =
                    (int)new SqlCommand(
                        "SELECT COUNT(*) FROM Employee",
                        con).ExecuteScalar();

                model.TotalCities =
                    (int)new SqlCommand(
                        "SELECT COUNT(DISTINCT City) FROM Employee",
                        con).ExecuteScalar();

                model.NewestEmployee =
                    Convert.ToString(
                        new SqlCommand(
                        @"SELECT TOP 1 FirstName
                  FROM Employee
                  ORDER BY BirthDate DESC",
                        con).ExecuteScalar());

                model.OldestEmployee =
                    Convert.ToString(
                        new SqlCommand(
                        @"SELECT TOP 1 FirstName
                  FROM Employee
                  ORDER BY BirthDate",
                        con).ExecuteScalar());

                model.AverageSalary =
                    Convert.ToDecimal(
                        new SqlCommand(
                        @"SELECT AVG(Salary)
                  FROM Employee",
                        con).ExecuteScalar());

                model.TotalPayroll =
                    Convert.ToDecimal(
                        new SqlCommand(
                        @"SELECT SUM(Salary)
                  FROM Employee",
                        con).ExecuteScalar());
            }

            return View(model);
        }

        public ActionResult ViewEmployee(int id)
        {
            Employee employee = new Employee();

            using (SqlConnection con = new SqlConnection(cs))
            {
                con.Open();

                SqlCommand cmd =
                    new SqlCommand(
                        "SELECT * FROM Employee WHERE EmployeeCode=@id",
                        con);

                cmd.Parameters.AddWithValue("@id", id);

                SqlDataReader dr = cmd.ExecuteReader();

                if (dr.Read())
                {
                    employee.EmployeeCode =
                        (int)dr["EmployeeCode"];

                    employee.FirstName =
                        dr["FirstName"].ToString();

                    employee.LastName =
                        dr["LastName"].ToString();

                    employee.City =
                        dr["City"].ToString();

                    employee.BirthDate =
                        (DateTime)dr["BirthDate"];

                    employee.Gender = dr["Gender"].ToString();

                    employee.Designation = dr["Designation"].ToString();

                    employee.Salary =
                        dr["Salary"] == DBNull.Value
                        ? 0
                        : Convert.ToDecimal(dr["Salary"]);
                }
            }

            return View(employee);
        }


        public ActionResult Delete(int id)
        {
            using (SqlConnection con = new SqlConnection(cs))
            {
                con.Open();

                SqlCommand cmd = new SqlCommand(
                    "DELETE FROM Employee WHERE EmployeeCode=@id",
                    con);

                cmd.Parameters.AddWithValue("@id", id);

                cmd.ExecuteNonQuery();
            }

            TempData["Success"] =
    "Employee deleted successfully.";

            return RedirectToAction("Index");
        }

    }
}