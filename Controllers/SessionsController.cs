using CourtSideAPI.Data;
using CourtSideAPI.Model;
using CourtSideAPI.Model.DTO;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;


[ApiController]
[Route("api/[controller]")]
public class SessionController : ControllerBase
{
    private readonly AppDbContext _db;

    public SessionController(AppDbContext db)
    {
        _db = db;
    }

    // POST: api/session
    [HttpPost]
    [Authorize(Roles = "Coach")]
    public async Task<IActionResult> PostSession([FromBody] AddSessionDTO payload)
    {
        try
        {
            var teamExists = await _db.Teams
                .AnyAsync(t => t.TeamId == payload.TeamId);

            if (!teamExists)
                return NotFound("Team not found.");

            var session = new Session
            {
                SessionId = Guid.NewGuid(),
                Title = payload.Title,
                ScheduledDate = payload.ScheduledDate,
                location = payload.location,
                Minutes = payload.Minutes,
                Status = payload.Status,
                recurrenceRule = payload.recurrenceRule,
                recurrenceEndDate = payload.recurrenceEndDate,
                Note = payload.Note,
                TeamId = payload.TeamId
            };

            _db.Sessions.Add(session);
            await _db.SaveChangesAsync();

            return CreatedAtAction(
                nameof(GetSession),
                new { id = session.SessionId },
                session
            );
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error: {ex}");

            return StatusCode(500, new
            {
                message = "An unexpected error occurred.",
                error = ex.Message
            });
        }
    }

    // GET: api/session/{id}
    [HttpGet("{id:guid}")]
    public async Task<IActionResult> GetSession(Guid id)
    {
        var session = await _db.Sessions
            .Include(s => s.Team)
            .FirstOrDefaultAsync(s => s.SessionId == id);

        if (session == null)
            return NotFound("Session not found.");

        return Ok(session);
    }


    // PATCH: api/session/{id}
    [HttpPatch("{id:guid}")]
    [Authorize(Roles = "Coach")]
    public async Task<IActionResult> PatchSession(Guid id,[FromBody] SessionUpdateDTO payload)
    {
        try
        {
            var session = await _db.Sessions
                .FirstOrDefaultAsync(s => s.SessionId == id);

            if (session == null)
                return NotFound("Session not found.");

            if (payload.Title != null)
                session.Title = payload.Title;

            if (payload.ScheduledDate.HasValue)
                session.ScheduledDate = payload.ScheduledDate.Value;

            if (payload.Location != null)
                session.location = payload.Location;

            if (payload.Minutes.HasValue)
                session.Minutes = payload.Minutes.Value;

            if (payload.RecurrenceRule != null)
                session.recurrenceRule = payload.RecurrenceRule;

            if (payload.RecurrenceEndDate.HasValue)
                session.recurrenceEndDate = payload.RecurrenceEndDate.Value;

            if (payload.Note != null)
                session.Note = payload.Note;

            await _db.SaveChangesAsync();

            return Ok(session);
        }catch(Exception ex)
        {
            Console.WriteLine($"Error: {ex}");

            return StatusCode(500, new
            {
                message = "An unexpected error occurred.",
                error = ex.Message
            });
        }
    
    }


    // DELETE: api/session/{id}
    [HttpDelete("{id:guid}")]
    [Authorize(Roles = "Coach")]
    public async Task<IActionResult> DeleteSession(Guid id)
    {
        var session = await _db.Sessions
            .FirstOrDefaultAsync(s => s.SessionId == id);

        if (session == null)
            return NotFound("Session not found.");

        _db.Sessions.Remove(session);

        await _db.SaveChangesAsync();

        return NoContent();
    }


    // GET: api/session/{sessionId}/attendance
    [HttpGet("{sessionId:guid}/attendance")]
    [Authorize(Roles = "Coach")]
    public async Task<IActionResult> GetAttendance(Guid sessionId)
    {
        var session = await _db.Sessions
            .Include(s => s.Team)
            .FirstOrDefaultAsync(s => s.SessionId == sessionId);

        if (session == null)
            return NotFound("Session not found.");

        var players = await _db.Users
            .OfType<Player>()
            .Where(p => p.TeamId == session.TeamId)
            .ToListAsync();

        var attendance = await _db.Attendances
            .Where(a => a.SessionId == sessionId)
            .ToListAsync();

        var result = players.Select(player =>
        {
            var existingAttendance = attendance
                .FirstOrDefault(a => a.PlayerId == player.Id);

            return new
            {
                PlayerId = player.Id,
                PlayerName = player.FullName,
                JerseyNumber = player.jerseryNumber,
                Position = player.Position,
                IsPresent = existingAttendance?.IsPresent ?? false
            };
        });

        return Ok(result);
    }


    // PATCH: api/session/{sessionId}/attendance/{playerId}
    [HttpPatch("{sessionId:guid}/attendance/{playerId:guid}")]
    [Authorize(Roles = "Coach")]
    public async Task<IActionResult> MarkAttendance(Guid sessionId,Guid playerId,[FromBody] AttendanceUpdateDTO payload)
    {
        var session = await _db.Sessions
            .FirstOrDefaultAsync(s => s.SessionId == sessionId);

        if (session == null)
            return NotFound("Session not found.");

        var player = await _db.Users
            .OfType<Player>()
            .FirstOrDefaultAsync(p =>
                p.Id == playerId &&
                p.TeamId == session.TeamId);

        if (player == null)
            return NotFound("Player not found in this team.");

        var attendance = await _db.Attendances
            .FirstOrDefaultAsync(a =>
                a.SessionId == sessionId &&
                a.PlayerId == playerId);

        if (attendance == null)
        {
            attendance = new Attendance
            {
                SessionId = sessionId,
                PlayerId = playerId,
                IsPresent = payload.IsPresent
            };

            _db.Attendances.Add(attendance);
        }
        else
        {
            attendance.IsPresent = payload.IsPresent;
        }

        await _db.SaveChangesAsync();

        return Ok(new
        {
            SessionId = sessionId,
            PlayerId = playerId,
            PlayerName = player.FullName,
            IsPresent = attendance.IsPresent
        });
    }

    // GET: api/team/session/{teamId}
    [HttpGet("session/{teamId:guid}")]
    [Authorize(Roles = "Coach,Player")]
    public async Task<IActionResult> GetAllTeamSessions(Guid teamId)
    {
        var teamExists = await _db.Teams
            .AnyAsync(t => t.TeamId == teamId);

        if (!teamExists)
            return NotFound("Team not found.");

        var sessions = await _db.Sessions
            .Where(s => s.TeamId == teamId)
            .OrderBy(s => s.ScheduledDate)
            .ToListAsync();

        return Ok(sessions);
    }
}
