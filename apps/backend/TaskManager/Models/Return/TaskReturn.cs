using System;
using TaskManager.Models.Enums;

namespace TaskManager.Models.Return
{
    public class TaskReturn
    {
        /// <summary>
        /// Id da tarefa
        /// </summary>
        public Guid Id { get; set; }

        /// <summary>
        /// Status da tarefa
        /// </summary>
        public TaskStatusEnum Status { get; set; }
        
        /// <summary>
        /// Descrição da tarefa
        /// </summary>
        public string Description { get; set; }

        /// <summary>
        /// Observações da tarefa
        /// </summary>
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