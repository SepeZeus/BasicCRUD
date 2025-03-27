using Microsoft.AspNetCore.Mvc;
using Moq;
using StudentEfCoreDemo.Application.Services;
using StudentEfCoreDemo.Domain.Entities;
using StudentEfCoreDemo.Domain.Interfaces;
using StudentEfCoreDemo.Presentation.Controllers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StudentEfCoreDemo.Tests.Presentation
{
    public class StudentControllerTests
    {
        private readonly Mock<IStudentRepository> _mockStudentRepository;
        private readonly StudentsService _studentsService;
        private readonly StudentController _studentController;

        public StudentControllerTests()
        {
            _mockStudentRepository = new Mock<IStudentRepository>();
            _studentsService = new StudentsService(_mockStudentRepository.Object);
            _studentController = new StudentController(_studentsService);
        }

        [Fact]
        public async Task GetAll_ShouldReturnStudents()
        {
            // Arrange
            var students = new List<Student> { new Student { Id = 1, FirstName = "John", LastName = "Doe", Age = 22 } };
            _mockStudentRepository.Setup(repo => repo.GetAllAsync()).ReturnsAsync(students);

            // Act
            var result = await _studentController.GetAll();

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            var returnedStudents = Assert.IsType<List<Student>>(okResult.Value);
            Assert.Single(returnedStudents);
        }
    }
}
