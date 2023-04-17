using AutoMapper;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.Data;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using YankiApi.DTOs.AuthDTOs;
using YankiApi.Entities;

namespace YankiApi.Controllers.V1
{
    /// <summary>
    /// Authorize Controller
    /// </summary>
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly IMapper _mapper;
        private readonly UserManager<AppUser> _userManager;
        private readonly IConfiguration _config;

        public AuthController(RoleManager<IdentityRole> roleManager, IMapper mapper, UserManager<AppUser> userManager, IConfiguration configuration)
        {
            _roleManager = roleManager;
            _mapper = mapper;
            _userManager = userManager;
            _config = configuration;
        }


        /// <summary>
        /// Register Action
        /// </summary>
        /// <param name="registerDto"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("register")]
        [Produces("application/json")]
        public async Task<IActionResult> Register([FromForm]RegisterDto registerDto)
        {
            AppUser user = _mapper.Map<AppUser>(registerDto);

            await _userManager.CreateAsync(user,registerDto.Password);

            await _userManager.AddToRoleAsync(user, "Member");

            return Ok();
        }
        /// <summary>
        /// Login Action
        /// </summary>
        /// <param name="loginDto"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("login")]
        public async Task<IActionResult> Login(LoginDto loginDto)
        {
            AppUser appUser = (AppUser)await _userManager.FindByEmailAsync(loginDto.Email);

            if (appUser == null) { return BadRequest(); }

            if (!await _userManager.CheckPasswordAsync(appUser, loginDto.Password))
            {
                return BadRequest();
            }

            var role = await _userManager.GetRolesAsync(appUser);

            List<Claim> claims = new List<Claim>
            {
                new Claim(ClaimTypes.Name,appUser.UserName),
                new Claim(ClaimTypes.NameIdentifier,appUser.Id),
                new Claim(ClaimTypes.Email,appUser.Email),
            };
            foreach (var r in role)
            {
                Claim claim = new Claim(ClaimTypes.Role, r);
                claims.Add(claim);
            }

            SymmetricSecurityKey key = new SymmetricSecurityKey(System.Text.Encoding.UTF8.GetBytes(_config.GetSection("JwtSetting:SecretKey").Value));

            SigningCredentials signing = new SigningCredentials(key, SecurityAlgorithms.HmacSha256Signature);

            JwtSecurityToken token = new JwtSecurityToken(claims: claims, signingCredentials: signing, expires: DateTime.UtcNow.AddHours(4));

            JwtSecurityTokenHandler handler = new JwtSecurityTokenHandler();

            return Ok(handler.WriteToken(token));
        }

        /// <summary>
        /// Email
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Route("email")]

        public async Task<IActionResult> ReadToken()
        {
            string token = HttpContext.Request.Headers.Authorization.ToString().Split(' ')[1];

            JwtSecurityTokenHandler jwtSecurityTokenHandler = new JwtSecurityTokenHandler();

            JwtSecurityToken test = (JwtSecurityToken)jwtSecurityTokenHandler.ReadToken(token);

            var email = test.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Email).Value;
            var name = test.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Name).Value;

            var data = new { email, name };
            return Ok(data);
        }


        //    [HttpGet]
        //    [Route("createRole")]
        //    public async Task<IActionResult> CreateRole()
        //    {   
        //        await _roleManager.CreateAsync(new IdentityRole("SuperAdmin"));
        //        await _roleManager.CreateAsync(new IdentityRole("Admin"));
        //        await _roleManager.CreateAsync(new IdentityRole("Member"));

        //        return Ok();
        //    }

    }
}
