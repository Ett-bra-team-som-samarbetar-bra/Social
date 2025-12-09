import { Outlet } from "react-router-dom";
import { Container, Row, Col } from "react-bootstrap";
import ConversationList from "./ConversationList";
import UserInfo from "./UserInfo";
import { useFocus } from "../Context/FocusContext";
import { useAuth } from "../Hooks/useAuth";

export default function Main() {
  const { user } = useAuth();
  const { focus } = useFocus();
  const userHeading = user ? "[P]Post" : "[[▓]P▣ó̶st";

  return (
    <main>
      <Container fluid className="h-100 pb-4">
        <Row className="h-100 px-5">
          {/* LEFT REGION */}
          <Col
            md={3}
            className={`h-100 d-flex flex-column flex-grow-1 region
              ${focus.region === "left" ? "active-region" : ""}`}
          >
            <UserInfo />
          </Col>

          {/* CENTER REGION */}
          <Col
            md={6}
            className={`h-100 d-flex flex-column region
              ${focus.region === "center" ? "active-region" : ""}`}
          >
            <h5 className="text-primary mb-3 keybind-header">{userHeading}</h5>
            <Outlet />
          </Col>

          {/* RIGHT REGION */}
          <Col
            md={3}
            className={`h-100 region
              ${focus.region === "right" ? "active-region" : ""}`}
          >
            <ConversationList />
          </Col>
        </Row>
      </Container>
    </main>
  );
}
