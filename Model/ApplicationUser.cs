using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore.Metadata;
using System.ComponentModel.DataAnnotations;
using System.Runtime.Intrinsics.X86;

namespace CourtSideAPI.Model
{

    public enum UserRole{ Coach,Admin,Player,Guest}
    public enum SyncStatus { Pending, Synced, Confict}

    public class ApplicationUser: IdentityUser<Guid>
    {

        [Required]
        public string FullName { get; set; } = "";

       
        public UserRole Role { get; set; }

        public string ssoProvide {  get; set; }

        public Boolean biometricEnabled { get; set; } = false;

        public string languagePref { get; set; } = "";

        public string  fcmToken { get; set; }

        public bool notifySessionReminders { get; set; } = true;

        public bool notifyBadgeAwards { get; set; } = true;

    }

    public class Coach : ApplicationUser
    {
        public int TeamId {  get; set; }
        public Team Team { get; set; }

    }

    public class Player : ApplicationUser
    {
        public int teamId { get; set; }
        public Team Team { get; set; }

        
        public int jerseryNumber { get; set; }

        public string Position { get; set; }

        public double height { get; set; }

        public DateTime DateOfBirth { get; set; }

        public bool isActive { get; set; }

        public  SyncStatus SyncStatus { get; set; }

    }

    public class Guest : ApplicationUser
    {

    }
}
