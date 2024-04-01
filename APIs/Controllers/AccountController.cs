

using ClassLibrary.Models.Users;
using APIs.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;

using WebProject.Data;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;

using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Text;

namespace APIs.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AccountController : ControllerBase
    {
        private readonly ApplicationDbContext db;
        private readonly UserManager<IdentityUser> _userManager;
        private readonly IConfiguration _configuration;
        private IdentityUser _user;
        public AccountController( ApplicationDbContext db = null, UserManager<IdentityUser> userManager = null, IConfiguration configuration = null)
        {
            this.db = db;
            _userManager = userManager;
            _configuration = configuration;
        }


        // POST: api/Account/register
        [HttpPost]
        [Route("register")]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<ActionResult> Register( [FromBody] ApiUserDto apiUserDto)
        {

            try
            {
                _user = new IdentityUser();
                _user.UserName = apiUserDto.Email;

                _user.Email = apiUserDto.Email;

                _user.PhoneNumber = apiUserDto.PhoneNumber;

                var result = await _userManager.CreateAsync(_user, apiUserDto.Password);

                if (result.Succeeded)
                {
                     var token = await GenerateToken();
                    var resp = new AuthResponseDto
                    {
                        Token = token,
                        UserId = _user.Id,
                        Email = apiUserDto.Email,
                    };
                    return Ok(resp);
                }
                else
                {
                    return BadRequest();
                }
               
            }
            catch (Exception ex)
            {

                return BadRequest(ex.Message);
            }

        }




        // POST: api/Account/login
        [HttpPost]
        [Route("login")]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<ActionResult> Login([FromBody] LoginDto loginDto)
        {  ResponseModel model=new ResponseModel();
           
            if(!_userManager.Users.Any(m=>m.Email== loginDto.Email))
            {
 model.status = false;
                model.message = "البريد الإلكتروني غير صحيح!";
                return Ok(model);
            }
            _user = await _userManager.FindByEmailAsync(loginDto.Email);
            bool isValidUser = await _userManager.CheckPasswordAsync(_user, loginDto.Password);

            if (_user == null || isValidUser == false)
            {
                model.status = false;
                model.message = "كلمة المرور غير صحيحة!";
                return Ok(model);
            }



            var token = await GenerateToken();
            var authResponse= new AuthResponseDto
            {
                Token = token,
                UserId = _user.Id,

                Email = _user.Email,
                PhoneNumber = _user.PhoneNumber,
            };


            if (authResponse == null)
            {
                 model = new ResponseModel { status = false, message = "كلمة المرور أو البريد الإلكتروني غير صحيح!" };
                return Ok(model);
            }
            model.data = authResponse;
            model.status=true;
            return Ok(model);
        }

        // POST: api/Account/refreshtoken

        private async Task<string> GenerateToken()
        {
            var securitykey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["JwtSettings:Key"]));

            var credentials = new SigningCredentials(securitykey, SecurityAlgorithms.HmacSha256);

            var roles = await _userManager.GetRolesAsync(_user);
            var roleClaims = roles.Select(x => new Claim(ClaimTypes.Role, x)).ToList();
            var userClaims = await _userManager.GetClaimsAsync(_user);

            var claims = new List<Claim>
            {
                new Claim(JwtRegisteredClaimNames.Sub, _user.Email),
                new Claim(JwtRegisteredClaimNames.NameId, _user.UserName),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                new Claim(JwtRegisteredClaimNames.Email, _user.Email),
                new Claim("uid", _user.Id),
            }
            .Union(userClaims).Union(roleClaims);

            var token = new JwtSecurityToken(
                issuer: _configuration["JwtSettings:Issuer"],
                audience: _configuration["JwtSettings:Audience"],
                claims: claims,
                expires: DateTime.Now.AddMinutes(Convert.ToInt32(_configuration["JwtSettings:DurationInMinutes"])),
                signingCredentials: credentials
                );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }
}
