import { Container, Row, Col } from "react-bootstrap";
import { useNavigate } from "react-router-dom";
import { useState } from "react";
import DividerLine from "../Components/DividerLine";
import RootButton from "../Components/RootButton";
import InfoModal from "../Components/InfoModal";
import { useAuth } from "../Hooks/useAuth";
import LogoutModal from "../Components/LogoutModal";

const asciiLogo = `
██████   ██████   ██████  ████████     █████   ██████  ██████ ███████ ███████ ███████
██   ██ ██    ██ ██    ██    ██       ██   ██ ██      ██      ██      ██      ██
██████  ██    ██ ██    ██    ██       ███████ ██      ██      █████   ███████ ███████
██   ██ ██    ██ ██    ██    ██       ██   ██ ██      ██      ██           ██      ██
██   ██  ██████   ██████     ██   ██  ██   ██  ██████  ██████ ███████ ███████ ███████
`;

export default function Header() {
  const { user, logout } = useAuth();
  const [showInfoModal, setShowInfoModal] = useState(false);
  const [showLogoutModal, setShowLogoutModal] = useState(false);

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
                onClick={() => setShowLogoutModal(true)}
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
              onClick={() => setShowInfoModal(true)}
            >
              Info
            </RootButton>
          </Col>
        </Row>
      </Container>
      <DividerLine variant="primary" className="mb-4" />

      <InfoModal show={showInfoModal} setShow={setShowInfoModal} onClose={() => setShowInfoModal(false)} />
      <LogoutModal show={showLogoutModal} setShow={setShowLogoutModal} onClose={() => setShowLogoutModal(false)} onLogout={logout} />
    </header>
  );
}