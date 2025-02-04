using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Xunit;
using StudentManagement.Data;
using StudentManagement.Models;
using StudentManagement.Controllers;
using Microsoft.AspNetCore.Mvc;


namespace StudentManagementTests
{
    public class StudentsControllerTests
    {
        [Fact]
        public async Task GetStudents_ReturnsAllStudents()
        {
            // DbContextOptions in-memory for database
            var options = new DbContextOptionsBuilder<StudentContext>()
                .UseInMemoryDatabase(databaseName: "StudentObject")
                .Options;

            //add fake students to testdata
            using (var context = new StudentContext(options))
            {
                context.Students.Add(new Student { Id = 1, FirstName = "John", LastName = "Doe", Age = 20 });
                context.Students.Add(new Student { Id = 2, FirstName = "Jane", LastName = "Smith", Age = 22 });
                await context.SaveChangesAsync();
            }

            // Use a clean instance of the context to run the test
            using (var context = new StudentContext(options))
            {
                var controller = new StudentsController(context);
                // Act
                var result = await controller.GetStudents();
                // Assert check that all students are returned
                var actionResult = Assert.IsType<ActionResult<IEnumerable<Student>>>(result);
                var studentsList = Assert.IsAssignableFrom<IEnumerable<Student>>(actionResult.Value);

                Assert.Equal(2, studentsList.Count());
            }
        }

        [Fact]
        public async Task GetStudent_ReturnsNotFound_WhenStudentDoesNotExist()
        {
            // in-memory database create
            var options = new DbContextOptionsBuilder<StudentContext>()
                .UseInMemoryDatabase(databaseName: "StudentDbTest_NotFound")
                .Options;

            // Use a clean instance of the context to run the test
            using (var context = new StudentContext(options))
            {
                var controller = new StudentsController(context);
                // Act
                var result = await controller.GetStudent(99);//does not exist
                                                             // Assert check that student is not found
                Assert.IsType<NotFoundResult>(result.Result);
            }
        }

        [Fact]
        public async Task PostStudent_CreatesStudent_Successfully()
        {
            // in-memory database create
            var options = new DbContextOptionsBuilder<StudentContext>()
                .UseInMemoryDatabase(databaseName: "StudentObjectPost")
                .Options;

            // Use a clean instance of the context to run the test
            using (var context = new StudentContext(options))
            {
                var controller = new StudentsController(context);
                int studentId = 1;
                Student student = new() { Id = studentId, FirstName = "Bob", LastName = "Bobb", Age = 40 };
                // Act
                var result = await controller.PostStudent(student);//add new student
                // Assert check that student was added

                var actionResult = Assert.IsType<ActionResult<Student>>(result);
                var createdAtActionResult = Assert.IsType<CreatedAtActionResult>(actionResult.Result);
                var createdStudent = Assert.IsType<Student>(createdAtActionResult.Value);
                Assert.Equal(studentId, createdStudent.Id);
            }
        }


        [Theory]
        [InlineData(0, "", "", -1)] // Invalid data: empty names and negative age
        [InlineData(1, "A", "B", 0)] // Edge case: minimum valid age
        [InlineData(2, "John", "Doe", 150)] // Edge case: extremely high age
        [InlineData(3, "VeryLongFirstNameVeryLongFirstNameVeryLongFirstName", "VeryLongLastNameVeryLongLastNameVeryLongLastName", 25)] // Edge case: very long names
        public async Task PostStudent_HandlesEdgeCases(int id, string firstName, string lastName, int age)
        {
            // in-memory database create
            var options = new DbContextOptionsBuilder<StudentContext>()
                .UseInMemoryDatabase(databaseName: "StudentObjectEdge")
                .Options;

            // Use a clean instance of the context to run the test
            using (var context = new StudentContext(options))
            {
                var controller = new StudentsController(context);
                Student student = new() { Id = id, FirstName = firstName, LastName = lastName, Age = age };
                // Act
                var result = await controller.PostStudent(student);//add new student

                if (string.IsNullOrEmpty(firstName) || string.IsNullOrEmpty(lastName) || age < 0)
                {
                    // Assert check that bad request is returned for invalid data
                    Assert.IsType<BadRequestResult>(result.Result);
                }
                else
                {
                    // Assert check that student was added
                    var actionResult = Assert.IsType<ActionResult<Student>>(result);
                    var createdAtActionResult = Assert.IsType<CreatedAtActionResult>(actionResult.Result);
                    var createdStudent = Assert.IsType<Student>(createdAtActionResult.Value);
                    Assert.Equal(id, createdStudent.Id);
                    Assert.Equal(firstName, createdStudent.FirstName);
                    Assert.Equal(lastName, createdStudent.LastName);
                    Assert.Equal(age, createdStudent.Age);
                }
            }
        }


    }
}