using social_backend.tests.Data;
using SocialBackend.Models;
using SocialBackend.Services;
using SocialBackend.Dto;
using SocialBackend.Exceptions;

namespace social_backend.tests;

public class PostServiceTests : TestBase
{
    [Fact]
    public async Task GetPosts_ReturnsThreePaginatedPosts()
    {
        var user1 = CreateTestUser("TestUser1");
        CreateTestPost(user1, [], "Title1", "Content1");
        CreateTestPost(user1, [], "Title2", "Content2");
        CreateTestPost(user1, [], "Title3", "Content3");
        Context.SaveChanges();

        var postService = new PostService(Context);
        var result = await postService.GetPosts(pageIndex: 1, pageSize: 3);

        Assert.Equal(3, result.Items.Count);
        Assert.Equal("Title1", result.Items[0].Title);
        Assert.Equal("Title2", result.Items[1].Title);
        Assert.Equal("Title3", result.Items[2].Title);
    }

    [Fact]
    public async Task GetPosts_ReturnsEmptyArray_WhenNoPosts()
    {
        var postService = new PostService(Context);
        var result = await postService.GetPosts(pageIndex: 1, pageSize: 3);

        Assert.Empty(result.Items);
    }

    [Fact]
    public async Task GetPosts_ReturnsPaginatedPosts_Correctly()
    {
        var user1 = CreateTestUser("TestUser1");

        for (int i = 1; i <= 10; i++)
            CreateTestPost(user1, [], $"Title{i}", $"Content{i}");

        Context.SaveChanges();

        var postService = new PostService(Context);
        var result = await postService.GetPosts(pageIndex: 2, pageSize: 3);

        Assert.Equal(3, result.Items.Count);
        Assert.Equal("Title4", result.Items[0].Title);
        Assert.Equal("Title5", result.Items[1].Title);
        Assert.Equal("Title6", result.Items[2].Title);
    }

    [Fact]
    public async Task GetPosts_IncludesComments_ForEachPost()
    {
        var user1 = CreateTestUser("TestUser1");
        var comment1 = CreateTestComment(user1, "Comment1", 1);
        var comment2 = CreateTestComment(user1, "Comment2", 2);

        CreateTestPost(user1, [comment1, comment2], "Title1", "Content1");
        Context.SaveChanges();

        var postService = new PostService(Context);
        var result = await postService.GetPosts(pageIndex: 1, pageSize: 3);
        var comments = result.Items[0].Comments.ToList();

        Assert.Single(result.Items);
        Assert.Equal(2, result.Items[0].Comments.Count);
        Assert.Equal("Comment1", comments[0].Content);
        Assert.Equal("Comment2", comments[1].Content);
    }

    [Fact]
    public async Task GetPosts_ReturnsPosts_InDescendingOrderByCreationDate()
    {
        var user1 = CreateTestUser("TestUser1");

        // Different timestamps
        CreateTestPost(user1, [], "Title1", "Content1");
        Thread.Sleep(10);
        CreateTestPost(user1, [], "Title2", "Content2");
        Thread.Sleep(10);
        CreateTestPost(user1, [], "Title3", "Content3");

        Context.SaveChanges();

        var postService = new PostService(Context);
        var result = await postService.GetPosts(pageIndex: 1, pageSize: 3);

        Assert.True(result.Items[0].CreatedAt < result.Items[1].CreatedAt);
        Assert.True(result.Items[1].CreatedAt < result.Items[2].CreatedAt);
    }

    [Fact]
    public async Task GetPosts_HandlesPostsWithoutComments()
    {
        var user1 = CreateTestUser("TestUser1");
        CreateTestPost(user1, [], "Title1", "Content1");
        Context.SaveChanges();

        var postService = new PostService(Context);
        var result = await postService.GetPosts(pageIndex: 1, pageSize: 3);

        Assert.Single(result.Items);
        Assert.Empty(result.Items[0].Comments);
    }

    [Fact]
    public async Task GetUserPosts_ReturnsOnlySpecifiedUsersPosts()
    {
        var user1 = CreateTestUser("TestUser1");
        var user2 = CreateTestUser("TestUser2");
        CreateTestPost(user1, [], "User1Title1", "User1Content1");
        CreateTestPost(user2, [], "User2Title1", "User2Content1");
        Context.SaveChanges();

        var postService = new PostService(Context);
        var result = await postService.GetUserPosts(pageIndex: 1, pageSize: 3, user1.Id);

        Assert.Single(result.Items);
        Assert.Equal("User1Title1", result.Items[0].Title);
        Assert.Equal("User1Content1", result.Items[0].Content);
    }

    [Fact]
    public async Task GetUserPosts_ReturnsEmptyArray_WhenUserHasNoPosts()
    {
        var user1 = CreateTestUser("TestUser1");
        Context.SaveChanges();

        var postService = new PostService(Context);
        var result = await postService.GetUserPosts(pageIndex: 1, pageSize: 3, user1.Id);

        Assert.Empty(result.Items);
    }

    [Fact]
    public async Task GetUserPosts_ShouldThrow_WhenNoUser()
    {
        var postService = new PostService(Context);
        await Assert.ThrowsAsync<NotFoundException>(async () =>
        {
            await postService.GetUserPosts(pageIndex: 1, pageSize: 3, 1337);
        });
    }

    [Fact]
    public async Task GetFollowingPosts_ReturnsPostsFromFollowedUsers()
    {
        var user1 = CreateTestUser("TestUser1");
        var user2 = CreateTestUser("TestUser2");
        var user3 = CreateTestUser("TestUser3");
        var user4 = CreateTestUser("TestUser4");

        user1.Following.Add(user2);
        user1.Following.Add(user3);

        CreateTestPost(user2, [], "User2Title1", "User2Content1");
        CreateTestPost(user3, [], "User3Title1", "User3Content1");
        CreateTestPost(user4, [], "User4Title1", "User4Content1");
        Context.SaveChanges();

        var postService = new PostService(Context);
        var result = await postService.GetFollowingPosts(pageIndex: 1, pageSize: 3, user1.Id);

        Assert.Equal(2, result.Items.Count);
        Assert.Equal("User2Title1", result.Items[0].Title);
        Assert.Equal("User3Title1", result.Items[1].Title);
    }

    [Fact]
    public async Task GetFollowingPosts_ShouldThrow_WhenNoUser()
    {
        var postService = new PostService(Context);
        await Assert.ThrowsAsync<NotFoundException>(async () =>
        {
            await postService.GetFollowingPosts(pageIndex: 1, pageSize: 3, 1337);
        });
    }

    [Fact]
    public async Task GetFollowingPosts_ReturnsEmptyArray_WhenNoFollowedUsersPosts()
    {
        var user1 = CreateTestUser("TestUser1");
        Context.SaveChanges();

        var postService = new PostService(Context);
        var result = await postService.GetFollowingPosts(pageIndex: 1, pageSize: 3, user1.Id);

        Assert.Empty(result.Items);
    }

    [Fact]
    public async Task GetFollowingPosts_ShouldReturnComments_ForEachPost()
    {
        var user1 = CreateTestUser("TestUser1");
        var user2 = CreateTestUser("TestUser2");
        var comment1 = CreateTestComment(user2, "Comment1", 1);
        var comment2 = CreateTestComment(user2, "Comment2", 2);

        user1.Following.Add(user2);
        CreateTestPost(user2, [comment1, comment2], "Title1", "Content1");
        Context.SaveChanges();

        var postService = new PostService(Context);
        var result = await postService.GetFollowingPosts(pageIndex: 1, pageSize: 3, user1.Id);
        var comments = result.Items[0].Comments.ToList();

        Assert.Single(result.Items);
        Assert.Equal(2, result.Items[0].Comments.Count);
        Assert.Equal("Comment1", comments[0].Content);
        Assert.Equal("Comment2", comments[1].Content);
    }

    [Fact]
    public async Task CreatePost_ShouldCreatePostSuccessfully()
    {
        var user1 = CreateTestUser("TestUser1");
        Context.SaveChanges();

        var createPostDto = new PostCreateDto
        {
            Title = "New Post Title",
            Content = "New Post Content"
        };

        var postService = new PostService(Context);
        var createdPostId = await postService.CreatePost(createPostDto, user1.Id);
        var createdPost = Context.Posts.Find(createdPostId);

        Assert.NotNull(createdPost);
        Assert.Equal(createdPostId, createdPost.Id);
        Assert.Equal("New Post Title", createdPost.Title);
        Assert.Equal("New Post Content", createdPost.Content);
        Assert.Equal(user1.Id, createdPost.UserId);
    }

    [Fact]
    public async Task CreatePost_ShouldThrow_WhenNoUser()
    {
        var createPostDto = new PostCreateDto
        {
            Title = "New Post Title",
            Content = "New Post Content"
        };

        var postService = new PostService(Context);
        await Assert.ThrowsAsync<NotFoundException>(async () =>
        {
            await postService.CreatePost(createPostDto, 1337);
        });
    }

    [Fact]
    public async Task DeletePost_ShouldThrow_WhenPostNotFound()
    {
        var user1 = CreateTestUser("TestUser1");
        Context.SaveChanges();

        var postService = new PostService(Context);
        await Assert.ThrowsAsync<NotFoundException>(async () =>
        {
            await postService.DeletePost(999, user1.Id);
        });
    }

    [Fact]
    public async Task DeletePost_ShouldThrow_WhenUserNotFound()
    {
        var user1 = CreateTestUser("TestUser1");
        var post1 = CreateTestPost(user1, [], "Title1", "Content1");
        Context.SaveChanges();

        var postService = new PostService(Context);
        await Assert.ThrowsAsync<NotFoundException>(async () =>
        {
            await postService.DeletePost(post1.Id, 999);
        });
    }

    [Fact]
    public async Task DeletePost_ShouldThrow_WhenUserNotAuthorized()
    {
        var user1 = CreateTestUser("TestUser1");
        var user2 = CreateTestUser("TestUser2");
        var post1 = CreateTestPost(user1, [], "Title1", "Content1");
        Context.SaveChanges();

        var postService = new PostService(Context);
        await Assert.ThrowsAsync<UnauthorizedException>(async () =>
        {
            await postService.DeletePost(post1.Id, user2.Id);
        });
    }

    [Fact]
    public async Task DeletePost_ShouldSetTitleAndContentToDeleted()
    {
        var user1 = CreateTestUser("TestUser1");
        var post1 = CreateTestPost(user1, [], "Title1", "Content1");
        Context.SaveChanges();

        var postService = new PostService(Context);
        await postService.DeletePost(post1.Id, user1.Id);

        var deletedPost = Context.Posts.Find(post1.Id);
        Assert.Equal("[Deleted]", deletedPost!.Title);
        Assert.Equal("[Deleted]", deletedPost!.Content);
    }

    [Fact]
    public async Task CreateComment_ShouldCreateCommentSuccessfully()
    {
        var user1 = CreateTestUser("TestUser1");
        var post1 = CreateTestPost(user1, [], "Title1", "Content1");
        Context.SaveChanges();

        var createCommentDto = new CommentCreateDto
        {
            Content = "New Comment Content"
        };

        var postService = new PostService(Context);
        var createdCommentId = await postService.CreateComment(createCommentDto, post1.Id, user1.Id);
        var createdComment = Context.Comments.Find(createdCommentId);
        var updatedPost = Context.Posts.Find(post1.Id);

        Assert.NotNull(createdComment);
        Assert.Equal(createdCommentId, createdComment.Id);
        Assert.Equal("New Comment Content", createdComment.Content);
        Assert.Equal(user1.Id, createdComment.UserId);
        Assert.Contains(updatedPost!.Comments, c => c.Id == createdCommentId && c.Content == "New Comment Content");
    }

    [Fact]
    public async Task CreateComment_ShouldThrow_WhenUserNotFound()
    {
        var user1 = CreateTestUser("TestUser1");
        var post1 = CreateTestPost(user1, [], "Title1", "Content1");
        Context.SaveChanges();

        var postService = new PostService(Context);
        var commentDto = new CommentCreateDto { Content = "New Comment Content" };

        await Assert.ThrowsAsync<NotFoundException>(async () =>
        {
            await postService.CreateComment(commentDto, post1.Id, 999);
        });
    }

    [Fact]
    public async Task CreateComment_ShouldThrow_WhenPostNotFound()
    {
        var user1 = CreateTestUser("TestUser1");
        Context.SaveChanges();

        var postService = new PostService(Context);
        var commentDto = new CommentCreateDto { Content = "New Comment Content" };

        await Assert.ThrowsAsync<NotFoundException>(async () =>
        {
            await postService.CreateComment(commentDto, 999, user1.Id);
        });
    }

    [Fact]
    public async Task GetComments_ShouldThrow_WhenPostNotFound()
    {
        var postService = new PostService(Context);
        await Assert.ThrowsAsync<NotFoundException>(async () =>
        {
            await postService.GetComments(pageIndex: 1, pageSize: 2, postId: 999);
        });
    }

    [Fact]
    public async Task GetComments_ShouldReturnPaginatedComments_ForGivenPost()
    {
        var user1 = CreateTestUser("TestUser1");
        var post1 = CreateTestPost(user1, [], "Title1", "Content1");

        for (int i = 1; i <= 5; i++)
        {
            var comment = CreateTestComment(user1, $"CommentContent{i}", id: i);
            post1.Comments.Add(comment);
        }
        Context.SaveChanges();

        var postService = new PostService(Context);
        var result = await postService.GetComments(pageIndex: 1, pageSize: 2, post1.Id);

        Assert.Equal(2, result.Items.Count);
        Assert.Equal("CommentContent1", result.Items[0].Content);
        Assert.Equal("CommentContent2", result.Items[1].Content);
    }

    [Fact]
    public async Task GetComments_ShouldReturnEmptyArray_WhenNoComments()
    {
        var user1 = CreateTestUser("TestUser1");
        var post1 = CreateTestPost(user1, [], "Title1", "Content1");
        Context.SaveChanges();

        var postService = new PostService(Context);
        var result = await postService.GetComments(pageIndex: 1, pageSize: 2, post1.Id);

        Assert.Empty(result.Items);
    }

    [Fact]
    public async Task UpdateLikeCount_ShouldUpdateLikeCountSuccessfully()
    {
        var user1 = CreateTestUser("TestUser1");
        var post1 = CreateTestPost(user1, [], "Title1", "Content1");
        Context.SaveChanges();

        Assert.Equal(0, post1.LikeCount); // Arrange Assert Act Assert 

        var postService = new PostService(Context);
        await postService.UpdateLikeCount(post1.Id, user1.Id);

        var updatedPost = Context.Posts.Find(post1.Id);
        Assert.Equal(1, updatedPost!.LikeCount);
    }

    [Fact]
    public async Task UpdateLikeCount_ShouldThrow_WhenPostNotFound()
    {
        var user1 = CreateTestUser("TestUser1");
        Context.SaveChanges();

        var postService = new PostService(Context);
        await Assert.ThrowsAsync<NotFoundException>(async () =>
        {
            await postService.UpdateLikeCount(999, user1.Id);
        });
    }

    [Fact]
    public async Task UpdateLikeCount_ShouldThrow_WhenUserNotFound()
    {
        var user1 = CreateTestUser("TestUser1");
        var post1 = CreateTestPost(user1, [], "Title1", "Content1");
        Context.SaveChanges();

        var postService = new PostService(Context);
        await Assert.ThrowsAsync<NotFoundException>(async () =>
        {
            await postService.UpdateLikeCount(post1.Id, 999);
        });
    }

    [Fact]
    public async Task UpdateLikeCount_ShouldNotIncrement_WhenUserAlreadyLiked()
    {
        var user1 = CreateTestUser("TestUser1");
        var post1 = CreateTestPost(user1, [], "Title1", "Content1");
        Context.SaveChanges();

        var postService = new PostService(Context);
        await postService.UpdateLikeCount(post1.Id, user1.Id); // First like
        var likeCountAfterFirst = Context.Posts.Find(post1.Id)!.LikeCount;

        await postService.UpdateLikeCount(post1.Id, user1.Id); // Try to like again
        var likeCountAfterSecond = Context.Posts.Find(post1.Id)!.LikeCount;

        Assert.Equal(likeCountAfterFirst, likeCountAfterSecond);
        Assert.Equal(1, likeCountAfterSecond);
    }

    [Fact]
    public async Task UpdateLikeCount_ShouldIncrement_WhenUserHasNotLiked()
    {
        var user1 = CreateTestUser("TestUser1");
        var post1 = CreateTestPost(user1, [], "Title1", "Content1");
        Context.SaveChanges();

        var postService = new PostService(Context);
        var result = await postService.UpdateLikeCount(post1.Id, user1.Id);

        Assert.Equal(1, result);
        var updatedPost = Context.Posts.Find(post1.Id);
        Assert.Equal(1, updatedPost!.LikeCount);
    }

    private User CreateTestUser(string username)
    {
        var user = new User
        {
            Username = username,
            Email = $"{username.ToLower()}@example.com",
            PasswordHash = "test123",
            Description = $"Desc for {username}"
        };

        Context.Users.Add(user);
        return user;
    }

    private Post CreateTestPost(User user, List<Comment> comments, string title, string content)
    {
        var post = new Post
        {
            User = user,
            UserId = user.Id,
            Title = title,
            Content = content,
            Comments = comments
        };

        Context.Posts.Add(post);
        return post;
    }

    private Comment CreateTestComment(User user, string content, int id = 1)
    {
        var comment = new Comment
        {
            Id = id,
            UserId = user.Id,
            User = user,
            Content = content
        };

        Context.Comments.Add(comment);
        return comment;
    }
}
