using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;

namespace CourtSideAPI.Model
{

    public enum UserRole{ Coach,Admin,Player,Guest}
    public enum SyncStatus { Pending, Synced, Conflict}

    public class ApplicationUser: IdentityUser<Guid>
    {

        [Required]
        public string FullName { get; set; } = "";

        //public UserRole Role { get; set; }

        public string SsoProvide { get; set; } = "";

        public Boolean BiometricEnabled { get; set; } = false;

        public string LanguagePref { get; set; } = "";

        public string FcmToken { get; set; } = "";

        public bool NotifySessionReminders { get; set; } = true;

        public bool NotifyBadgeAwards { get; set; } = true;

      
    }

    public class Coach : ApplicationUser
    {
        public int TeamId { get; set; }
        public Team Team { get; set; }

    }

    public class Player : ApplicationUser
    {
        public int TeamId { get; set; }
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
