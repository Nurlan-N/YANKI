using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Data;
using System.Text.RegularExpressions;
using YankiApi.DataAccessLayer;
using YankiApi.DTOs.CategoryDTOs;
using YankiApi.Entities;

namespace YankiApi.Controllers.V1
{
    /// <summary>
    /// Subscribe For New Product
    /// </summary>
    [Route("api/[controller]")]
    [ApiController]
    public class SubscribeController : ControllerBase
    {
        private readonly AppDbContext _context;

        public SubscribeController(AppDbContext context)
        {
            _context = context;
        }
        /// <summary>
        /// New Subscribe
        /// </summary>
        /// <param name="email"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("create")]
        [ProducesResponseType(201)]
        [ProducesResponseType(400)]
        [Produces("application/json")]
        public async Task<IActionResult> Post(string email)
        {
            if (email == null)
            {
                return BadRequest("Email required");
            }

            if (!Regex.IsMatch(email, @"^[^@\s]+@[^@\s]+\.[^@\s]+$")) 
            {
                return BadRequest("Invalid email format");
            }

            if (_context.Subscribes.Any(s => s.Email.ToLower() == email.ToLower()))
            {
                return BadRequest("This Email Has Already Been Added");
            }

            Subscribe subscribe = new()
            {
                Email = email,
            };

            await _context.Subscribes.AddAsync(subscribe);
            await _context.SaveChangesAsync();

            return Ok($"{subscribe.Email} This email subscribed successfully");
        }
    }
}
