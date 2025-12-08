using Microsoft.AspNetCore.Authorization;

namespace SocialBackend.Controllers;

[ApiController]
[Route("api/post")]
public class PostController(IPostService postService) : ControllerBase
{
    private readonly IPostService _postService = postService;

    [Authorize]
    [HttpGet("all/{pageIndex:int?}/{pageSize:int?}")]
    public async Task<ActionResult<List<PostResponseDto>>> GetPosts(int pageIndex = 1, int pageSize = 10)
    {
        var posts = await _postService.GetPosts(pageIndex, pageSize);
        return Ok(posts);
    }

    [Authorize]
    [HttpGet("user-posts/{userId}/{pageIndex:int?}/{pageSize:int?}")]
    public async Task<ActionResult<List<PostResponseDto>>> GetUserPosts(int userId, int pageIndex = 1, int pageSize = 10)
    {
        var userPosts = await _postService.GetUserPosts(pageIndex, pageSize, userId);
        return Ok(userPosts);
    }

    [Authorize]
    [HttpGet("follower-posts/{pageIndex:int?}/{pageSize:int?}")]
    public async Task<ActionResult<List<PostResponseDto>>> GetFollowingPosts(int pageIndex = 1, int pageSize = 10)
    {
        var UserId = HttpContext.GetUserId();
        var userPosts = await _postService.GetFollowingPosts(pageIndex, pageSize, UserId);
        return Ok(userPosts);
    }

    [Authorize]
    [HttpGet("user-posts/{id}")]
    public async Task<ActionResult<PostResponseDto>> GetPostWithComments(int id)
    {
        var post = await _postService.GetPostWithComments(id);
        return Ok(post);
    }

    [Authorize]
    [HttpPost]
    public async Task<ActionResult<int>> CreatePost([FromBody] PostCreateDto dto)
    {
        var UserId = HttpContext.GetUserId();
        var postId = await _postService.CreatePost(dto, UserId);
        return Ok(postId);
    }

    [Authorize]
    [HttpDelete("{postId}")]
    public async Task<ActionResult<int>> DeletePost(int postId)
    {
        var UserId = HttpContext.GetUserId();
        var deletedPostId = await _postService.DeletePost(postId, UserId);
        return Ok(deletedPostId);
    }

    [Authorize]
    [HttpPut("like/{postId}")]
    public async Task<ActionResult<int>> UpdateLikeCount(int postId)
    {
        var userId = HttpContext.GetUserId();
        var updatedLikeCount = await _postService.UpdateLikeCount(postId, userId);
        return Ok(updatedLikeCount);
    }

    [Authorize]
    [HttpGet("comments/{postId}/{pageIndex}/{pageSize}")]
    public async Task<ActionResult<PaginatedList<CommentResponseDto>>> GetComments(int postId, int pageIndex = 1, int pageSize = 10)
    {
        var comments = await _postService.GetComments(pageIndex, pageSize, postId);
        return Ok(comments);
    }

    [Authorize]
    [HttpPost("comments/{postId}")]
    public async Task<ActionResult<int>> CreateComment(int postId, [FromBody] CommentCreateDto dto)
    {
        var userId = HttpContext.GetUserId();
        var commentId = await _postService.CreateComment(dto, postId, userId);
        return Ok(commentId);
    }
}
