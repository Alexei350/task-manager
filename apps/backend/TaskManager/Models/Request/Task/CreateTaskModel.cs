using System;
using TaskManager.Models.Enums;

namespace TaskManager.Models.Request.Task
{
    public class CreateTaskModel
    {
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
        /// Tempo gasto na tarefa
        /// </summary>
        public TimeSpan? TimeSpent { get; set; }

        /// <summary>
        /// Identifica o prazo da tarefa
        /// </summary>
        public DateTime? DueDate { get; set; }
    }
}