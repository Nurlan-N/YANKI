using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Newtonsoft.Json;
using System.Data;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using YankiApi.DataAccessLayer;
using YankiApi.DTOs.AuthDTOs;
using YankiApi.DTOs.WishlistDTOs;
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
        private readonly AppDbContext _context;
        private readonly SignInManager<AppUser> _signInManager;

        public AuthController(RoleManager<IdentityRole> roleManager, IMapper mapper, UserManager<AppUser> userManager, IConfiguration configuration, AppDbContext context, SignInManager<AppUser> signInManager)
        {
            _roleManager = roleManager;
            _mapper = mapper;
            _userManager = userManager;
            _config = configuration;
            _context = context;
            _signInManager = signInManager;
        }


        /// <summary>
        /// Register Action
        /// </summary>
        /// <param name="registerDto"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("register")]
        [Produces("application/json")]
        public async Task<IActionResult> Register(RegisterDto registerDto)
        {
            AppUser user = _mapper.Map<AppUser>(registerDto);

            await _userManager.CreateAsync(user, registerDto.Password);

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
            var userId = await _userManager.GetUserIdAsync(appUser);
            Address address = await _context.Addresses.Where(a => a.UserId == userId).FirstOrDefaultAsync();
            List<Wishlist> wishlist = await _context.Wishlists.Where(w => w.UserId == userId).ToListAsync();
            string wishlistCoocies = null;

            if (appUser.Wishlist != null && appUser.Wishlist.Count > 0)
            {
                List<WishlistPostDto> wishlistVMs = new();

                foreach (Wishlist wishlist1 in appUser.Wishlist)
                {
                    WishlistPostDto wishlistVM = new()
                    {
                        Id = (int)wishlist1.ProductId,
                        Count = wishlist1.Count,
                    };
                    wishlistVMs.Add(wishlistVM);
                }

                wishlistCoocies = JsonConvert.SerializeObject(wishlistVMs);

                HttpContext.Response.Cookies.Append("wishlist", wishlistCoocies);
            }

            else
            {
                HttpContext.Response.Cookies.Append("wishlist", "");

            }

            List<Claim> claims = new List<Claim>
            {
                new Claim(ClaimTypes.Name,appUser.UserName),
                new Claim(ClaimTypes.NameIdentifier,appUser.Id),
                new Claim(ClaimTypes.Email,appUser.Email),
                new Claim(ClaimTypes.Surname,appUser.SurName),
                new Claim(ClaimTypes.Country,address.Country),
                new Claim(ClaimTypes.MobilePhone,address?.Phone),
                new Claim(ClaimTypes.PostalCode,address.PostalCode),
            };
            foreach (var r in role)
            {
                Claim claim = new Claim(ClaimTypes.Role, r);
                claims.Add(claim);
            }


            SymmetricSecurityKey key = new(System.Text.Encoding.UTF8.GetBytes(_config.GetSection("JwtSetting:SecretKey").Value));

            SigningCredentials signing = new(key, SecurityAlgorithms.HmacSha256Signature);

            JwtSecurityToken token = new(claims: claims, signingCredentials: signing, expires: DateTime.UtcNow.AddHours(4));

            JwtSecurityTokenHandler handler = new();

            return Ok(handler.WriteToken(token));
        }

        /// <summary>
        /// ReadToken
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Authorize]
        [Route("profile")]
        [Produces("application/json")]
        public IActionResult Profile()
        {
            var token = HttpContext.Request.Headers.Authorization.ToString().Split(' ')[1];

            JwtSecurityTokenHandler jwtSecurityTokenHandler = new();

            JwtSecurityToken test = (JwtSecurityToken)jwtSecurityTokenHandler.ReadToken(token);

            var email = test?.Claims?.FirstOrDefault(c => c.Type == ClaimTypes.Email)?.Value;
            var name = test?.Claims?.FirstOrDefault(c => c.Type == ClaimTypes.Name)?.Value;
            var role = test?.Claims?.FirstOrDefault(c => c.Type == ClaimTypes.Role)?.Value;
            var phone = test?.Claims?.FirstOrDefault(c => c.Type == ClaimTypes.MobilePhone)?.Value;
            var postalcode = test?.Claims?.FirstOrDefault(c => c.Type == ClaimTypes.PostalCode)?.Value;
            var surname = test?.Claims?.FirstOrDefault(c => c.Type == ClaimTypes.Surname)?.Value;
            var country = test?.Claims?.FirstOrDefault(c => c.Type == ClaimTypes.Country)?.Value;

            var data = new { email, name, role, phone, postalcode, surname, country };
            return Ok(data);
        }

        /// <summary>
        /// Update User Action
        /// </summary>
        /// <param name="userDto"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("updateuser")]
        [Produces("application/json")]
        public async Task<IActionResult> UpdateUser(UserDto userDto)
        {
            // Get the current user's ID from the JWT token
            var currentUserId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            // Check if the user exists
            AppUser user = await _userManager.FindByIdAsync(currentUserId);

            if (user == null)
            {
                return Unauthorized();
            }

            // Update the user's information
            user.Name = userDto.Name;
            user.SurName = userDto.SurName;
            user.UserName = userDto.UserName;
            user.Email = userDto.Email;
            user.PhoneNumber = userDto.Phone;
            user.Country = userDto.Country;
            user.PostalCode = userDto.PostalCode;

            // Change the user's password if a new one was provided
            if (!string.IsNullOrEmpty(userDto.Password))
            {
                var token = await _userManager.GeneratePasswordResetTokenAsync(user);
                await _userManager.ResetPasswordAsync(user, token, userDto.Password);
            }

            // Save the changes to the database
            var result = await _userManager.UpdateAsync(user);

            if (result.Succeeded)
            {
                return Ok();
            }

            return BadRequest(result.Errors);
        }

    }
}
