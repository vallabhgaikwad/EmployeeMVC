using EmployeeMVC.Models;
using System.Configuration;
using System.Data.SqlClient;
using System.Web.Mvc;

namespace EmployeeMVC.Controllers
{
    public class AccountController : Controller
    {
        string cs = ConfigurationManager
            .ConnectionStrings["EmployeeDBConnection"]
            .ConnectionString;

        public ActionResult Login()
        {
            return View();
        }

        [HttpPost]
        public ActionResult Login(LoginModel model)
        {
            using (SqlConnection con = new SqlConnection(cs))
            {
                con.Open();

                SqlCommand cmd = new SqlCommand(
                    @"SELECT *
                      FROM Users
                      WHERE Username=@Username
                      AND Password=@Password",
                    con);

                cmd.Parameters.AddWithValue(
                    "@Username",
                    model.Username);

                cmd.Parameters.AddWithValue(
                    "@Password",
                    model.Password);

                SqlDataReader dr = cmd.ExecuteReader();

                if (dr.Read())
                {
                    Session["Username"] =
                        dr["Username"].ToString();

                    return RedirectToAction(
      "Dashboard",
      "Employee");
                }
            }

            ViewBag.Error =
                "Invalid Username or Password";

            return View();
        }

        public ActionResult Logout()
        {
            Session.Clear();

            return RedirectToAction("Login");
        }
    }
}