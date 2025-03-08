using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using WebApi04.Abstraction;
using WebApi04.Dto;
using WebApi04.Models;
using WebApi04.RSATools;

namespace WebApi04.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        //private readonly IUserRepository _repository;
        private readonly IUserRepository _userRepository;
        private readonly IConfiguration _configuration;

        public UserController(IConfiguration _configuration, IUserRepository _userRepository)
        {
            this._configuration = _configuration;
            this._userRepository = _userRepository;
        }


        [HttpPost]
        public ActionResult<int> AddUser(UserDto userDTO) //было user
        {
            try
            {

                //return Ok(_repository.AddUser(userDTO)); //было user
                return Ok(_userRepository.AddUser(userDTO));
            }
            catch (Exception ex)
            {

                return StatusCode(409, ex.Message); // было без , ex.Message
            }
            //using (var context = new UserContext)
        }

        // И на этом семинар закончился

        [HttpGet]
        public ActionResult<RoleId> CheckUser(LoginDto loginDTO)
        {
            try
            {
                var roleId = _userRepository.CheckUser(loginDTO);
                return Ok(roleId);
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);

            }
        }

        [AllowAnonymous]
        [HttpPost("login")]
        public ActionResult Login([FromBody] LoginDto loginDTO)
        {
            var user = _userRepository.Authenticate(loginDTO);

            if (user == null)
                return NotFound("Invalid credentials");

            var token = GenerateJwtToken(user);
            return Ok(token);

        }


        private string GenerateJwtToken(UserDto user)
        {

            //var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["Jwt:Key"]));
            //-->удалена после RSA
            var securityKey = new RsaSecurityKey(RSAExtensions.GeneratePrivateKey());

            //var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);
            var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.RsaSha256Signature);
            var claims = new[]
            {
                new Claim(ClaimTypes.NameIdentifier, user.Name),
                new Claim(ClaimTypes.Role, user.Role.ToString())
            };

            var token = new JwtSecurityToken(_configuration["Jwt:Issuer"],
                _configuration["Jwt:Audience"],
                claims,
                expires: DateTime.Now.AddMinutes(8),
                signingCredentials: credentials);

            return new JwtSecurityTokenHandler().WriteToken(token);

        }
    }
}
