using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using TaskManager.Models.Base.Entities;
using TaskManager.Models.Enums;

namespace TaskManager.Models.Entities
{
    public class Task : BaseEntity
    {
        /// <summary>
        /// Id do usuário ao qual a tarefa pertence
        /// </summary>
        public Guid UserId { get; set; }
        
        [ForeignKey("UserId")]
        public User User { get; set; }

        /// <summary>
        /// Status da tarefa
        /// </summary>
        public TaskStatusEnum Status { get; set; }
        
        /// <summary>
        /// Descrição da tarefa
        /// </summary>
        [MaxLength(255)]
        public string Description { get; set; }

        /// <summary>
        /// Observações da tarefa
        /// </summary>
        [MaxLength(255)]
        public string Observation { get; set; }

        /// <summary>
        /// Identifica a data de criação da tarefa
        /// </summary>
        public DateTime CreationDate { get; set; }

        /// <summary>
        /// Tempo gasto na tarefa
        /// </summary>
        public TimeSpan? TimeSpent { get; set; }

        /// <summary>
        /// Identifica o prazo da tarefa
        /// </summary>
        public DateTime? DueDate { get; set; }

        /// <summary>
        /// Identifica a data de finalização da tarefa
        /// </summary>
        public DateTime? CompletedDate { get; set; }
    }
}