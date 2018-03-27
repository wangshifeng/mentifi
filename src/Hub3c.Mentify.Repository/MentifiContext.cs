using Hub3c.Mentify.Repository.Models;
using Microsoft.EntityFrameworkCore;

namespace Hub3c.Mentify.Repository
{
    public class MentifiContext : DbContext
    {
        public MentifiContext(DbContextOptions<MentifiContext> options) : base(options)
        {
        }

        public DbSet<SystemUser> SystemUser { get; set; }
        public DbSet<Business> Business { get; set; }
        public DbSet<BusinessToBusiness> BusinessToBusiness { get; set; }
        public DbSet<SystemUserDevice> SystemUserDevice { get; set; }
        public DbSet<LookupTypeCode> LookupTypeCode { get; set; }
        public DbSet<Notification> Notification { get; set; }

        public DbSet<EduAdditionalActivity> EduAdditionalActivity { get; set; }
        public DbSet<EduInformalExperience> EduInformalExperience { get; set; }
        public DbSet<EduSubjectExperience> EduSubjectExperience { get; set; }
        public DbSet<EduSubjectPreference> EduSubjectPreference { get; set; }
        public DbSet<MentifiApplicationUsage> MentifiApplicationUsage { get; set; }
        public DbSet<EduUser> EduUser { get; set; }
        public DbSet<Education> Education { get; set; }
        public DbSet<Experience> Experience { get; set; }
        public DbSet<BusinessBulletinBoard> BusinessBulletinBoard { get; set; }
        public DbSet<DocumentRegister> DocumentRegister { get; set; }
        public DbSet<MentifiTask> MentifiTask { get; set; }
        public DbSet<MentifiChannelTask> MentifiChannelTask { get; set; }
        public DbSet<MentifiGoal> MentifiGoal { get; set; }
        public DbSet<MentifiGoalProgress> MentifiGoalProgress { get; set; }
        public DbSet<Message> Message { get; set; }
        public DbSet<MentifiMessageBoardPostChecker> MentifiMessageBoardPostChecker { get; set; }
        public DbSet<Project> Project { get; set; }
        public DbSet<MentifiMessage> MentifiMessage { get; set; }
        public DbSet<InvitationLink> InvitationLink { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<DocumentRegister>()
                .HasKey(c => new { c.DocumentId, c.RowGuid });
        }
    }
}
