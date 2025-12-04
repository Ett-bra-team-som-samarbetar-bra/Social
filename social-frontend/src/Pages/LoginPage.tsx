import { useState } from "react";
import RootButton from "../Components/RootButton";
import { useAuth } from "../Hooks/useAuth";

export default function LoginPage() {
  const [username, setUsername] = useState("");
  const [password, setPassword] = useState("");
  const [loginError, setLoginError] = useState<string | null>(null);
  const { login } = useAuth();

  async function onSubmit(e: React.FormEvent) {
    e.preventDefault();
    const result = await login(username, password);

    if (!result.ok) {
      setLoginError(result.error || "Login failed, please try again");
      return;
    }
  }

  return (
    <div className="json-box-container mt-5">
      <form onSubmit={onSubmit} className="json-box">
        <pre className="json-pre">
          {`{
  "username": "`}
          <input
            className="json-input"
            value={username}
            autoComplete="off"
            onChange={(e) => setUsername(e.target.value)}
          />
          {`",
  "password": "`}
          <input
            type="password"
            autoComplete="off"
            className="json-input"
            value={password}
            onChange={(e) => setPassword(e.target.value)}
          />
          {`"
}`}
        </pre>

        {loginError && (
          <div className="json-error-box">
            {loginError.split("\n").map((line, i) => (
              <div key={i}>• {line}</div>
            ))}
          </div>
        )}

        <RootButton keyLabel="Enter" type="submit" className="mt-4 w-100">
          Login
        </RootButton>
      </form>
    </div>
  );
}
