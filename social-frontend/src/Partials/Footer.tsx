import { Col, Row } from "react-bootstrap";
import DividerLine from "../Components/DividerLine";
import { useNavigate } from "react-router-dom";
import { useEffect, useState } from "react";

export default function Footer() {
  const [typed, setTyped] = useState("");
  const navigate = useNavigate();

  const copyrightString = `©${new Date().getFullYear()} Root.Access. All rights res̵̄̇e̷̿ͅr̵͙͑͘v̶͆e̵̜͐ḏ̶́͘根`
  const typeSpeed = 60;

  useEffect(() => {
    setTyped("");
    let i = 0;
    const interval = setInterval(() => {
      setTyped(copyrightString.slice(0, i + 1));
      i++;
      if (i >= copyrightString.length) clearInterval(interval);
    }, typeSpeed);
    return () => clearInterval(interval);
  }, [copyrightString]);

  return (
    <footer>
      <DividerLine variant="primary" />

      <Row className="m-0 py-3">
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

          {/* <p className="pt-2 mt-1 m-0 text-size-small">
            © {new Date().getFullYear()} Root.Access. All rights reserved.
          </p> */}

          <p className="pt-2 mt-1 m-0 text-size-small" style={{ fontFamily: "monospace" }}>
            {typed}
            {typed.length < copyrightString.length && (
              <span className="blinking-cursor">|</span>
            )}
          </p>

        </Col>
        <Col md={3} />
      </Row>
    </footer >
  );
}