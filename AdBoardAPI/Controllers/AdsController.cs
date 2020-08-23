using AdBoardAPI.CustomCache.CustomCacheController;
using AdBoardAPI.ImageFileMgr;
using AdBoardAPI.Models.AdModel;
using AdBoardAPI.Options;
using AdBoardAPI.Pagination;
using AdBoardAPI.Pagination.Filter;
using AdBoardAPI.Pagination.Searcher;
using AdBoardAPI.Pagination.Sorter;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AdBoardAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]    
    public class AdsController : ControllerBase
    {
        private readonly AdBoardContext _context;
        private readonly AppConfiguration _options;
        private ICustomImageCacheController _cacheController;

        public AdsController(AdBoardContext context, AppConfiguration options, ICustomImageCacheController cacheController)
        {
            _context = context;
            _options = options;
            _cacheController = cacheController;
        }

        // GET: api/Ads/5
        /// <summary>
        /// Возвращает объявление с указанным <c>id</c>, если оно существует
        /// </summary> 
        /// <param name="id">Уникальный идентификатор объявления</param>
        /// <response code="200">ОК</response>
        /// <response code="404">Объявление не найдено</response>
        [HttpGet("{id}")]
        [ProducesResponseType(typeof(Ad), 200)]
        public async Task<ActionResult<Ad>> GetAd(Guid id)
        {
            var ad = await _context.Ads.FindAsync(id);

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
        [HttpGet]
        [ProducesResponseType(typeof(IEnumerable<Ad>), 200)]
        public async Task<ActionResult<IEnumerable<Ad>>> GetAds()
        {
            return await _context.Ads.ToListAsync();
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
        [HttpGet("index")]
        [ProducesResponseType(typeof(PageView<Ad>), 200)]
        public async Task<ActionResult<PageView<Ad>>> GetAds(string user, string subject, string content, int? rating,
            DateTime? created, string search, string sortBy, int? page, int? onPage)
        {
            if (!_context.Ads.Any()) return NoContent();

            var listMaker = new ListMaker<Ad>(_context.Ads);
            var list = listMaker.MakeList();

            if (!string.IsNullOrEmpty(search))
            {
                var adsSearcher = new AdsSearcher(list, search);
                list = adsSearcher.Search();
            }

            var adsFilter = new AdsFilter(list, user, subject, content, rating, created);
            list = adsFilter.Filter();

            if (!string.IsNullOrEmpty(sortBy))
            {
                var adsSorter = new AdsSorter(list, sortBy);
                list = adsSorter.Sort();
            }

            var paginator = new Paginator<Ad>(list, page, onPage);
            var ads = await paginator.MakePageViewAsync();

            return ads;
        }


        // PUT: api/Ads/5
        /// <summary>
        /// Изменяет объявление с указанным <c>id</c>, если оно существует, в соответствии с содержимым <c>adDTO</c>
        /// </summary>
        /// <param name="id">Уникальный идентификатор объявления</param>
        /// <param name="adDTO">Измененное объявление</param>
        /// <response code="200">Объявление изменено</response>
        /// <response code="404">Исходное объявление не найдено</response>
        [HttpPut("{id}")]
        public async Task<IActionResult> PutAd(Guid id, [FromForm]AdDTO adDTO)
        {
            var ad = await _context.Ads.FindAsync(id);
            if (ad == null)
            {
                return NotFound();
            }
            
            if (adDTO.User != Guid.Empty)
            {
                if (!UserExists(adDTO.User)) return NotFound();
                ad.User = adDTO.User;
            }

            if (!string.IsNullOrEmpty(adDTO.Subject)) ad.Subject = adDTO.Subject;
            if (!string.IsNullOrEmpty(adDTO.Content)) ad.Content = adDTO.Content;

            if (adDTO.Image != null)
            {
                IImageFileManager imageManager = new ImageFileManager(_options, _cacheController);
                var imageUrl = imageManager.GenerateURL(ad.Id.ToString(), adDTO.Image.FileName);
                try
                {
                    await imageManager.UploadImageAsync(adDTO.Image, imageUrl);
                }
                catch (Exception)
                {
                    return Problem("Возникла ошибка при загрузке изображения", null,
                        StatusCodes.Status500InternalServerError, "Ошибка загрузки изображения");
                }
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

            return Ok();
        }

        // POST: api/Ads
        /// <summary>
        /// Добавляет в базу данных новое объявление
        /// </summary>
        /// <param name="adDTO">Данные нового объявления</param>
        /// <response code="201">Объявление добавлено</response>
        /// <response code="404">Пользователь, указанный в качестве автора, не найден</response>
        [HttpPost]
        [ProducesResponseType(typeof(Ad), 201)]
        public async Task<ActionResult<Ad>> PostAd([FromForm] AdDTO adDTO)
        {
            if (!UserExists(adDTO.User)) return NotFound();

            var maxAds = _options.PublishOptions.MaxAdsByUser;
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
                IImageFileManager imageManager = new ImageFileManager(_options, _cacheController);
                imageUrl = imageManager.GenerateURL(adId.ToString(), adDTO.Image.FileName);
                try
                {
                    await imageManager.UploadImageAsync(adDTO.Image, imageUrl);
                }
                catch (Exception)
                {
                    return Problem("Возникла ошибка при загрузке изображения", null,
                        StatusCodes.Status500InternalServerError, "Ошибка загрузки изображения");
                }
            }

            var ad = new Ad
            {
                Id = adId,
                Created = DateTime.Now,
                Number = maxNumber + 1,
                Rating = new Random().Next(-5, 10),
                User = adDTO.User,
                Subject = adDTO.Subject,
                Content = adDTO.Content ?? adDTO.Subject,
                ImageURL = imageUrl
            };

            await _context.Ads.AddAsync(ad);
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
    }
}
