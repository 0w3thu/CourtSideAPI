using CourtSideAPI.Data;
using CourtSideAPI.Model;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

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

        [HttpPost("register-user")]
        public async Task<IActionResult> Register([FromBody]ApplicationUserDTO user)
        {
            var userExists = await _userManager.FindByEmailAsync(user.Email);

            if(userExists != null) 
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


            var result = await _userManager.CreateAsync(newUser,user.Password);

            if (!result.Succeeded)
            {
                return BadRequest(result.Errors);
               // return BadRequest("User could not be craeted");
            }

            
            return Created(nameof(Register),$"User {user.Email} created");
        }
    }
}
