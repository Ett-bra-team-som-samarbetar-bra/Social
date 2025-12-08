import { useEffect, useState } from "react";
import { useParams } from "react-router-dom";
import PostComponent from "../Components/PostComponent";
import { type Post } from "../Types/post";
import { useAuth } from "../Hooks/useAuth";
import { usePostActions } from "../Hooks/usePostActions";

const apiUrl = import.meta.env.VITE_API_URL;

export default function CommentsPage() {
  const { user, updateUser } = useAuth();
  const { id } = useParams();
  const [post, setPost] = useState<Post | null>(null);
  const [error, setError] = useState<string | null>(null);
  const [loading, setLoading] = useState<boolean>();
  const { likePost, openPost } = usePostActions({
    apiUrl,
    post,
    setPost,
    updateUser,
    setError,
  })

  useEffect(() => {
    async function loadPost() {
      const result = await fetch(`${apiUrl}/api/post/user-posts/${id}/`, {
        method: "GET",
        credentials: "include",
      });

      if (!result.ok) {
        setError("Failed to load posts");
        setLoading(false);
        return;
      }
      const postData = await result.json();

      setPost(postData);
    }
    loadPost();
  }, [id]);
  return <div>
    {post && <PostComponent
      id={post.userId}
      key={post.id}
      title={post.title}
      content={post.content}
      username={post.username}
      userId={post.userId}
      likes={post.likeCount}
      commentCount={post.comments.length}
      createdAt={post.createdAt}
      onLike={() => likePost(post.id)}
      onComment={() => openPost(post.id)}
      hasLiked={user?.likedPostIds?.includes(post.id) ?? false}
    />
    }
  </div>;
}
