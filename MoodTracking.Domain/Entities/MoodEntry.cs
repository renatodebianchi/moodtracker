using System;
using System.Collections.Generic;
using Domain.Entities;
using MoodTracking.Domain.ValueObjects;

namespace MoodTracking.Domain.Entities
{
    /// <summary>
    /// Representa um registro de humor do usuário.
    /// </summary>
    public class MoodEntry : GenericEntity
    {
        /// <summary>
        /// Nível de humor (Indisposto a Eufórico).
        /// </summary>
        public MoodLevel Nivel { get; set; }
        /// <summary>
        /// Descrição opcional do sentimento.
        /// </summary>
        public string? Descricao { get; set; }
        /// <summary>
        /// Data e hora do registro.
        /// </summary>
        public DateTime DataHora { get; set; } = DateTime.Now;
        /// <summary>
        /// Id do usuário associado.
        /// </summary>
        public Guid? UsuarioId { get; set; }
        /// <summary>
        /// Usuário associado ao registro.
        /// </summary>
        public virtual User? Usuario { get; set; }
    }

    /// <summary>
    /// Representa um usuário do sistema.
    /// </summary>
    public class User : GenericEntity
    {
        /// <summary>
        /// Nome do usuário.
        /// </summary>
        public string Nome { get; set; } = string.Empty;
        /// <summary>
        /// Email do usuário.
        /// </summary>
        public string? Email { get; set; }
        /// <summary>
        /// Senha protegida (hash) para persistência.
        /// </summary>
        public string? SenhaProtegida { get; set; }
        private SecurePassword _senha = new SecurePassword("");
        /// <summary>
        /// Define a senha do usuário e atualiza o hash.
        /// </summary>
        public void DefinirSenha(string senha)
        {
            _senha = new SecurePassword(senha);
            SenhaProtegida = _senha.Hash;
        }
        /// <summary>
        /// Retorna o objeto SecurePassword da senha do usuário.
        /// </summary>
        public SecurePassword ObterSenha()
        {
            return _senha;
        }
        /// <summary>
        /// Registros de humor associados ao usuário.
        /// </summary>
        public virtual ICollection<MoodEntry> Registros { get; set; } = new List<MoodEntry>();
    }

    /// <summary>
    /// Enum para os níveis de humor.
    /// </summary>
    public enum MoodLevel
    {
        Indisposto = 1,
        Cansado = 2,
        Normal = 3,
        Animado = 4,
        Eufórico = 5
    }
}
