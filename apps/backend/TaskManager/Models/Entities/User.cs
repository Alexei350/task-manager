using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using TaskManager.Models.Base.Entities;
using TaskManager.Models.Enums;

namespace TaskManager.Models.Entities
{
    public class User : BaseEntitySoft
    {
        /// <summary>
        /// Hash da senha
        /// </summary>
        [MaxLength(64)]
        public string Password { get; set; }

        /// <summary>
        /// Salt da senha
        /// </summary>
        [MaxLength(32)]
        public string Salt { get; set; }

        /// <summary>
        /// Permissão do usuário
        /// </summary>
        public UserRoleEnum Role { get; set; }

        /// <summary>
        /// Email
        /// </summary>
        [MaxLength(64)]
        public string Email { get; set; }

        /// <summary>
        /// Nome
        /// </summary>
        [MaxLength(64)]
        public string Name { get; set; }

        /// <summary>
        /// Último Refresh Token gerado
        /// </summary>
        [MaxLength(64)]
        public string RefreshToken { get; set; }

        /// <summary>
        /// Última geração de Token
        /// </summary>
        public DateTime? LastAccess { get; set; }

        /// <summary>
        /// Linguagem utilizada
        /// </summary>
        [MaxLength(5)]
        public string Culture { get; set; }
        
        /// <summary>
        /// Id (sub) do usuário no Google
        /// </summary>
        [MaxLength(64)]
        public string GoogleUserId { get; set; }
        
        public ICollection<Task> Tasks { get; set; }
    }
}