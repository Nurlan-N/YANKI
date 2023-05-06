using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Data;
using YankiApi.DataAccessLayer;
using YankiApi.DTOs.AuthDTOs;
using YankiApi.DTOs.UserDTOs;
using YankiApi.Entities;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;

namespace YankiApi.Controllers.V1
{
    /// <summary>
    /// User Controller API
    /// </summary>
    [Route("api/[controller]")]
    [ApiController]
    [ProducesResponseType(201)]
    [ProducesResponseType(400)]
    [Produces("application/json")]
    public class UserController : ControllerBase
    {
        private readonly UserManager<AppUser> _userManager;
        private readonly AppDbContext _context;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly IMapper _mapper;

        public UserController(UserManager<AppUser> userManager, AppDbContext context, RoleManager<IdentityRole> roleManager)
        {
            _userManager = userManager;
            _context = context;
            _roleManager = roleManager;
        }
        /// <summary>
        /// Get All Users
        /// </summary>
        /// <param name="page"></param>
        /// <param name="limit"></param>
        /// <returns></returns>
        [HttpGet]
        [Authorize(Roles = "SuperAdmin,Admin")]
        public async Task<IActionResult> GetUsers(int page, int limit)
        {
            IEnumerable<UserGetDto> users = await _userManager.Users.Where(u => u.UserName != User.Identity.Name)
                .Select(x => new UserGetDto
                {
                    Id = x.Id,
                    Name = x.Name,
                    SurName = x.SurName,
                    Email = x.Email,
                    UserName = x.UserName,
                    LockoutEnd = (DateTimeOffset)x.LockoutEnd
                })
                .ToListAsync();


            foreach (var user in users)
            {
                var userRole = _context.UserRoles.FirstOrDefault(u => u.UserId == user.Id);
                string? roleId = userRole?.RoleId;
                var role = _context.Roles.FirstOrDefault(r => r.Id == roleId);
                string? roleName = role?.Name;

                user.RoleName = roleName;
            }
            var userGets = new
            {
                users =  users.Skip((page - 1) * limit).Take(limit).ToList(),
                count =  users.Count(),
            };

            return Ok(userGets);
        }

        /// <summary>
        /// Create New User 
        /// </summary>
        /// <param name="registerDto"></param>
        /// <returns></returns>
        [HttpPost]
        [Authorize(Roles = "SuperAdmin")]
        public async Task<IActionResult> CreateUser(RegisterDto registerDto)
        {
            if (!ModelState.IsValid) { return BadRequest(); }
            AppUser user = new AppUser
            {
                Name = registerDto.Name,
                Email = registerDto.Email,
                SurName = registerDto.SurName,
                UserName = registerDto.UserName,
                Phone = registerDto.Phone,

            };
            
            string? roleId = _roleManager.Roles.FirstOrDefault(c => c.Name != "SuperAdmin" && c.Name == registerDto.RoleName)?.Name;
            if (roleId == null)
            {
                return BadRequest();
            }
            IdentityResult identityResult = await _userManager.CreateAsync(user, registerDto.Password);
            if (!identityResult.Succeeded)
            {
                foreach (IdentityError identityError in identityResult.Errors)
                {
                    ModelState.AddModelError("", identityError.Description);
                }
                return BadRequest();
            }
            await _userManager.AddToRoleAsync(user, roleId);

            return Ok();
        }
        /// <summary>
        /// Change User Role
        /// </summary>
        /// <param name="dto"></param>
        /// <returns></returns>
        [HttpPut]
        [Route("change-role")]
        [Authorize(Roles = "SuperAdmin")]

        public async Task<IActionResult> ChangeRole(UserRolePutDto dto)
        {

            if (!ModelState.IsValid) { return BadRequest(); }


            AppUser appUser = await _userManager.FindByIdAsync(dto.UserId);

            if (appUser == null) { return NotFound(); }

            string roleId = _context.UserRoles.FirstOrDefault(u => u.UserId == dto.UserId).RoleId;
            string roleName = _context.Roles.FirstOrDefault(r => r.Id == roleId).Name;
            string newRoleName = _roleManager.Roles.FirstOrDefault(c => c.Name != "SuperAdmin" && c.Name == dto.RoleName)?.Name;
            if (newRoleName == null)
            {
                return BadRequest();
            }

            await _userManager.RemoveFromRoleAsync(appUser, roleName);
            await _userManager.AddToRoleAsync(appUser, newRoleName);

            return Ok();
        }


        /// <summary>
        /// Block User
        /// </summary>
        /// <param name="id"></param>
        /// <param name="blockDate"></param>
        /// <returns></returns>
        [HttpPut]
        [Authorize(Roles = "SuperAdmin, Admin")]
        [Route("block")]
        [ProducesResponseType(204)]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        public async Task<IActionResult> BlockUser(string id, DateTimeOffset blockDate)
        {
            if (id == null) BadRequest();

            AppUser user = await _userManager.FindByIdAsync(id);

            if (user == null) { return NotFound(); }

            DateTimeOffset? lockoutEnd = blockDate  ;
            await _userManager.SetLockoutEndDateAsync(user, lockoutEnd);


            return Ok();
        }
        /// <summary>
        /// Un Block User
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        [HttpPut]
        [Authorize(Roles = "SuperAdmin, Admin")]
        [Route("unblock")]
        [ProducesResponseType(204)]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        public async Task<IActionResult> Unblock(string userId)
        {
            if (string.IsNullOrEmpty(userId)) { return NotFound(); }

            AppUser user = await _userManager.FindByIdAsync(userId);
            if (user == null) { return NotFound(); }

            await _userManager.SetLockoutEndDateAsync(user, null);

            return Ok();
        }
    }
}
