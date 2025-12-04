import RootButton from "../Components/RootButton";
import { useEffect, useState } from "react";
import { type Post, type PostCreateDto } from "../Types/post";
import { useAuth } from "../Hooks/useAuth";
import CreatePost from "../Components/CreatePostComponent";
import PostComponent from "../Components/PostComponent";

const apiUrl = import.meta.env.VITE_API_URL;

export default function StartPage() {
  const { user } = useAuth();
  const [activeTab, setActiveTab] = useState<"all" | "following" | "mine">(
    "all"
  );
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
  }, [activeTab]);

  async function handleSubmit(title: string, content: string) {
    const request: PostCreateDto = { title: title, content: content };
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
  return (
    <div>
      <CreatePost onSubmit={handleSubmit} />
      <div className="tab-buttons d-flex gap-2">
        <RootButton onClick={() => setActiveTab("all")}>All</RootButton>
        <RootButton onClick={() => setActiveTab("following")}>
          Following
        </RootButton>
        <RootButton onClick={() => setActiveTab("mine")}>My Posts</RootButton>
      </div>

      {/* Loading */}
      {loading && (
        <div className="text-center text-secondary mt-3">Loading...</div>
      )}

      {/* Error */}
      {error && <div className="text-danger text-center mt-3">{error}</div>}
      {/* Posts */}
      {!loading &&
        posts.map((post) => (
          <PostComponent
            key={post.id}
            title={post.title}
            content={post.content}
            username={post.username}
            userId={post.userId}
            onLike={() => console.log("Liked:", post.id)}
            onComment={() => console.log("Comment on:", post.id)}
          />
        ))}
    </div>
  );
}
