using Microsoft.AspNetCore.Mvc;
using MoodTracking.Domain.Entities;
using Domain.Interfaces.Repositories;
using System;
using System.Threading.Tasks;
using System.Linq;
using MoodTracking.Api.DTOs;
using System.Collections.Generic;
using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using System.IdentityModel.Tokens.Jwt;
using Microsoft.Extensions.Configuration;
using MoodTracking.Domain.ValueObjects;
using Google.Apis.Auth;
using System.ComponentModel.DataAnnotations;

namespace MoodTracking.Api.Controllers
{
    /// <summary>
    /// Controller para operações CRUD e autenticação de usuários.
    /// </summary>
    [ApiController]
    [Route("api/[controller]")]
    public class UsersController : ControllerBase
    {
        private readonly IUserRepository _userRepository;
        private readonly IConfiguration _configuration;

        /// <summary>
        /// Construtor do UsersController.
        /// </summary>
        public UsersController(IUserRepository userRepository, IConfiguration configuration)
        {
            _userRepository = userRepository;
            _configuration = configuration;
        }

        /// <summary>
        /// Retorna todos os usuários cadastrados.
        /// </summary>
        [HttpGet]
        [Authorize]
        public async Task<ActionResult<IEnumerable<UserReadDto>>> GetAll()
        {
            var users = (await _userRepository.GetAllAsync()).ToList();
            return Ok(users.Select(u => new UserReadDto { Id = u.Id, Nome = u.Nome, Email = u.Email ?? string.Empty }));
        }

        /// <summary>
        /// Retorna um usuário pelo Id.
        /// </summary>
        [HttpGet("{id}")]
        [Authorize]
        public async Task<ActionResult<UserReadDto>> GetById(Guid id)
        {
            var user = await _userRepository.GetByIdAsync(id);
            if (user == null) return NotFound();
            return Ok(new UserReadDto { Id = user.Id, Nome = user.Nome, Email = user.Email ?? string.Empty });
        }

        /// <summary>
        /// Cria um novo usuário.
        /// </summary>
        [HttpPost]
        [AllowAnonymous]
        public async Task<IActionResult> Create([FromBody] UserCreateDto dto)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);
            var user = new User { Nome = dto.Nome, Email = dto.Email };
            user.DefinirSenha(dto.Senha);
            var id = await _userRepository.AddAsync(user);
            return CreatedAtAction(nameof(GetById), new { id }, new UserReadDto { Id = user.Id, Nome = user.Nome, Email = user.Email ?? string.Empty });
        }

        /// <summary>
        /// Realiza login e retorna o token JWT.
        /// </summary>
        [HttpPost("login")]
        [AllowAnonymous]
        public async Task<IActionResult> Login([FromBody] UserLoginDto dto)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);
            var user = await _userRepository.GetByEmailAsync(dto.Email);
            if (user == null || string.IsNullOrEmpty(user.SenhaProtegida)) return Unauthorized();
            if (!PasswordHasher.VerifyPassword(dto.Senha, user.SenhaProtegida)) return Unauthorized();
            // Gere o token JWT
            var tokenHandler = new JwtSecurityTokenHandler();
            var jwtKey = _configuration["Jwt:Key"];
            if (string.IsNullOrEmpty(jwtKey))
                throw new InvalidOperationException("JWT Key não configurada. Defina Jwt:Key no appsettings.json.");
            var key = Encoding.ASCII.GetBytes(jwtKey);
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new[]
                {
                    new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                    new Claim(ClaimTypes.Name, user.Nome),
                    new Claim(ClaimTypes.Email, user.Email ?? "")
                }),
                Expires = DateTime.UtcNow.AddHours(2),
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
            };
            var token = tokenHandler.CreateToken(tokenDescriptor);
            return Ok(new { token = tokenHandler.WriteToken(token) });
        }

        /// <summary>
        /// Realiza login/cadastro via SSO Google.
        /// </summary>
        [HttpPost("google")]
        [AllowAnonymous]
        public async Task<IActionResult> GoogleLogin([FromBody] SsoTokenDto dto)
        {
            if (string.IsNullOrEmpty(dto?.Token)) return BadRequest();
            // Validação do token Google
            var payload = await GoogleJsonWebSignature.ValidateAsync(dto.Token);
            if (payload == null || string.IsNullOrEmpty(payload.Email)) return Unauthorized();
            var user = await _userRepository.GetByEmailAsync(payload.Email);
            if (user == null)
            {
                user = new User { Nome = payload.Name ?? payload.Email, Email = payload.Email };
                await _userRepository.AddAsync(user);
            }
            // Gera JWT
            var token = GenerateJwtToken(user);
            return Ok(new { token });
        }

        /// <summary>
        /// Realiza login/cadastro via SSO Apple.
        /// </summary>
        [HttpPost("apple")]
        [AllowAnonymous]
        public async Task<IActionResult> AppleLogin([FromBody] SsoTokenDto dto)
        {
            if (string.IsNullOrEmpty(dto?.Token)) return BadRequest();
            // Validação do token Apple (exemplo simplificado)
            var handler = new JwtSecurityTokenHandler();
            var jwt = handler.ReadJwtToken(dto.Token);
            var email = jwt.Claims.FirstOrDefault(c => c.Type == "email")?.Value;
            var name = jwt.Claims.FirstOrDefault(c => c.Type == "name")?.Value;
            if (string.IsNullOrEmpty(email)) return Unauthorized();
            var user = await _userRepository.GetByEmailAsync(email);
            if (user == null)
            {
                user = new User { Nome = name ?? email, Email = email };
                await _userRepository.AddAsync(user);
            }
            var token = GenerateJwtToken(user);
            return Ok(new { token });
        }

        /// <summary>
        /// Realiza login ou registro via SSO (Google ou Apple) e retorna o token JWT.
        /// </summary>
        /// <param name="dto">Token SSO e provedor (Google/Apple).</param>
        /// <returns>JWT para autenticação.</returns>
        [HttpPost("sso")]
        [AllowAnonymous]
        public async Task<IActionResult> SsoLogin([FromBody] SsoTokenDto dto)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);
            string email = string.Empty;
            string nome = string.Empty;

            if (dto.Provider?.ToLower() == "google")
            {
                // Validação do token Google
                try
                {
                    var payload = await Google.Apis.Auth.GoogleJsonWebSignature.ValidateAsync(dto.Token);
                    email = payload.Email;
                    nome = payload.Name;
                }
                catch
                {
                    return Unauthorized("Token Google inválido.");
                }
            }
            else if (dto.Provider?.ToLower() == "apple")
            {
                // Validação simplificada do token Apple (implementar validação real em produção)
                // TODO: Implementar validação real do token Apple
                email = dto.Email; // O frontend deve enviar o email junto, pois Apple pode não fornecer sempre
                nome = dto.Name ?? "Apple User";
                if (string.IsNullOrEmpty(email))
                    return Unauthorized("Token Apple inválido ou email não fornecido.");
            }
            else
            {
                return BadRequest("Provedor SSO não suportado.");
            }

            // Busca ou cria usuário
            var user = await _userRepository.GetByEmailAsync(email);
            if (user == null)
            {
                user = new User { Nome = nome, Email = email };
                // SSO users: senha protegida pode ser null
                user.SenhaProtegida = null;
                await _userRepository.AddAsync(user);
            }

            // Gere o token JWT
            var tokenHandler = new JwtSecurityTokenHandler();
            var jwtKey = _configuration["Jwt:Key"];
            if (string.IsNullOrEmpty(jwtKey))
                throw new InvalidOperationException("JWT Key não configurada. Defina Jwt:Key no appsettings.json.");
            var key = Encoding.ASCII.GetBytes(jwtKey);
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new[]
                {
                    new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                    new Claim(ClaimTypes.Name, user.Nome),
                    new Claim(ClaimTypes.Email, user.Email ?? "")
                }),
                Expires = DateTime.UtcNow.AddHours(2),
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
            };
            var token = tokenHandler.CreateToken(tokenDescriptor);
            return Ok(new { token = tokenHandler.WriteToken(token) });
        }

        private string GenerateJwtToken(User user)
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var jwtKey = _configuration["Jwt:Key"];
            if (string.IsNullOrEmpty(jwtKey))
                throw new InvalidOperationException("JWT Key não configurada. Defina Jwt:Key no appsettings.json.");
            var key = Encoding.ASCII.GetBytes(jwtKey);
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new[]
                {
                    new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                    new Claim(ClaimTypes.Name, user.Nome),
                    new Claim(ClaimTypes.Email, user.Email ?? "")
                }),
                Expires = DateTime.UtcNow.AddHours(2),
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
            };
            var token = tokenHandler.CreateToken(tokenDescriptor);
            return tokenHandler.WriteToken(token);
        }

        /// <summary>
        /// Atualiza um usuário existente.
        /// </summary>
        [HttpPut("{id}")]
        [Authorize]
        public async Task<IActionResult> Update(Guid id, [FromBody] UserCreateDto dto)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);
            var user = await _userRepository.GetByIdAsync(id);
            if (user == null) return NotFound();
            user.Nome = dto.Nome;
            user.Email = dto.Email;
            user.DefinirSenha(dto.Senha);
            await _userRepository.UpdateAsync(user);
            return Ok(new UserReadDto { Id = user.Id, Nome = user.Nome, Email = user.Email ?? string.Empty });
        }

        /// <summary>
        /// Remove um usuário pelo Id.
        /// </summary>
        [HttpDelete("{id}")]
        [Authorize]
        public async Task<IActionResult> Delete(Guid id)
        {
            var deletedId = await _userRepository.DeleteAsync(id);
            return Ok(deletedId);
        }
    }
}
