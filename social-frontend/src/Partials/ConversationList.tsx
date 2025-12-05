import { Col } from "react-bootstrap";
import { useEffect, useRef, useState } from "react";
import { useNavigate } from "react-router-dom";
import { useSignalR } from "../Hooks/useSignalR";
import { useAuth } from "../Hooks/useAuth";
import { useHotKey } from "../Hooks/useHotKey";

export interface ConversationDto {
    userId: number;
    username: string;
    hasUnreadMessages: boolean;
    lastMessageAt: string;
}

export default function ConversationList() {
    const [conversations, setConversations] = useState<ConversationDto[]>([]);
    const [focused, setFocused] = useState(false);
    const [selectedIndex, setSelectedIndex] = useState<number | null>(null);
    const navigate = useNavigate();
    const { user } = useAuth();
    const listRef = useRef<HTMLDivElement>(null);

    const fetchConversations = async () => {
        const res = await fetch("http://localhost:5174/api/message/conversations", {
            credentials: "include"
        });
        if (!res.ok) return;
        const data = await res.json();
        setConversations(data);
    };

    useEffect(() => {
        if (!user) {
            setConversations([]);
            return;
        }
        fetchConversations();
    }, [user]);

    useSignalR(user?.id ?? 0, () => {
        fetchConversations();
    });

    useHotKey("M", () => {
        setFocused(true);
        setSelectedIndex(0);
        listRef.current?.focus();
    });

    useEffect(() => {
        if (!focused) return;

        const handleKeyDown = (e: KeyboardEvent) => {
            if (!conversations.length) return;

            if (e.key === "Escape") {
                e.preventDefault();
                setFocused(false);
                setSelectedIndex(null);
                listRef.current?.blur();
                return;
            }

            if (e.key === "ArrowDown") {
                e.preventDefault();
                setSelectedIndex(prev =>
                    prev === null ? 0 : Math.min(prev + 1, conversations.length - 1)
                );
            } else if (e.key === "ArrowUp") {
                e.preventDefault();
                setSelectedIndex(prev =>
                    prev === null ? 0 : Math.max(prev - 1, 0)
                );
            } else if (e.code === "Space" && selectedIndex !== null) {
                e.preventDefault();
                navigate(`/messages/${conversations[selectedIndex].userId}`);
            }
        };

        window.addEventListener("keydown", handleKeyDown);
        return () => window.removeEventListener("keydown", handleKeyDown);
    }, [focused, selectedIndex, conversations, navigate]);


    const handleMouseSelect = (index: number) => {
        setSelectedIndex(index);
    };

    return (
        <Col className="conversation-aside ">
            <h5 className="text-primary mb-3 text-uppercase">[M]Messages</h5>

            <div
                ref={listRef}
                tabIndex={0}
                className={`p-2 conversation-list ${focused ? "focused" : ""}`}
                onBlur={() => setFocused(false)}
            >
                <p className="text-primary">Hit [SPACE] to choose</p>
                <p className="text-primary">Hit [ESC] to escape</p>
                {conversations.map((c, i) => (
                    <div
                        key={c.userId}
                        className={`conversation-item ${selectedIndex === i ? "selected" : ""}`}
                        onClick={() => {
                            handleMouseSelect(i);
                            navigate(`/messages/${c.userId}`);
                        }}
                        onMouseEnter={() => handleMouseSelect(i)}
                    >
                        @{c.username}
                    </div>
                ))}
            </div>
        </Col>
    );
}
