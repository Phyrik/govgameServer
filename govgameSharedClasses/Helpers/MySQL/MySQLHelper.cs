using govgameSharedClasses.Models.MySQL;
using Microsoft.EntityFrameworkCore;

namespace govgameSharedClasses.Helpers.MySQL
{
    public partial class MySQLHelper
    {
        static string connectionString = "server=localhost;user=root;database=govgame;port=3306;password='A`T7fYQ!tP6;N[K$';";

        public class DatabaseContext : DbContext
        {
            public DbSet<Country> countries { get; set; }
            public DbSet<Email> emails { get; set; }
            public DbSet<InvitedMinister> invitedministers { get; set; }
            public DbSet<MinistryEnum> ministryenums { get; set; }
            public DbSet<Notification> notifications { get; set; }
            public DbSet<UserEmail> useremails { get; set; }
            public DbSet<User> users { get; set; }

            protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
            {
                optionsBuilder.UseMySql(connectionString, ServerVersion.AutoDetect(connectionString));

                base.OnConfiguring(optionsBuilder);
            }
        }
    }
}
