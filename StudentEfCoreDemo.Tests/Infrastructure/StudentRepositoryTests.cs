using Microsoft.EntityFrameworkCore;
using StudentEfCoreDemo.Domain.Entities;
using StudentEfCoreDemo.Infrastructure.Persistence;
using StudentEfCoreDemo.Infrastructure.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StudentEfCoreDemo.Tests.Infrastructure
{
    public class StudentRepositoryTests
    {
        private async Task<StudentDbContext> GetDatabaseContext()
        {
            var options = new DbContextOptionsBuilder<StudentDbContext>()
                .UseInMemoryDatabase(databaseName: "StudentDbTest")
                .Options;
            var databaseContext = new StudentDbContext(options);
            databaseContext.Database.EnsureCreated();

            if(await databaseContext.Students.CountAsync() <= 0)
            {
                databaseContext.Students.Add(new Student { Age = 22, FirstName = "John", LastName = "Doe" });
                await databaseContext.SaveChangesAsync();
            }
            return databaseContext;
        }

        [Fact]
        public async Task GetAllAsync_ShouldReturnStudents()
        {
            // Arrange
            var databaseContext = await GetDatabaseContext();
            var studentRepository = new StudentRepository(databaseContext);
            // Act
            var result = await studentRepository.GetAllAsync();
            // Assert
            Assert.NotEmpty(result);
            Assert.Equal("John", result[0].FirstName);
            Assert.Equal("Doe", result[0].LastName);
        }

    }
}
