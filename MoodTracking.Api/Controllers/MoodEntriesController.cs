using Microsoft.AspNetCore.Mvc;
using MoodTracking.Domain.Entities;
using Domain.Interfaces.Repositories;
using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;

namespace MoodTracking.Api.Controllers
{
    /// <summary>
    /// Controller responsável pelo registro e consulta de entradas de humor dos usuários.
    /// </summary>
    [ApiController]
    [Route("api/[controller]")]
    public class MoodEntriesController : ControllerBase
    {
        private readonly IGenericRepository<MoodEntry> _moodRepository;
        private readonly IUserRepository _userRepository;

        /// <summary>
        /// Inicializa uma nova instância do <see cref="MoodEntriesController"/>.
        /// </summary>
        /// <param name="moodRepository">Repositório de entradas de humor.</param>
        /// <param name="userRepository">Repositório de usuários.</param>
        public MoodEntriesController(IGenericRepository<MoodEntry> moodRepository, IUserRepository userRepository)
        {
            _moodRepository = moodRepository;
            _userRepository = userRepository;
        }

        /// <summary>
        /// Registra uma nova entrada de humor para o usuário autenticado.
        /// </summary>
        /// <param name="entry">Objeto MoodEntry contendo os dados do humor.</param>
        /// <returns>Retorna a entrada registrada ou erro de autenticação/validação.</returns>
        [HttpPost]
        [Authorize]
        public async Task<IActionResult> RegisterMood([FromBody] MoodEntry entry)
        {
            if (entry == null) return BadRequest();
            // Obtém o usuário autenticado
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (userIdClaim == null) return Unauthorized();
            if (Guid.TryParse(userIdClaim, out var userId))
            {
                entry.UsuarioId = userId;
                entry.DataHora = DateTime.Now;
                await _moodRepository.AddAsync(entry);
                return Ok(entry);
            }
            return Unauthorized();
        }

        /// <summary>
        /// Obtém todas as entradas de humor do usuário autenticado.
        /// </summary>
        /// <returns>Lista de entradas de humor do usuário autenticado.</returns>
        [HttpGet]
        [Authorize]
        public async Task<IActionResult> GetMyMoods()
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (userIdClaim == null) return Unauthorized();
            if (Guid.TryParse(userIdClaim, out var userId))
            {
                var moods = (await _moodRepository.GetAllAsync()).Where(m => m.UsuarioId == userId).ToList();
                return Ok(moods);
            }
            return Unauthorized();
        }
    }
}
