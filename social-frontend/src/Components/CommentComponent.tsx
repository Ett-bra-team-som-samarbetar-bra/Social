import { useNavigate } from "react-router-dom";
import { type Comment } from "../Types/post";

interface Props {
  comment: Comment;
}

export default function CommentComponent({ comment }: Props) {
  const date = new Date(comment.createdAt);
  const navigate = useNavigate();
  const dateFormatted = date.toLocaleString().slice(0, -3);

  return (
    <div className="post-box">
      <div className="d-flex justify-content-between align-items-center ms-1 mb-2">
        <h2 className="post-title clickable"
        onClick={() => navigate(`/user/${comment.userId}`)}>
          @{comment.username}
        </h2>
        <div className="d-flex align-items-center gap-1 post-info">
          <i className="bi bi-clock-fill"></i> {dateFormatted}
        </div>
      </div>

      <div className="post-body mt-3 ms-1">
        <pre className="post-content">{comment.content}</pre>
      </div>
    </div>
  );
}