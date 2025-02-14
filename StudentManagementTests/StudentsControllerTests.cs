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
                }
            }
        }


        [Fact]
        public async Task PutStudent_UpdatesStudent()
        {
            // in-memory database create
            var options = new DbContextOptionsBuilder<StudentContext>()
                .UseInMemoryDatabase(databaseName: "StudentObjectPut")
                .Options;

            // Add a student to the in-memory database
            using (var context = new StudentContext(options))
            {
                Student initialStudent = new() { Id = 1, FirstName = "John", LastName = "Doe", Age = 20 };
                context.Students.Add(initialStudent);
                await context.SaveChangesAsync();
            }

            // Use a new instance of the context to run the update test
            using (var context = new StudentContext(options))
            {
                var controller = new StudentsController(context);

                // Verify that the student was added
                var result = await controller.GetStudents();
                var actionResult = Assert.IsType<ActionResult<IEnumerable<Student>>>(result);
                var studentsList = Assert.IsAssignableFrom<IEnumerable<Student>>(actionResult.Value);
                Assert.Single(studentsList);

                // Detach the existing entity
                var existingStudent = await context.Students.FindAsync(1);
                if (existingStudent != null)
                {
                    context.Entry(existingStudent).State = EntityState.Detached;
                }


                // Update the student
                int studentId = 1;
                Student updatedStudent = new() { Id = studentId, FirstName = "Gob", LastName = "Smacker", Age = 12 };
                var updateResult = await controller.PutStudent(studentId, updatedStudent);

                // Assert check that the update was successful
                Assert.IsType<NoContentResult>(updateResult);
            }

            // Use another new instance of the context to verify the update
            using (var context = new StudentContext(options))
            {
                // Verify that the student was actually updated in the context
                var studentInDb = await context.Students.FindAsync(1);
                Assert.NotNull(studentInDb);
                Assert.Equal("Gob", studentInDb.FirstName);
                Assert.Equal("Smacker", studentInDb.LastName);
                Assert.Equal(12, studentInDb.Age);
            }
        }



        [Theory]
        [InlineData(1,1,"", "", -1)] // Invalid data: empty names and negative age
        [InlineData(1,1,"A", "B", 0)] // Edge case: minimum valid age
        [InlineData(1,1,"John", "Doe", 150)] // Edge case: extremely high age
        [InlineData(99, 1, "Gob", "Smacker", 12)] // Edge case: student does not exist
        [InlineData(-1, -1, "Meat", "Meat", 8)] // Edge case: student does not exist
        public async Task PutStudent_HandlesEdgeCases(int id, int studentId, string firstName, string lastName, int age)
        {
            // in-memory database create
            var options = new DbContextOptionsBuilder<StudentContext>()
                .UseInMemoryDatabase(databaseName: "StudentObjectPut")
                .Options;

            // Use a new instance of the context to run the update test
            using (var context = new StudentContext(options))
            {
                var controller = new StudentsController(context);

                //// Detach the existing entity if it exists
                //var existingStudent = await context.Students.FindAsync(1);
                //if (existingStudent != null)
                //{
                //    context.Entry(existingStudent).State = EntityState.Detached;
                //}

                // Update the student
                Student updatedStudent = new() { Id = id, FirstName = firstName, LastName = lastName, Age = age };
                var updateResult = await controller.PutStudent(studentId, updatedStudent);
                    // Assert check that bad request or not found is returned for invalid data or non-existent student
                if (id != studentId && id != -1)
                {
                        Assert.IsType<BadRequestResult>(updateResult);
                }
                else if (id == -1 || studentId == -1)
                {
                    Assert.IsType<NotFoundResult>(updateResult);
                }
                else
                {
                    Assert.IsType<NoContentResult>(updateResult);
                }
            }

            // Use another new instance of the context to verify the update if it was successful
            if (id == 1 && !string.IsNullOrEmpty(firstName) && !string.IsNullOrEmpty(lastName) && age >= 0)
            {
                using (var context = new StudentContext(options))
                {
                    // Verify that the student was actually updated in the context
                    var studentInDb = await context.Students.FindAsync(1);
                    Assert.NotNull(studentInDb);
                    Assert.Equal(firstName, studentInDb.FirstName);
                    Assert.Equal(lastName, studentInDb.LastName);
                    Assert.Equal(age, studentInDb.Age);
                }
            }
        }





        [Fact]
        public async Task DeleteStudent_RemovesStudentFromDB()
        {
            // in-memory database create
            var options = new DbContextOptionsBuilder<StudentContext>()
                .UseInMemoryDatabase(databaseName: "StudentObjectDelete")
                .Options;

            // Add a student to the in-memory database
            using (var context = new StudentContext(options))
            {
                Student initialStudent = new() { Id = 1, FirstName = "John", LastName = "Doe", Age = 20 };
                context.Students.Add(initialStudent);
                await context.SaveChangesAsync();
            }

            // Use a new instance of the context to run the delete test
            using (var context = new StudentContext(options))
            {
                var controller = new StudentsController(context);

                // Delete the student
                int studentId = 1;
                var deleteResult = await controller.DeleteStudent(studentId);

                // Assert check that the delete was successful
                Assert.IsType<NoContentResult>(deleteResult);
            }

            // Use another new instance of the context to verify the deletion
            using (var context = new StudentContext(options))
            {
                // Verify that the student was actually removed from the context
                var studentInDb = await context.Students.FindAsync(1);
                Assert.Null(studentInDb);
            }
        }



        [Theory]
        [InlineData(0)] // non-existant student
        [InlineData(-1)] // non-existant student -- try negative value
        [InlineData(1)] // existing student
        public async Task DeleteStudents_RemovesStudentsFromDB(int studentId)
        {
            // in-memory database create
            var options = new DbContextOptionsBuilder<StudentContext>()
                .UseInMemoryDatabase(databaseName: "StudentObjectDelete")
                .Options;

            // Add a student to the in-memory database
            using (var context = new StudentContext(options))
            {
                var existingStudent = await context.Students.FindAsync(1);
                if (existingStudent == null)
                {
                    Student initialStudent = new() { Id = 1, FirstName = "John", LastName = "Doe", Age = 20 };
                    context.Students.Add(initialStudent);
                    await context.SaveChangesAsync();
                }
            }

            // Use a new instance of the context to run the delete test
            using (var context = new StudentContext(options))
            {
                var controller = new StudentsController(context);


                // Delete the student
                var deleteResult = await controller.DeleteStudent(studentId);

                if (studentId == 0 || studentId == -1)
                {
                    // Assert check that the delete was not successful
                    Assert.IsType<NotFoundResult>(deleteResult);
                }
                else
                {

                    // Assert check that the delete was successful
                    Assert.IsType<NoContentResult>(deleteResult);
                }
            }

        }

        }
    }