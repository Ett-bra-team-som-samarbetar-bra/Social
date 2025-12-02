import { Col } from "react-bootstrap";
const userList = [
    "Kalven",
    "KeklordXXX",
    "Joggis",
    "1337_girl_irl",
    "RandomUser",
    "RandomUser",
    "RandomUser",
    "KeklordXXX",
    "Joggis",
    "1337_girl_irl",
    "RandomUser",
    "RandomUser",
    "RandomUser",
    "KeklordXXX",
];
export default function ConversationList() {
    return (
        <Col className="conversation-aside ps-3">
            <h5 className="text-primary mb-3">Messages</h5>

            <div className="conversation-list">
                {userList.map(u => (
                    <div
                        key={u}
                        className="conversation-item"
                    >
                        @{u}
                    </div>
                ))}
            </div>
        </Col>
    );
}
