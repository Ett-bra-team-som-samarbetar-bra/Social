import RootButton from "../Components/RootButton";

interface PostComponentProps {
  title: string;
  content: string;
  username: string;
  userId: number;
  onLike: () => void;
  onComment: () => void;
}

export default function PostComponent({
  title,
  content,
  username,
  onLike,
  onComment,
}: PostComponentProps) {
  return (
    <div className="post-box">
      <div className="post-header gap-3">
        <h2 className="post-title">@{username} -</h2>
        <h4 className="post-title">{title}</h4>
      </div>

      <div className="post-body">
        <pre className="post-content">{content}</pre>
      </div>

      <div className="post-actions d-flex gap-2 mt-3">
        <RootButton keyLabel="L" onClick={onLike} className="flex-grow-1">
          Like
        </RootButton>

        <RootButton keyLabel="C" onClick={onComment} className="flex-grow-1">
          Comment
        </RootButton>
      </div>
    </div>
  );
}
