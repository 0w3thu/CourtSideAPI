namespace CourtSideAPI.Model.DTO
{
    public class UpdateTeamDTO
    {
        public string? TeamName { get; set; } = string.Empty;

        public Division? Division { get; set; }

        public string? logoURL { get; set; } = string.Empty;
    }
}
