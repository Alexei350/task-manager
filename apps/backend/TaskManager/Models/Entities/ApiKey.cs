using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using TaskManager.Models.Base.Entities;

namespace TaskManager.Models.Entities
{
    public class ApiKey : BaseEntitySoft
    {
        /// <summary>
        /// Nome amigável para a chave
        /// </summary>
        [MaxLength(100)]
        public string Name { get; set; }

        /// <summary>
        /// Prefixo da chave para identificação visual (ex: tm_sk_1234...)
        /// </summary>
        [MaxLength(20)]
        public string Prefix { get; set; }

        /// <summary>
        /// Hash da chave para validação
        /// </summary>
        [MaxLength(128)]
        public string KeyHash { get; set; }

        /// <summary>
        /// Data de expiração (opcional)
        /// </summary>
        public DateTime? ExpiresAt { get; set; }

        /// <summary>
        /// Data de exclusão
        /// </summary>
        public DateTime? DeletedAt { get; set; }

        /// <summary>
        /// Última vez que a chave foi usada
        /// </summary>
        public DateTime? LastUsedAt { get; set; }

        /// <summary>
        /// ID do usuário dono da chave
        /// </summary>
        public Guid UserId { get; set; }

        /// <summary>
        /// Usuário dono da chave
        /// </summary>
        [ForeignKey("UserId")]
        public virtual User User { get; set; }
    }
}
