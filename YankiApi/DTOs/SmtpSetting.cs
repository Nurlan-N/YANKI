namespace YankiApi.DTOs
{
    /// <summary>
    /// Smtp Settings
    /// </summary>
    public class SmtpSetting
    {
        /// <summary>
        /// Email Host
        /// </summary>
        public string? Host { get; set; }
        /// <summary>
        /// Smtp Port
        /// </summary>
        public int? Port { get; set; }
        /// <summary>
        /// Admin Email
        /// </summary>
        public string? Email { get; set; }
        /// <summary>
        /// Password
        /// </summary>
        public string? Password { get; set; }

    }
}
