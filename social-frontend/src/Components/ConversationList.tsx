import { Col } from "react-bootstrap";
import { useEffect, useState } from "react";
import { useNavigate } from "react-router-dom";

export interface ConversationDto {
    userId : number;
    username: string;
    hasUnreadMessages: boolean;
    lastMessageAt: string;
}

export default function ConversationList() {
    const [conversations, setConversations] = useState<ConversationDto[]>([]);

    const navigate = useNavigate();

    const fetchConversations = async () => {
        const res = await fetch("http://localhost:5174/api/message/conversations", {
            credentials: "include"
        });

        if (!res.ok) return;
        const data = await res.json();
        setConversations(data);
    };

    useEffect(() => {
        fetchConversations();
    }, []);

    // TODO: Set up SignalR listener 


    return (
        <Col className="conversation-aside ps-3">
            <h5 className="text-primary mb-3">Messages</h5>

            <div className="conversation-list">
                {conversations.map(c => (
                    <div
                        key={c.userId }
                        className="conversation-item"
                        onClick={() => navigate(`/messages/${c.userId }`)}

                    >
                        @{c.username}
                    </div>
                ))}
            </div>
        </Col>
    );
}
