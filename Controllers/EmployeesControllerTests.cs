using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.InMemory;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EmployeeAdminPortal.Controllers;
using EmployeeAdminPortal.Models;
using EmployeeAdminPortal.Data;
using Microsoft.AspNetCore.Mvc;

using Xunit;
using System.Collections.Generic;
using System.Linq;
using EmployeeAdminPortal.Models.Entities;


namespace EmployeeAdminPortal.Tests.Controllers
{
    public class EmployeesControllerTests
    {
        private ApplicationDbContext GetInMemoryDbContext()
        {
            var options = new DbContextOptionsBuilder<ApplicationDbContext>()
                          .UseInMemoryDatabase(databaseName: "EmployeeTestDb")
                          .Options;

            var dbContext = new ApplicationDbContext(options);
            //ensuredeleted is used to run getallemployees
            dbContext.Database.EnsureDeleted();
            //up line
            dbContext.Employees.AddRange(new List<Employee>
            {
                new Employee { Name = "John Doe", Email = "john@example.com", Phone = "123456789", Salary = 50000 },
                new Employee { Name = "Jane Smith", Email = "jane@example.com", Phone = "987654321", Salary = 60000 }
            });
            dbContext.SaveChanges();

            return dbContext;
        }

        [Fact]
        public void GetAllEmployees_ReturnsAllEmployees()
        {
            // Arrange
            var dbContext = GetInMemoryDbContext();
            var controller = new EmployeesController(dbContext);

            // Act
            var result = controller.GetAllEmployees();

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            var employees = Assert.IsType<List<Employee>>(okResult.Value);
            Assert.Equal(2, employees.Count);
        }
        [Fact]
        public void DeleteEmployee_ExistingEmployee_ReturnsOk()
        {
            // Arrange
            var dbContext = GetInMemoryDbContext();
            var controller = new EmployeesController(dbContext);
            var existingEmployeeId = dbContext.Employees.First().Id;

            // Act
            var result = controller.DeleteEmployee(existingEmployeeId);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            var deletedEmployee = Assert.IsType<Employee>(okResult.Value);
            Assert.Equal(existingEmployeeId, deletedEmployee.Id);
        }

        [Fact]
        public void DeleteEmployee_NonExistingEmployee_ReturnsNotFound()
        {
            // Arrange
            var dbContext = GetInMemoryDbContext();
            var controller = new EmployeesController(dbContext);
            var nonExistingEmployeeId = Guid.NewGuid();

            // Act
            var result = controller.DeleteEmployee(nonExistingEmployeeId);

            // Assert
            Assert.IsType<NotFoundResult>(result);
        }
        [Fact]
        public void UpdateEmployee_ExistingEmployee_ReturnsOk()
        {
            // Arrange
            var dbContext = GetInMemoryDbContext();
            var controller = new EmployeesController(dbContext);
            var existingEmployeeId = dbContext.Employees.First().Id;

            var updateEmployeeDto = new UpdataEmployeeDto
            {
                Name = "Updated Name",
                Email = "updated@example.com",
                Phone = "111222333",
                Salary = 55000
            };

            // Act
            var result = controller.UpdateEmployee(existingEmployeeId, updateEmployeeDto);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            var updatedEmployee = Assert.IsType<Employee>(okResult.Value);
            Assert.Equal(updateEmployeeDto.Name, updatedEmployee.Name);
        }

        [Fact]
        public void UpdateEmployee_NonExistingEmployee_ReturnsNotFound()
        {
            // Arrange
            var dbContext = GetInMemoryDbContext();
            var controller = new EmployeesController(dbContext);
            var nonExistingEmployeeId = Guid.NewGuid();

            var updateEmployeeDto = new UpdataEmployeeDto
            {
                Name = "Updated Name",
                Email = "updated@example.com",
                Phone = "111222333",
                Salary = 55000
            };

            // Act
            var result = controller.UpdateEmployee(nonExistingEmployeeId, updateEmployeeDto);

            // Assert
            Assert.IsType<NotFoundResult>(result);
        }
        [Fact]
        public void AddEmployee_ValidEmployee_ReturnsOk()
        {
            // Arrange
            var dbContext = GetInMemoryDbContext();
            var controller = new EmployeesController(dbContext);

            var newEmployeeDto = new AddEmployeeDto
            {
                Name = "New Employee",
                Email = "newemployee@example.com",
                Phone = "123456789",
                Salary = 45000
            };

            // Act
            var result = controller.AddEmployee(newEmployeeDto);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            var employee = Assert.IsType<Employee>(okResult.Value);
            Assert.Equal(newEmployeeDto.Name, employee.Name);
        }
        [Fact]
        public void GetEmployeeById_ExistingEmployee_ReturnsOk()
        {
            // Arrange
            var dbContext = GetInMemoryDbContext();
            var controller = new EmployeesController(dbContext);
            var existingEmployeeId = dbContext.Employees.First().Id;

            // Act
            var result = controller.GetEmployeeById(existingEmployeeId);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            var employee = Assert.IsType<Employee>(okResult.Value);
            Assert.Equal(existingEmployeeId, employee.Id);
            Assert.Equal(dbContext.Employees.First().Name,employee.Name);
        }

        [Fact]
        public void GetEmployeeById_NonExistingEmployee_ReturnsNotFound()
        {
            // Arrange
            var dbContext = GetInMemoryDbContext();
            var controller = new EmployeesController(dbContext);
            var nonExistingEmployeeId = Guid.NewGuid();

            // Act
            var result = controller.GetEmployeeById(nonExistingEmployeeId);

            // Assert
            Assert.IsType<NotFoundResult>(result);
        }

        [Fact]
       public void UpdateEmployee_UpdatesEmployeeButFailsOnSalary()
        {
            // Arrange
            var dbContext = GetInMemoryDbContext();
            var controller = new EmployeesController(dbContext);

            var existingEmployee = dbContext.Employees.First();
            var updateDto = new UpdataEmployeeDto
            {
                Name = "Updated Name",
                Email = "updated@example.com",
                Phone = "123123123",
                Salary = 80000  // Correct salary
            };

            // Act
            var result = controller.UpdateEmployee(existingEmployee.Id, updateDto);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            var updatedEmployee = okResult.Value as Employee;

            // Deliberate error: Expecting the wrong salary, causing the test to fail.
            Assert.Equal(90000, updatedEmployee.Salary);  // Wrong salary expected
        }

    }
}


