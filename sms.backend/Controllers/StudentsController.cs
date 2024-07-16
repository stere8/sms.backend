using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using sms.backend.Data;

[ApiController]
[Route("[controller]")]
public class StudentsController : ControllerBase
{
    private readonly SchoolContext _context;
    private readonly ILogger<StudentsController> _logger;

    public StudentsController(SchoolContext context, ILogger<StudentsController> logger)
    {
        _context = context;
        _logger = logger;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<Student>>> GetStudents()
    {
        _logger.LogInformation("Getting all students");
        return await _context.Students.ToListAsync();
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<Student>> GetStudent(int id)
    {
        _logger.LogInformation("Getting student with ID: {Id}", id);
        var student = await _context.Students.FindAsync(id);
        if (student == null)
        {
            _logger.LogWarning("Student with ID: {Id} not found", id);
            return NotFound();
        }
        return student;
    }

    [HttpPost]
    public async Task<ActionResult<Student>> PostStudent(Student student)
    {
        _logger.LogInformation("Creating new student");
        _context.Students.Add(student);
        await _context.SaveChangesAsync();
        return CreatedAtAction(nameof(GetStudent), new { id = student.StudentId }, student);
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> PutStudent(int id, Student student)
    {
        _logger.LogInformation("Updating student with ID: {Id}", id);
        if (id != student.StudentId)
        {
            return BadRequest();
        }
        _context.Entry(student).State = EntityState.Modified;
        await _context.SaveChangesAsync();
        return NoContent();
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteStudent(int id)
    {
        _logger.LogInformation("Deleting student with ID: {Id}", id);
        var student = await _context.Students.FindAsync(id);
        if (student == null)
        {
            return NotFound();
        }
        _context.Students.Remove(student);
        await _context.SaveChangesAsync();
        return NoContent();
    }
}
