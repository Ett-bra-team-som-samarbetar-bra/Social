namespace social_backend.Controllers;

[ApiController]
[Route("api/[controller]")]
public class PostController(IPostService postService) : ControllerBase
{
    private readonly IPostService _postService = postService;

    [HttpGet("{pageIndex}/{pageSize}")]
    public async Task<ActionResult<List<PostResponseDto>>> GetPosts(int pageIndex = 1, int pageSize = 10)
    {
        var posts = await _postService.GetPosts(pageIndex, pageSize);
        return Ok(posts);
    }

    [HttpGet("user-posts/{pageIndex}/{pageSize}")]
    public async Task<ActionResult<List<PostResponseDto>>> GetUserPosts(int pageIndex = 1, int pageSize = 10)
    {
        var UserId = HttpContext.GetUserId();
        var userPosts = await _postService.GetUserPosts(pageIndex, pageSize, UserId);
        return Ok(userPosts);
    }

    [HttpGet("follower-posts/{pageIndex}/{pageSize}")]
    public async Task<ActionResult<List<PostResponseDto>>> GetFollowingPosts(int pageIndex = 1, int pageSize = 10)
    {
        var UserId = HttpContext.GetUserId();
        var userPosts = await _postService.GetUserPosts(pageIndex, pageSize, UserId);
        return Ok(userPosts);
    }

    [HttpPut("like/{postId}")]
    public async Task<ActionResult<int>> UpdateLikeCount(int postId)
    {
        var userId = HttpContext.GetUserId();
        var updatedLikeCount = await _postService.UpdateLikeCount(postId, userId);
        return Ok(updatedLikeCount);
    }

    [HttpPost]
    public async Task<ActionResult<int>> Create([FromBody] PostCreateDto dto)
    {
        var UserId = HttpContext.GetUserId();
        var postId = await _postService.CreatePost(dto, UserId);
        return Ok(postId);
    }

    [HttpGet("comments/{postId}/{pageIndex}/{pageSize}")]
    public async Task<ActionResult<PaginatedList<CommentResponseDto>>> GetComments(int postId, int pageIndex = 1, int pageSize = 10)
    {
        var comments = await _postService.GetComments(pageIndex, pageSize, postId);
        return Ok(comments);
    }

    [HttpPost("comments/{postId}")]
    public async Task<ActionResult<int>> CreateComment(int postId, [FromBody] CommentCreateDto dto)
    {
        var userId = HttpContext.GetUserId();
        var commentId = await _postService.CreateComment(dto, postId, userId);
        return Ok(commentId);
    }
}
