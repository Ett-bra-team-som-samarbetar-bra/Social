namespace SocialBackend.Data;

public class DatabaseContext : DbContext, IDatabaseContext
{
    public DbSet<User> Users { get; set; }
    public DbSet<Post> Posts { get; set; }
    public DbSet<Comment> Comments { get; set; }
    public DbSet<Message> Messages { get; set; }

    public DatabaseContext(DbContextOptions<DatabaseContext> options) : base(options)
    {
    }

    protected override void OnConfiguring(DbContextOptionsBuilder options)
    {
        if (!options.IsConfigured)
            options.UseSqlite("Data Source=./Data/social.db");
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        modelBuilder.Entity<Message>()
            .HasOne(m => m.SendingUser)
            .WithMany(u => u.MessagesSent)
            .HasForeignKey(m => m.SendingUserId)
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<Message>()
            .HasOne(m => m.ReceivingUser)
            .WithMany(u => u.MessagesReceived)
            .HasForeignKey(m => m.ReceivingUserId)
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<User>()
            .HasMany(u => u.Following)
            .WithMany(u => u.Followers)
            .UsingEntity(j => j.ToTable("UserFollowers"));

        modelBuilder.Entity<User>()
            .HasMany(u => u.LikedPosts)
            .WithMany(p => p.LikedBy)
            .UsingEntity(j => j.ToTable("UserLikedPosts"));

        modelBuilder.Entity<Post>()
            .HasOne(p => p.User)
            .WithMany(u => u.Posts)
            .HasForeignKey(p => p.UserId)
            .OnDelete(DeleteBehavior.Cascade);

    }

    public async Task<int> SaveChangesAsync()
    {
        return await base.SaveChangesAsync();
    }
}