using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using AdBoardAPI.Models;
using Microsoft.Extensions.Configuration;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Hosting;
using System.Collections.Generic;
using AdBoardAPI.ImageFileMgr;
using System.Net;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Logging;

namespace AdBoardAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AdsController : ControllerBase
    {
        private readonly AdBoardContext _context;
        private IConfiguration _configuration;
        private IWebHostEnvironment _env;
        private ILogger<AdsController> _logger;

        public AdsController(AdBoardContext context, IConfiguration configuration, IWebHostEnvironment env, ILogger<AdsController> logger)
        {
            _context = context;
            _configuration = configuration;
            _env = env;
            _logger = logger;
        }

        // GET: api/Ads/5
        /// <summary>
        /// Возвращает объявление с указанным <c>id</c>, если оно существует
        /// </summary> 
        /// <param name="id">Уникальный идентификатор объявления</param>
        /// <response code="200">ОК</response>
        /// <response code="404">Объявление не найдено</response>
        /// <response code="500">Ошибка сервера</response>
        [HttpGet("{id}")]
        [ProducesResponseType(typeof(User), 200)]
        public async Task<ActionResult<Ad>> GetAd(Guid id)
        {
            Ad ad;
            try
            {                
                ad = await _context.Ads.FindAsync(id);
            }
            catch (SqlException ex)
            {
                _logger.LogError($"Ошибка доступа к базе данных.\n{ex.Message}");
                return Problem("База данных недоступна", null, StatusCodes.Status500InternalServerError, "Ошибка доступа к базе данных");
            }
            catch(Exception ex)
            {
                _logger.LogError($"Ошибка при исполнении метода.\n{ex.Message}");
                return Problem("Ошибка сервера", null, StatusCodes.Status500InternalServerError, "Ошибка сервера");
            }

            if (ad == null)
            {
                return NotFound();
            }

            return ad;
        }

        // GET: api/Ads
        /// <summary>
        /// Возвращает список всех объявлений, внесенных в базу данных, если они существуют
        /// </summary> 
        /// <response code="200">ОК</response>
        /// <response code="500">Ошибка сервера</response>     
        [HttpGet]
        [ProducesResponseType(typeof(IEnumerable<Ad>), 200)]
        public async Task<ActionResult<IEnumerable<Ad>>> GetAds()
        {
            try
            {
                return await _context.Ads.ToListAsync();
            }
            catch (SqlException ex)
            {
                _logger.LogError($"Ошибка доступа к базе данных.\n{ex.Message}");
                return Problem("База данных недоступна", null, StatusCodes.Status500InternalServerError, "Ошибка доступа к базе данных");
            }
            catch (Exception ex)
            {
                _logger.LogError($"Ошибка при исполнении метода.\n{ex.Message}");
                return Problem("Ошибка сервера", null, StatusCodes.Status500InternalServerError, "Ошибка сервера");
            }
        }

        // GET: api/Ads/index
        /// <summary>
        /// Возвращает страницу с списком объявлений в соответствии с параметрами фильтрации, сортировки и пагинации
        /// </summary>
        /// <param name="user">Значение отбора по GUID пользователя. Отбор по строгому соответствию</param>
        /// <param name="subject">Значение отбора по содержимому заголовка объявления. Отбор по частичному совпадению</param>
        /// <param name="content">Значение отбора по содержимому объявления. Отбор по частичному совпадению</param>
        /// <param name="rating">Значение отбора по рейтингу объявления</param>
        /// <param name="created">Значение отбора по дате публикации. Отбор в пределах указанной даты (время от 00:00:00 до 23:59:59)</param>
        /// <param name="search">Значение для поиска в списке объявлений. Поиск осуществляется по полям number, subject, content</param>
        /// <param name="sortBy">Параметр сортировки списка. Для сортировки по возрастанию указывается только 
        /// имя поля (напр. user), для сортировки по убыванию - имя поля со знаком "-" (напр., user-)</param>
        /// <param name="page">Номер страницы списка. По умолчанию - 1</param>
        /// <param name="onPage">Количество строк на странице. По умолчанию - 10.</param>
        /// <response code="200">ОК</response>
        /// <response code="204">Список объявлений пуст</response>
        /// <response code="500">Ошибка сервера</response>
        [HttpGet("index")]
        [ProducesResponseType(typeof(PageView), 200)]
        public async Task<ActionResult<PageView>> GetAds(string user, string subject, string content, int? rating, DateTime? created, string search, string sortBy, int? page, int? onPage)
        {
            try
            {
                if (_context.Ads.Count() == 0) return NoContent();
                var paginator = new Paginator(_context, user, subject, content, rating, created, search, sortBy, page, onPage);
                var ads = await paginator.MakePageAsync();
                return ads;
            }
            catch (SqlException ex)
            {
                _logger.LogError($"Ошибка доступа к базе данных.\n{ex.Message}");
                return Problem("База данных недоступна", null, StatusCodes.Status500InternalServerError, "Ошибка доступа к базе данных");
            }
            catch (Exception ex)
            {
                _logger.LogError($"Ошибка при исполнении метода.\n{ex.Message}");
                return Problem("Ошибка сервера", null, StatusCodes.Status500InternalServerError, "Ошибка сервера");
            }
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
        public async Task<IActionResult> PutAd(Guid id, AdDTO adDTO)
        {
            Ad ad;

            try
            {
                ad = await _context.Ads.FindAsync(id);
            }
            catch (SqlException ex)
            {
                _logger.LogError($"Ошибка доступа к базе данных.\n{ex.Message}");
                return Problem("База данных недоступна", null, StatusCodes.Status500InternalServerError, "Ошибка доступа к базе данных");
            }
            catch (Exception ex)
            {
                _logger.LogError($"Ошибка при исполнении метода.\n{ex.Message}");
                return Problem("Ошибка сервера", null, StatusCodes.Status500InternalServerError, "Ошибка сервера");
            }

            if (ad == null)
            {
                return NotFound();
            }

            ad.User = adDTO.User;
            ad.Subject = adDTO.Subject;
            ad.Content = adDTO.Content;

            IImageFileManager imageManager = new ImageFileManager(_configuration);
            var imageURL = imageManager.GenerateURL(ad.Id.ToString(), adDTO.Image.FileName);
            try
            {
                await imageManager.UploadImageAsync(adDTO.Image, imageURL);
            }
            catch(Exception)
            {
                return Problem("Возникла ошибка при загрузке изображения", null, StatusCodes.Status500InternalServerError, "Ошибка загрузки изображения");
            }

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
            catch (SqlException ex)
            {
                _logger.LogError($"Ошибка доступа к базе данных.\n{ex.Message}");
                return Problem("База данных недоступна", null, StatusCodes.Status500InternalServerError, "Ошибка доступа к базе данных");
            }
            catch (Exception ex)
            {
                _logger.LogError($"Ошибка при исполнении метода.\n{ex.Message}");
                return Problem("Ошибка сервера", null, StatusCodes.Status500InternalServerError, "Ошибка сервера");
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
        public async Task<ActionResult<Ad>> PostAd([FromForm] AdDTO adDTO)
        {
            Ad ad;
            try
            {
                if (!UserExists(adDTO.User)) return NotFound();

                if (!int.TryParse(_configuration.GetSection("AppConfiguration")["maxAdsByUser"], out var maxAds))
                {
                    return Problem("Неверно указан параметр maxAds в настройках приложения", null, StatusCodes.Status500InternalServerError, "Ошибка сервера");
                }

                if (await _context.Users.Where(user => user.Id == adDTO.User).CountAsync() >= maxAds)
                {
                    return BadRequest();
                }

                var maxNumber = 0;
                if (await _context.Ads.CountAsync() > 0)
                {
                    maxNumber = _context.Ads.Max(x => x.Number);
                }

                var adId = Guid.NewGuid();
                var imageUrl = "";

                if (adDTO.Image != null)
                {
                    IImageFileManager imageManager = new ImageFileManager(_configuration);
                    imageUrl = imageManager.GenerateURL(adId.ToString(), adDTO.Image.FileName);
                    try
                    {
                        await imageManager.UploadImageAsync(adDTO.Image, imageUrl);
                    }
                    catch (Exception)
                    {
                        return Problem("Возникла ошибка при загрузке изображения", null, StatusCodes.Status500InternalServerError, "Ошибка загрузки изображения");
                    }
                }

                ad = new Ad
                {
                    Id = adId,
                    Created = DateTime.Now,
                    Number = maxNumber + 1,
                    Rating = new Random().Next(-5, 10),
                    User = adDTO.User,
                    Subject = adDTO.Subject,
                    Content = adDTO.Content is null ? adDTO.Subject : adDTO.Content,
                    ImageURL = imageUrl
                };

                _context.Ads.Add(ad);
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateException)
            {
                return Problem("Проблема при добавлении записи в базу данных", null, StatusCodes.Status500InternalServerError, "Ошибка сервера", null);
            }
            catch (SqlException ex)
            {
                _logger.LogError($"Ошибка доступа к базе данных.\n{ex.Message}");
                return Problem("База данных недоступна", null, StatusCodes.Status500InternalServerError, "Ошибка доступа к базе данных");
            }
            catch (Exception ex)
            {
                _logger.LogError($"Ошибка при исполнении метода.\n{ex.Message}");
                return Problem("Ошибка сервера", null, StatusCodes.Status500InternalServerError, "Ошибка сервера");
            }

            return CreatedAtAction(nameof(GetAd), new { id = ad.Id }, ad);
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
            try
            {
                var ad = await _context.Ads.FindAsync(id);
                if (ad == null)
                {
                    return NotFound();
                }

                _context.Ads.Remove(ad);
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateException)
            {
                return Problem("Ошибка удаления записи из базы данных", null, StatusCodes.Status500InternalServerError, "Ошибка сервера", null);
            }
            catch (SqlException ex)
            {
                _logger.LogError($"Ошибка доступа к базе данных.\n{ex.Message}");
                return Problem("База данных недоступна", null, StatusCodes.Status500InternalServerError, "Ошибка доступа к базе данных");
            }
            catch (Exception ex)
            {
                _logger.LogError($"Ошибка при исполнении метода.\n{ex.Message}");
                return Problem("Ошибка сервера", null, StatusCodes.Status500InternalServerError, "Ошибка сервера");
            }

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
    }
}
