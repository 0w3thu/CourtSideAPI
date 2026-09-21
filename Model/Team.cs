namespace CourtSideAPI.Model
{
    public enum Division
    {
        Elite,
        Premier,
        Championship,
        National,
        Regional,
        Development
    }

    public class Team
    {
        public Guid TeamId { get; set; } = Guid.NewGuid();

        public string TeamName { get; set; } = string.Empty;

        public Division Division { get; set; }

        public string logoURL { get; set; } = string.Empty;
        public DateTime createsAt { get; set; }

        // One Team has one Coach
        public Guid CoachId { get; set; } 
        public Coach Coach { get; set; }

        public ICollection<Player> Players { get; set; } = new List<Player>();
    }
}
