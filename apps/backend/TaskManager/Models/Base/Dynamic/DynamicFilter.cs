using System.Collections.Generic;

namespace TaskManager.Models.Base.Dynamic
{
    public class DynamicFilter
    {
        /// <summary>
        /// Conector (OR, AND)
        /// </summary>
        public string Connector { get; set; }

        /// <summary>
        /// Itens do filtro
        /// </summary>
        public IList<DynamicFilterItem> Values { get; set; }
    }

    public class DynamicFilterItem
    {
        /// <summary>
        /// Operação (Contains, Equals, GreaterThanOrEquals, LessThanOrEquals)
        /// </summary>
        public string Operation { get; set; }

        /// <summary>
        /// Nome da propriedade a ser comparada
        /// </summary>
        public string PropertyName { get; set; }

        /// <summary>
        /// Valor a ser comparado com a propriedade
        /// </summary>
        public string Value { get; set; }
    }
}