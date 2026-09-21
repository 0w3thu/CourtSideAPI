namespace CourtSideAPI.Model.DTO
{
    public class AuthResultDTO
    {
        public string Token { get; set; }
        public string RefreshToken { get; set; }
        public DateTime ExpireAt { get; set; }

        public AuthUserDTO User { get; set; }
    }

    public class AuthUserDTO
    {
        public string UserId { get; set; } = string.Empty;

        public string FullName { get; set; } = string.Empty;

        public string Role { get; set; } = string.Empty;

        public string LanguagePref { get; set; } = string.Empty;
    }
}
