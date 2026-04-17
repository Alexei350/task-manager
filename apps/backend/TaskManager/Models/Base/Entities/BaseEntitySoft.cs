namespace TaskManager.Models.Base.Entities
{
    public class BaseEntitySoft : BaseEntity
    {
        /// <summary>
        /// Indica se o registro está deletado logicamente
        /// </summary>
        public bool Deleted { get; set; }
    }
}