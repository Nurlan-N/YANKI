using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text.Json.Serialization;
using System.Text.Json;
using YankiApi.DataAccessLayer;
using YankiApi.DTOs.ProductDTOs;
using YankiApi.Entities;

namespace YankiApi.Controllers.V1
{
    /// <summary>
    /// Wishlist
    /// </summary>
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(Roles = "Member")]
    public class BasketController : ControllerBase
    {
        private readonly AppDbContext _context;
        private readonly UserManager<AppUser> _userManager;

        public BasketController(AppDbContext context, UserManager<AppUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }



        /// <summary>
        /// Add Basket Item
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("add")]
        [Authorize]
        [ProducesResponseType(201)]
        [ProducesResponseType(400)]
        [Produces("application/json")]
        public async Task<IActionResult> Post(int? id)
        {
            if (id == null) { return BadRequest(); }

            if (!await _context.Products.AnyAsync(p => !p.IsDeleted && p.Id == id))
            {
                return NotFound();
            }

            List<Basket> baskets = await _context.Baskets.ToListAsync();
            if (baskets.Exists(w => w.ProductId == id))
            {
                baskets.Find(b => b.ProductId == id).Count += 1;
            }

            if (User.Identity.IsAuthenticated)
            {
                AppUser appUser = await _userManager.Users.Include(u => u.Baskets.Where(b => !b.IsDeleted)).FirstOrDefaultAsync(u => u.NormalizedUserName == User.Identity.Name.ToUpperInvariant());

                if (appUser.Baskets.Any(b => b.ProductId == id))
                {
                    appUser.Baskets.FirstOrDefault(b => b.ProductId == id).Count = baskets.FirstOrDefault(b => b.ProductId == id).Count;
                }
                else
                {
                    Basket dbBasket = new()
                    {
                        ProductId = id,
                        Count = 1,
                    };

                    appUser.Baskets.Add(dbBasket);

                }
                await _context.SaveChangesAsync();
            }
            return Ok();

        }
        /// <summary>
        /// Get Basket Products
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Produces("application/json")]
        [Authorize]
        public async Task<IActionResult> Get()
        {
            var authHeader = HttpContext.Request.Headers["Authorization"];
            if (string.IsNullOrEmpty(authHeader) || !authHeader.ToString().StartsWith("Bearer "))
            {
                return Unauthorized();
            }

            var token = authHeader.ToString().Split(' ')[1];

            var tokenHandler = new JwtSecurityTokenHandler();
            var jwtToken = tokenHandler.ReadJwtToken(token);

            var userId = jwtToken.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier)?.Value;

            if (string.IsNullOrEmpty(userId))
            {
                return Unauthorized();
            }

            var options = new JsonSerializerOptions
            {
                ReferenceHandler = ReferenceHandler.Preserve
            };

            //var baskets = await _context.Baskets
            //    //.Include(b => b.Product)
            //    .Where(w => w.UserId == userId && !w.IsDeleted)
            //    .ToListAsync();
            List<Basket> baskets = await _context.Baskets.Where(w => w.UserId == userId && !w.IsDeleted).ToListAsync();
            var productIds = baskets.Select(w => w.ProductId).ToList();

            var products = await _context.Products.Where(p => productIds.Contains(p.Id)).ToListAsync();

            return Ok(products);
           // return Ok(System.Text.Json.JsonSerializer.Serialize(baskets, options));
        }
        /// <summary>
        /// Delete Wishlist Item
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpDelete]
        [Route("delete/{id}")]
        [ProducesResponseType(204)]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        [Authorize]
        public async Task<IActionResult> Delete(int id)
        {
            Basket basket = await _context.Baskets.FirstOrDefaultAsync(w => w.ProductId == id);

            if (basket == null) { return NotFound(); }

            if (User.Identity.IsAuthenticated)
            {
                AppUser appUser = await _userManager.Users.Include(u => u.Baskets.Where(w => !w.IsDeleted)).FirstOrDefaultAsync(u => u.NormalizedUserName == User.Identity.Name.ToUpperInvariant());

                Basket userBasket = appUser.Baskets.FirstOrDefault(w => w.ProductId == id);

                if (userBasket == null) { return BadRequest(); }

                _context.Baskets.Remove(basket);
                await _context.SaveChangesAsync();

            }
            return NoContent();
        }


        //var productIds = baskets.Select(w => w.ProductId).ToList();

        //var products = await _context.Products.Where(p => productIds.Contains(p.Id)).ToListAsync();
    }
}
