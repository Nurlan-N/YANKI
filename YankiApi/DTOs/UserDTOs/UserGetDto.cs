namespace YankiApi.DTOs.UserDTOs
{
    /// <summary>
    /// User Get
    /// </summary>
    public class UserGetDto
    {
        /// <summary>
        /// User Id
        /// </summary>
        public string Id { get; set; }
        /// <summary>
        /// User Name
        /// </summary>
        public string Name { get; set; }
        /// <summary>
        /// User Email
        /// </summary>
        public string Email { get; set; }
        /// <summary>
        /// User Surname
        /// </summary>
        public string SurName { get; set; }
        /// <summary>
        /// User  Nick
        /// </summary>
        public string UserName { get; set; }
        /// <summary>
        /// User Role
        /// </summary>
        public string RoleName { get; set; }
        /// <summary>
        /// User Login
        /// </summary>
        public DateTimeOffset? LockoutEnd { get; set; }
    }
}
