import { useEffect, useState } from "react";
import { useParams } from "react-router-dom";
import PostComponent from "../Components/PostComponent";
import { type Post } from "../Types/post";
import { useAuth } from "../Hooks/useAuth";
import { usePostActions } from "../Hooks/usePostActions";
import CommentComponent from "../Components/CommentComponent";
import RootButton from "../Components/RootButton";

const apiUrl = import.meta.env.VITE_API_URL;

export default function CommentsPage() {
  const { user, updateUser } = useAuth();
  const { id } = useParams();
  const [post, setPost] = useState<Post | null>(null);
  const [error, setError] = useState<string | null>(null);
  const [loading, setLoading] = useState<boolean>();
  const [commentInput, setCommentInput] = useState("");
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

  async function submitComment() {
    if (!post) return;

    const newComment = {
      userId: user!.id,
      username: user!.username,
      content: commentInput,
      createdAt: new Date().toISOString(),
    };

    setPost({
      ...post,
      comments: [...post.comments, newComment],
    });

    setCommentInput("");

    const result = await fetch(`${apiUrl}/api/post/comments/${post.id}`, {
      method: "POST",
      credentials: "include",
      headers: { "Content-Type": "application/json" },
      body: JSON.stringify({ content: newComment.content }),
    });

    if (!result.ok) {
      setError("Failed to post comment.");

      setPost({
        ...post,
        comments: post.comments.filter((c) => c !== newComment),
      });

      return;
    }
  }
  return <div className="overflow-y-auto">
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
    <div className="mt-4">
      <h5>Add a Comment</h5>

      <textarea
        className="form-control mb-2 text-primary"
        rows={3}
        value={commentInput}
        onChange={(e) => setCommentInput(e.target.value)}
        placeholder="Write a comment..."
      />

      <RootButton
        keyLabel="Enter"
        className="btn btn-primary"
        onClick={() => submitComment()}
        disabled={!commentInput.trim()}
      >
        Post Comment
      </RootButton>
    </div>
    {post && post.comments.length > 0 && (
      <div className="mt-4">
        <h5>Comments</h5>
        {post.comments.map((comment) => (
          <CommentComponent key={`${comment.createdAt}-${comment.userId}`}
            comment={comment} />
        ))}
      </div>
    )}

    {post && post.comments.length === 0 && (
      <p className="text-muted mt-3">No comments yet. Be the first!</p>
    )}

    {loading && (
      <div className="text-center text-secondary mt-3">Loading...</div>
    )}
    {error && <div className="text-danger text-center mt-3">{error}</div>}
    <div className="d-flex flex-column gap-3"></div>

  </div>;
}
