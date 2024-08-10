using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using sms.backend.Data;
using sms.backend.Models;
using sms.backend.Views;

namespace sms.backend.Controllers;

[ApiController]
[Route("[controller]")]
public class TeacherEnrollmentsController : ControllerBase
{
    private readonly SchoolContext _context;
    private readonly ILogger<TeacherEnrollmentsController> _logger;

    public TeacherEnrollmentsController(SchoolContext context, ILogger<TeacherEnrollmentsController> logger)
    {
        _context = context;
        _logger = logger;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<TeacherEnrollmentsViews>>> GetTeacherEnrollments()
    {
        _logger.LogInformation("Getting all teacher enrollments");
        var enrollments = await _context.TeacherEnrollments.ToListAsync();
        var teachers = await _context.Staff.ToListAsync();
        var classes = await _context.Classes.ToListAsync();

        var returnedViewsList = new List<TeacherEnrollmentsViews>();

        foreach (var enroll in enrollments)
        {
            var teacher = teachers.FirstOrDefault(s => s.StaffId == enroll.StaffId);
            var classItem = classes.FirstOrDefault(c => c.ClassId == enroll.ClassId);

            if (teacher != null && classItem != null)
            {
                var teacherName = $"{teacher.FirstName} {teacher.LastName}";
                returnedViewsList.Add(new TeacherEnrollmentsViews()
                {
                    EnrollmentRef = enroll.TeacherEnrollmentID,
                    AssignedClass = classItem.Name,
                    EnrolledTeacher = teacherName,
                    AssignedLesson = teacher.SubjectExpertise
                });
            }
        }

        _logger.LogInformation("Successfully retrieved teacher enrollments.");
        return Ok(returnedViewsList);
    }

    [HttpGet("{staffId}/{classId}")]
    public async Task<ActionResult<TeacherEnrollmentsViews>> GetTeacherEnrollment(int staffId, int classId)
    {
        _logger.LogInformation("Getting teacher enrollment for Staff ID: {StaffId} and Class ID: {ClassId}", staffId, classId);
        var enrollment = await _context.TeacherEnrollments.FindAsync(staffId, classId);
        if (enrollment == null)
        {
            _logger.LogWarning("Teacher enrollment for Staff ID: {StaffId} and Class ID: {ClassId} not found", staffId, classId);
            return NotFound();
        }

        var teacher = await _context.Staff.FindAsync(staffId);
        var classItem = await _context.Classes.FindAsync(classId);

        if (teacher == null || classItem == null)
        {
            _logger.LogWarning("Teacher or Class not found");
            return NotFound();
        }

        var teacherName = $"{teacher.FirstName} {teacher.LastName}";
        var teacherEnrollmentView = new TeacherEnrollmentsViews()
        {
            EnrollmentRef = enrollment.TeacherEnrollmentID,
            AssignedClass = classItem.Name,
            EnrolledTeacher = teacherName,
            AssignedLesson = teacher.SubjectExpertise
        };

        return Ok(teacherEnrollmentView);
    }

    [HttpPost]
    public async Task<ActionResult<TeacherEnrollments>> PostTeacherEnrollment(TeacherEnrollments enrollment)
    {
        _logger.LogInformation("Creating new teacher enrollment");
        _context.TeacherEnrollments.Add(enrollment);
        await _context.SaveChangesAsync();
        return CreatedAtAction(nameof(GetTeacherEnrollment), new { staffId = enrollment.StaffId, classId = enrollment.ClassId }, enrollment);
    }

    [HttpDelete("{staffId}/{classId}")]
    public async Task<IActionResult> DeleteTeacherEnrollment(int staffId, int classId)
    {
        _logger.LogInformation("Deleting teacher enrollment for Staff ID: {StaffId} and Class ID: {ClassId}", staffId, classId);
        var enrollment = await _context.TeacherEnrollments.FindAsync(staffId, classId);
        if (enrollment == null)
        {
            return NotFound();
        }

        _context.TeacherEnrollments.Remove(enrollment);
        await _context.SaveChangesAsync();
        return NoContent();
    }
}
