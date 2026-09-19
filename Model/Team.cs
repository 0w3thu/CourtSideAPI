namespace CourtSideAPI.Model
{
    public class Team
    {
        public int TeamId { get; set; }

        public string TeamName { get; set; }

        public string Division { get; set; }

        public string logoURL { get; set; }
        public DateTime createsAt { get; set; }
    }
}
