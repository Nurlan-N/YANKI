using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;
using System.Text.Json;
using YankiApi.DataAccessLayer;
using YankiApi.DTOs.CategoryDTOs;
using YankiApi.DTOs.ProductDTOs;
using YankiApi.Entities;
using YankiApi.Extentions;
using YankiApi.Helpers;

namespace YankiApi.Controllers.V1
{
    [Route("api/[controller]")]
    [ApiController]
    public class CategoryController : ControllerBase
    {
        private readonly AppDbContext _context;
        private readonly IMapper _mapper;
        private readonly IWebHostEnvironment _webHostEnvironment;
        private readonly IHttpContextAccessor _contextAccessor;

        public CategoryController(AppDbContext context, IMapper mapper, IHttpContextAccessor contextAccessor, IWebHostEnvironment webHostEnvironment)
        {
            _context = context;
            _mapper = mapper;
            _contextAccessor = contextAccessor;
            _webHostEnvironment = webHostEnvironment;
        }

        /// <summary>
        /// Create Category
        /// </summary>
        ///  <remarks>
        /// Sample request:
        ///
        ///     POST api/category
        ///     {
        ///        "Name": "Test",
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
        [Authorize(Roles = "SuperAdmin, Admin")]
        [ProducesResponseType(201)]
        [ProducesResponseType(400)]
        [Produces("application/json")]
        public async Task<IActionResult> Post([FromForm] CategoryPostDto Dto)
        {
            Category category = _mapper.Map<Category>(Dto);
            await _context.Categories.AddAsync(category);
            await _context.SaveChangesAsync();

            return Ok();
        }

        /// <summary>
        /// Get All Category
        /// </summary>
        /// <param></param>
        /// <returns></returns>
        /// <response code="400">Object Invalid</response>
        [HttpGet]
        public async Task<IActionResult> Get()
        {
            return Ok(await _context.Categories.Where(s => !s.IsDeleted).ToListAsync());
        }
        /// <summary>
        /// Get Category For ID
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("{id}")]
        public async Task<IActionResult> Get(int id)
        {
            if (id <= 0 || id == null)
            {
                return BadRequest("Yalnış İd!!!");
            }

            var options = new JsonSerializerOptions
            {
                ReferenceHandler = ReferenceHandler.Preserve
            };

            Category category = await _context.Categories.Include(c => c.Products).Where(c => !c.IsDeleted).FirstOrDefaultAsync(c => c.Id == id);

            return Ok(JsonSerializer.Serialize(category, options));
        }


        /// <summary>
        /// Update Category
        /// </summary>
        /// <param name="dto"></param>
        /// <returns></returns>
        [HttpPut]
        [Authorize(Roles = "SuperAdmin, Admin")]
        [ProducesResponseType(204)]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        [Route("update-category")]
        [Produces("application/json")]
        public async Task<IActionResult> Put([FromForm] CategoryUpdateDto dto)
        {
            await _context.SaveChangesAsync();

            return Ok();
        }        




        /// <summary>
        /// Delete Category
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpDelete]
        public async Task<IActionResult> Delete(int id)
        {
            if (id == null) { return BadRequest(); }

            Category category = await _context.Categories.FirstOrDefaultAsync(p => !p.IsDeleted && p.Id == id);

            if (category == null) { return NotFound(); }

            category.IsDeleted = true;
            category.DeletedBy = "System";
            category.DeletedAt = DateTime.UtcNow.AddHours(4);

            await _context.SaveChangesAsync();

            return Ok();
        }
    }
}
