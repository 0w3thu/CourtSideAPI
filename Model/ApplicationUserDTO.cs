using System.ComponentModel.DataAnnotations;

namespace CourtSideAPI.Model
{
    public class ApplicationUserDTO
    {
        [Required]
        public string FullName { get; set; } = string.Empty;

        [Required]
        public string Email { get; set; } = string.Empty;

        public UserRole Role { get; set; }

        public string Password { get; set; } =  string.Empty;
    }
}
