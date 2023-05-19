namespace YankiApi.Entities
{
    /// <summary>
    /// Email Confirm
    /// </summary>
    public class VerificationCodes
    {
        /// <summary>
        /// Code Id
        /// </summary>
        public int Id { get; set; }
        /// <summary>
        /// User Email
        /// </summary>
        public string? Email { get; set; }
        /// <summary>
        /// Code
        /// </summary>
        public string? Code { get; set; }

    }
}
