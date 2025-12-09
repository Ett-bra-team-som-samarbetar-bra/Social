import { useState } from "react";
import RootButton from "../Components/RootButton";
import { useNavigate } from "react-router-dom";

interface CreatePostProps {
  onSubmit: (title: string, content: string) => Promise<void>;
  userId: number;
  username: string;
}

export default function CreatePost({ onSubmit, userId, username }: CreatePostProps) {
  const [title, setTitle] = useState("");
  const [content, setContent] = useState("");
  const navigate = useNavigate();
  const contentLimit = 300;

  function handleSubmit(e: React.FormEvent) {
    e.preventDefault();

    if (!title.trim() || !content.trim()) return;

    const trimmedTitle = title.trim();
    const trimmedContent = content.trim();

    const capitalizedTitle = trimmedTitle.charAt(0).toUpperCase() + trimmedTitle.slice(1);
    const capitalizedContent = trimmedContent.charAt(0).toUpperCase() + trimmedContent.slice(1);

    onSubmit(capitalizedTitle, capitalizedContent);
    setTitle("");
    setContent("");
  }

  return (
    <div className="create-post-box">
      <h2 className="post-title clickable ms-1 mb-2"
        onClick={() => navigate(`/user/${userId}`)}>
        @{username}
      </h2>

      <div className="post-header gap-3 d-flex justify-content-between align-items-center">
        <div className="d-flex gap-3 align-items-center">
          <h4 className="post-title glitch">[{title || "Title"}]</h4>
        </div>
      </div>

      <form onSubmit={handleSubmit} className="create-post-form">

        {/* Title Input */}
        <input
          className="create-post-input post-body mt-2"
          placeholder="Title"
          maxLength={30}
          value={title}
          onChange={(e) => setTitle(e.target.value)}
        />

        {/* Content Textarea */}
        <textarea
          className="create-post-input create-post-content"
          placeholder="Writeḻ̸̨̳̰̩̅a̶̻͈̩͗͗̏͘͝ͅl̶̘̟̙̥̹̽͋͊̅̕d̸̥͌͋̓k̷̻͑͐..."
          value={content}
          onChange={(e) => setContent(e.target.value.slice(0, contentLimit))}
          rows={4}
        />

        {/* Character Counter and Button */}
        <div className="d-flex justify-content-between align-items-center">
          <div className="content-counter">
            {content.length}/{contentLimit}
          </div>

          <RootButton keyLabel="S" type="submit" fontsize={13}
            className="post-button-fixed-size">
            Create
          </RootButton>
        </div>
      </form>
    </div>
  );
}
