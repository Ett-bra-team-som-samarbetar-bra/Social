namespace SocialBackend.Data;

public interface ISocialContext
{
    DbSet<User> Users { get; set; }
    DbSet<Post> Posts { get; set; }
    DbSet<Comment> Comments { get; set; }
    DbSet<Message> Messages { get; set; }
}
