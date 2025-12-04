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
    <div className="json-box-container">
      <form onSubmit={onSubmit} className="json-box">
        <pre className="json-pre">
          {`{
  "username": "`}
          <input
            className="json-input"
            value={username}
            onChange={(e) => setUsername(e.target.value)}
            placeholder=""
          />
          {`",
  "password": "`}
          <input
            type="password"
            className="json-input"
            value={password}
            onChange={(e) => setPassword(e.target.value)}
            placeholder=""
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

        <RootButton keyLabel="Enter" type="submit" className="mt-3 w-100">
          Login
        </RootButton>
      </form>
    </div>
  );
}
