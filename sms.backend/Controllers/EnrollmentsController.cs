using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using sms.backend.Data;
using System.Collections.Generic;
using System.Threading.Tasks;
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

        List<EnrollmentsViews> retunrnedViewsList = [];

        foreach (var enroll in enrollments)
        {
            var studentFirstName = students.First(student => student.StudentId == enroll.ClassId).FirstName;
            var studentLastName = students.First(student => student.StudentId == enroll.ClassId).LastName;
            var student = $"{studentFirstName} {studentLastName}";
            var classItem = classes.First(Class => Class.ClassId == enroll.ClassId).Name;

            retunrnedViewsList.Add(new EnrollmentsViews()
                { EnrollmentRef = enroll.EnrollmentID, EnrolledClass = classItem, EnrolledStudent = student });
        }


        _logger.LogInformation("Successfully retrieved enrollments.");
        return Ok(retunrnedViewsList);
    }

    [HttpGet("{studentId}/{classId}")]
    public async Task<ActionResult<Enrollment>> GetEnrollment(int studentId, int classId)
    {
        _logger.LogInformation("Getting enrollment for Student ID: {StudentId} and Class ID: {ClassId}", studentId,
            classId);
        var enrollment = await _context.Enrollments.FindAsync(studentId, classId);
        if (enrollment == null)
        {
            _logger.LogWarning("Enrollment for Student ID: {StudentId} and Class ID: {ClassId} not found", studentId,
                classId);
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
        return CreatedAtAction(nameof(GetEnrollment),
            new { studentId = enrollment.StudentId, classId = enrollment.ClassId }, enrollment);
    }

    [HttpDelete("{studentId}/{classId}")]
    public async Task<IActionResult> DeleteEnrollment(int studentId, int classId)
    {
        _logger.LogInformation("Deleting enrollment for Student ID: {StudentId} and Class ID: {ClassId}", studentId,
            classId);
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