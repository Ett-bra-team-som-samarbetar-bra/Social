import { Container, Row, Col } from "react-bootstrap";
import { useNavigate } from "react-router-dom";
import DividerLine from "../Components/DividerLine";
import RootButton from "../Components/RootButton";
import { useAuth } from "../Hooks/useAuth";

const asciiLogo = `
██████   ██████   ██████  ████████     █████   ██████  ██████ ███████ ███████ ███████
██   ██ ██    ██ ██    ██    ██       ██   ██ ██      ██      ██      ██      ██
██████  ██    ██ ██    ██    ██       ███████ ██      ██      █████   ███████ ███████
██   ██ ██    ██ ██    ██    ██       ██   ██ ██      ██      ██           ██      ██
██   ██  ██████   ██████     ██   ██  ██   ██  ██████  ██████ ███████ ███████ ███████
`;

export default function Header() {
  const { user } = useAuth();
  const navigate = useNavigate();

  return (
    <header>
      <Container fluid>
        <Row className="py-4 px-5">
          <Col md={8}>
            <pre className="ascii-logo" onClick={() => navigate("/")}>
              <span className="cursor-pointer user-select-none">
                {asciiLogo}
              </span>
            </pre>
          </Col>

          <Col md={4} className="d-flex align-items-center justify-content-end">
            {user && (
              <RootButton
                className=""
                keyLabel="L"
                textColor="primary"
                backgroundColor="transparent"
                fontsize={14}
              >
                Logout
              </RootButton>
            )}
            <RootButton
              keyLabel="I"
              textColor="primary"
              backgroundColor="transparent"
              fontsize={14}
            >
              Info
            </RootButton>
          </Col>
        </Row>
      </Container>
      <DividerLine variant="primary" className="mb-4" />
    </header>
  );
}