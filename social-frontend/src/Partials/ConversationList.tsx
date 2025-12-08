import { Col } from "react-bootstrap";
import { useEffect, useRef, useState } from "react";
import { useLocation, useNavigate } from "react-router-dom";
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
    const location = useLocation();
    const activeChatId = Number(location.pathname.split("/").pop());

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

    useEffect(() => {
        if (user && activeChatId) {
            const timer = setTimeout(() => {
                fetchConversations();
            }, 300);
            return () => clearTimeout(timer);
        }
    }, [activeChatId, user]);

    useSignalR(user?.id ?? 0, (msg) => {
        const isActiveChat = msg.sendingUserId === activeChatId || msg.receivingUserId === activeChatId;

        if (!isActiveChat) {
            fetchConversations();
        }
    });

    useHotKey("M", () => {
        if (!user) return;

        setFocused(true);
        listRef.current?.focus();
    });

    useEffect(() => {
        if (!focused) return;

        const handleKeyDown = (e: KeyboardEvent) => {
            if (!conversations.length) return;

            if (e.key === "Escape") {
                e.preventDefault();
                setFocused(false);
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

    const messageHeading = user ? "[M]Messages" : "[░▒▓]Mess■ges̴͊";

    return (
        <Col className="conversation-aside ">
            <h5 className="text-primary mb-3 keybind-header">{messageHeading}</h5>
            <div
                ref={listRef}
                tabIndex={0}
                className={`conversation-list ${focused ? "focused" : ""}`}
                onBlur={() => setFocused(false)}
            >
                {user && <>
                    <p className="text-primary my-0">[SPACE] to choose</p>
                    <p className="text-primary">[ESC] to escape</p>
                </>}
                {conversations.map((c, i) => (
                    <div
                        key={c.userId}
                        className={`conversation-item ${selectedIndex === i ? "selected" : ""}`}
                        onClick={() => {
                            setSelectedIndex(i);
                            navigate(`/messages/${c.userId}`);
                        }}
                    >
                        {selectedIndex === i ? "> " : "  "}
                        @{c.username} {c.hasUnreadMessages && <span className="text-primary">⬤</span>}
                    </div>
                ))}
            </div>
        </Col >
    );
}
