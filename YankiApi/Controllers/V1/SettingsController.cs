using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System;
using YankiApi.DataAccessLayer;
using YankiApi.DTOs.SettingDTOs;
using YankiApi.Entities;

namespace YankiApi.Controllers.V1
{
    [Route("api/[controller]")]
    [ApiController]
    public class SettingsController : ControllerBase
    {

        private readonly AppDbContext _context;
        private readonly IMapper _mapper;

        public SettingsController(AppDbContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }


        /// <summary>
        /// Create Settings
        /// </summary>
        ///  <remarks>
        /// Sample request:
        ///
        ///     POST api/settings
        ///     {
        ///        "key": "Test",
        ///        "value": "test"
        ///     }
        ///
        /// </remarks>
        /// <param name="setting"></param>
        /// <returns>A newly created  setting Id</returns> 
        /// <response code="400">Object Invalid</response>
        /// <response code="409">Name Already Exist</response>
        /// <response code="201">Name Already Exist</response>
        [HttpPost]
        [Route("create")]
        [ProducesResponseType(201)]
        [ProducesResponseType(400)]
        [Produces("application/json")]
        public async Task<IActionResult> Post(SettingPostDto settingPostDto)
        {
            Setting setting = _mapper.Map<Setting>(settingPostDto);
            await _context.Settings.AddAsync(setting);
            await _context.SaveChangesAsync();

            return Ok(setting);
        }


        /// <summary>
        /// Get All Settings
        /// </summary>
        /// <param></param>
        /// <returns></returns>
        /// <response code="400">Object Invalid</response>
        [HttpGet]
        public async Task<IActionResult> Get()
        {
            return Ok(await _context.Settings.Where(s => !s.IsDeleted).ToListAsync());
        }
        /// <summary>
        /// Get Setting For ID
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

            var setting = await _context.Settings
                .FirstOrDefaultAsync(s => s.Id == id && !s.IsDeleted);

            if (setting == null)
            {
                return NotFound($"Id uyğunsuzdur: {id}");
            }

            var settingGetDto = _mapper.Map<SettingGetDto>(setting);

            return Ok(settingGetDto);
        }
        /// <summary>
        /// Update Setting
        /// </summary>
        /// <param name="id"></param>
        /// <param name="settingUpdateDto"></param>
        /// <returns></returns>
        [HttpPut]
        [ProducesResponseType(204)]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        public async Task<IActionResult> Put([FromForm] SettingUpdateDto settingUpdateDto)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);

            Setting setting = await _context.Settings.FirstOrDefaultAsync(c => c.Id == settingUpdateDto.Id);
            if (setting == null) return NotFound("Id Is InCorrect");

            if (_context.Settings.Any(s => s.Key.ToLower() == settingUpdateDto.Key.ToLower()))
            {
                return Conflict($"{settingUpdateDto.Key} Already Exist");
            }

            setting.Key = settingUpdateDto.Key;
            setting.Value = settingUpdateDto.Value;
            await _context.SaveChangesAsync();

            return Ok();
        }

        /// <summary>
        /// Delete a setting by id
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        /// <response code="400">Object Invalid</response>
        /// <response code="404">Not Found</response>
        /// <response code="204">No Content</response>
        [HttpDelete("{id}")]
        [ProducesResponseType(204)]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        public async Task<IActionResult> Delete(int id)
        {
            if (id <= 0)
            {
                return BadRequest("Id boşdur.");
            }

            var setting = await _context.Settings
                .FirstOrDefaultAsync(s => s.Id == id && !s.IsDeleted);

            if (setting == null)
            {
                return NotFound($"Id uyğunsuzdur: {id}");
            }

            setting.IsDeleted = true;
            _context.Settings.Update(setting);
            await _context.SaveChangesAsync();

            return NoContent();
        }





    }
}
