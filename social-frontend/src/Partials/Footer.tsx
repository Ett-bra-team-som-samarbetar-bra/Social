import { Col, Row } from "react-bootstrap";
import DividerLine from "../Components/DividerLine";
import { useNavigate } from "react-router-dom";

export default function Footer() {
  const navigate = useNavigate();

  return (
    <footer>
      <DividerLine variant="primary" />

      <Row className="m-0">
        <Col md={3} className="d-flex align-items-center justify-content-left">
          <span className="Japanse-char-big ps-5 cursor-pointer" onClick={() => navigate("/")}> 根鍵 </span>
        </Col>

        <Col md={6}>
          <p className="pt-2 m-0">
            [01010010 01101111 01101111 01110100 00101110 01000001 01100011 01100011 01100101 01110011 01110011]
          </p>

          <p className="pt-2 m-0">
            "52 6F 6F 74 2E 41 63 63 65 73 73"
          </p>

          <p className="pt-2 m-0 pb-3">
            © {new Date().getFullYear()} Root.Access. All rights reserved.
          </p>

        </Col>
        <Col md={3} />
      </Row>
    </footer >
  );
}