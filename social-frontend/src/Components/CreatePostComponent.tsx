import { useState } from "react";
import RootButton from "../Components/RootButton";

interface CreatePostProps {
  onSubmit: (title: string, content: string) => Promise<void>;
}

export default function CreatePost({ onSubmit }: CreatePostProps) {
  const [title, setTitle] = useState("");
  const [content, setContent] = useState("");

  const contentLimit = 300;

  function handleSubmit(e: React.FormEvent) {
    e.preventDefault();

    if (!title.trim() || !content.trim()) return;

    onSubmit(title, content);
    setTitle("");
    setContent("");
  }

  return (
    <div className="create-post-box">
      <form onSubmit={handleSubmit} className="create-post-form">
        {/* Title Input */}
        <input
          className="create-post-input create-post-title"
          placeholder="Post title..."
          maxLength={100}
          value={title}
          onChange={(e) => setTitle(e.target.value)}
        />

        {/* Content Textarea */}
        <textarea
          className="create-post-input create-post-content"
          placeholder="Write your post..."
          value={content}
          onChange={(e) => setContent(e.target.value.slice(0, contentLimit))}
          rows={4}
        />

        {/* Character Counter */}
        <div className="content-counter">
          {content.length}/{contentLimit}
        </div>

        <RootButton keyLabel="S" type="submit">
          Create Post
        </RootButton>
      </form>
    </div>
  );
}
