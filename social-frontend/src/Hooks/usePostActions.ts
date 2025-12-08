import { useNavigate } from "react-router-dom";
import type { Post } from "../Types/post";

interface UsePostActionsProps {
    apiUrl: string;
    post?: Post | null;
    setPost?: React.Dispatch<React.SetStateAction<Post | null>>
    posts?: Post[];
    setPosts?: React.Dispatch<React.SetStateAction<Post[]>>;
    updateUser: (fn: (prev: any) => any) => void;
    setError: (mesg: string) => void;
}

export function usePostActions({
    apiUrl,
    post,
    setPost,
    posts,
    setPosts,
    updateUser,
    setError,
}: UsePostActionsProps) {
    const navigate = useNavigate();

    async function likePost(id: number) {
        if (post && setPost) {
            setPost({ ...post, likeCount: post.likeCount + 1 });
        }

        if (posts && setPosts) {


            setPosts((prev) =>
                prev.map((post) =>
                    post.id === id ? { ...post, likeCount: post.likeCount + 1 } : post
                )
            );
        }

        const result = await fetch(`${apiUrl}/api/post/like/${id}`, {
            method: "PUT",
            credentials: "include",
        });

        if (!result.ok) {
            setError("Something went wrong");

            if (post && setPost) {
                setPost({ ...post, likeCount: post.likeCount });
            }

            // rollback list
            if (posts && setPosts) {
                setPosts(prev =>
                    prev.map(p => p.id === id ? { ...p, likeCount: p.likeCount - 1 } : p)
                );
            }
            return;
        }

        updateUser((prev) => ({
            ...prev,
            likedPostId: [...prev.likedPostIds, id],
        }));
    }

    function openPost(id: number) {
        navigate(`/post/${id}`);
    }

    return {
        likePost,
        openPost,
    }
}