using MoodTracking.Domain.Entities;

namespace Domain.Interfaces.Repositories
{
    /// <summary>
    /// Interface para repositório de usuários.
    /// </summary>
    public interface IUserRepository : IGenericRepository<User>
    {
        /// <summary>
        /// Obtém um usuário pelo e-mail.
        /// </summary>
        /// <param name="email">E-mail do usuário.</param>
        /// <returns>Usuário correspondente ou null se não encontrado.</returns>
        Task<User?> GetByEmailAsync(string email);
    }
}
