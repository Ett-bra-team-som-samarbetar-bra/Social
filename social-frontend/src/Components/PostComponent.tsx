import RootButton from "../Components/RootButton";
import { useNavigate } from "react-router-dom";

interface PostComponentProps {
  id: number;
  title: string;
  content: string;
  username: string;
  userId: number;
  likes: number;
  commentCount: number;
  createdAt: Date;
  onLike: () => void;
  onComment: () => void;
  hideButtons?: boolean;
  hasLiked: boolean;
}

export default function PostComponent({
  id,
  title,
  content,
  username,
  likes,
  commentCount,
  createdAt,
  onLike,
  onComment,
  hasLiked,
  hideButtons = false,
}: PostComponentProps) {
  const navigate = useNavigate();
  const date = new Date(createdAt);
  const dateFormatted = date.toLocaleString().slice(0, -3);

  return (
    <div className="post-box">
      <h2 className="post-title clickable ms-1 mb-2"
        onClick={() => navigate(`/user/${id}`)}>
        @{username}
      </h2>

      <div className="post-header gap-3 d-flex justify-content-between align-items-center">
        <div className="d-flex gap-3 align-items-center">
          <h4 className="post-title">[{title}]</h4>
        </div>
      </div>

      <div className="post-body mt-3 ms-1">
        <pre className="post-content">{content}</pre>
      </div>

      <div className="post-actions d-flex justify-content-between align-items-center mt-1 ms-1">
        <div className="d-flex align-items-center gap-3 post-info">
          <div className="d-flex align-items-center gap-1">
            <i className="bi bi-heart-fill"></i> {likes}
          </div>

          <div className="d-flex align-items-center gap-1">
            <i className="bi bi-chat-left-text-fill"></i> {commentCount}
          </div>

          <div className="d-flex align-items-center gap-1">
            <i className="bi bi-clock-fill"></i> {dateFormatted}
          </div>
        </div>

        <div className="d-flex gap-2">
          <RootButton
            keyLabel="L"
            fontsize={13}
            onClick={onLike}
            disabled={hasLiked}
            className={`post-button-fixed-size ${hideButtons ? 'invisible' : ''}`}
          >
            {hasLiked ? "Liked" : "Like"}
          </RootButton>

          <RootButton
            keyLabel="C"
            onClick={onComment}
            fontsize={13}
            className={`post-button-fixed-size ${hideButtons ? 'invisible' : ''}`}
          >
            Comment
          </RootButton>
        </div>
      </div>
    </div >
  );
}
