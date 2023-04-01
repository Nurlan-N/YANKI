using System.ComponentModel.DataAnnotations;

namespace YankiApi.Entities
{
    public class Setting : BaseEntity
    {
        public string? Key { get; set; }
        public string? Value { get; set; }
    }
}
