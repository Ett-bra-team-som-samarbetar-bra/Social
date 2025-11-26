namespace SocialBackend.Data;

public interface IDatabaseContext
{
    DbSet<User> Users { get; set; }
    DbSet<Post> Posts { get; set; }
    DbSet<Comment> Comments { get; set; }
    DbSet<Message> Messages { get; set; }
    Task<int> SaveChangesAsync();
}
