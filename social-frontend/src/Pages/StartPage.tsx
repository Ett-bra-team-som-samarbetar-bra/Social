import { useAuth } from "../Hooks/useAuth";
import { useEffect, useState } from "react";
import { useNavigate } from "react-router-dom";
import { type Post, type PostCreateDto } from "../Types/post";
import CreatePost from "../Components/CreatePostComponent";
import RootButton from "../Components/RootButton";
import PostComponent from "../Components/PostComponent";
import PostAlertMessage from "../Components/PostAlertMessage";

const apiUrl = import.meta.env.VITE_API_URL;

export default function StartPage() {
  const navigate = useNavigate();
  const { user, updateUser } = useAuth();
  const [activeTab, setActiveTab] = useState<"all" | "following" | "mine" | "create">(
    "all"
  );
  const [posts, setPosts] = useState<Post[]>([]);
  const [loading, setLoading] = useState(false);
  const [error, setError] = useState<string | null>(null);
  const [pageIndex] = useState<number>(1);
  const [pageSize] = useState<number>(10);

  const endpoints = {
    all: `${apiUrl}/api/post/all/${pageIndex}/${pageSize}`,
    following: `${apiUrl}/api/post/follower-posts/${pageIndex}/${pageSize}`,
    mine: `${apiUrl}/api/post/user-posts/${user?.id}/${pageIndex}/${pageSize}`,
    create: "",
  };

  // Fetch posts
  useEffect(() => {
    if (activeTab === "mine" && !user?.id) return;
    if (activeTab === "create") return;

    let ignore = false;

    async function fetchPosts() {
      setLoading(true);
      setError(null);

      const result = await fetch(endpoints[activeTab], {
        method: "GET",
        credentials: "include",
      });

      if (!result.ok) {
        if (!ignore) {
          try {
            const errorData = await result.json();
            setError(errorData.error || "Failed to load posts");
          } catch {
            setError("Failed to load posts");
          }
        }
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
  }, [activeTab, user?.id]);

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

    setActiveTab("all");
  }

  async function handleLike(id: number) {
    setPosts((prev) =>
      prev.map((post) =>
        post.id === id ? { ...post, likeCount: post.likeCount + 1 } : post
      )
    );

    const result = await fetch(`${apiUrl}/api/post/like/${id}`, {
      method: "PUT",
      credentials: "include",
    });

    if (!result.ok) {
      setError("Something went wrong.");

      // Rollback UI
      setPosts((prev) =>
        prev.map((post) =>
          post.id === id ? { ...post, likeCount: post.likeCount - 1 } : post
        )
      );

      return;
    }

    // Update UserDto likedPostIds
    updateUser((prev) => ({
      ...prev,
      likedPostIds: [...prev.likedPostIds, id],
    }));
  }

  async function handleComment(id: number) {
    navigate(`/post/${id}`);
  }

  const userHeading = user ? "[P]Post" : "[[▓]P▣ó̶st";

  return (
    <>
      <h5 className="text-primary mb-3 keybind-header">{userHeading}</h5>

      <div className="d-flex flex-column h-100">
        <div className="d-flex gap-1 justify-content-between">
          <div className="d-flex gap-1">
            <RootButton className="post-outline post-tab-fixed-size" onClick={() => setActiveTab("all")}>Global</RootButton>
            <RootButton className="post-outline post-tab-fixed-size" onClick={() => setActiveTab("following")}>Follow</RootButton>
            <RootButton className="post-outline post-tab-fixed-size" onClick={() => navigate(`/user/${user!.id}`)}>Profile</RootButton>
          </div>
          <RootButton className="post-outline post-tab-fixed-size" onClick={() => setActiveTab("create")}>Create</RootButton>
        </div>

        {activeTab === "create" ? (
          <div className="">
            <CreatePost
              onSubmit={handleSubmit}
              userId={user!.id}
              username={user!.username}
            />
          </div>
        ) : (
          <>
            {loading && (
              <PostAlertMessage
                message={"Loading..."}
                isErrorMessage={false} />
            )}
            {error &&
              <PostAlertMessage
                message={error}
                isErrorMessage={true} />
            }
            {!loading && !error &&
              <div className="d-flex flex-column overflow-y-auto gap-3 post-outline mb-4">
                {posts.map((post) => (
                  <PostComponent
                    id={post.userId}
                    key={post.id}
                    title={post.title}
                    content={post.content}
                    username={post.username}
                    userId={post.userId}
                    likes={post.likeCount}
                    commentCount={post.comments.length}
                    createdAt={post.createdAt}
                    onLike={() => handleLike(post.id)}
                    onComment={() => handleComment(post.id)}
                    hasLiked={user?.likedPostIds?.includes(post.id) ?? false}
                  />
                ))}
              </div>
            }
          </>
        )}
      </div>
    </>
  );
}
