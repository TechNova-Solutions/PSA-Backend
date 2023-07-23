using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using PSA_Business_Logic.Dtos;
using PSA_Business_Logic.Model;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace PSA_Business_Logic.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly UserManager<User> _userManager;
        private readonly ApplicationSettings _appSettings;
        private readonly IConfiguration _configuration;


        public AuthController(UserManager<User> userManager, IOptions<ApplicationSettings> appSettings, IConfiguration configuration)
        {
            _userManager = userManager;
            _appSettings = appSettings.Value;
            _configuration = configuration;
        }

        [HttpPost]
        [Route("Register")]
        //POST : /api/Auth/Register
        public async Task<IActionResult> Register(UserDto userDto)
        {
            var existingUser = await _userManager.FindByEmailAsync(userDto.Email);
            if (existingUser is not null)
            {
                return Conflict("Email address is already Taken. Please use another email address");
            }
            var user = new User()
            {
                FirstName = userDto.FirstName,
                LastName = userDto.LastName,
                UserName = userDto.Username,
                Email = userDto.Email,
                PhoneNumber = userDto.ContactNumber,
                Address = userDto.Address,
                District = userDto.District
            };
            var result = await _userManager.CreateAsync(user, userDto.Password);
            await _userManager.AddToRoleAsync(user, userDto.Role);
            if (result.Succeeded)
            {
                return StatusCode(StatusCodes.Status201Created, result);
            }
            return StatusCode(statusCode: StatusCodes.Status422UnprocessableEntity, result.Errors);
        }

        [HttpPost]
        [Route("Login")]
        //POST : /api/ApplicationUser/Login

        public async Task<IActionResult> Login(LoginDto loginDto)
        {
            var user = await _userManager.FindByNameAsync(loginDto.Username);
            var isPasswordValid = await _userManager.CheckPasswordAsync(user, loginDto.Password);
            if (user is not null && isPasswordValid)
            {
                //Get role assigned to the user
                var role = await _userManager.GetRolesAsync(user);
                IdentityOptions _options = new IdentityOptions();

                var tokenDescriptor = new SecurityTokenDescriptor
                {
                    Subject = new ClaimsIdentity(new Claim[]
                    {
                        new Claim(JwtRegisteredClaimNames.Sub,user.Id.ToString()),
                        new Claim(ClaimTypes.Role, role.FirstOrDefault())
                    }),
                    Expires = DateTime.UtcNow.AddMinutes(5),
                    SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_appSettings.JWT_Secret)),
                    SecurityAlgorithms.HmacSha256Signature)
                };
                var tokenHandler = new JwtSecurityTokenHandler();
                var securityToken = tokenHandler.CreateToken(tokenDescriptor);
                var token = tokenHandler.WriteToken(securityToken);
                return Ok(new { token });
            }
            else
            {
                return StatusCode(StatusCodes.Status401Unauthorized, "Invalid username or password");
            }
        }
    }
}
