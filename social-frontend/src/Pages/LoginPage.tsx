import { useState } from "react";
import RootButton from "../Components/RootButton";
import { useAuth } from "../Hooks/useAuth";
import { useNavigate } from "react-router-dom";

export default function LoginPage() {
  const { login } = useAuth();

  const [username, setUsername] = useState("");
  const [password, setPassword] = useState("");
  const [loginError, setLoginError] = useState<string | null>(null);
  const navigate = useNavigate();

  async function onSubmit(e: React.FormEvent) {
    e.preventDefault();
    const result = await login(username, password);

    if (!result.ok) {
      setLoginError(result.error || "Login failed, please try again");
      return;
    }
  }

  return (
    <>
      <div className="d-flex flex-column h-100 align-items-center justify-content-center">
        <div>
          <div className="d-flex gap-1 justify-content-between">
            <div className="d-flex gap-1">
              <RootButton className="post-outline post-tab-fixed-size" onClick={() => navigate("/login")}>Login</RootButton>
              <RootButton className="post-outline post-tab-fixed-size" onClick={() => navigate("/register")}>Register</RootButton>
            </div>
          </div>

          <div className="json-box-container">
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
        </div>
      </div>
    </>
  );
}
