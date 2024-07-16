using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using sms.backend.Data;
using sms.backend.Models;

namespace sms.backend.Controllers;

[ApiController]
[Route("[controller]")]
public class TimetablesController(SchoolContext context, ILogger<TimetablesController> logger)
    : ControllerBase
{
    [HttpGet]
    public async Task<ActionResult<IEnumerable<Timetable>>> GetTimetables()
    {
        logger.LogInformation("Getting all timetables");
        return await context.Timetables.ToListAsync();
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<Timetable>> GetTimetable(int id)
    {
        logger.LogInformation("Getting timetable with ID: {Id}", id);
        var timetable = await context.Timetables.FindAsync(id);
        if (timetable == null)
        {
            logger.LogWarning("Timetable with ID: {Id} not found", id);
            return NotFound();
        }
        return timetable;
    }

    [HttpPost]
    public async Task<ActionResult<Timetable>> PostTimetable(Timetable timetable)
    {
        logger.LogInformation("Creating new timetable");
        context.Timetables.Add(timetable);
        await context.SaveChangesAsync();
        return CreatedAtAction(nameof(GetTimetable), new { id = timetable.TimetableId }, timetable);
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> PutTimetable(int id, Timetable timetable)
    {
        if (id != timetable.TimetableId)
        {
            return BadRequest();
        }
        context.Entry(timetable).State = EntityState.Modified;
        await context.SaveChangesAsync();
        return NoContent();
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteTimetable(int id)
    {
        logger.LogInformation("Deleting timetable with ID: {Id}", id);
        var timetable = await context.Timetables.FindAsync(id);
        if (timetable == null)
        {
            return NotFound();
        }
        context.Timetables.Remove(timetable);
        await context.SaveChangesAsync();
        return NoContent();
    }
}