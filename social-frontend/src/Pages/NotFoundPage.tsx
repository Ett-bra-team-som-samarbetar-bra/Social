import { Col, Container, Row } from 'react-bootstrap';

export default function NotFoundPage() {
  return (
    <Container fluid className="h-100 d-flex align-items-center">
      <Row className="justify-content-center w-100">
        <Col md={6} className="text-center">
          <h1 className="text-primary">404</h1>
          <h4 className="mb-4">Not Found</h4>
        </Col>
      </Row>
    </Container>
  );
}
