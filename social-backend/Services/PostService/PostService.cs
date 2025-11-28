namespace SocialBackend.Services;

public interface IPostService
{
    Task<List<PostResponseDto>> GetAllPosts();
    Task<int> CreatePost(PostCreateDto dto);
    Task<PaginatedList<PostResponseDto>> GetPosts(int pageIndex, int pageSize);
    Task<PaginatedList<PostResponseDto>> GetUserPosts(int pageIndex, int pageSize, User user);
    Task<PaginatedList<PostResponseDto>> GetFollowingPosts(int pageIndex, int pageSize, User user);
    Task<int> CreateComment(CommentCreateDto dto, int postId);
    Task<List<CommentResponseDto>> GetAllComments(int postId);
    Task<PaginatedList<CommentResponseDto>> GetComment(int pageIndex, int pageSize, int postId);
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

    public async Task<PaginatedList<PostResponseDto>> GetUserPosts(int pageIndex, int pageSize, User user)
    {
        ArgumentNullException.ThrowIfNull(user);

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

    public async Task<PaginatedList<PostResponseDto>> GetFollowingPosts(int pageIndex, int pageSize, User user)
    {
        ArgumentNullException.ThrowIfNull(user);

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

    public async Task<int> CreatePost(PostCreateDto dto)
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

        return post.Id;
    }

    public async Task<int> CreateComment(CommentCreateDto dto, int postId)
    {
        var post = await _db.Posts.FindAsync(postId)
            ?? throw new KeyNotFoundException("Post not found");

        var comment = new Comment
        {
            UserId = dto.User.Id,
            User = dto.User,
            Content = dto.Content
        };

        _db.Comments.Add(comment);
        post.Comments.Add(comment); // TODO ???
        await _db.SaveChangesAsync();

        return comment.Id;
    }

    public async Task<PaginatedList<CommentResponseDto>> GetComment(int pageIndex, int pageSize, int postId)
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

    public async Task<List<PostResponseDto>> GetAllPosts()
    {
        var posts = await _db.Posts
            .Include(p => p.User)
            .Include(p => p.Comments)
            .ToListAsync();

        return posts.Select(ToPostDto).ToList();
    }

    public async Task<List<CommentResponseDto>> GetAllComments(int postId)
    {
        var post = await _db.Posts
            .Include(p => p.Comments)
            .ThenInclude(c => c.User)
            .FirstOrDefaultAsync(p => p.Id == postId)
            ?? throw new KeyNotFoundException("Post not found");

        return post.Comments.Select(ToCommentDto).ToList();
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
            Comments = post.Comments
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
