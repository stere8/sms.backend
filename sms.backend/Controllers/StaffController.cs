using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using sms.backend.Data;
using System.Collections.Generic;
using System.Threading.Tasks;

[ApiController]
[Route("[controller]")]
public class StaffController : ControllerBase
{
    private readonly SchoolContext _context;
    private readonly ILogger<StaffController> _logger;

    public StaffController(SchoolContext context, ILogger<StaffController> logger)
    {
        _context = context;
        _logger = logger;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<Staff>>> GetStaff()
    {
        _logger.LogInformation("Getting all staff members");
        return await _context.Staff.ToListAsync();
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<Staff>> GetStaff(int id)
    {
        _logger.LogInformation("Getting staff member with ID: {Id}", id);
        var staff = await _context.Staff.FindAsync(id);
        if (staff == null)
        {
            _logger.LogWarning("Staff member with ID: {Id} not found", id);
            return NotFound();
        }
        return staff;
    }

    [HttpPost]
    public async Task<ActionResult<Staff>> PostStaff(Staff staff)
    {
        _logger.LogInformation("Creating new staff member");
        _context.Staff.Add(staff);
        await _context.SaveChangesAsync();
        return CreatedAtAction(nameof(GetStaff), new { id = staff.StaffId }, staff);
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> PutStaff(int id, Staff staff)
    {
        _logger.LogInformation("Updating staff member with ID: {Id}", id);
        if (id != staff.StaffId)
        {
            return BadRequest();
        }
        _context.Entry(staff).State = EntityState.Modified;
        await _context.SaveChangesAsync();
        return NoContent();
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteStaff(int id)
    {
        _logger.LogInformation("Deleting staff member with ID: {Id}", id);
        var staff = await _context.Staff.FindAsync(id);
        if (staff == null)
        {
            return NotFound();
        }
        _context.Staff.Remove(staff);
        await _context.SaveChangesAsync();
        return NoContent();
    }
}
