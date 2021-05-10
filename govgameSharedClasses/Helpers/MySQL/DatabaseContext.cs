using govgameSharedClasses.Models.MySQL;
using Microsoft.EntityFrameworkCore;
using System;

namespace govgameSharedClasses.Helpers.MySQL
{
    public class DatabaseContext : DbContext
    {
        static string connectionString = "server=localhost;user=root;database=govgame;port=3306;password='A`T7fYQ!tP6;N[K$';";

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

            modelBuilder.Entity<User>().Property(user => user.Ministry).HasConversion(dbValue => dbValue.ToString(), dbValue => (MinistryHelper.MinistryCode)Enum.Parse(typeof(MinistryHelper.MinistryCode), dbValue));

            base.OnModelCreating(modelBuilder);
        }
    }
}
