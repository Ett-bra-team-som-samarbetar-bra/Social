

import Col from "react-bootstrap/esm/Col";
import RootButton from "../Components/RootButton";
import Row from "react-bootstrap/esm/Row";
import { Container, Form } from "react-bootstrap";

const currentUserId = 1;

const messages = [
    { id: 1, sendingUserId: 2, sendingUserName: "Chad", content: "Tvätta keken för fan", createdAt: "1990-10-01T10:00:00Z" },
    { id: 2, sendingUserId: 1, sendingUserName: "KeklordXXX", content: "Kommer aldrig hända fattaruväl", createdAt: "2012-02-01T06:24:00Z" },
    { id: 3, sendingUserId: 2, sendingUserName: "Chad", content: "Du måste tvätta keken nu!", createdAt: "2025-11-01T12:02:00Z" },
    { id: 4, sendingUserId: 1, sendingUserName: "KeklordXXX", content: "Nej", createdAt: "2025-11-01T12:05:00Z" },
    { id: 5, sendingUserId: 2, sendingUserName: "Chad", content: "Jo", createdAt: "2025-11-01T12:06:00Z" },
    { id: 6, sendingUserId: 1, sendingUserName: "KeklordXXX", content: "Nej", createdAt: "2025-11-01T12:07:00Z" },
    { id: 7, sendingUserId: 2, sendingUserName: "Chad", content: "Jo", createdAt: "2025-11-01T12:08:00Z" },
    { id: 8, sendingUserId: 1, sendingUserName: "KeklordXXX", content: "Nej", createdAt: "2025-11-01T12:09:00Z" },
    { id: 9, sendingUserId: 2, sendingUserName: "Chad", content: "Jo", createdAt: "2025-11-01T12:10:00Z" },
    { id: 4, sendingUserId: 1, sendingUserName: "KeklordXXX", content: "Nej", createdAt: "2025-11-01T12:05:00Z" },
    { id: 5, sendingUserId: 2, sendingUserName: "Chad", content: "Jo", createdAt: "2025-11-01T12:06:00Z" },
    { id: 6, sendingUserId: 1, sendingUserName: "KeklordXXX", content: "Nej", createdAt: "2025-11-01T12:07:00Z" },
    { id: 7, sendingUserId: 2, sendingUserName: "Chad", content: "Jo", createdAt: "2025-11-01T12:08:00Z" },
    { id: 8, sendingUserId: 1, sendingUserName: "KeklordXXX", content: "Nej", createdAt: "2025-11-01T12:09:00Z" },
    { id: 9, sendingUserId: 2, sendingUserName: "Chad", content: "Jo", createdAt: "2025-11-01T12:10:00Z" },
    { id: 4, sendingUserId: 1, sendingUserName: "KeklordXXX", content: "Nej", createdAt: "2025-11-01T12:05:00Z" },
    { id: 5, sendingUserId: 2, sendingUserName: "Chad", content: "Jo", createdAt: "2025-11-01T12:06:00Z" },
    { id: 6, sendingUserId: 1, sendingUserName: "KeklordXXX", content: "Nej", createdAt: "2025-11-01T12:07:00Z" },
    { id: 7, sendingUserId: 2, sendingUserName: "Chad", content: "Jo", createdAt: "2025-11-01T12:08:00Z" },
    { id: 8, sendingUserId: 1, sendingUserName: "KeklordXXX", content: "Nej", createdAt: "2025-11-01T12:09:00Z" },
    { id: 9, sendingUserId: 2, sendingUserName: "Chad", content: "Jo", createdAt: "2025-11-01T12:10:00Z" },
];

export default function MessagePage() {
    return (
        <div className="m-auto">
            <Container className=" bg-dark text-primary border border-primary">
                <Row>
                    <Col className="d-flex flex-column px-0 py-4">
                        <Row className="align-items-between mb-4 px-4">
                            <Col>
                                <h3 className="text-secondary m-0">
                                    @{messages[1].sendingUserName}
                                </h3>
                            </Col>

                            <Col xs="auto" className="d-flex gap-2">
                                <RootButton keyLabel="L" className="small-button">Load older</RootButton>
                                <RootButton keyLabel="P" className="small-button">Scroll up</RootButton>
                                <RootButton keyLabel="N" className="small-button">Scroll down</RootButton>
                            </Col>
                        </Row>

                        <div
                            className="flex-grow-1 overflow-auto border-top border-bottom border-secondary p-2"
                            style={{ minHeight: 500, maxHeight: 500 }}
                        >
                            {messages.map(msg => (
                                <div
                                    key={msg.id}
                                    className={`position-relative mb-2 ${msg.sendingUserId === currentUserId ? "text-primary" : "text-secondary"
                                        }`}
                                >
                                    <span className="fw-bold">
                                        {"<"}{msg.sendingUserName}{"> "}
                                    </span>
                                    {msg.content}

                                    <span className="position-absolute end-0 small">
                                        {new Date(msg.createdAt).toLocaleDateString()}{" "}
                                        {new Date(msg.createdAt).toLocaleTimeString()}
                                    </span>
                                </div>
                            ))}
                        </div>

                        <Row className="mt-4 align-items-center px-4">
                            <Col>
                                <Form.Control
                                    className="bg-transparent border-primary text-primary rounded-0"
                                    placeholder="Type a message..."
                                />
                            </Col>
                            <Col xs="auto">
                                <RootButton keyLabel="Enter">Send</RootButton>
                            </Col>
                        </Row>
                    </Col>
                </Row>
            </Container>
        </div>
    );
}