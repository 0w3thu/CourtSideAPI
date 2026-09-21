using CourtSideAPI.Data;
using CourtSideAPI.Model;
using CourtSideAPI.Model.DTO;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

[Authorize]
[Route("api/[controller]")]
[ApiController]
public class TeamsController : ControllerBase
{
    private readonly AppDbContext _context;
    public TeamsController(AppDbContext context)
    {
        _context = context;
    }

    // GET teams that belong to the couch currently Logged In: api/Team
    [HttpGet]
    [Authorize(Roles = "Coach")]
    public async Task<ActionResult<IEnumerable<Team>>> GetTeam()
    {
        try
        {
            var userIdString = User.FindFirstValue(ClaimTypes.NameIdentifier);

            if (!Guid.TryParse(userIdString, out var userId))
            {
                return Unauthorized();
            }

            var teams = await _context.Teams
                .Where(t => t.CoachId == userId)
                .ToListAsync();

            return Ok(teams);
        }
        catch (Exception ex) 
        {
            return StatusCode(500, new
            {
                message = "An error occurred while retrieving your teams.",
                error = ex.Message
            });
        }
    }

    // GET Specific Team: api/Team/5
    [HttpGet("{teamid}")]
    [Authorize(Roles = "Coach")]
    public async Task<ActionResult<Team>> GetTeam(Guid teamid)
    {
        try
        {
            var userIdString = User.FindFirstValue(ClaimTypes.NameIdentifier);

            if (!Guid.TryParse(userIdString, out var userId))
            {
                return Unauthorized();
            }

            var team = await _context.Teams
                .FirstOrDefaultAsync(t =>
                    t.TeamId == teamid &&
                    t.CoachId == userId);

            if (team == null)
            {
                return NotFound();
            }

            return Ok(team);

        } catch(Exception ex)
        {
            return StatusCode(500, new
            {
                message = "An error occurred while retrieving your teams.",
                error = ex.Message
            });
        }
    }


    //POST Team: api/Team
    [HttpPost]
    [Authorize(Roles = "Coach")]
    public async Task<ActionResult<Team>> AddTeam(AddTeamDTO dto)
    {
        try
        {
            var userIdString = User.FindFirstValue(ClaimTypes.NameIdentifier);

            if (!Guid.TryParse(userIdString, out var userId))
            {
                return Unauthorized();
            }

            var team = new Team
            {
                TeamId = Guid.NewGuid(),
                TeamName = dto.TeamName,
                Division = dto.Division,
                logoURL = dto.LogoUrl,
                CoachId = userId
            };

            _context.Teams.Add(team);

            await _context.SaveChangesAsync();

            return CreatedAtAction(
                nameof(GetTeam),
                new { teamid = team.TeamId },
                team
            );
        }
        catch (DbUpdateException ex)
        {
            Console.WriteLine($"Database Error: {ex}");

            return StatusCode(500, new
            {
                message = "An error occurred while creating the team.",
                error = ex.Message
            });
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

    // PATCH: api/Team
    [HttpPatch("{teamid}")]
    [Authorize(Roles = "Coach")]
    public async Task<IActionResult> PatchTeam(Guid teamid,UpdateTeamDTO dto)
    {
        try
        {
            var userIdString = User.FindFirstValue(ClaimTypes.NameIdentifier);

            if (!Guid.TryParse(userIdString, out var userId))
            {
                return Unauthorized();
            }

            var team = await _context.Teams
                .FirstOrDefaultAsync(t =>
                    t.TeamId == teamid &&
                    t.CoachId == userId);

            if (team == null)
            {
                return NotFound();
            }

            if (dto.TeamName != null)
            {
                team.TeamName = dto.TeamName;
            }

            if (dto.Division.HasValue)
            {
                team.Division = dto.Division.Value;
            }

            if (dto.logoURL != null)
            {
                team.logoURL = dto.logoURL;
            }

            await _context.SaveChangesAsync();

            return NoContent();
        }
        catch (DbUpdateConcurrencyException)
        {
            return Conflict("The team was modified by another request.");
        }
        catch (DbUpdateException)
        {
            return StatusCode(500, "An error occurred while updating the team.");
        }
        catch (Exception)
        {
            return StatusCode(500, "An unexpected error occurred.");
        }
    }


    [HttpDelete("{teamid}")]
    [Authorize(Roles = "Coach")]
    public async Task<IActionResult> DeleteTeam(Guid teamid)
    {
        try
        {
            var userIdString = User.FindFirstValue(ClaimTypes.NameIdentifier);

            if (!Guid.TryParse(userIdString, out var userId))
            {
                return Unauthorized();
            }

            var team = await _context.Teams
                .FirstOrDefaultAsync(t =>
                    t.TeamId == teamid &&
                    t.CoachId == userId);

            if (team == null)
            {
                return NotFound();
            }

            _context.Teams.Remove(team);

            await _context.SaveChangesAsync();

            return NoContent();
        }
        catch (DbUpdateException)
        {
            return StatusCode(500, "The team could not be deleted.");
        }
        catch (Exception)
        {
            return StatusCode(500, "An unexpected error occurred.");
        }
    }

    //Additional Endpoints
    [HttpPost("{teamid}/players/{playerid}")]
    [Authorize(Roles = "Coach")]
    public async Task<IActionResult> AddPlayerToTeam(Guid teamid,Guid playerid)
    {
        try
        {
            var userIdString = User.FindFirstValue(ClaimTypes.NameIdentifier);


            if (!Guid.TryParse(userIdString, out var coachId))
            {
                return Unauthorized();
            }

            // Make sure the team belongs to the logged-in coach
            var team = await _context.Teams
                .FirstOrDefaultAsync(t =>
                    t.TeamId == teamid &&
                    t.CoachId == coachId);

            if (team == null)
            {
                return NotFound("Team not found.");
            }

            // Find the existing player
            var player = await _context.Players
                .FirstOrDefaultAsync(p => p.Id == playerid);

            if (player == null)
            {
                return NotFound("Player not found.");
            }

            // Add player to team
            player.TeamId = teamid;

            await _context.SaveChangesAsync();

            return NoContent();
        }
        catch (DbUpdateException)
        {
            return StatusCode(500, "An error occurred while adding the player to the team.");
        }
        catch (Exception)
        {
            return StatusCode(500, "An unexpected error occurred.");
        }
    }

    [HttpGet("{teamid}/players")]
    [Authorize(Roles = "Coach,Player")]
    public async Task<ActionResult<IEnumerable<Player>>> GetTeamPlayers(Guid teamid)
    {
        try
        {
            var userIdString = User.FindFirstValue(ClaimTypes.NameIdentifier);

            if (!Guid.TryParse(userIdString, out var userId))
            {
                return Unauthorized();
            }

            var teamExists = await _context.Teams
                .AnyAsync(t => t.TeamId == teamid && t.CoachId == userId);

            if (!teamExists)
            {
                return NotFound();
            }

            // ADDED: ownership / membership check, split by role.
            if (User.IsInRole("Coach"))
            {
                var ownsTeam = await _context.Teams
                    .AnyAsync(t => t.TeamId == teamid && t.CoachId == userId);

                if (!ownsTeam)
                {
                    return Forbid();
                }
            }
            else
            {
                var isOnTeam = await _context.Players
                    .AnyAsync(p => p.Id == userId && p.TeamId == teamid);

                if (!isOnTeam)
                {
                    return Forbid();
                }
            }


            var players = await _context.Players
                .Where(p => p.TeamId == teamid)
                .ToListAsync();

            return Ok(players);
        }
        catch (Exception)
        {
            return StatusCode(500, "An unexpected error occurred.");
        }
    }

    

}
