import { Outlet } from "react-router-dom";
import { Container, Row, Col } from "react-bootstrap";
import ConversationList from "./ConversationList";
import UserInfo from "./UserInfo";

export default function Main() {
  return (
    <main>
      <Container fluid className="h-100 pb-4">
        <Row className="h-100 px-5">
          <Col md={3} className="h-100 d-flex flex-column flex-grow-1">
            <UserInfo />
          </Col>

          <Col md={6} className="h-100 d-flex flex-column">
            <Outlet />
          </Col>

          <Col md={3}>
            <ConversationList />
          </Col>
        </Row>
      </Container>
    </main>
  );
}
