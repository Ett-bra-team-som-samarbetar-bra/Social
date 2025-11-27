namespace SocialBackend.Services;

public interface IPostService
{
    public Task<List<Post>> GetAllPosts();
    public Task<PaginatedList<Post>> GetPosts(int pageIndex, int pageSize);
    public Task<PaginatedList<Post>> GetUserPosts(int pageIndex, int pageSize, User user);
    public Task<PaginatedList<Post>> GetFollowingPosts(int pageIndex, int pageSize, User user);
    public Task<Post> CreatePost(CreatePostDto dto);
    public Task<Comment> CreateComment(CreateCommentDto dto, int postId);
}
