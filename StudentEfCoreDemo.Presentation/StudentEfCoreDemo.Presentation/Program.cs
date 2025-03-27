using Microsoft.EntityFrameworkCore;
using StudentEfCoreDemo.Application.Services;
using StudentEfCoreDemo.Domain.Interfaces;
using StudentEfCoreDemo.Infrastructure.Persistence;
using StudentEfCoreDemo.Infrastructure.Repositories;

namespace StudentEfCoreDemo.Presentation
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.

            builder.Services.AddControllers();
            // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();

            var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
            builder.Services.AddDbContext<StudentDbContext>(options => options.UseSqlServer(connectionString));
            builder.Services.AddScoped<IStudentRepository, StudentRepository>();
            builder.Services.AddScoped<StudentsService>();
            var app = builder.Build();

            // Configure the HTTP request pipeline.
            //if (app.Environment.IsDevelopment())
            //{

            //}

            app.UseSwagger();
            app.UseSwaggerUI();

            app.UseHttpsRedirection();

            app.UseAuthorization();


            app.MapControllers();

            app.Run();
        }
    }
}
