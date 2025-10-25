namespace EduConnect_API.Utilities
{
    public class CorreoConfigUtility
    {
        public string Host { get; set; } = string.Empty;
        public int Port { get; set; }
        public string Email { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
        public bool EnableSSL { get; set; }
        public string DisplayName { get; set; } = string.Empty;
    }
}
