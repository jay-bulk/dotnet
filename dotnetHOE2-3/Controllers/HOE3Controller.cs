using HandsOnEx.Models;
using Microsoft.AspNetCore.Mvc;

namespace HandsOnEx.Controllers
{
  public class HOE3Controller : Controller
  {
    public IActionResult Index()
    {

      Employee?[] employees = Employee.GetEmployees();

      return View(employees[2]);
    }
  }
}
