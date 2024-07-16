using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using sms.backend.Data;

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
    public async Task<ActionResult<IEnumerable<Class>>> GetClasses()
    {
        _logger.LogInformation("Getting all classes");
        return await _context.Classes.ToListAsync();
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<Class>> GetClass(int id)
    {
        _logger.LogInformation("Getting class with ID: {Id}", id);
        var classItem = await _context.Classes.FindAsync(id);
        if (classItem == null)
        {
            _logger.LogWarning("Class with ID: {Id} not found", id);
            return NotFound();
        }
        return classItem;
    }

    [HttpPost]
    public async Task<ActionResult<Class>> PostClass(Class classItem)
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
