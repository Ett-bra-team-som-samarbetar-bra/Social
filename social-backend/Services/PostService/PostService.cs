namespace SocialBackend.Services;

public class PostService(DatabaseContext dbContext) : IPostService
{
    private readonly IDatabaseContext _db = dbContext;

    public async Task<List<Post>> GetAllPosts()
    {
        return await _db.Posts
            .Include(p => p.User)
            .Include(p => p.Comments)
            .ToListAsync();
    }

    public async Task<PaginatedList<Post>> GetPosts(int pageIndex, int pageSize)
    {
        var posts = await _db.Posts
            .Include(p => p.User)
            .Include(p => p.Comments)
            .OrderBy(b => b.CreatedAt)
            .Skip((pageIndex - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();

        return await GetPaginatedPosts(posts, pageIndex, pageSize);
    }

    public async Task<PaginatedList<Post>> GetUserPosts(int pageIndex, int pageSize, User user)
    {
        var posts = await _db.Posts
            .Where(p => p.UserId == user.Id)
            .Include(p => p.User)
            .Include(p => p.Comments)
            .OrderBy(b => b.CreatedAt)
            .Skip((pageIndex - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();

        return await GetPaginatedPosts(posts, pageIndex, pageSize);
    }

    public async Task<PaginatedList<Post>> GetFollowingPosts(int pageIndex, int pageSize, User user)
    {
        var followingIds = user.Following.Select(u => u.Id).ToList();

        var posts = await _db.Posts
            .Where(p => followingIds.Contains(p.UserId))
            .Include(p => p.User)
            .Include(p => p.Comments)
                .ThenInclude(c => c.User)
            .OrderBy(b => b.CreatedAt)
            .Skip((pageIndex - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();

        return await GetPaginatedPosts(posts, pageIndex, pageSize);
    }

    public async Task<Post> CreatePost(CreatePostDto dto)
    {
        var post = new Post
        {
            UserId = dto.User.Id,
            User = dto.User,
            Title = dto.Title,
            Content = dto.Content
        };

        _db.Posts.Add(post);
        await _db.SaveChangesAsync();

        return post;
    }

    public async Task<Comment> CreateComment(CreateCommentDto dto, int postId)
    {
        var post =
            await _db.Posts.FindAsync(postId)
            ?? throw new Exception("Post not found");

        var comment = new Comment
        {
            UserId = dto.User.Id,
            User = dto.User,
            Content = dto.Content
        };

        _db.Comments.Add(comment);
        post.Comments.Add(comment);     // TODO ???
        await _db.SaveChangesAsync();

        return comment;
    }

    public async Task<PaginatedList<Comment>> GetComment(int pageIndex, int pageSize, int postId)
    {
        var post = await _db.Posts
            .Include(p => p.Comments)
            .ThenInclude(c => c.User)
            .FirstOrDefaultAsync(p => p.Id == postId)
            ?? throw new Exception("Post not found");

        var comments = post.Comments.ToList();

        return GetPaginatedComments(comments, pageIndex, pageSize);
    }

    private async Task<PaginatedList<Post>> GetPaginatedPosts(List<Post> posts, int pageIndex, int pageSize)
    {
        var count = await _db.Posts.CountAsync();
        var totalPages = (int)Math.Ceiling(count / (double)pageSize);

        return new PaginatedList<Post>(posts, pageIndex, totalPages);
    }

    private static PaginatedList<Comment> GetPaginatedComments(List<Comment> comment, int pageIndex, int pageSize)
    {
        var count = comment.Count;
        var totalPages = (int)Math.Ceiling(count / (double)pageSize);

        return new PaginatedList<Comment>(comment, pageIndex, totalPages);
    }
}
