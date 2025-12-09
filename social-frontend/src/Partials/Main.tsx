import { Outlet, useNavigate } from "react-router-dom";
import { Container, Row, Col } from "react-bootstrap";
import ConversationList from "./ConversationList";
import UserInfo from "./UserInfo";
import { useFocus } from "../Context/FocusContext";
import { useAuth } from "../Hooks/useAuth";
import { useHotKey } from "../Hooks/useHotKey";

export default function Main() {
  const { user } = useAuth();
  const { focus, setRegion } = useFocus();

  const navigate = useNavigate();
  const userHeading = user ? "[P]Post" : "[[▓]P▣ó̶st";
  const isActiveRegion = focus.region === "center";

  // Hotfix spaghetti
  useHotKey("p", () => {
    setRegion("center");
    navigate("/");
  }, "global");

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
            <h5
              className={`text-primary mb-3 w-25 py-1 
                ${user ? 'cursor-pointer' : ''}  
                ${isActiveRegion ? 'bg-primary text-dark' : 'keybind-header'}`
              }
              onClick={user ? () => navigate(`/`) : undefined}>{userHeading}
            </h5>
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