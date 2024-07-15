using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using sms.backend.Data;


[ApiController]
[Route("[controller]")]
public class ClassesController : ControllerBase
{
    private readonly SchoolContext _context;

    public ClassesController(SchoolContext context)
    {
        _context = context;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<Class>>> GetClasses()
    {
        return await _context.Classes.ToListAsync();
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<Class>> GetClass(int id)
    {
        var classItem = await _context.Classes.FindAsync(id);
        if (classItem == null)
        {
            return NotFound();
        }
        return classItem;
    }

    [HttpPost]
    public async Task<ActionResult<Class>> PostClass(Class classItem)
    {
        _context.Classes.Add(classItem);
        await _context.SaveChangesAsync();
        return CreatedAtAction(nameof(GetClass), new { id = classItem.Id }, classItem);
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> PutClass(int id, Class classItem)
    {
        if (id != classItem.Id)
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