import { useEffect, useState } from "react";
import { useParams } from "react-router-dom";

const apiUrl = import.meta.env.VITE_API_URL;

export default function CommentsPage() {
  const { id } = useParams();
  const [post, setPost] = useState();
  const [error, setError] = useState();
  const [loading, setLoading] = useState();

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

      setPosts(postData.items);
    }
    loadPost();
  }, [id]);
  return <></>;
}
