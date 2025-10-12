namespace EduConnect_API.Dtos
{
    public class JwtSettingsDto
    {
        public bool ValidateIssuerSigningKey { get; set; }
        /// <summary></summary>
        public string IssuerSigningKey { get; set; } = string.Empty;

        /// <summary></summary>
        public bool ValidateIssuer { get; set; } = true;

        /// <summary></summary>
        public string ValidIssuer { get; set; } = string.Empty;

        /// <summary></summary>
        public bool ValidateAudience { get; set; } = true;

        /// <summary></summary>
        public string ValidAudience { get; set; } = string.Empty;

        /// <summary></summary>
        public bool RequireExpirationTime { get; set; }

        /// <summary></summary>
        public bool FlagExpirationTimeHours { get; set; }

        /// <summary></summary>
        public int ExpirationTimeHours { get; set; }

        /// <summary></summary>
        public bool FlagExpirationTimeMinutes { get; set; }

        /// <summary></summary>
        public int ExpirationTimeMinutes { get; set; }

        /// <summary></summary>
        public bool ValidateLifetime { get; set; } = true;
    }
}
