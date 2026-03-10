using Microsoft.EntityFrameworkCore;
using SocialLibrary.API.Entities;

namespace SocialLibrary.API.Data
{
    public class SocialLibraryContext : DbContext
    {
        public SocialLibraryContext(DbContextOptions<SocialLibraryContext> options)
            : base(options)
        {
        }

        public DbSet<User> Users { get; set; }
        public DbSet<Follow> Follows { get; set; }
        public DbSet<Content> Contents { get; set; }
        public DbSet<Rating> Ratings { get; set; }
        public DbSet<Review> Reviews { get; set; }
        public DbSet<UserList> UserLists { get; set; } = null!;
        public DbSet<UserListItem> UserListItems { get; set; } = null!;



        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // UserName ve Email benzersiz olsun
            modelBuilder.Entity<User>()
                .HasIndex(u => u.UserName)
                .IsUnique();

            modelBuilder.Entity<User>()
                .HasIndex(u => u.Email)
                .IsUnique();

            modelBuilder.Entity<Follow>()
                .HasOne(f => f.Follower)
                .WithMany(u => u.Following)
                .HasForeignKey(f => f.FollowerId)
                .OnDelete(DeleteBehavior.NoAction);

            modelBuilder.Entity<Follow>()
                .HasOne(f => f.Followed)
                .WithMany(u => u.Followers)
                .HasForeignKey(f => f.FollowedId)
                .OnDelete(DeleteBehavior.NoAction);

            modelBuilder.Entity<Follow>()
                .HasIndex(f => new { f.FollowerId, f.FollowedId })
                .IsUnique(); // aynı kişiyi iki kere takip edemesin

            // Content
            modelBuilder.Entity<Content>(entity =>
            {
                entity.Property(c => c.Title)
                      .IsRequired()
                      .HasMaxLength(200);

                entity.Property(c => c.Type)
                      .IsRequired();
            });

            // Rating
            modelBuilder.Entity<Rating>(entity =>
            {
                entity.HasKey(r => r.Id);

                entity.Property(r => r.Score)
                      .IsRequired();

                // 1 user + 1 content => 1 rating
                entity.HasIndex(r => new { r.UserId, r.ContentId })
                      .IsUnique();

                entity.HasOne(r => r.User)
                      .WithMany(u => u.Ratings)
                      .HasForeignKey(r => r.UserId)
                      .OnDelete(DeleteBehavior.Cascade);

                entity.HasOne(r => r.Content)
                      .WithMany(c => c.Ratings)
                      .HasForeignKey(r => r.ContentId)
                      .OnDelete(DeleteBehavior.Cascade);
            });

            // Review
            modelBuilder.Entity<Review>(entity =>
            {
                entity.HasKey(r => r.Id);

                entity.Property(r => r.Text)
                      .IsRequired();

                entity.HasOne(r => r.User)
                      .WithMany(u => u.Reviews)
                      .HasForeignKey(r => r.UserId)
                      .OnDelete(DeleteBehavior.Cascade);

                entity.HasOne(r => r.Content)
                      .WithMany(c => c.Reviews)
                      .HasForeignKey(r => r.ContentId)
                      .OnDelete(DeleteBehavior.Cascade);
            });

            modelBuilder.Entity<UserList>(entity =>
            {
                entity.Property(l => l.Name)
                    .IsRequired()
                    .HasMaxLength(200);

                entity.HasOne(l => l.User)
                    .WithMany(u => u.Lists)
                    .HasForeignKey(l => l.UserId)
                    .OnDelete(DeleteBehavior.Cascade);
            });

            modelBuilder.Entity<UserListItem>(entity =>
            {
                entity.HasOne(li => li.UserList)
                    .WithMany(l => l.Items)
                    .HasForeignKey(li => li.UserListId)
                    .OnDelete(DeleteBehavior.Cascade);

                entity.HasOne(li => li.Content)
                    .WithMany()              // Content tarafında navigation zorunlu değil
                    .HasForeignKey(li => li.ContentId)
                    .OnDelete(DeleteBehavior.Cascade);

                // Bir içerik aynı listede sadece 1 kez olsun
                entity.HasIndex(li => new { li.UserListId, li.ContentId })
                    .IsUnique();
            });

        }
    }
}
