using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using AdBoardAPI.Models;

// ReSharper disable All

namespace AdBoardAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AdsController : ControllerBase
    {
        private readonly AdBoardContext _context;

        public AdsController(AdBoardContext context)
        {
            _context = context;
        }

        // GET: api/Ads
        /// <summary>
        /// Возвращает список всех объявлений, опубликованных в базе данных, если они существуют
        /// </summary>
        /// <response code="200">ОК</response>
        /// <response code="204">Список объявлений пуст</response>
        /// <response code="500">Ошибка сервера</response>
        [HttpGet(Name = "GetAds")]
        [ProducesResponseType(typeof(IEnumerable<Ad>), 200)]
        public async Task<ActionResult<IEnumerable<Ad>>> GetAds()
        {
            if (_context.Ads.Count() == 0) return NoContent();
            return await _context.Ads.ToListAsync();
        }

        // GET: api/Ads/byUser/5
        /// <summary>
        /// Возвращает список всех объявлений, опубликованных пользователем с указанным <c>userId</c>, если они существуют
        /// </summary>
        /// <param name="userId">Уникальный идентификатор пользователя.</param>
        /// <response code="200">ОК</response>
        /// <response code="404">Объявление не найдено</response>
        /// <response code="500">Ошибка сервера</response>
        [HttpGet("byUser/{userId}")]
        [ProducesResponseType(typeof(IEnumerable<Ad>), 200)]
        public async Task<ActionResult<IEnumerable<Ad>>> GetAdsByUser(Guid userId)
        {
            var result = await _context.Ads.Where(x => x.User == userId).ToListAsync();
            
            if (result == null) return NotFound();
            return result;
        }

        //GET: api/Ads/byId/5
        /// <summary>
        /// Возвращает объявление с указанным <c>id</c>, если оно существует
        /// </summary>
        /// <param name="id">Уникальный идентификатор объявления</param>
        /// <response code="200">ОК</response>
        /// <response code="404">Объявление не найдено</response>
        /// <response code="500">Ошибка сервера</response>
        [HttpGet("byId/{id}")]
        public async Task<ActionResult<Ad>> GetAd(Guid id)
        {
            var ad = await _context.Ads.FindAsync(id);

            if (ad == null)
            {
                return NotFound();
            }

            return ad;
        }

        // PUT: api/Ads/5
        /// <summary>
        /// Изменяет объявление с указанным <c>id</c>, если оно существует, в соответствии с содержимым <c>adDTO</c>
        /// </summary>
        /// <param name="id">Уникальный идентификатор объявления</param>
        /// <param name="adDTO">Измененное объявление</param>
        /// <response code="200">Объявление изменено</response>
        /// <response code="404">Исходное объявление не найдено</response>
        /// <response code="500">Ошибка сервера</response>
        [HttpPut("{id}")]
        public async Task<IActionResult> PutAd(Guid id, Ad adDTO)
        {
            var ad = await _context.Ads.FindAsync(id);
            if (ad == null)
            {
                return NotFound();
            }

            ad.Content = adDTO.Content;
            ad.ImageFullsize = adDTO.ImageFullsize;
            ad.ImagePreview = adDTO.ImagePreview;
            ad.User = adDTO.User;

            _context.Entry(ad).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!AdExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return Ok();
        }

        // POST: api/Ads
        /// <summary>
        /// Добавляет в базу данных новое объявление
        /// </summary>
        /// <param name="adDTO">Данные нового объявления</param>
        /// <response code="201">Объявление добавлено</response>
        /// <response code="404">Пользователь, указанный в качестве автора, не найден</response>
        /// <response code="500">Ошибка сервера</response>
        [HttpPost]
        [ProducesResponseType(typeof(Ad), 201)]
        public async Task<ActionResult<Ad>> PostAd(AdDTO adDTO)
        {
            if (!UserExists(adDTO.User)) return NotFound();

            var maxNumber = 0;
            if (_context.Ads.Count() > 0)
            {
                maxNumber = _context.Ads.Max(x => x.Number);
            }

            var ad = new Ad
            {
                Id = new Guid(),
                Created = DateTime.Now,
                Number = maxNumber + 1,
                Rating = new Random().Next(-5, 10),
                ImagePreview = MakePreview(adDTO.ImageFullsize),
                User = adDTO.User,
                Content = adDTO.Content,
                ImageFullsize = adDTO.ImageFullsize
            };

            _context.Ads.Add(ad);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetAd), new {id = ad.Id}, ad);
        }

        // DELETE: api/Ads/5
        /// <summary>
        /// Удаляет из базы объявление с указанным <c>id</c>
        /// </summary>
        /// <param name="id">Уникальный идентификатор объявления</param>
        /// <response code="200">Объявление удалено</response>
        /// <response code="404">Объявление не найдено</response>
        /// <response code="500">Ошибка сервера</response>
        [HttpDelete("{id}")]
        public async Task<ActionResult<Ad>> DeleteAd(Guid id)
        {
            var ad = await _context.Ads.FindAsync(id);
            if (ad == null)
            {
                return NotFound();
            }

            _context.Ads.Remove(ad);
            await _context.SaveChangesAsync();

            return Ok();
        }

        private bool AdExists(Guid id)
        {
            return _context.Ads.Any(e => e.Id == id);
        }
        
        private bool UserExists(Guid id)
        {
            return _context.Users.Any(e => e.Id == id);
        }

        private string MakePreview(string ImageFullsize)
        {
            return "image preview";
        }
    }
}
