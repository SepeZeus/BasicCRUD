using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using StudentEfCoreDemo.Application.Services;

namespace StudentEfCoreDemo.Presentation.Controllers
{

    [ApiController]
    [Route("api/[controller]")]
    public class StudentsController : ControllerBase
    {
        private readonly StudentsService studentsService;

        public StudentsController(StudentContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<Student>>> GetStudents()
        {
            return await _context.Students.ToListAsync();
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<Student>> GetStudent(int id)
        {
            var student = await _context.Students.FindAsync(id);
            if (student == null)
            {
                return NotFound();
            }
            return student;
        }

        //creates student
        [HttpPost]
        public async Task<ActionResult<Student>> PostStudent(Student student)
        {
            if (string.IsNullOrEmpty(student.FirstName) || string.IsNullOrEmpty(student.LastName) || student.Age < 0)
            {
                return BadRequest();
            }

            _context.Students.Add(student);
            await _context.SaveChangesAsync();
            return CreatedAtAction(nameof(GetStudent), new { id = student.Id }, student);
        }


        //updates student
        [HttpPut("{id}")]
        public async Task<IActionResult> PutStudent(int id, Student student)
        {
            if (id != student.Id)
            {
                return BadRequest();
            }

            _context.Entry(student).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!StudentExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteStudent(int id)
        {
            var student = await _context.Students.FindAsync(id);
            if (student == null) { return NotFound(); }

            _context.Students.Remove(student);
            await _context.SaveChangesAsync();
            return NoContent();
        }

        private bool StudentExists(int id)
        {
            return _context.Students.Any(e => e.Id == id);
        }
    }
}