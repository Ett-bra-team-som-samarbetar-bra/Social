import { useEffect, useRef, useState, useCallback } from "react";
import { Container, Row, Col, Form } from "react-bootstrap";
import RootButton from "../Components/RootButton";
import { useHotKey } from "../Hooks/useHotKey";
import { useSignalR } from "../Hooks/useSignalR";
import { useAuth } from "../Hooks/useAuth";
import { useParams } from "react-router-dom";
import type MessageDto from "../Types/message";

export default function MessagePage() {
    const { user, loading } = useAuth();
    const { id } = useParams();
    const messagesContainerRef = useRef<HTMLDivElement>(null);
    const messageEndRef = useRef<HTMLDivElement>(null);
    const inputRef = useRef<HTMLInputElement>(null);
    const [loadingOlder, setLoadingOlder] = useState(false);

    const [messages, setMessages] = useState<MessageDto[]>([]);

    const currentUserId = user?.id ?? 0;
    const receivingUserId = Number(id);

    const fetchMessages = async (before?: string) => {
        const url = new URL(`http://localhost:5174/api/message/${receivingUserId}`);
        if (before) url.searchParams.append("before", before);

        const res = await fetch(url.toString(), { credentials: "include" });
        const data: MessageDto[] = await res.json();

        if (before) {
            setMessages(prev => [...data, ...prev]);
        } else {
            setMessages(Array.isArray(data) ? data : []);
            scrollToBottom();
        }
    };

    useEffect(() => {
        if (!id) return;

        fetchMessages();
    }, [id, receivingUserId]);

    const handleReceiveMessage = useCallback((message: MessageDto) => {
        const isRelevant =
            (message.sendingUserId === currentUserId && message.receivingUserId === receivingUserId) ||
            (message.sendingUserId === receivingUserId && message.receivingUserId === currentUserId);

        if (isRelevant) {
            setMessages(prev => [...prev, message]);

            setTimeout(() => {
                scrollToBottom();
            }, 50);
        }
    }, [currentUserId, receivingUserId]);

    const { sendMessage } = useSignalR(currentUserId, handleReceiveMessage);

    const handleSend = useCallback(() => {
        if (inputRef.current?.value.trim()) {
            sendMessage({ receivingUserId, content: inputRef.current.value });
            inputRef.current.value = "";
        }
    }, [sendMessage, receivingUserId]);

    useHotKey("Enter", () => {
        const input = inputRef.current;

        if (!input) return;

        if (document.activeElement === input) {
            handleSend();
        } else {
            input.focus();
        }
    });

    const scrollToBottom = () => {
        requestAnimationFrame(() => {
            messageEndRef.current?.scrollIntoView({ block: "end", behavior: "smooth" });
        });
    };

    const scrollToTop = () => {
        requestAnimationFrame(() => {
            messagesContainerRef.current?.scrollTo({ top: 0, behavior: "smooth" });
        });
    };

    const getOtherUsername = () => {
        const msg = messages.find(m => m.sendingUserId === receivingUserId || m.receivingUserId === receivingUserId);
        if (!msg) return "";
        return msg.sendingUserId === receivingUserId ? msg.sendingUserName : msg.receivingUserName;
    }

    const loadOlderMessages = async () => {
        if (loadingOlder || messages.length === 0) return;
        setLoadingOlder(true);
        const oldestMessage = messages[0];
        await fetchMessages(oldestMessage.createdAt);
        setLoadingOlder(false);
        scrollToTop();
    };

    if (loading) return <div>Loading...</div>;
    if (!user) return <div>Please log in</div>;
    if (!id) return <div>Please select a conversation</div>;

    return (
        <div className="m-auto">
            <Container className="bg-dark text-primary border border-primary">
                <Row>
                    <Col className="d-flex flex-column px-0 py-4">
                        <Row className="align-items-between mb-4 px-4">
                            <Col>
                                <h3 className="text-secondary m-0">
                                    @{getOtherUsername()}
                                </h3>
                            </Col>

                            <Col xs="auto" className="d-flex gap-2">
                                <RootButton keyLabel="L" className="small-button" onClick={loadOlderMessages}>Load older</RootButton>
                                <RootButton keyLabel="P" className="small-button" onClick={scrollToTop}>Scroll up</RootButton>
                                <RootButton keyLabel="N" className="small-button" onClick={scrollToBottom}>Scroll down</RootButton>
                            </Col>
                        </Row>

                        <div
                            ref={messagesContainerRef}
                            className="flex-grow-1 overflow-auto border-top border-bottom border-secondary p-2"
                            style={{ minHeight: 500, maxHeight: 500 }}
                        >
                            {messages.map(msg => (
                                <div
                                    key={msg.id}
                                    className={`position-relative mb-2 ${msg.sendingUserId === currentUserId ? "text-primary" : "text-secondary"}`}
                                >
                                    <span className="fw-bold">{"<"}{msg.sendingUserName}{"> "}</span>
                                    {msg.content}
                                    <span className="position-absolute end-0 small">
                                        {new Date(msg.createdAt).toLocaleDateString()}{" "}
                                        {new Date(msg.createdAt).toLocaleTimeString()}
                                    </span>
                                </div>
                            ))}
                            <div ref={messageEndRef} />
                        </div>

                        <Row className="mt-4 align-items-center px-4">
                            <Col>
                                <Form.Control
                                    ref={inputRef}
                                    className="bg-transparent border-primary text-primary rounded-0"
                                    placeholder="Type a message..."
                                    onKeyDown={e => {
                                        if (e.key === "Enter" && !e.shiftKey) {
                                            e.preventDefault();
                                            handleSend();
                                        }
                                    }}
                                />
                            </Col>
                            <Col xs="auto">
                                <RootButton keyLabel="Enter" onClick={handleSend}>Send</RootButton>
                            </Col>
                        </Row>
                    </Col>
                </Row>
            </Container>
        </div>
    );
}