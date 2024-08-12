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
                    EnrollmentRef = enroll.EnrollmentId,
                    EnrolledClass = classItem.Name,
                    EnrolledStudent = studentName
                });
            }
        }

        _logger.LogInformation("Successfully retrieved enrollments.");
        return Ok(returnedViewsList);
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<Enrollment>> GetEnrollmentById(int id)
    {
        _logger.LogInformation("Getting enrollment with ID: {Id}", id);
        var enrollment = await _context.Enrollments.FirstOrDefaultAsync(enrol => enrol.EnrollmentId == id);
        if (enrollment == null)
        {
            _logger.LogWarning("Enrollment with ID: {Id} not found", id);
            return NotFound();
        }

        return enrollment;
    }

    [HttpGet("{studentId}/{classId}")]
    public async Task<ActionResult<Enrollment>> GetEnrollment(int studentId, int classId)
    {
        _logger.LogInformation("Getting enrollment for Student ID: {StudentId} and Class ID: {ClassId}", studentId, classId);
        var enrollment = await _context.Enrollments.FirstOrDefaultAsync(e => e.StudentId == studentId && e.ClassId == classId);
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
        _context.Enrollments.Add(new Enrollment
        {
            StudentId = enrollment.StudentId,
            ClassId = enrollment.ClassId,
            // Do not include EnrollmentID
        });

        await _context.SaveChangesAsync();
        return CreatedAtAction(nameof(GetEnrollment), new { studentId = enrollment.StudentId, classId = enrollment.ClassId }, enrollment);
    }


    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateEnrollment(int id, Enrollment updatedEnrollment)
    {
        if (id != updatedEnrollment.EnrollmentId)
        {
            return BadRequest("EnrollmentID mismatch.");
        }

        var existingEnrollment = await _context.Enrollments.AsNoTracking().FirstOrDefaultAsync(e => e.EnrollmentId == id);
        if (existingEnrollment == null)
        {
            return NotFound();
        }

        // Update only non-identity fields
        existingEnrollment.StudentId = updatedEnrollment.StudentId;
        existingEnrollment.ClassId = updatedEnrollment.ClassId;
        // Add other fields as necessary

        _context.Entry(existingEnrollment).State = EntityState.Modified;

        try
        {
            await _context.SaveChangesAsync();
        }
        catch (DbUpdateConcurrencyException)
        {
            if (!_context.Enrollments.Any(e => e.EnrollmentId == id))
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


    [HttpDelete("{studentId}/{classId}")]
    public async Task<IActionResult> DeleteEnrollment(int studentId, int classId)
    {
        _logger.LogInformation("Deleting enrollment for Student ID: {StudentId} and Class ID: {ClassId}", studentId, classId);
        var enrollment = await _context.Enrollments.FirstOrDefaultAsync(e => e.StudentId == studentId && e.ClassId == classId);
        if (enrollment == null)
        {
            return NotFound();
        }

        _context.Enrollments.Remove(enrollment);
        await _context.SaveChangesAsync();
        return NoContent();
    }
}
