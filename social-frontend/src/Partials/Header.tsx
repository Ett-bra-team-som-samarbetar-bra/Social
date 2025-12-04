import { Container, Row, Col } from "react-bootstrap";
import { useNavigate } from "react-router-dom";
import DividerLine from "../Components/DividerLine";
import RootButton from "../Components/RootButton";

const asciiLogo = `
██████   ██████   ██████  ████████     █████   ██████  ██████ ███████ ███████ ███████
██   ██ ██    ██ ██    ██    ██       ██   ██ ██      ██      ██      ██      ██
██████  ██    ██ ██    ██    ██       ███████ ██      ██      █████   ███████ ███████
██   ██ ██    ██ ██    ██    ██       ██   ██ ██      ██      ██           ██      ██
██   ██  ██████   ██████     ██   ██  ██   ██  ██████  ██████ ███████ ███████ ███████
`;

export default function Header() {
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
            <RootButton
              keyLabel="I"
              textColor="primary"
              backgroundColor="transparent"
              fontsize={0.8}
              onClick={() => { alert("TODO"); }}
            >
              Info
            </RootButton>
          </Col>

        </Row>
      </Container>
      <DividerLine variant="primary" className="mb-4" />
    </header >
  );
}