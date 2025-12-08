import { Container, Row, Col } from "react-bootstrap";
import { useNavigate } from "react-router-dom";
import { useState } from "react";
import DividerLine from "../Components/DividerLine";
import RootButton from "../Components/RootButton";
import InfoModal from "../Components/InfoModal";

const asciiLogo = `
██████   ██████   ██████  ████████     █████   ██████  ██████ ███████ ███████ ███████
██   ██ ██    ██ ██    ██    ██       ██   ██ ██      ██      ██      ██      ██
██████  ██    ██ ██    ██    ██       ███████ ██      ██      █████   ███████ ███████
██   ██ ██    ██ ██    ██    ██       ██   ██ ██      ██      ██           ██      ██
██   ██  ██████   ██████     ██   ██  ██   ██  ██████  ██████ ███████ ███████ ███████
`;

export default function Header() {
  const navigate = useNavigate();
  const [showModal, setShowModal] = useState(false);

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
              fontsize={14}
              onClick={() => setShowModal(true)}
            >
              Info
            </RootButton>
          </Col>
        </Row>
      </Container>
      <DividerLine variant="primary" className="mb-4" />

      <InfoModal show={showModal} setShow={setShowModal} onClose={() => setShowModal(false)} />
    </header>
  );
}