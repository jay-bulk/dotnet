using System.Xml;
using Microsoft.AspNetCore.Authorization.Infrastructure;

namespace HandsOnEx.Models {
    public class Employee {

        public string? Name { get; set; }

        public string? Department{ get; set; }

        public decimal? Salary { get; set; }

        public bool isFullTime { get; private set; }

        public Employee(bool empStatus=true) {
            isFullTime = empStatus;
        }

        public static Employee?[] GetEmployees() {
            Employee naj = new Employee() { 
            Name = "Najmudheen", 
            Department = "Data Science",
            Salary=120000m
            };
            Employee arvind = new Employee() { 
            Name = "Arvind", 
            Department = "Finance",
            Salary=120300m
            };
            Employee harshini = new Employee() { 
            Name = "Harshini", 
            Department = "CIS",
            Salary=260300m
            };

            Employee manika = new Employee() { 
            Name = "Manika", 
            Department = "CIS",
            Salary=260300m
            };

            return new Employee?[] { naj, arvind, harshini, manika, null }
        }
    }
}