using System.ComponentModel.DataAnnotations;

namespace YankiApi.Entities
{
    public class BaseEntity
    {
        public int Id { get; set; }
        public bool IsDeleted { get; set; }
        public string? CreatedBy { get; set; }
        public Nullable<DateTime> CreatedAt { get; set; }
        public string? UpdatetBy { get; set; }
        public Nullable<DateTime> UpdatetAt { get; set; }
        public string? DeletedBy { get; set; }
        public Nullable<DateTime> DeletedAt { get; set; }
    }
}
