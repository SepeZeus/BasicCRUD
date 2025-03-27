using StudentEfCoreDemo.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StudentEfCoreDemo.Tests.Domain
{
    public class StudentTests
    {
        [Fact]
        public void Student_Should_Have_Valid_Properties()
        {
            //Arrange
            var student = new Student
            {
                Id = 1,
                FirstName = "John",
                LastName = "Doe",
                Age = 22
            };

            //Act & Assert
            Assert.Equal(1, student.Id);
            Assert.Equal("John", student.FirstName);
            Assert.Equal("Doe", student.LastName);
            Assert.Equal(22, student.Age);
        }
    }
}
