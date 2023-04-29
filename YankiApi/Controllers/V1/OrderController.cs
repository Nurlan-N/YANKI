﻿using AutoMapper;
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
    [Authorize(Roles = "Member")]
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
                Name = orderDto.Name,
                SurName = orderDto.Surname,
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
