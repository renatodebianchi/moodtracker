using System.ComponentModel.DataAnnotations;

namespace MoodTracking.Api.DTOs
{
    /// <summary>
    /// DTO para requisição de registro de usuário.
    /// </summary>
    public class UserCreateDto
    {
        /// <summary>
        /// Nome do usuário.
        /// </summary>
        [Required]
        [StringLength(100, MinimumLength = 2)]
        public string Nome { get; set; } = string.Empty;

        /// <summary>
        /// E-mail do usuário.
        /// </summary>
        [Required]
        [EmailAddress]
        public string Email { get; set; } = string.Empty;

        /// <summary>
        /// Senha do usuário.
        /// </summary>
        [Required]
        [StringLength(100, MinimumLength = 6)]
        public string Senha { get; set; } = string.Empty;
    }

    /// <summary>
    /// DTO para requisição de login de usuário.
    /// </summary>
    public class UserLoginDto
    {
        /// <summary>
        /// E-mail do usuário.
        /// </summary>
        [Required]
        [EmailAddress]
        public string Email { get; set; } = string.Empty;

        /// <summary>
        /// Senha do usuário.
        /// </summary>
        [Required]
        [StringLength(100, MinimumLength = 6)]
        public string Senha { get; set; } = string.Empty;
    }

    /// <summary>
    /// DTO para resposta de autenticação de usuário.
    /// </summary>
    public class UserReadDto
    {
        /// <summary>
        /// Identificador do usuário.
        /// </summary>
        public Guid Id { get; set; }

        /// <summary>
        /// Nome do usuário.
        /// </summary>
        public string Nome { get; set; } = string.Empty;

        /// <summary>
        /// E-mail do usuário.
        /// </summary>
        public string Email { get; set; } = string.Empty;
    }

    /// <summary>
    /// DTO para login/registro via SSO (Google/Apple).
    /// </summary>
    public class SsoTokenDto
    {
        /// <summary>
        /// Token de autenticação SSO (Google ou Apple).
        /// </summary>
        public string Token { get; set; } = null!;
        /// <summary>
        /// Provedor SSO ("google" ou "apple").
        /// </summary>
        public string? Provider { get; set; }
        /// <summary>
        /// Email do usuário (usado para Apple SSO).
        /// </summary>
        public string? Email { get; set; }
        /// <summary>
        /// Nome do usuário (opcional, usado para Apple SSO).
        /// </summary>
        public string? Name { get; set; }
    }
}
