using Domain;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace Persistence
{
    public class DataContext : IdentityDbContext<User>
    {
        public DataContext(DbContextOptions options) : base(options)
        {
        }
        
        public DbSet<Activity> Activities { get; set; }
        
        public DbSet<ActivityAttendee> ActivityAttendees { get; set; }
        
        public DbSet<Photo> Photos { get; set; }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            // define composed primary key
            builder.Entity<ActivityAttendee>(action =>
            {
                action.HasKey(activityAttendee => new {activityAttendee.UserId, activityAttendee.ActivityId});
            });

            // define relations
            builder.Entity<ActivityAttendee>()
                .HasOne(activityAttendee => activityAttendee.User)
                .WithMany(user => user.Activities)
                .HasForeignKey(activityAttendee => activityAttendee.UserId);
            
            builder.Entity<ActivityAttendee>()
                .HasOne(activityAttendee => activityAttendee.Activity)
                .WithMany(activity => activity.Attendees)
                .HasForeignKey(activityAttendee => activityAttendee.ActivityId);
        }
    }
}