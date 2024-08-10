using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using sms.backend.Data;
using sms.backend.Models;
using sms.backend.Views;
using Microsoft.Extensions.Logging;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

[ApiController]
[Route("[controller]")]
public class ClassesController : ControllerBase
{
    private readonly SchoolContext _context;
    private readonly ILogger<ClassesController> _logger;

    public ClassesController(SchoolContext context, ILogger<ClassesController> logger)
    {
        _context = context;
        _logger = logger;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<ClassesView>>> GetClasses()
    {
        _logger.LogInformation("Getting all classes");
        var classes = await _context.Classes.ToListAsync();
        List<ClassesView> ClassViewToReturn = new List<ClassesView>();

        foreach (var oneClass in classes)
        {
            ClassViewToReturn.Add(Fill(oneClass.ClassId));
        }

        return ClassViewToReturn;
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<ClassesView>> GetClass(int id)
    {
        _logger.LogInformation("Getting class with ID: {Id}", id);
        var classItem = await _context.Classes.FindAsync(id);
        if (classItem == null)
        {
            _logger.LogWarning("Class with ID: {Id} not found", id);
            return NotFound();
        }
        var wegot = Fill(classItem.ClassId);
        
        return wegot;
    }

    private ClassesView Fill(int classID)
    {
        List<int> studentsId = _context.Enrollments
            .Where(e => e.ClassId == classID)
            .Select(e => e.StudentId)
            .ToList();
        
        List<Student> students = _context.Students
            .Where(student => studentsId.Contains(student.StudentId))
            .ToList();
        
        List<int> teacherIds = _context.TeacherEnrollments
            .Where(e => e.ClassId == classID)
            .Select(e => e.StaffId)
            .ToList();
        
        List<Staff> classTeachers = _context.Staff
            .Where(teacher => teacherIds.Contains(teacher.StaffId))
            .ToList();
        Class? viewdClass = _context.Classes.FirstOrDefault(classes => classes != null && classes.ClassId == classID);

        List<TimetableView> timetable = GetTimetableForClass(classID);

        return new ClassesView()
        {
            ClassStudents = students,
            ClassTeachers = classTeachers,
            ViewedClass = viewdClass,
            ClassTimetable = timetable
        };
    }

    private List<TimetableView> GetTimetableForClass(int classId)
    {
        var timetables = _context.Timetables
            .Where(t => t.ClassId == classId)
            .Join(_context.Lessons,
                timetable => timetable.LessonId,
                lesson => lesson.LessonId,
                (timetable, lesson) => new TimetableView
                {
                    DayOfWeek = timetable.DayOfWeek,
                    LessonName = lesson.Name,
                    StartTime = timetable.StartTime,
                    EndTime = timetable.EndTime
                })
            .ToList();

        return timetables;
    }

    [HttpPost]
    public async Task<ActionResult<Class>> PostClass(Class? classItem)
    {
        _logger.LogInformation("Creating new class");
        _context.Classes.Add(classItem);
        await _context.SaveChangesAsync();
        return CreatedAtAction(nameof(GetClass), new { id = classItem.ClassId }, classItem);
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> PutClass(int id, Class classItem)
    {
        _logger.LogInformation("Updating class with ID: {Id}", id);
        if (id != classItem.ClassId)
        {
            return BadRequest();
        }
        _context.Entry(classItem).State = EntityState.Modified;
        await _context.SaveChangesAsync();
        return NoContent();
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteClass(int id)
    {
        _logger.LogInformation("Deleting class with ID: {Id}", id);
        var classItem = await _context.Classes.FindAsync(id);
        if (classItem == null)
        {
            return NotFound();
        }
        _context.Classes.Remove(classItem);
        await _context.SaveChangesAsync();
        return NoContent();
    }
}
