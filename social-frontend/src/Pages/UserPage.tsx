import { useParams } from "react-router-dom";
import { useEffect, useState } from "react";
import UserProfileComponent from "../Components/UserProfileComponent";
import { type UserProfileDto } from "../Types/auth";
import { useAuth } from "../Hooks/useAuth";
import type { Post } from "../Types/post";
import PostComponent from "../Components/PostComponent";
const apiUrl = import.meta.env.VITE_API_URL;

export default function UserPage() {
  const { user, updateUser } = useAuth();
  const { id } = useParams();
  const [userData, setUserData] = useState<UserProfileDto | null>(null);
  const [posts, setPosts] = useState<Post[]>([]);
  const [loading, setLoading] = useState(false);
  const [error, setError] = useState<string | null>(null);
  const [pageIndex, setPageIndex] = useState<number>(1);
  const [pageSize, setPageSize] = useState<number>(10);

  useEffect(() => {
    async function loadUser() {
      const result = await fetch(`${apiUrl}/api/user/id/${id}`, {
        credentials: "include",
      });

      if (!result.ok) return; // Add error handling later

      setUserData(await result.json());
    }

    async function loadPosts() {
      const result = await fetch(
        `${apiUrl}/api/post/user-posts/${id}/${pageIndex}/${pageSize}`,
        {
          method: "GET",
          credentials: "include",
        }
      );

      if (!result.ok) {
        setError("Failed to load posts");
        setLoading(false);
        return;
      }
      const postData = await result.json();

      setPosts(postData.items);
    }

    loadUser();
    loadPosts();
  }, [id]);

  if (!userData) return <div>Loading...</div>;

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

  async function handleFollowToggle() {
    if (!userData) return;
    if (userData.isFollowing) {
      await fetch(`${apiUrl}/api/user/unfollow`, {
        method: "PUT",
        credentials: "include",
        headers: { "Content-Type": "application/json" },
        body: JSON.stringify({ userId: id }),
      });
      setUserData(
        (prev) =>
          prev && {
            ...prev,
            isFollowing: false,
            followerCount: prev.followerCount - 1,
          }
      );
    } else {
      await fetch(`${apiUrl}/api/user/follow`, {
        method: "PUT",
        credentials: "include",
        headers: { "Content-Type": "application/json" },
        body: JSON.stringify({ userId: id }),
      });
      setUserData(
        (prev) =>
          prev && {
            ...prev,
            isFollowing: true,
            followerCount: prev.followerCount + 1,
          }
      );
    }
  }

  return (
    <div>
      <UserProfileComponent
        username={userData.username}
        description={userData.description}
        createdAt={userData.createdAt}
        postCount={userData.postCount}
        followerCount={userData.followerCount}
        followingCount={userData.followingCount}
        isFollowing={userData.isFollowing}
        isOwnProfile={userData.isOwnProfile}
        onFollowToggle={handleFollowToggle}
      />

      {!loading &&
        posts.map((post) => (
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
            onComment={() => console.log("Comment on:", post.id)}
            hasLiked={user?.likedPostIds?.includes(post.id) ?? false}
          />
        ))}
      {loading && (
        <div className="text-center text-secondary mt-3">Loading...</div>
      )}
      {error && <div className="text-danger text-center mt-3">{error}</div>}
    </div>
  );
}
