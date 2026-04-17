using System.Collections.Generic;

namespace TaskManager.Models.Base.Dynamic
{
    public class DynamicOrderBy
    {
        public IList<DynamicOrderByItem> Fields { get; set; }
    }

    public class DynamicOrderByItem
    {
        public string PropertyName { get; set; }
        public string Direction { get; set; }
    }
}