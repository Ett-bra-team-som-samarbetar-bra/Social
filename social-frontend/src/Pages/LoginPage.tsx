import { useState } from "react";
import RootButton from "../Components/RootButton";
import { useAuth } from "../Hooks/useAuth";

export default function LoginPage() {
  const [username, setUsername] = useState("");
  const [password, setPassword] = useState("");
  const { login } = useAuth();

  function onSubmit(e: React.FormEvent) {
    e.preventDefault();
    login(username, password);
  }

  return (
    <div className="json-box-container">
      <form onSubmit={onSubmit} className="json-box">
        <pre className="json-pre">
          {`{
  "username": "`}
          <input
            className="json-input"
            value={username}
            onChange={(e) => setUsername(e.target.value)}
            placeholder="your username"
          />
          {`",
  "password": "`}
          <input
            type="password"
            className="json-input"
            value={password}
            onChange={(e) => setPassword(e.target.value)}
            placeholder="your password"
          />
          {`"
}`}
        </pre>

        <RootButton keyLabel="Enter" type="submit" className="mt-3 w-100">
          Login
        </RootButton>
      </form>
    </div>
  );
}
