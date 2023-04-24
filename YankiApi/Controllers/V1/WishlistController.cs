using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Newtonsoft.Json;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using YankiApi.DataAccessLayer;
using YankiApi.DTOs.WishlistDTOs;
using YankiApi.Entities;

namespace YankiApi.Controllers.V1
{
    /// <summary>
    /// Wishlist
    /// </summary>
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(Roles = "Member")]
    public class WishlistController : ControllerBase
    {
        private readonly AppDbContext _context;
        private readonly IMapper _mapper;
        private readonly UserManager<AppUser> _userManager;

        /// <summary>
        /// Context , Mapper and UserManager
        /// </summary>
        /// <param name="context"></param>
        /// <param name="mapper"></param>
        /// <param name="userManager"></param>
        public WishlistController(AppDbContext context, IMapper mapper, UserManager<AppUser> userManager)
        {
            _context = context;
            _mapper = mapper;
            _userManager = userManager;
        }
        /// <summary>
        /// Add Wishlist Item
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("add")]
        [ProducesResponseType(201)]
        [ProducesResponseType(400)]
        [Produces("application/json")]
        [Authorize]
        public async Task<IActionResult> Post(int? id)
        {
            if (id == null) { return BadRequest(); }

            if (!await _context.Products.AnyAsync(p => !p.IsDeleted && p.Id == id))
            {
                return NotFound();
            }

            List<Wishlist> wishlists = await _context.Wishlists.ToListAsync();
            if (wishlists.Any(w => w.ProductId == id))
            {
                return BadRequest();
            }

            if (User.Identity.IsAuthenticated)
            {
                AppUser appUser = await _userManager.Users.Include(u => u.Wishlist.Where(b => !b.IsDeleted)).FirstOrDefaultAsync(u => u.NormalizedUserName == User.Identity.Name.ToUpperInvariant());

                if (appUser.Wishlist.Any(b => b.ProductId == id))
                {
                    appUser.Wishlist.FirstOrDefault(b => b.ProductId == id).Count = wishlists.FirstOrDefault(b => b.Id == id).Count;
                }
                else
                {
                    Wishlist dbWishlist = new()
                    {
                        ProductId = id,
                        Count = 1,
                    };

                    appUser.Wishlist.Add(dbWishlist);
                    await _context.SaveChangesAsync();

                }
            }
            return Ok();


        }

        /// <summary>
        /// Get Wishlist Product
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Produces("application/json")]
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

            var wishlists = await _context.Wishlists.Where(w => w.UserId == userId && !w.IsDeleted).ToListAsync();
            var productIds = wishlists.Select(w => w.ProductId).ToList();

            var products = await _context.Products.Where(p => productIds.Contains(p.Id)).ToListAsync();

            return Ok(products);
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
            Wishlist wishlist = await _context.Wishlists.FirstOrDefaultAsync(w => w.ProductId == id);

            if (wishlist == null) { return NotFound(); }

            if (User.Identity.IsAuthenticated)
            {
                AppUser appUser = await _userManager.Users.Include(u => u.Wishlist.Where(w => !w.IsDeleted)).FirstOrDefaultAsync(u => u.NormalizedUserName == User.Identity.Name.ToUpperInvariant());

                Wishlist userWishlist = appUser.Wishlist.FirstOrDefault(w => w.ProductId == id);

                if (userWishlist == null) { return BadRequest(); }

                _context.Wishlists.Remove(wishlist);
                await _context.SaveChangesAsync();

            }
            return NoContent();
        }





    }
}
