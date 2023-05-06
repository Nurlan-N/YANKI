using System.ComponentModel.DataAnnotations;

namespace YankiApi.Entities
{
    /// <summary>
    /// App Settings
    /// </summary>
    public class Setting : BaseEntity
    {
        /// <summary>
        /// Name
        /// </summary>
        public string? Key { get; set; }
        /// <summary>
        /// Value
        /// </summary>
        public string? Value { get; set; }
    }
}
