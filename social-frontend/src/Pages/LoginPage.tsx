import { Form, Button } from "react-bootstrap";

export default function LoginPage() {
  function onSubmit() {}
  return (
    <>
      <Form>
        <Form.Group className="mb-3" controlId="formUsername">
          <Form.Label>Username</Form.Label>
          <Form.Control type="text" placeholder="Enter Username" />
        </Form.Group>

        <Form.Group className="mb-3" controlId="formPassword">
          <Form.Label>Password</Form.Label>
          <Form.Control type="password" placeholder="Password" />
        </Form.Group>

        <Button variant="primary" type="submit" onClick={() => onSubmit()}>
          Submit
        </Button>
      </Form>
    </>
  );
}
