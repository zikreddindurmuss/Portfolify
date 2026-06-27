using Microsoft.EntityFrameworkCore;
using Portfolify.Domain.Entities;

namespace Portfolify.Infrastructure.Persistence;

public sealed class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

    public DbSet<User> Users => Set<User>();
    public DbSet<Profile> Profiles => Set<Profile>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<User>(entity =>
        {
            entity.ToTable("users");
            entity.HasKey(u => u.Id);
            entity.Property(u => u.Id).HasColumnName("id");
            entity.Property(u => u.Email).HasColumnName("email").HasMaxLength(256).IsRequired();
            entity.HasIndex(u => u.Email).IsUnique();
            entity.Property(u => u.Username).HasColumnName("username").HasMaxLength(64).IsRequired();
            entity.HasIndex(u => u.Username).IsUnique();
            entity.Property(u => u.Slug).HasColumnName("slug").HasMaxLength(64).IsRequired();
            entity.HasIndex(u => u.Slug).IsUnique();
            entity.Property(u => u.PasswordHash).HasColumnName("password_hash").IsRequired();
            entity.Property(u => u.Bio).HasColumnName("bio").HasMaxLength(500);
            entity.Property(u => u.AvatarUrl).HasColumnName("avatar_url").HasMaxLength(1024);
            entity.Property(u => u.GitHubUrl).HasColumnName("github_url").HasMaxLength(512);
            entity.Property(u => u.LinkedInUrl).HasColumnName("linkedin_url").HasMaxLength(512);
            entity.Property(u => u.BlogUrl).HasColumnName("blog_url").HasMaxLength(512);
            entity.Property(u => u.CreatedAt).HasColumnName("created_at");
            entity.Property(u => u.UpdatedAt).HasColumnName("updated_at");
        });

        modelBuilder.Entity<Profile>(entity =>
        {
            entity.ToTable("profiles");
            entity.HasKey(p => p.Id);
            entity.Property(p => p.Id).HasColumnName("id");
            entity.Property(p => p.UserId).HasColumnName("user_id").IsRequired();
            entity.HasIndex(p => p.UserId).IsUnique();
            entity.Property(p => p.Bio).HasColumnName("bio").HasMaxLength(500);
            entity.Property(p => p.AvatarUrl).HasColumnName("avatar_url").HasMaxLength(1024);
            entity.Property(p => p.GithubUrl).HasColumnName("github_url").HasMaxLength(512);
            entity.Property(p => p.LinkedinUrl).HasColumnName("linkedin_url").HasMaxLength(512);
            entity.Property(p => p.BlogUrl).HasColumnName("blog_url").HasMaxLength(512);
            entity.Property(p => p.CreatedAt).HasColumnName("created_at");
            entity.Property(p => p.UpdatedAt).HasColumnName("updated_at");

            entity.HasOne(p => p.User)
                  .WithMany()
                  .HasForeignKey(p => p.UserId)
                  .OnDelete(DeleteBehavior.Cascade);
        });
    }
}
