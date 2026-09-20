using System.Security.Principal;

namespace CourtSideAPI.Model.DTO
{
    public class TokenRequestDTO
    {
        public string Token { get; set; }
        public string RefreshToken { get; set; }
    }
}
  