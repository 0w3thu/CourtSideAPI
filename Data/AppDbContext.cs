using CourtSideAPI.Model;
using Microsoft.EntityFrameworkCore;

namespace CourtSideAPI.Data
{
    public class AppDbContext(DbContextOptions<AppDbContext> options) : IdentityDbContext<ApplicationUser>(options)
    {

    }
}
