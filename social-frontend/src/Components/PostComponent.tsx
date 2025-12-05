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
}: PostComponentProps) {
  const navigate = useNavigate();
  const date = new Date(createdAt);
  const dateFormatted = date.toLocaleString();
  return (
    <div className="post-box">
      <div className="post-header gap-3 ">
        <h2
          onClick={() => navigate(`/user/${id}`)}
          className="post-title clickable"
        >
          @{username}
        </h2>
        <h4 className="post-title">{title}</h4>
      </div>

      <div className="post-body">
        <pre className="post-content">{content}</pre>
      </div>

      <div className="d-flex align-items-center justify-content-end gap-4 mt-2 post-info">
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

      <div className="post-actions d-flex gap-2 mt-3">
        <RootButton
          keyLabel="L"
          onClick={onLike}
          disabled={hasLiked}
          className="flex-grow-1"
        >
          {hasLiked ? "Liked ❤︎" : "Like"}
        </RootButton>

        <RootButton keyLabel="C" onClick={onComment} className="flex-grow-1">
          Comment
        </RootButton>
      </div>
    </div>
  );
}
