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
