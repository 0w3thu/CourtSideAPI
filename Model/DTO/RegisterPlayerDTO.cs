using System.ComponentModel.DataAnnotations;

namespace CourtSideAPI.Model.DTO
{
    public class RegisterPlayerDTO
    {
        
            [Required(ErrorMessage = "Fullname Required")]
            public string FullName { get; set; } = string.Empty;

            [Required]
            [EmailAddress(ErrorMessage = "Invalid email address format.")]
            public string Email { get; set; } = string.Empty;

            [Required(ErrorMessage = "Password Required")]
            public string Password { get; set; } = string.Empty;


            //Player specific Properties
           
            [Required(ErrorMessage = "Jersey Number Required")]
            public int jerseryNumber { get; set; }

            [Required(ErrorMessage = "Position Required")]
            public string Position { get; set; } = string.Empty;

            [Required(ErrorMessage = "Height Required")]
            public double height { get; set; }

            [Required(ErrorMessage = "Date Of Birth Required")]
            public DateTime DateOfBirth { get; set; }
        
    }
}
