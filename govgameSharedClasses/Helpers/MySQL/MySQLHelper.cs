using govgameSharedClasses.Models.MySQL;
using Microsoft.EntityFrameworkCore;

namespace govgameSharedClasses.Helpers.MySQL
{
    public partial class MySQLHelper
    {
        static string connectionString = "server=localhost;user=root;database=govgame;port=3306;password='A`T7fYQ!tP6;N[K$';";

        public class DatabaseContext : DbContext
        {
            public DbSet<Country> Countries { get; set; }
            public DbSet<Email> Emails { get; set; }
            public DbSet<InvitedMinister> InvitedMinisters { get; set; }
            public DbSet<MinistryEnum> MinistryEnums { get; set; }
            public DbSet<Notification> Notifications { get; set; }
            public DbSet<UserEmail> UserEmails { get; set; }
            public DbSet<User> Users { get; set; }

            protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
            {
                optionsBuilder.UseMySql(connectionString, ServerVersion.AutoDetect(connectionString));

                base.OnConfiguring(optionsBuilder);
            }

            protected override void OnModelCreating(ModelBuilder modelBuilder)
            {
                modelBuilder.Entity<InvitedMinister>().HasKey(invitedMinister => new { invitedMinister.Username, invitedMinister.CountryName, invitedMinister.Ministry });
                modelBuilder.Entity<UserEmail>().HasKey(userEmail => new { userEmail.EmailId, userEmail.SendingUsername, userEmail.ReceivingUsername });

                base.OnModelCreating(modelBuilder);
            }
        }
    }
}
