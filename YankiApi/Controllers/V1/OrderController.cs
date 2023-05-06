using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using YankiApi.DataAccessLayer;
using YankiApi.DTOs.BasketDTOs;
using YankiApi.DTOs.OrderDTOs;
using YankiApi.Entities;

namespace YankiApi.Controllers.V1
{
    /// <summary>
    /// Order Controller
    /// </summary>
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class OrderController : ControllerBase
    {
        private readonly AppDbContext _context;
        private readonly UserManager<AppUser> _userManager;
        private readonly IMapper _mapper;


        public OrderController(AppDbContext context, UserManager<AppUser> userManager, IMapper mapper)
        {
            _context = context;
            _userManager = userManager;
            _mapper = mapper;
        }
        /// <summary>
        /// Get All Orders
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Authorize(Roles = "SuperAdmin , Admin")]
        [ProducesResponseType(201)]
        [ProducesResponseType(400)]
        [Produces("application/json")]
        public async Task<IActionResult> Get(int? id)
        {
            if(id != null && id > 0)
            {
                Order order = await _context.Orders.Include(o => o.OrderItems).ThenInclude(i => i.Product).FirstOrDefaultAsync( o => !o.IsDeleted && o.Id == id);

                return Ok(order);
            }

            List<Order> orders = await _context.Orders.Include(o => o.OrderItems).Where(o => !o.IsDeleted).ToListAsync();

            return Ok(orders);
        }



        [HttpGet]
        [Route("user")]
        [ProducesResponseType(201)]
        [ProducesResponseType(400)]
        [Produces("application/json")]
        public async Task<IActionResult> GetOrder()
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


            List<Order> orders = await _context.Orders
                .Include(o => o.OrderItems)
                .ThenInclude(oi => oi.Product)
                .Where(o => o.UserId == userId)
                .ToListAsync();

            return Ok(orders);
        }

        [HttpPost]
        [Route("checkout")]
        public async Task<IActionResult> Checkout(OrderPostDto orderDto)
        {
            var authHeader = HttpContext.Request.Headers["Authorization"];
            var token = authHeader.ToString().Split(' ')[1];
            var tokenHandler = new JwtSecurityTokenHandler();
            var jwtToken = tokenHandler.ReadJwtToken(token);

            AppUser appUser = await _userManager.Users
                .Include(u => u.Orders)
                .Include(u => u.Baskets.Where(ub => !ub.IsDeleted))
                .FirstOrDefaultAsync(u => u.NormalizedUserName == User.Identity.Name.ToUpperInvariant());


            if (appUser == null)
            {
                return Unauthorized();
            }
            List<OrderItem> orderItems = appUser.Baskets.Select(basket => new OrderItem
            {
                Count = basket.Count,
                ProductId = basket.Id,
                Price = basket.Price,
                CreatedAt = DateTime.UtcNow.AddHours(4),
                CreatedBy = $"{appUser.Name} {appUser.SurName}"
            }).ToList();

            Order order = new()
            {
                UserId = appUser.Id,
                Country = orderDto.Country,
                Phone = orderDto.Phone,
                PostalCode = orderDto.Postalcode,
                TotalPrice = orderItems.Sum(o => o.Price),
                Name = orderDto.Name,
                SurName = orderDto.Surname,
                Email = orderDto.Email,
                No = (appUser.Orders?.Count() ?? 0) > 0 ? appUser.Orders.Last().No + 1 : 1,
                OrderItems = orderItems,
                CreatedAt = DateTime.UtcNow.AddHours(4),
                CreatedBy = $"{appUser.Name} {appUser.SurName}"
                
                
            };


            foreach (Basket basket in appUser.Baskets)
            {
                _context.Baskets.Remove(basket);
            }
            await _context.Orders.AddAsync(order);
            await _context.SaveChangesAsync();

            return Ok();
        }
    }
}
