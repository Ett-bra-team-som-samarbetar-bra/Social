using SocialBackend.tests.Data;
using SocialBackend.Models;
using SocialBackend.Services;
using Xunit.Sdk;
using Xunit.Abstractions;
using SocialBackend.Dto;
using SocialBackend.Exceptions;

namespace SocialBackend.tests.Services;

public class PostServiceTestOrderer : ITestCaseOrderer
{
    public IEnumerable<TTestCase> OrderTestCases<TTestCase>(IEnumerable<TTestCase> testCases)
        where TTestCase : ITestCase
    {
        return testCases.OrderBy(tc => tc.TestMethod.Method.Name);
    }
}

[TestCaseOrderer("social_backend.tests.PostServiceTestOrderer", "social-backend.tests")]
public class PostServiceSequentialTests : TestBase
{
    private readonly PostService _postService = null!;

    public PostServiceSequentialTests() : base()
    {
        _postService = new PostService(Context, CreateLogger<PostService>());
    }

    protected override void SeedData()
    {
        var user = new User { Id = 1, Username = "testuser", Email = "test@example.com", PasswordHash = "test", Description = "test" };
        var post1 = new Post { User = user, UserId = 1, Title = "First", Content = "First post" };
        var post2 = new Post { User = user, UserId = 1, Title = "Second", Content = "Second post" };

        Context.Users.Add(user);
        Context.Posts.AddRange(post1, post2);
        Context.SaveChanges();
    }

    [Fact]
    public async Task Test01_GetPosts_ShouldReturnDtoPosts_TwoPages()
    {
        // arrange
        int pageSize = 1;

        // act
        var firstPage = await _postService.GetPosts(1, pageSize);
        var secondPage = await _postService.GetPosts(2, pageSize);

        // assert
        Assert.NotNull(firstPage);
        Assert.Single(firstPage.Items);
        Assert.Equal("Second", firstPage.Items[0].Title);
        Assert.NotNull(secondPage);
        Assert.Single(secondPage.Items);
        Assert.Equal("First", secondPage.Items[0].Title);
    }

    [Fact]
    public async Task Test02_GetPosts_ShouldReturnDtoPosts_OnePage()
    {
        // arrange
        int pageSize = 2;

        // act
        var page = await _postService.GetPosts(1, pageSize);

        // assert
        Assert.NotNull(page);
        Assert.Equal(2, page.Items.Count);
        Assert.Contains(page.Items, p => p.Title == "First");
        Assert.Contains(page.Items, p => p.Title == "Second");
    }

    [Fact]
    public async Task Test03_GetUserPosts_ShouldReturnUserPostsDto()
    {
        // arrange
        var user = await Context.Users.FindAsync(1);
        var userId = user!.Id;
        int pageSize = 2;

        // act
        var page = await _postService.GetUserPosts(1, pageSize, userId);

        // assert
        Assert.NotNull(page);
        Assert.Equal(2, page.Items.Count);
        Assert.Contains(page.Items, p => p.Title == "First");
        Assert.Contains(page.Items, p => p.Title == "Second");
    }

    [Fact]
    public async Task Test04_GetUserPosts_ShouldThrow_WhenNoUser()
    {
        // arrange
        var user = await Context.Users.FindAsync(2);
        var userId = 1337;
        int pageSize = 2;

        // act & assert
        await Assert.ThrowsAsync<NotFoundException>(async () =>
        {
            await _postService.GetUserPosts(1, pageSize, userId);
        });
    }

    [Fact]
    public async Task Test05_GetFollowingPosts_ShouldThrow_WhenNoUser()
    {
        // arrange
        var user = await Context.Users.FindAsync(1337);
        var userId = 1337;
        int pageSize = 2;

        // act & assert
        await Assert.ThrowsAsync<NotFoundException>(async () =>
        {
            await _postService.GetFollowingPosts(1, pageSize, userId);
        });
    }

    [Fact]
    public async Task Test06_CreatePost_ShouldCreatePost()
    {
        // arrange
        var user = await Context.Users.FindAsync(1);
        var userId = user!.Id;

        var dto = new PostCreateDto
        {
            Title = "New Post",
            Content = "This is a new post."
        };

        // act
        var postId = await _postService.CreatePost(dto, userId);

        // assert
        var createdPost = await Context.Posts.FindAsync(postId);
        Assert.NotNull(createdPost);
        Assert.Equal("New Post", createdPost!.Title);
        Assert.Equal("This is a new post.", createdPost.Content);
        Assert.Equal(1, createdPost.UserId);
    }

    [Fact]
    public async Task Test07_GetPosts_ShouldReturnDtoPosts_ThreePages_ThreePosts()
    {
        // arrange
        var user = await Context.Users.FindAsync(1);
        var userId = user!.Id;
        var dto = new PostCreateDto
        {
            Title = "New Post",
            Content = "This is a new post."
        };
        await _postService.CreatePost(dto, userId);

        int pageSize = 1;

        // act
        var firstPage = await _postService.GetPosts(1, pageSize);
        var secondPage = await _postService.GetPosts(2, pageSize);
        var thirdPage = await _postService.GetPosts(3, pageSize);

        // assert
        Assert.NotNull(firstPage);
        Assert.Single(firstPage.Items);
        Assert.Equal("New Post", firstPage.Items[0].Title);

        Assert.NotNull(secondPage);
        Assert.Single(secondPage.Items);
        Assert.Equal("Second", secondPage.Items[0].Title);

        Assert.NotNull(thirdPage);
        Assert.Single(thirdPage.Items);
        Assert.Equal("First", thirdPage.Items[0].Title);
    }
}
