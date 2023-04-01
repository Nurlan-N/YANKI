using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
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

            return Ok(product);
        }
    }
}
