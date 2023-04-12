﻿using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using YankiApi.DataAccessLayer;
using YankiApi.DTOs.ProductDTOs;
using YankiApi.DTOs.SettingDTOs;
using YankiApi.Entities;

namespace YankiApi.Controllers.V1
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProductController : ControllerBase
    {
        private readonly AppDbContext _context;
        private readonly IMapper _mapper;

        public ProductController(AppDbContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }
        /// <summary>
        /// Create Product
        /// </summary>
        ///  <remarks>
        /// Sample request:
        ///
        ///     POST api/product
        ///     {
        ///        "Title": "Test",
        ///        "Price": "50"
        ///        "DiscountPrice": "40"
        ///        "Extax": "10"
        ///        "Count": "40"
        ///        "Description": "Test"
        ///        "Seria": "Test"
        ///        "İmage": "Test"
        ///     }
        ///
        /// </remarks>
        /// <param name="product"></param>
        /// <returns>A newly created  setting Id</returns> 
        /// <response code="400">Object Invalid</response>
        /// <response code="409">Name Already Exist</response>
        /// <response code="201">Name Already Exist</response>
        [HttpPost]
        [Route("create")]
        [ProducesResponseType(201)]
        [ProducesResponseType(400)]
        [Produces("application/json")]
        public async Task<IActionResult> Post([FromForm]ProductPostDto productPostDto)
        {
            Product product = _mapper.Map<Product>(productPostDto);
            await _context.Products.AddAsync(product);
            await _context.SaveChangesAsync();

            return Ok();
        }
        /// <summary>
        /// Get All Product
        /// </summary>
        /// <param></param>
        /// <returns></returns>
        /// <response code="400">Object Invalid</response>
        [HttpGet]
        public async Task<IActionResult> Get()
        {
            return Ok(await _context.Products.Where(s => !s.IsDeleted).ToListAsync());
        }

        /// <summary>
        /// Get Product For ID
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet("{id}")]
        public async Task<IActionResult> Get(int id)
        {
            if (id <= 0)
            {
                return BadRequest("Id boşdur.");
            }

            var product = await _context.Products
                .FirstOrDefaultAsync(s => s.Id == id && !s.IsDeleted);

            if (product == null)
            {
                return NotFound($"Id uyğunsuzdur: {id}");
            }

            var productGetDto = _mapper.Map<ProductGetDto>(product);

            return Ok(productGetDto);
        }
        /// <summary>
        /// Update Product
        /// </summary>
        /// <param name="id"></param>
        /// <param name="productUpdateDto"></param>
        /// <returns></returns>
        [HttpPut]
        [ProducesResponseType(204)]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        [Produces("application/json")]
        public async Task<IActionResult> Put([FromForm]ProductUpdateDto productUpdateDto)
        {
            
            await _context.SaveChangesAsync();

            return Ok();
        }
        /// <summary>
        /// Delete Product
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpDelete]
        public async Task<IActionResult> Delete(int id)
        {
            if (id == null) { return BadRequest(); }

            Product product = await _context.Products.FirstOrDefaultAsync(p => !p.IsDeleted && p.Id == id);

            if (product == null) { return NotFound(); }

            product.IsDeleted = true;
            product.DeletedBy = "System";
            product.DeletedAt = DateTime.UtcNow.AddHours(4);

            await _context.SaveChangesAsync();

            return Ok();
        }
    }
}
