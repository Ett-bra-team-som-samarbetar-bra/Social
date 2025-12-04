import { useState } from "react";
import RootButton from "../Components/RootButton";
import { useAuth } from "../Hooks/useAuth";
import { useNavigate } from "react-router-dom";

export default function LoginPage() {
  const navigate = useNavigate();
  const [username, setUsername] = useState("");
  const [password, setPassword] = useState("");
  const [loginError, setLoginError] = useState<string | null>(null);
  const [loginSuccess, setLoginSuccess] = useState<string | null>(null);
  const { login } = useAuth();

  async function onSubmit(e: React.FormEvent) {
    e.preventDefault();
    const result = await login(username, password);

    if (!result.ok) {
      setLoginError(result.error || "Login failed, please try again");
      return;
    }

    setLoginSuccess("Log in successful. Enjoy your Root Access");

    setTimeout(() => {
      navigate("/");
    }, 2500);
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

        {loginError && (
          <div className="json-error-box">
            {loginError.split("\n").map((line, i) => (
              <div key={i}>• {line}</div>
            ))}
          </div>
        )}

        {loginSuccess && (
          <div className="json-error-box">
            {loginSuccess.split("\n").map((line, i) => (
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
