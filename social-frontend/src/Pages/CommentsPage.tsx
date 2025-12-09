import { useEffect, useRef, useState } from "react";
import { useParams } from "react-router-dom";
import PostComponent from "../Components/PostComponent";
import { type Post } from "../Types/post";
import { useAuth } from "../Hooks/useAuth";
import { usePostActions } from "../Hooks/usePostActions";
import CommentComponent from "../Components/CommentComponent";
import RootButton from "../Components/RootButton";
import PostAlertMessage from "../Components/PostAlertMessage";

const apiUrl = import.meta.env.VITE_API_URL;

export default function CommentsPage() {
  const { user, updateUser } = useAuth();
  const { id } = useParams();
  const messagesContainerRef = useRef<HTMLDivElement>(null);
  const messageEndRef = useRef<HTMLDivElement>(null);
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

    scrollToBottom();
  }

  const scrollToBottom = () => {
    requestAnimationFrame(() => {
      if (messagesContainerRef.current) {
        messagesContainerRef.current.scrollTo({
          top: messagesContainerRef.current.scrollHeight + 100,
          behavior: "smooth"
        });
      }
    });
  };

  return (
    <>
      <div className="overflow-y-auto">
        <RootButton className="post-outline non-interactive post-tab-fixed-size">Post</RootButton>
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
        {!loading && !error && post &&
          <div className="d-flex flex-column overflow-y-auto gap-3 post-outline mb-4">
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
              onLike={() => likePost(post.id)}
              onComment={() => openPost(post.id)}
              hasLiked={user?.likedPostIds?.includes(post.id) ?? false}
              hideButtons={true}
            />
          </div>
        }

        <div
          className="d-flex flex-column"
          style={{ maxHeight: '100%' }}>
          <RootButton className="post-outline non-interactive post-tab-fixed-size">Comments</RootButton>

          <div
            className="d-flex flex-column overflow-y-auto gap-3 post-outline mb-4 flex-grow-1"
            ref={messagesContainerRef}>

            {post && post.comments.length > 0 && (
              post.comments.map((comment) => (
                <CommentComponent key={`${comment.createdAt}-${comment.userId}`}
                  comment={comment} />
              ))
            )}
            <div ref={messageEndRef} />

            <div className="sticky-bottom bg-body">
              <div className="d-flex align-items-stretch">
                <input
                  className="create-post-input flex-grow-1"
                  placeholder="..."
                  maxLength={100}
                  value={commentInput}
                  onChange={(e) => setCommentInput(e.target.value)}
                  style={{ height: 'auto' }}
                />
                <RootButton
                  keyLabel="Enter"
                  onClick={() => submitComment()}
                  disabled={!commentInput.trim()}
                  className=""
                >
                </RootButton>
              </div>
            </div>
          </div>
        </div>
      </div>
    </>)
}
