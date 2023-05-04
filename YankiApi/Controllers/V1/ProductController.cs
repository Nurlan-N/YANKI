using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Drawing.Printing;
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

        /// <summary>
        /// Context and Mapper
        /// </summary>
        /// <param name="context"></param>
        /// <param name="mapper"></param>
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
        public async Task<IActionResult> Post(ProductPostDto productPostDto)
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
        [ProducesResponseType(201)]
        [ProducesResponseType(400)]
        [Produces("application/json")]
        public async Task<IActionResult> Get(int page, int limit, int? categoryId)
        {
            var query = _context.Products.Where(s => !s.IsDeleted);

            if (categoryId != null && categoryId > 0)
            {
                query = query.Where(s => s.CategoryId == categoryId);
            }

            var products = new
            {
                product = await query.Skip((page - 1) * limit).Take(limit).ToListAsync(),
                count = await query.CountAsync(),
            };


            return Ok(products);
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

            Product product = await _context.Products
                .FirstOrDefaultAsync(s => s.Id == id && !s.IsDeleted);

            if (product == null)
            {
                return NotFound($"Id uyğunsuzdur: {id}");
            }

            ProductGetDto productGetDto = _mapper.Map<ProductGetDto>(product);

            return Ok(productGetDto);
        }
        /// <summary>
        /// Update Product
        /// </summary>
        /// <param name="id"></param>
        /// <param name="productUpdateDto"></param>
        /// <returns></returns>
        [HttpPut]
        [Route("update")]
        [ProducesResponseType(204)]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        [Produces("application/json")]
        public async Task<IActionResult> Put(ProductUpdateDto productUpdateDto)
        {

            await _context.SaveChangesAsync();

            return Ok();
        }

        /// <summary>
        /// Search Product
        /// </summary>
        /// <param name="search"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("search")]
        public async Task<IActionResult> Search(string search)
        {
            if (search != null)
            {
                return Ok(await _context.Products
                .Where(p => p.IsDeleted == false && p.Title.ToLower().Contains(search.ToLower())).ToListAsync());
            }else { return BadRequest(); }
            
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
