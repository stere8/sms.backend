using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Threading.Tasks;
using sms.backend.Data;

[ApiController]
[Route("[controller]")]
public class EnrollmentsController : ControllerBase
{
    private readonly SchoolContext _context;

    public EnrollmentsController(SchoolContext context)
    {
        _context = context;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<Enrollment>>> GetEnrollments()
    {
        return await _context.Enrollments.Include(e => e.Student).Include(e => e.Class).ToListAsync();
    }

    [HttpGet("{studentId}/{classId}")]
    public async Task<ActionResult<Enrollment>> GetEnrollment(int studentId, int classId)
    {
        var enrollment = await _context.Enrollments.FindAsync(studentId, classId);
        if (enrollment == null)
        {
            return NotFound();
        }
        return enrollment;
    }

    [HttpPost]
    public async Task<ActionResult<Enrollment>> PostEnrollment(Enrollment enrollment)
    {
        _context.Enrollments.Add(enrollment);
        await _context.SaveChangesAsync();
        return CreatedAtAction(nameof(GetEnrollment), new { studentId = enrollment.StudentId, classId = enrollment.ClassId }, enrollment);
    }

    [HttpDelete("{studentId}/{classId}")]
    public async Task<IActionResult> DeleteEnrollment(int studentId, int classId)
    {
        var enrollment = await _context.Enrollments.FindAsync(studentId, classId);
        if (enrollment == null)
        {
            return NotFound();
        }
        _context.Enrollments.Remove(enrollment);
        await _context.SaveChangesAsync();
        return NoContent();
    }
}