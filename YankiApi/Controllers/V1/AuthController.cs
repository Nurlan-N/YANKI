using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Newtonsoft.Json;
using System.Data;
using System.IdentityModel.Tokens.Jwt;
using System.Net;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using YankiApi.DataAccessLayer;
using YankiApi.DTOs.AuthDTOs;
using YankiApi.DTOs.WishlistDTOs;
using YankiApi.Entities;
using YankiApi.Interfaces;

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
        private readonly IEmailSender _emailSender;

        public AuthController(RoleManager<IdentityRole> roleManager, IMapper mapper, UserManager<AppUser> userManager, IConfiguration configuration, AppDbContext context, SignInManager<AppUser> signInManager, IEmailSender emailSender)
        {
            _roleManager = roleManager;
            _mapper = mapper;
            _userManager = userManager;
            _config = configuration;
            _context = context;
            _signInManager = signInManager;
            _emailSender = emailSender;
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

            if (appUser == null ) { return BadRequest("Email or password is incorrect"); }
            if (appUser?.LockoutEnd != null) return BadRequest("Your account has been blocked " + appUser.LockoutEnd);

            if (!await _userManager.CheckPasswordAsync(appUser, loginDto.Password))
            {
                return BadRequest("Email or password is incorrect");
            }

            var role = await _userManager.GetRolesAsync(appUser);
            var userId = await _userManager.GetUserIdAsync(appUser);
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
                new Claim(ClaimTypes.Name,appUser.Name),
                new Claim(ClaimTypes.GivenName,appUser.UserName),
                new Claim(ClaimTypes.NameIdentifier,appUser.Id),
                new Claim(ClaimTypes.Email,appUser.Email),
                new Claim(ClaimTypes.Surname,appUser.SurName),
                new Claim(ClaimTypes.Country,(appUser.Country != null ? appUser.Country : "Azerbaijan")),
                new Claim(ClaimTypes.MobilePhone,(appUser.Phone != null? appUser.Phone : "+994")),
                new Claim(ClaimTypes.PostalCode,(appUser.PostalCode !=null ? appUser.PostalCode : "5000")),
            };
            foreach (var r in role)
            {
                Claim claim = new Claim(ClaimTypes.Role, r);
                claims.Add(claim);
            }


            SymmetricSecurityKey key = new(System.Text.Encoding.UTF8.GetBytes(_config.GetSection("JwtSetting:SecretKey").Value));

            SigningCredentials signing = new(key, SecurityAlgorithms.HmacSha256Signature);

            JwtSecurityToken token = new(claims: claims, signingCredentials: signing, expires: DateTime.UtcNow.AddHours(240));

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
            var username = test?.Claims?.FirstOrDefault(c => c.Type == ClaimTypes.GivenName)?.Value;
            var role = test?.Claims?.FirstOrDefault(c => c.Type == ClaimTypes.Role)?.Value;
            var phone = test?.Claims?.FirstOrDefault(c => c.Type == ClaimTypes.MobilePhone)?.Value;
            var postalcode = test?.Claims?.FirstOrDefault(c => c.Type == ClaimTypes.PostalCode)?.Value;
            var surname = test?.Claims?.FirstOrDefault(c => c.Type == ClaimTypes.Surname)?.Value;
            var country = test?.Claims?.FirstOrDefault(c => c.Type == ClaimTypes.Country)?.Value;

            var data = new { email, name, role, phone, postalcode, surname, country, username };
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
            var currentUserId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            AppUser user = await _userManager.FindByIdAsync(currentUserId);

            if (user == null)
            {
                return Unauthorized("User is not found");
            }

            user.Name = userDto.Name != null && userDto.Name != "" ? userDto.Name : user.Name;
            user.UserName = userDto.UserName != null && userDto.UserName != "" ? userDto.UserName : user.UserName;
            user.SurName = userDto.SurName != null && userDto.SurName != "" ? userDto.SurName : user.SurName;
            user.UserName = userDto.UserName != null && userDto.UserName != "" ? userDto.UserName : user.UserName;
            user.Email = userDto.Email != null && userDto.Email != "" ? userDto.Email : user.Email;
            user.PhoneNumber = userDto.Phone != null && userDto.Phone != "" ? userDto.Phone : user.Phone;
            user.Country = userDto.Country != null && userDto.Country != "" ? userDto.Country : user.Country;
            user.PostalCode = userDto.PostalCode != null && userDto.PostalCode != "" ? userDto.PostalCode : user.PostalCode;

            if (!await _userManager.CheckPasswordAsync(user, userDto.Password))
            {
                return BadRequest("Password doesn't match");
            }
            if (!string.IsNullOrEmpty(userDto.Password))
            {
                string token = await _userManager.GeneratePasswordResetTokenAsync(user);
                await _userManager.ResetPasswordAsync(user, token, userDto.NewPassword);
            }

            var result = await _userManager.UpdateAsync(user);
            var role = await _userManager.GetRolesAsync(user);


            if (result.Succeeded)
            {
                List<Claim> userClaims = new ()
            {
                new Claim(ClaimTypes.Name,user.Name),
                new Claim(ClaimTypes.GivenName,user.UserName),
                new Claim(ClaimTypes.NameIdentifier,user.Id),
                new Claim(ClaimTypes.Email,user.Email),
                new Claim(ClaimTypes.Surname,user.SurName),
                new Claim(ClaimTypes.Country,user.Country),
                new Claim(ClaimTypes.MobilePhone,user?.PhoneNumber),
                new Claim(ClaimTypes.PostalCode,user.PostalCode),
            };
                foreach (var r in role)
                {
                    Claim claim = new (ClaimTypes.Role, (string)r);
                    userClaims.Add(claim);
                }

                var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_config.GetSection("JwtSetting:SecretKey").Value));
                var signing = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
                var expires = DateTime.Now.AddDays(Convert.ToDouble(_config["Jwt:ExpireDays"]));
                var token = new JwtSecurityToken(
                    _config["Jwt:Issuer"],
                    _config["Jwt:Issuer"],
                    userClaims,
                    expires: expires,
                    signingCredentials: signing
                );

                return Ok(new { Token = new JwtSecurityTokenHandler().WriteToken(token) });
            }

            return BadRequest(result.Errors);
        }


        /// <summary>
        /// Reset Password User
        /// </summary>
        /// <param name="email"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("resetpassword")]
        [Produces("application/json")]
        public async Task<IActionResult> ResetPasswordAction(string email)
        {
            AppUser user = await _userManager.FindByEmailAsync(email);

            if (user == null)
            {
                return BadRequest("User not found");
            }

            var newPassword = GenerateRandomPassword();

            var result = await _userManager.RemovePasswordAsync(user);
            if (result.Succeeded)
            {
                result = await _userManager.AddPasswordAsync(user, newPassword);
                if (result.Succeeded)
                {
                    var message = $"New password: {newPassword}";
                    await _emailSender.SendEmailAsync(email, "Reset Password", message);

                    return Ok("Your password has been reset and your new password has been emailed.");
                }
            }

            return BadRequest("Password reset failed.");
        }

        private string GenerateRandomPassword(int length = 15)
        {
            const string validChars = "ABCDEFGHJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789!@#$%^&*?_-";
            var random = new Random();
            var chars = new char[length];

            for (int i = 0; i < length; i++)
            {
                chars[i] = validChars[random.Next(0, validChars.Length)];
            }
            chars[0]= '-';
            return new string(chars);
        }
    }
}
