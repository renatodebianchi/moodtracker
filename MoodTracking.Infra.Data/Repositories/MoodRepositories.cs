using Infra.Data.Contexts;
using MoodTracking.Domain.Entities;
using Domain.Interfaces.Repositories;
using Infra.Data.Repositories.EntityFramework;
using Microsoft.EntityFrameworkCore;

namespace MoodTracking.Infra.Data.Repositories
{
    public class MoodEntryRepository : BaseRepositoryEntityFramework<MoodEntry>, IGenericRepository<MoodEntry>
    {
        public MoodEntryRepository(ApplicationDbContext context) : base(context) { }
    }

    public class UserRepository : BaseRepositoryEntityFramework<User>, IUserRepository
    {
        public UserRepository(ApplicationDbContext context) : base(context) { }

        /// <summary>
        /// Obtém um usuário pelo e-mail.
        /// </summary>
        /// <param name="email">E-mail do usuário.</param>
        /// <returns>Usuário correspondente ou null se não encontrado.</returns>
        public User? GetByEmail(string email)
        {
            return _context.Set<User>().FirstOrDefault(u => u.Email == email);
        }

        /// <summary>
        /// Obtém um usuário pelo e-mail de forma assíncrona.
        /// </summary>
        /// <param name="email">E-mail do usuário.</param>
        /// <returns>Usuário correspondente ou null se não encontrado.</returns>
        public async Task<User?> GetByEmailAsync(string email)
        {
            return await _context.Set<User>().FirstOrDefaultAsync(u => u.Email == email);
        }
    }
}
