using Moq;
using StudentEfCoreDemo.Application.Services;
using StudentEfCoreDemo.Domain.Entities;
using StudentEfCoreDemo.Domain.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StudentEfCoreDemo.Tests.Application
{
    public class StudentsServiceTests
    {
        private readonly Mock<IStudentRepository> _mockStudentRepository;
        private readonly StudentsService _studentsService;

        public StudentsServiceTests()
        {
            _mockStudentRepository = new Mock<IStudentRepository>();
            _studentsService = new StudentsService(_mockStudentRepository.Object);
        }

        [Fact]
        public async Task GetAllStudentsAsync_ShouldReturnStudents()
        {
            //Arrange
            var students = new List<Student>
            {
                new Student { Id = 1, FirstName = "John", LastName = "Doe", Age = 22 },
                new Student { Id = 1, FirstName = "bob", LastName = "bob", Age = 23 },
            };
            _mockStudentRepository.Setup(repo => repo.GetAllAsync()).ReturnsAsync(students);

            //Act
            var result = await _studentsService.GetAllStudentsAsync();

            //Assert
            Assert.Equal(2, result.Count);
            Assert.Equal("John", result[0].FirstName);
            Assert.Equal("Doe", result[0].LastName);
        }

        [Fact]
        public async Task GetStudentByIdAsync_ShouldReturnStudent_WhenStudentExists()
        {
            // Arrange
            var student = new Student { Id = 1, FirstName = "John", LastName = "Doe", Age = 22 };
            _mockStudentRepository.Setup(repo => repo.GetByIdAsync(1)).ReturnsAsync(student);

            // Act
            var result = await _studentsService.GetStudentByIdAsync(1);

            // Assert
            Assert.NotNull(result);
            Assert.Equal("John", result.FirstName);
            Assert.Equal("Doe", result.LastName);

        }
    }
}