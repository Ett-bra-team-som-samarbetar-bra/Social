import { useEffect, useRef, useState } from "react";
import { useParams } from "react-router-dom";
import PostComponent from "../Components/PostComponent";
import { type Post } from "../Types/post";
import { useAuth } from "../Hooks/useAuth";
import { usePostActions } from "../Hooks/usePostActions";
import CommentComponent from "../Components/CommentComponent";
import RootButton from "../Components/RootButton";
import PostAlertMessage from "../Components/PostAlertMessage";
import { usePagination } from "../Hooks/usePagination";

const apiUrl = import.meta.env.VITE_API_URL;

export default function CommentsPage() {
  const { user, updateUser } = useAuth();
  const { id } = useParams();
  const messagesContainerRef = useRef<HTMLDivElement>(null);
  const messageEndRef = useRef<HTMLDivElement>(null);
  const [post, setPost] = useState<Post | null>(null);
  const [error, setError] = useState<string | null>(null);
  const [loadingPost, setLoadingPost] = useState<boolean>(false);
  const [commentInput, setCommentInput] = useState("");
  const { likePost, openPost } = usePostActions({
    apiUrl,
    post,
    setPost,
    updateUser,
    setError,
  });

  const [pageSize] = useState<number>(10);

  const fetchCommentsPage = async (page: number, size: number) => {
    if (!id) return [] as any[];

    const endpoint = `${apiUrl}/api/post/comments/${id}/${page}/${size}`;
    const res = await fetch(endpoint, {
      method: "GET",
      credentials: "include",
    });
    if (!res.ok) {
      try {
        const err = await res.json();
        throw new Error(err.error || "Failed to load comments");
      } catch {
        throw new Error("Failed to load comments");
      }
    }

    const data = await res.json();
    if (Array.isArray(data)) return data;
    return (data.items ?? []) as any[];
  };

  const {
    items: commentsItems,
    loading: loadingComments,
    error: commentsError,
    hasMore,
    loadMore,
  } = usePagination<any>(fetchCommentsPage, pageSize, [id]);

  useEffect(() => {
    if (!post) return;
    setPost((p) => (p ? { ...p, comments: commentsItems } : p));
  }, [commentsItems]);

  useEffect(() => {
    async function loadPost() {
      setLoadingPost(true);
      const result = await fetch(`${apiUrl}/api/post/user-posts/${id}/`, {
        method: "GET",
        credentials: "include",
      });

      if (!result.ok) {
        setError("Failed to load posts");
        setLoadingPost(false);
        return;
      }
      const postData = await result.json();

      setPost(postData);
      setLoadingPost(false);
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
          behavior: "smooth",
        });
      }
    });
  };

  return (
    <>
      <div className="overflow-y-auto">
        <RootButton className="post-outline non-interactive post-tab-fixed-size">
          Post
        </RootButton>
        {(loadingPost || loadingComments) && (
          <PostAlertMessage message={"Loading..."} isErrorMessage={false} />
        )}
        {(error || commentsError) && (
          <PostAlertMessage
            message={error ?? commentsError ?? ""}
            isErrorMessage={true}
          />
        )}
        {!(loadingPost || loadingComments) &&
          !(error || commentsError) &&
          post && (
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
          )}

        <div className="d-flex flex-column" style={{ maxHeight: "100%" }}>
          <RootButton className="post-outline non-interactive post-tab-fixed-size">
            Comments
          </RootButton>

          <div
            className="d-flex flex-column overflow-y-auto gap-3 post-outline mb-4 flex-grow-1"
            ref={messagesContainerRef}
          >
            {post && (
              <>
                {post.comments.length > 0 ? (
                  post.comments.map((comment) => (
                    <CommentComponent
                      key={`${comment.createdAt}-${comment.userId}`}
                      comment={comment}
                    />
                  ))
                ) : (
                  <div className="text-center text-muted">No comments yet.</div>
                )}

                <div ref={messageEndRef} />
              </>
            )}

            <div className="sticky-bottom bg-body">
              <div className="d-flex align-items-stretch">
                <input
                  className="create-post-input flex-grow-1"
                  placeholder="..."
                  maxLength={100}
                  value={commentInput}
                  onChange={(e) => setCommentInput(e.target.value)}
                  style={{ height: "auto" }}
                />
                <RootButton
                  keyLabel="Enter"
                  onClick={() => submitComment()}
                  disabled={!commentInput.trim()}
                  className=""
                ></RootButton>
              </div>
            </div>
          </div>
        </div>
      </div>
      {/* Load more comments button (directly below the comment list) */}
      <div className="d-flex justify-content-center my-2">
        <RootButton
          className="btn btn-outline-primary"
          onClick={() => loadMore()}
          disabled={!hasMore || loadingComments}
        >
          {loadingComments
            ? "Loading..."
            : hasMore
            ? "Load more comments"
            : "No more comments"}
        </RootButton>
      </div>
    </>
  );
}
