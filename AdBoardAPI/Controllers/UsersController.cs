using AdBoardAPI.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

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
        }

        // GET: api/Users
        /// <summary>
        /// Возвращает список всех пользователей, внесенных в базу данных, если они существуют
        /// </summary>
        /// <response code="200">ОК</response>
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
    }
}
