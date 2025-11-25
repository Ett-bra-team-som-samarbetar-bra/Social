namespace SocialBackend.Data;

public class SocialContext : DbContext, ISocialContext
{
    public DbSet<User> Users { get; set; }
    public DbSet<Post> Posts { get; set; }
    public DbSet<Comment> Comments { get; set; }
    public DbSet<Message> Messages { get; set; }

    public SocialContext(DbContextOptions<SocialContext> options) : base(options)
    {
    }

    protected override void OnConfiguring(DbContextOptionsBuilder options)
    {
        options.UseSqlite();
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

    }
}