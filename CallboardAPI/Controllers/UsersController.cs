using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using AdBoardAPI.Models;

namespace AdBoardAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UsersController : ControllerBase
    {
        private readonly AdBoardContext _context;

        public UsersController(AdBoardContext context)
        {
            _context = context;
            FillUsers(_context);
        }

        // GET: api/Users
        /// <summary>
        /// Возвращает список всех пользователей, внесенных в базу данных, если они существуют
        /// </summary>
        /// <response code="200">ОК</response>
        /// <response code="500">Ошибка сервера</response>
        [HttpGet]
        [ProducesResponseType(typeof(IEnumerable<User>), 200)]
        public async Task<ActionResult<IEnumerable<User>>> GetUsers()
        {
            return await _context.Users.ToListAsync();
        }

        // GET: api/Users/5
        /// <summary>
        /// Возвращает пользователя с указанным <c>id</c>, если он существует
        /// </summary> 
        /// <param name="id">Уникальный идентификатор пользователя</param>
        /// <response code="200">ОК</response>
        /// <response code="404">Пользователь не найден</response>
        /// <response code="500">Ошибка сервера</response>
        [HttpGet("{id}")]
        [ProducesResponseType(typeof(User), 200)]
        public async Task<ActionResult<User>> GetUser(Guid id)
        {
            var user = await _context.Users.FindAsync(id);

            if (user == null)
            {
                return NotFound();
            }

            return user;
        }

        private void FillUsers(AdBoardContext context)
        {
            var user = new User
            {
                Id = new Guid("a24b9f8c-c41f-456f-9c1f-2bb6f35252b0"), 
                Name = "Иванов И. И."
            };
            context.Users.Add(user);

            user = new User
            {
                Id = new Guid("6ed0af04-2307-4a69-93c6-079b2ef88e21"),
                Name = "Петров П.П."
            };
            context.Users.Add(user);

            user = new User
            {
                Id = new Guid("dacfb01c-2cb0-4321-bea4-42b3f238d85a"),
                Name = "Сидоров С.С."
            };
            context.Users.Add(user);
            context.SaveChanges();
        }
    }
}
