using System.ComponentModel.DataAnnotations.Schema;

namespace CourtSideAPI.Model
{
    public class RefreshToken
    {
        public int Id { get; set; }

        public Guid UserId { get; set; }

        public string Token { get; set; }

        public string JwtId { get; set; }

        public bool isRevoked { get; set; }

        public DateTime DateAdded {  get; set; }

        public DateTime DateExpired { get; set; }

        [ForeignKey(nameof(UserId))]
        public ApplicationUser User { get; set; }
    }

}
