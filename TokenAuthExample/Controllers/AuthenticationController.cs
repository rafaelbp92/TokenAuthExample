using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using TokenAuthExample.Data;
using TokenAuthExample.Data.Models;
using TokenAuthExample.Data.ViewModels;

namespace TokenAuthExample.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class AuthenticationController : ControllerBase
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly AppDbContext _appDbContext;
        private readonly IConfiguration _configuration;

        public AuthenticationController(UserManager<ApplicationUser> userManager,
            RoleManager<IdentityRole> roleManager,
            AppDbContext appDbContext,
            IConfiguration configuration)
        {
            _userManager = userManager;
            _roleManager = roleManager;
            _appDbContext = appDbContext;
            _configuration = configuration;
        }

        [HttpPost("register-user")]
        public async Task<IActionResult> Register([FromBody]RegisterVM registerVM)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest("Missing user informations");
            }

            var userExists = await _userManager.FindByEmailAsync(registerVM.Email);
            if (userExists != null)
            {
                return BadRequest($"User with email {registerVM.Email} is already registered");
            }

            ApplicationUser newUser = new ApplicationUser()
            {
                Email = registerVM.Email,
                FirstName = registerVM.FirstName,
                LastName = registerVM.LastName,
                UserName = registerVM.UserName,
                SecurityStamp = Guid.NewGuid().ToString(),
            };

            var result = await _userManager.CreateAsync(newUser, registerVM.Password);

            if (result.Succeeded) return Ok("User created");
            else return BadRequest("User could not be created");
        }
    }
}
