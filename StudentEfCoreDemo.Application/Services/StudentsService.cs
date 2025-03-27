using StudentEfCoreDemo.Domain.Entities;
using StudentEfCoreDemo.Domain.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StudentEfCoreDemo.Application.Services
{
    public class StudentsService
    {
        private readonly IStudentRepository _studentRepository;

        public StudentsService(IStudentRepository studentRepository)
        {
            _studentRepository = studentRepository;
        }

        public async Task<List<Student>> GetAllStudentsAsync()
        {
            return await _studentRepository.GetAllAsync();
        }

        public async Task<Student?> GetStudentByIdAsync(int id)
        {
            return await _studentRepository.GetByIdAsync(id);
        }

        public async Task AddStudentAsync(Student student)
        {
            await _studentRepository.AddAsync(student);
        }

        public async Task UpdateStudentAsync(Student student)
        {
            await _studentRepository.UpdateAsync(student);
        }

        public async Task DeleteStudentAsync(int id)
        {
            await _studentRepository.DeleteAsync(id);
        }

    }
}
