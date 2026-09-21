using System.ComponentModel.DataAnnotations;

namespace CourtSideAPI.Model.DTO
{
    public class AddSessionDTO
    {
        [Required(ErrorMessage = "Title Required")]
        public string Title { get; set; } = string.Empty;

        [Required(ErrorMessage = "Scheduled Date Required")]
        public DateTime ScheduledDate { get; set; }

        [Required(ErrorMessage = "Location Required")]
        public string location { get; set; }

        [Required(ErrorMessage = "Minutes Required")]
        public int Minutes { get; set; }

        [Required(ErrorMessage = "Status Required")]
        public Status Status { get; set; }

        public string? recurrenceRule { get; set; }
        public DateTime recurrenceEndDate { get; set; }

        public string? Note { get; set; }

        [Required(ErrorMessage = "TeamId Required")]
        public Guid TeamId { get; set; }
    }
}
