using System;

namespace TaskManager.Models.Return
{
    public class UserReturn
    {
        /// <summary>
        /// Id do registro
        /// </summary>
        public Guid Id { get; set; }
        
        /// <summary>
        /// Email
        /// </summary>
        public string Email { get; set; } 

        /// <summary>
        /// Nome
        /// </summary>
        public string Name { get; set; } 
    }
}