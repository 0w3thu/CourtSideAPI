using CourtSideAPI.Data;
using CourtSideAPI.Model;
using CourtSideAPI.Model.DTO;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Identity.Client.NativeInterop;
using Microsoft.IdentityModel.Tokens;
using System.Globalization;
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


        //Refresh Tokens
        private readonly TokenValidationParameters _tokenValidationParameters;

        public AuthenticationController(UserManager<ApplicationUser> userManager, RoleManager<IdentityRole<Guid>> roleManager, AppDbContext db, IConfiguration configuration, TokenValidationParameters tokenValidationParameters)
        {
            _userManager = userManager;
            _roleManager = roleManager;
            _db = db;
            _configuration  = configuration;
            _tokenValidationParameters = tokenValidationParameters;
        }

        [HttpPost("register/coach")]
        public async Task<IActionResult> Register([FromBody]RegisterCoachDTO user)
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

                Coach newUser = new Coach()
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

                await _userManager.AddToRoleAsync(newUser, "Coach");
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

        [HttpPost("register/player")]
        public async Task<IActionResult> RegisterPlayer([FromBody] RegisterPlayerDTO user)
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

                Player newUser = new Player()
                {
                    FullName = user.FullName,
                    UserName = user.Email,
                    Email = user.Email,
                    jerseryNumber = user.jerseryNumber,
                    Position = user.Position,
                    height = user.height,
                    DateOfBirth = user.DateOfBirth,
                    SecurityStamp = Guid.NewGuid().ToString()

                };


                var result = await _userManager.CreateAsync(newUser, user.Password);

                if (!result.Succeeded)
                {
                    return BadRequest(result.Errors);
                    // return BadRequest("User could not be craeted");
                }

                await _userManager.AddToRoleAsync(newUser, "Player");
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
                    var tokenValue = await GenerateJwtTokenAsync(existingUser, ""); 
                    return Ok(tokenValue);
                }

                return Unauthorized("Email or password is incorrect. Please try again");

            } catch (Exception ex)
            {
                Console.WriteLine($"Registration Error: {ex.Message}");

                return StatusCode(500, new
                {
                    message = "An error occured during Login"
                });
            }

        }

        [HttpPost("refresh-token")]
        public async Task<IActionResult> RefreshToken([FromBody] TokenRequestDTO payload)
        {
            try 
            {
                var result = await VerifyAndGenerateTokenAysnc(payload);

                if (result == null) return BadRequest("Invalid Tokens");

                return Ok(result);

            }
            catch (Exception ex) when (ex.Message == "Token has not expired yet")
            {
                return BadRequest("Token has not expired yet");
            }
            catch (Exception ex) 
            {
                Console.WriteLine($"Registration Error: {ex.Message}");

                return StatusCode(500, new
                {
                    message = "An error occured during Token Refreshing"
                });

            }
        }

        private async Task<AuthResultDTO> VerifyAndGenerateTokenAysnc(TokenRequestDTO payload)
        {
            try
            {
                var jwtTokenHanler = new JwtSecurityTokenHandler();


                var refreshTokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = _tokenValidationParameters.IssuerSigningKey,

                    ValidateIssuer = true,
                    ValidIssuer = _tokenValidationParameters.ValidIssuer,

                    ValidateAudience = true,
                    ValidAudience = _tokenValidationParameters.ValidAudience,

                    ValidateLifetime = false,
                    ClockSkew = TimeSpan.Zero
                };


                //Add Validations

                //Check 1. Check JWT Format
                var tokenInVerification = jwtTokenHanler.ValidateToken(payload.Token, refreshTokenValidationParameters, out var validatedToken);

                //2. Encryption algorithm 
                if (validatedToken is JwtSecurityToken jwtSecurityToken)
                {
                    var result = jwtSecurityToken.Header.Alg.Equals(SecurityAlgorithms.HmacSha256, StringComparison.InvariantCultureIgnoreCase);

                    if (result == false) return null;

                }

                //Check 3. Validate expiry date
                var utcExpiryDate = long.Parse(tokenInVerification.Claims.FirstOrDefault(x => x.Type == JwtRegisteredClaimNames.Exp).Value);

                var expiryDate = UnixTimeStampToDateTimeUTC(utcExpiryDate);
                if (expiryDate > DateTime.UtcNow) throw new Exception("Token has not expired yet");

                //Check 4 . Refresh token exists in the DB
                var dbRefreshToken = await _db.RefreshTokens.FirstOrDefaultAsync(n => n.Token == payload.RefreshToken);

                if (dbRefreshToken is null) throw new Exception("Refresh token does not exist in  our database");
                else
                {
                    //Check 5 - Validate Id
                    var jti = tokenInVerification.Claims.FirstOrDefault(x => x.Type == JwtRegisteredClaimNames.Jti).Value;

                    if (dbRefreshToken.JwtId != jti) throw new Exception("Token does not match");

                    if (dbRefreshToken.DateExpired <= DateTime.UtcNow) throw new Exception("Your refresh token has expired please re-authenticate ");

                    if (dbRefreshToken.isRevoked) throw new Exception("Refresh Token is revoked");


                    //Generate new Token (with existing refresh token)
                    var dbUserData = await _userManager.FindByIdAsync(dbRefreshToken.UserId.ToString());

                    var newTokenResponse = GenerateJwtTokenAsync(dbUserData, payload.RefreshToken);

                    return await newTokenResponse;
                }
            } catch(Exception ex)
            {
                 
                Console.WriteLine($"Refresh Token Error: {ex.Message}");
                throw;
            }
      
        }


        private DateTime UnixTimeStampToDateTimeUTC(long unixTimeStamp)
        {
            var dateTimeVal = new DateTime(1970, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc);
            dateTimeVal = dateTimeVal.AddSeconds(unixTimeStamp);
            return dateTimeVal;
        }

        private async Task<AuthResultDTO> GenerateJwtTokenAsync(ApplicationUser user, string exisitingRefreshToken)
        {
            var authClaims = new List<Claim>()
             {
               new Claim(ClaimTypes.Name, user.UserName ?? ""),
               new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
               new Claim(JwtRegisteredClaimNames.Email, user.Email ?? ""),
               new Claim(JwtRegisteredClaimNames.Sub, user.Email ?? ""),
               new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
             };

            
            var userRoles = await _userManager.GetRolesAsync(user);

            foreach (var role in userRoles)
            {
                authClaims.Add(new Claim(ClaimTypes.Role, role));
            }

            var authSigninKey = new SymmetricSecurityKey(Encoding.ASCII.GetBytes(_configuration["JWT:Secret"]));
            
            var token = new JwtSecurityToken(
                issuer: _configuration["JWT:Issuer"],
                audience: _configuration["JWT:Audience"],
                expires: DateTime.UtcNow.AddMinutes(1), //5-10 mins
                claims: authClaims,
                signingCredentials: new SigningCredentials(authSigninKey, SecurityAlgorithms.HmacSha256)
                );

            var jwtToken = new JwtSecurityTokenHandler().WriteToken(token);
            var refreshToken = new RefreshToken();
            

            if (String.IsNullOrEmpty(exisitingRefreshToken))
            {
                 refreshToken = new RefreshToken()
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
            }

            var response = new AuthResultDTO()
            {
                Token = jwtToken,
                RefreshToken = (string.IsNullOrEmpty(exisitingRefreshToken)) ? refreshToken.Token : exisitingRefreshToken,
                ExpireAt = token.ValidTo
            };

            return response;

        }
    }
}
