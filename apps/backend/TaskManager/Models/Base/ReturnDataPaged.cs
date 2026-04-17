using System.Collections.Generic;

namespace TaskManager.Models.Base
{
    public class ReturnDataPaged<T> : ReturnData<T>
    {
        /// <summary>
        /// Total de registros
        /// </summary>
        public int TotalRecords { get; set; }

        /// <summary>
        /// Total de páginas
        /// </summary>
        public int TotalPages { get; set; }

        /// <summary>
        /// Lista de registros
        /// </summary>
        public new IList<T> Data { get; set; }
    }
}