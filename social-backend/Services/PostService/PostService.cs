namespace SocialBackend.Services;

public interface IPostService
{
    Task<int> CreatePost(PostCreateDto dto, int userId);
    Task<PaginatedList<PostResponseDto>> GetPosts(int pageIndex, int pageSize);
    Task<PaginatedList<PostResponseDto>> GetUserPosts(int pageIndex, int pageSize, int user);
    Task<PaginatedList<PostResponseDto>> GetFollowingPosts(int pageIndex, int pageSize, int user);
    Task<int> CreateComment(CommentCreateDto dto, int postId, int userId);
    Task<PaginatedList<CommentResponseDto>> GetComments(int pageIndex, int pageSize, int postId);
    Task<int> UpdateLikeCount(int postId, int userId);
}

public class PostService(DatabaseContext dbContext) : IPostService
{
    private readonly IDatabaseContext _db = dbContext;

    public async Task<PaginatedList<PostResponseDto>> GetPosts(int pageIndex, int pageSize)
    {
        var posts = await _db.Posts
            .Include(p => p.User)
            .Include(p => p.Comments)
            .OrderBy(b => b.CreatedAt)
            .Skip((pageIndex - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();

        var paginatedPosts = await GetPaginatedPosts(posts, pageIndex, pageSize);
        var dtoPosts = paginatedPosts.Items.Select(ToPostDto).ToList();

        return new PaginatedList<PostResponseDto>(dtoPosts, pageIndex, paginatedPosts.TotalPages);
    }

    public async Task<PaginatedList<PostResponseDto>> GetUserPosts(int pageIndex, int pageSize, int userId)
    {
        var user = await _db.Users.FindAsync(userId)
            ?? throw new KeyNotFoundException("User not found");

        var posts = await _db.Posts
            .Where(p => p.UserId == user.Id)
            .Include(p => p.User)
            .Include(p => p.Comments)
            .OrderBy(b => b.CreatedAt)
            .Skip((pageIndex - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();

        var paginatedPosts = await GetPaginatedPosts(posts, pageIndex, pageSize);
        var dtoPosts = paginatedPosts.Items.Select(ToPostDto).ToList();

        return new PaginatedList<PostResponseDto>(dtoPosts, pageIndex, paginatedPosts.TotalPages);
    }

    public async Task<PaginatedList<PostResponseDto>> GetFollowingPosts(int pageIndex, int pageSize, int userId)
    {
        var user = await _db.Users.FindAsync(userId)
            ?? throw new KeyNotFoundException("User not found");

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

        var paginatedPosts = await GetPaginatedPosts(posts, pageIndex, pageSize);
        var dtoPosts = paginatedPosts.Items.Select(ToPostDto).ToList();

        return new PaginatedList<PostResponseDto>(dtoPosts, pageIndex, paginatedPosts.TotalPages);
    }

    public async Task<int> CreatePost(PostCreateDto dto, int userId)
    {
        var user = await _db.Users.FindAsync(userId)
            ?? throw new KeyNotFoundException("User not found");

        var post = new Post
        {
            UserId = user.Id,
            User = user,
            Title = dto.Title,
            Content = dto.Content
        };

        _db.Posts.Add(post);
        await _db.SaveChangesAsync();

        return post.Id;
    }

    public async Task<int> CreateComment(CommentCreateDto dto, int postId, int userId)
    {
        var user = await _db.Users.FindAsync(userId)
            ?? throw new KeyNotFoundException("User not found");

        var post = await _db.Posts.FindAsync(postId)
            ?? throw new KeyNotFoundException("Post not found");

        var comment = new Comment
        {
            UserId = user.Id,
            User = user,
            Content = dto.Content
        };

        _db.Comments.Add(comment);
        post.Comments.Add(comment); // TODO ???
        await _db.SaveChangesAsync();

        return comment.Id;
    }

    public async Task<PaginatedList<CommentResponseDto>> GetComments(int pageIndex, int pageSize, int postId)
    {
        var post = await _db.Posts
            .Include(p => p.Comments)
            .ThenInclude(c => c.User)
            .FirstOrDefaultAsync(p => p.Id == postId)
            ?? throw new KeyNotFoundException("Post not found");

        var comments = post.Comments.ToList();
        var paginatedComments = GetPaginatedComments(comments, pageIndex, pageSize);
        var dto = paginatedComments.Items.Select(ToCommentDto).ToList();

        return new PaginatedList<CommentResponseDto>(dto, pageIndex, paginatedComments.TotalPages);
    }

    public async Task<int> UpdateLikeCount(int postId, int userId)
    {
        var user = await _db.Users.FindAsync(userId)
            ?? throw new KeyNotFoundException("User not found");

        var post = await _db.Posts.FindAsync(postId)
            ?? throw new KeyNotFoundException("Post not found");

        if (user.LikedPosts.Any(p => p.Id == postId))
        {
            return post.LikeCount;
        }

        post.LikeCount++;
        user.LikedPosts.Add(post);
        await _db.SaveChangesAsync();

        return post.LikeCount;
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

    private static PostResponseDto ToPostDto(Post post)
    {
        return new PostResponseDto
        {
            UserId = post.UserId,
            UserName = post.User.Username,
            CreatedAt = post.CreatedAt,
            UpdatedAt = post.UpdatedAt,
            Title = post.Title,
            Content = post.Content,
            LikeCount = post.LikeCount,
            Comments = post.Comments.Select(ToCommentDto).ToList()
        };
    }

    private static CommentResponseDto ToCommentDto(Comment comment)
    {
        return new CommentResponseDto
        {
            UserId = comment.UserId,
            UserName = comment.User.Username,
            Content = comment.Content,
            CreatedAt = comment.CreatedAt
        };
    }
}
