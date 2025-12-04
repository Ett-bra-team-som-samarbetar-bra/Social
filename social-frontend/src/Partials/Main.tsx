import { Outlet } from "react-router-dom";
import { Container, Row, Col } from "react-bootstrap";
import NavBar from "./NavBar";
import ConversationList from "../Components/ConversationList";
import UserInfo from "./UserInfo";
export default function Main() {
  return (
    <main>
      <Container fluid className="h-100">
        <Row className="h-100 px-5">
          <Col md={3}>
            <h5 className="text-primary mb-3">User Info</h5>
            <UserInfo />
          </Col>

          <Col md={6}>
            <NavBar />
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
