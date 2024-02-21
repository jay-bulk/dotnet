using HandsOnEx.Models;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;

namespace HandsOnEx.Controllers
{
  public class HOE2Controller : Controller
  {

    public IActionResult Index()
    {
      // Null conditional operator

      //         List<string> list = new();

      //         foreach(Employee? aEmployee in Employee.GetEmployees()) {
      //           string? name = aEmployee?.Name;
      //           string? dept = aEmployee.Department;
      //           decimal? salary = aEmployee.Salary;
      //           string? fullTime = aEmployee.isFullTime.ToString();

      //           list.Add($"Name: {name}, Department: {dept}, Salary: {salary:c0}, FullTime: {fullTime}");
      //         }

      //         return View();
      //       }

      // Null Coalescing operation

      //List<string> list = new();

      //foreach(Employee? aEmployee in Employee.GetEmployees()) {
      //  string? name = aEmployee?.Name ?? "<Unknown>";
      //  string? dept = aEmployee.Department ?? "<Unknown>";
      //  decimal? salary = aEmployee.Salary ?? 0;
      //  string? fullTime = aEmployee.isFullTime.ToString() ?? "<Unknown>";

      //  list.Add($"Name: {name}, Department: {dept}, Salary: {salary:c0}, FullTime: {fullTime}");
      //}

      //return View();

      // Pattern Matching 

      //object[] data = new object[] { 100M, 32, "at the ranch", "on the slopes", 20.32M, "in a cubicle", 78 };

      //decimal total = 0;

      //for (int i=0; i < data.length; ++i {
      //    if (data[i] is decimal d) {
      //    total += d;
      //    }

      //}
      //return View(new string[] {$"Total: {total:n2}"});
      //
      //

      // type inferencing
      //
      //var departments = new[] { "IT", "HR", "Accounting", "Finance", "Ops" };

      //return View(departments);
      //

      // Anonymous types
      //
      var commodities = new[] {
          new {Commodity = "Oil", Price= 82.39M },
          new {Commodity = "Gold", Price= 2016.93M },
          new {Commodity = "Corn", Price= 4.17M },
          new {Commodity = "Coffee", Price= 1.90M },
          new {Commodity = "Cattle", Price= 1.85M }
        };


      return View(commoities.Select(c => $"Commoditi Name: {c.Commodity}, Price: {c.Price:c}"));

    }

  }
}
