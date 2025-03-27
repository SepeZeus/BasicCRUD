using Microsoft.EntityFrameworkCore;
using StudentEfCoreDemo.Domain.Entities;

namespace StudentEfCoreDemo.Infrastructure.Persistence
{
    public class StudentDbContext : DbContext
    {
        public StudentDbContext(DbContextOptions<StudentDbContext> options) : base(options) { }

        public DbSet<Student> Students { get; set; } = null!;
    }
}
