import { type Comment } from "../Types/post";

interface Props {
    comment: Comment;
}

export default function CommentComponent({ comment }: Props) {
    return (
        <div className="border rounded p-3 mb-2 bg-light">
            <strong>{comment.username}</strong>
            <p className="mb-1">{comment.content}</p>
            <small className="text-muted">
                {new Date(comment.createdAt).toLocaleString()}
            </small>
        </div>
    );
}