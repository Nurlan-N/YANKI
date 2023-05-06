namespace YankiApi.DTOs.UserDTOs
{
    /// <summary>
    /// Change User Role
    /// </summary>
    public class UserRolePutDto
    {
        /// <summary>
        /// User Id
        /// </summary>
        public string? UserId { get; set; }
        /// <summary>
        /// Role Id
        /// </summary>
        public string? RoleName { get; set; }
    }
}
