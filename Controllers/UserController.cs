using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Token.Model;

namespace Token.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserController(IConfiguration _config) : ControllerBase
    {
        [HttpGet]
        [Authorize]
        public ActionResult GetUser()
        {
            var _listUser = new List<UserDto>
            {
                new UserDto { IdUser = 1, UserName = "Jorge",   Email = "erudito.tv@gmail.com"},
                new UserDto { IdUser = 2, UserName = "Erudito", Email = "erudito.dev@gmail.com"},
            };

            return Ok(_listUser);
        }

        [HttpPost]
        public ActionResult Login([FromBody] UserDto userDto)
        {
            var jwtSection = _config.GetSection("Jwt").Get<JwtDto>();

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSection!.Key));

            var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            List<Claim> claims = new List<Claim>
            {
                new Claim(JwtRegisteredClaimNames.Name, userDto.UserName),
                new Claim(JwtRegisteredClaimNames.Email, userDto.Email)
            };

            var tokenDescriptor = new JwtSecurityToken(
                    issuer: jwtSection.Issuer,
                    audience: jwtSection.Audience,
                    claims: claims,
                    expires: DateTime.UtcNow.AddYears(jwtSection.AccessExpiration),
                    signingCredentials: credentials
                    );

            var tokenHandler = new JwtSecurityTokenHandler();

            var token = tokenHandler.WriteToken(tokenDescriptor);

            return Ok(token);
        }
    }
}