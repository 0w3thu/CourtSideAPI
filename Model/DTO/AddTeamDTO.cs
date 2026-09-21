using System.ComponentModel.DataAnnotations;

namespace CourtSideAPI.Model.DTO
{
    public class AddTeamDTO
    {
        [Required(ErrorMessage = "Team Name Required")]
        public string TeamName { get; set; } = "";

        [Required(ErrorMessage = "Division Required")]
        public Division Division { get; set; }

        public string? LogoUrl { get; set; } = "No logo";
        

    }
}
