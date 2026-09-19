using CourtSideAPI.Data;
using CourtSideAPI.Model;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Identity.Client.NativeInterop;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace CourtSideAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthenticationController : ControllerBase
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly RoleManager<IdentityRole<Guid>> _roleManager;
        private readonly AppDbContext _db;
        private readonly IConfiguration _configuration;

        public AuthenticationController(UserManager<ApplicationUser> userManager, RoleManager<IdentityRole<Guid>> roleManager, AppDbContext db, IConfiguration configuration)
        {
            _userManager = userManager;
            _roleManager = roleManager;
            _db = db;
            _configuration  = configuration;
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody]ApplicationUserDTO user)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest("Please provide all required fields");
            }

            try
            {
                var userExists = await _userManager.FindByEmailAsync(user.Email);

                if (userExists != null)
                {
                    return BadRequest($"User {user.Email} already exists");

                }

                ApplicationUser newUser = new ApplicationUser()
                {
                    FullName = user.FullName,
                    UserName = user.Email,
                    Email = user.Email,
                    SecurityStamp = Guid.NewGuid().ToString()
                };


                var result = await _userManager.CreateAsync(newUser, user.Password);

                if (!result.Succeeded)
                {
                    return BadRequest(result.Errors);
                    // return BadRequest("User could not be craeted");
                }


                return Created(nameof(Register), $"User {user.Email} created");
            }
            catch (Exception ex) 
            {
                Console.WriteLine($"Registration Error: {ex.Message}");

                return StatusCode(500, new
                {
                    message = "An error occured during Registration"
                });
            }
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginDTO user)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest("Please provide all required fields");
            }

            try
            {
                var existingUser = await _userManager.FindByEmailAsync(user.Email);


                if (existingUser != null && await _userManager.CheckPasswordAsync(existingUser, user.Password))
                {
                    var tokenValue = await GenerateJwtToken(existingUser);
                    return Ok(tokenValue);
                }

                return Unauthorized();

            } catch (Exception ex)
            {
                Console.WriteLine($"Registration Error: {ex.Message}");

                return StatusCode(500, new
                {
                    message = "An error occured during Login"
                });
            }

        }

        private async Task<AuthResultDTO> GenerateJwtToken(ApplicationUser user)
        {
            var authClaims = new List<Claim>()
             {
               new Claim(ClaimTypes.Name, user.UserName ?? ""),
               new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
               new Claim(JwtRegisteredClaimNames.Email, user.Email ?? ""),
               new Claim(JwtRegisteredClaimNames.Sub, user.Email ?? ""),
               new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
             };

            var authSigninKey = new SymmetricSecurityKey(Encoding.ASCII.GetBytes(_configuration["JWT:Secret"]));
            
            var token = new JwtSecurityToken(
                issuer: _configuration["JWT:Issuer"],
                audience: _configuration["JWT:Audience"],
                expires: DateTime.UtcNow.AddMinutes(1), //5-10 mins
                claims: authClaims,
                signingCredentials: new SigningCredentials(authSigninKey, SecurityAlgorithms.HmacSha256)
                );

            var jwtToken = new JwtSecurityTokenHandler().WriteToken(token);

            var refreshToken = new RefreshToken()
            {
                JwtId = token.Id,
                isRevoked = false,
                UserId = user.Id,
                DateAdded = DateTime.UtcNow,
                DateExpired = DateTime.UtcNow.AddMonths(6),
                Token = Guid.NewGuid().ToString()
            };

            await _db.RefreshTokens.AddAsync(refreshToken);
            await _db.SaveChangesAsync();

            var response = new AuthResultDTO()
            {
                Token = jwtToken,
                RefreshToken = refreshToken.Token,
                ExpireAt = token.ValidTo
            };

            return response;

        }
    }
}
