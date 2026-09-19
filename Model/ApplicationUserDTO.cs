using System.ComponentModel.DataAnnotations;

namespace CourtSideAPI.Model
{
    public class ApplicationUserDTO
    {
        [Required (ErrorMessage = "Fullname Required")]
        public string FullName { get; set; } = string.Empty;

        [Required]
        [EmailAddress(ErrorMessage = "Invalid email address format.")]
        public string Email { get; set; } = string.Empty;

        [Required(ErrorMessage = "Password Required")]
        public string Password { get; set; } =  string.Empty;
    }
}
