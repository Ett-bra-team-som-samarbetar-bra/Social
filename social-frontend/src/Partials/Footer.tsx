import { Col, Row } from "react-bootstrap";
import DividerLine from "../Components/DividerLine";
import { useNavigate } from "react-router-dom";

export default function Footer() {
  const navigate = useNavigate();

  return (
    <footer>
      <DividerLine variant="primary" />

      <Row className="m-0 py-2 pb-3">
        <Col md={3} className="d-flex align-items-center justify-content-left">
          <span className="Japanse-char-big ps-5 cursor-pointer" onClick={() => navigate("/")}> 根鍵 </span>
        </Col>

        <Col md={6} className="text-center">
          <p className="m-0 text-size-small">
            {"${01010010 01101111 01101111 01110100 00101110 01000001 01100011 01100011 01100101 01110011 01110011}"}
          </p>

          <p className="m-0 text-size-small">
            {"0x52_0x6F_0x6F_0x74_0x2E_0x41_0x63_0x63_0x65_0x73_0x73"}
          </p>

          <p className="pt-2 mt-1 m-0 text-size-small">
            © {new Date().getFullYear()} Root.Access. All rights reserved.
          </p>

        </Col>
        <Col md={3} />
      </Row>
    </footer >
  );
}