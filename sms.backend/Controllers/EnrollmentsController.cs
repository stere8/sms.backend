using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using sms.backend.Data;
using sms.backend.Views;

[ApiController]
[Route("[controller]")]
public class EnrollmentsController : ControllerBase
{
    private readonly SchoolContext _context;
    private readonly ILogger<EnrollmentsController> _logger;

    public EnrollmentsController(SchoolContext context, ILogger<EnrollmentsController> logger)
    {
        _context = context;
        _logger = logger;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<EnrollmentsViews>>> GetEnrollments()
    {
        _logger.LogInformation("Getting all enrollments");
        var enrollments = await _context.Enrollments.ToListAsync();
        var students = await _context.Students.ToListAsync();
        var classes = await _context.Classes.ToListAsync();

        var returnedViewsList = new List<EnrollmentsViews>();

        foreach (var enroll in enrollments)
        {
            var student = students.FirstOrDefault(s => s.StudentId == enroll.StudentId);
            var classItem = classes.FirstOrDefault(c => c.ClassId == enroll.ClassId);

            if (student != null && classItem != null)
            {
                var studentName = $"{student.FirstName} {student.LastName}";
                returnedViewsList.Add(new EnrollmentsViews()
                {
                    EnrollmentRef = enroll.EnrollmentID,
                    EnrolledClass = classItem.Name,
                    EnrolledStudent = studentName
                });
            }
        }

        _logger.LogInformation("Successfully retrieved enrollments.");
        return Ok(returnedViewsList);
    }

    [HttpGet("{studentId}/{classId}")]
    public async Task<ActionResult<Enrollment>> GetEnrollment(int studentId, int classId)
    {
        _logger.LogInformation("Getting enrollment for Student ID: {StudentId} and Class ID: {ClassId}", studentId, classId);
        var enrollment = await _context.Enrollments.FindAsync(studentId, classId);
        if (enrollment == null)
        {
            _logger.LogWarning("Enrollment for Student ID: {StudentId} and Class ID: {ClassId} not found", studentId, classId);
            return NotFound();
        }

        return enrollment;
    }

    [HttpPost]
    public async Task<ActionResult<Enrollment>> PostEnrollment(Enrollment enrollment)
    {
        _logger.LogInformation("Creating new enrollment");
        _context.Enrollments.Add(enrollment);
        await _context.SaveChangesAsync();
        return CreatedAtAction(nameof(GetEnrollment), new { studentId = enrollment.StudentId, classId = enrollment.ClassId }, enrollment);
    }

    [HttpDelete("{studentId}/{classId}")]
    public async Task<IActionResult> DeleteEnrollment(int studentId, int classId)
    {
        _logger.LogInformation("Deleting enrollment for Student ID: {StudentId} and Class ID: {ClassId}", studentId, classId);
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
