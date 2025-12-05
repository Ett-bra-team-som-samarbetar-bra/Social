import RootButton from "../Components/RootButton";
import { useEffect, useState } from "react";
import { type Post, type PostCreateDto } from "../Types/post";
import { useAuth } from "../Hooks/useAuth";
import CreatePost from "../Components/CreatePostComponent";
import PostComponent from "../Components/PostComponent";

const apiUrl = import.meta.env.VITE_API_URL;

export default function StartPage() {
  const { user, updateUser } = useAuth();
  const [activeTab, setActiveTab] = useState<"all" | "following" | "mine">("all");
  const [posts, setPosts] = useState<Post[]>([]);
  const [loading, setLoading] = useState(false);
  const [error, setError] = useState<string | null>(null);
  const [pageIndex, setPageIndex] = useState<number>(1);
  const [pageSize, setPageSize] = useState<number>(10);

  const endpoints = {
    all: `${apiUrl}/api/post/all/${pageIndex}/${pageSize}`,
    following: `${apiUrl}/api/post/follower-posts/${pageIndex}/${pageSize}`,
    mine: `${apiUrl}/api/post/user-posts/${user?.id}/${pageIndex}/${pageSize}`,
  };

  // Fetch posts
  useEffect(() => {
    if (activeTab === "mine" && !user?.id) return;
    let ignore = false;

    async function fetchPosts() {
      setLoading(true);
      setError(null);

      const result = await fetch(endpoints[activeTab], {
        method: "GET",
        credentials: "include",
      });

      if (!result.ok) {
        if (!ignore) setError("Failed to load posts");
        setLoading(false);
        return;
      }

      const data = await result.json();
      if (!ignore) setPosts(data.items ?? []);

      setLoading(false);
    }

    fetchPosts();
    return () => {
      ignore = true;
    };
  }, [activeTab, user?.id]); // <- Add missing dependency so mine loads correctly

  // Handle creating posts
  async function handleSubmit(title: string, content: string) {
    const request: PostCreateDto = { title, content };
    const result = await fetch(`${apiUrl}/api/post`, {
      method: "POST",
      credentials: "include",
      headers: { "Content-Type": "application/json" },
      body: JSON.stringify(request),
    });

    if (!result.ok) {
      setError("Failed to create post");
      setLoading(false);
    }
  }

  // Handle liking posts
  async function handleLike(id: number) {
    // Optimistic LikeCount bump
    setPosts(prev =>
      prev.map(post =>
        post.id === id
          ? { ...post, likeCount: post.likeCount + 1 }
          : post
      )
    );

    const result = await fetch(`${apiUrl}/api/post/like/${id}`, {
      method: "PUT",
      credentials: "include",
    });

    if (!result.ok) {
      setError("Something went wrong.");

      // Rollback UI
      setPosts(prev =>
        prev.map(post =>
          post.id === id
            ? { ...post, likeCount: post.likeCount - 1 }
            : post
        )
      );

      return;
    }

    // Update UserDto likedPostIds
    updateUser(prev => ({
      ...prev,
      likedPostIds: [...prev.likedPostIds, id],
    }));
  }

  return (
    <div>
      <CreatePost onSubmit={handleSubmit} />

      <div className="tab-buttons d-flex gap-2">
        <RootButton onClick={() => setActiveTab("all")}>All</RootButton>
        <RootButton onClick={() => setActiveTab("following")}>Following</RootButton>
        <RootButton onClick={() => setActiveTab("mine")}>My Posts</RootButton>
      </div>

      {loading && <div className="text-center text-secondary mt-3">Loading...</div>}
      {error && <div className="text-danger text-center mt-3">{error}</div>}

      {!loading &&
        posts.map((post) => (
          <PostComponent
            key={post.id}
            title={post.title}
            content={post.content}
            username={post.username}
            userId={post.userId}
            likes={post.likeCount}
            commentCount={post.comments.length}
            createdAt={post.createdAt}
            onLike={() => handleLike(post.id)}
            onComment={() => console.log("Comment on:", post.id)}
            hasLiked={user?.likedPostIds?.includes(post.id) ?? false}
          />
        ))}
    </div>
  );
}
