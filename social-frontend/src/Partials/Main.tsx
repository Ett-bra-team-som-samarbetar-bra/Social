import { Outlet } from "react-router-dom";
import { Container, Row, Col } from "react-bootstrap";
import NavBar from "./NavBar";
import ConversationList from "../Components/ConversationList";
export default function Main() {
  return (
    <main>
      <Container fluid className="h-100">
        <Row className="h-100">
          <Col md={3} >
            <h5 className="text-secondary mb-3">User Info</h5>

          </Col>

          <Col md={6}>
            <NavBar />
            <Outlet />
          </Col>

          <Col md={3} >
            <ConversationList />
          </Col>
        </Row>
      </Container>
    </main>
  );
}
