import { useState } from "react";
import RootButton from "../Components/RootButton";
import { useAuth } from "../Hooks/useAuth";
import { registerSchema } from "../Validation/registerSchema";
import { useNavigate } from "react-router-dom";

export default function RegisterPage() {
  const navigate = useNavigate();
  const [username, setUsername] = useState("");
  const [email, setEmail] = useState("");
  const [password, setPassword] = useState("");
  const [description, setDescription] = useState("");
  const [formError, setFormError] = useState<string | null>(null);
  const [registerSuccess, setRegisterSuccess] = useState<string | null>(null);
  const { register } = useAuth();

  async function onSubmit(e: React.FormEvent) {
    e.preventDefault();

    setRegisterSuccess(null);

    const result = registerSchema.safeParse({
      username,
      email,
      password,
      description,
    });

    if (!result.success) {
      const message = result.error.issues
        .map((issue) => issue.message)
        .join("\n");

      setFormError(message);
      return;
    }

    setFormError(null);

    const registerResult = await register(
      username,
      email,
      password,
      description
    );

    if (!registerResult.ok) {
      setFormError(
        registerResult.error || "Registration failed. Please try again"
      );
      return;
    }

    setRegisterSuccess("Your account was successfully created!");

    setTimeout(() => {
      navigate("/login");
    }, 2500);
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
                  autoComplete="off"
                  value={username}
                  maxLength={30}
                  onChange={(e) => setUsername(e.target.value)}
                  placeholder=""
                />
                {`",
  "password": "`}
                <input
                  type="password"
                  autoComplete="off"
                  className="json-input"
                  value={password}
                  onChange={(e) => setPassword(e.target.value)}
                  placeholder=""
                />
                {`",
  "email": "`}
                <input
                  className="json-input"
                  autoComplete="off"
                  value={email}
                  onChange={(e) => setEmail(e.target.value)}
                  placeholder=""
                />
                {`",
  "description": "`}
                <textarea
                  className="json-input json-textarea"
                  autoComplete="off"
                  value={description}
                  onChange={(e) => setDescription(e.target.value.slice(0, 300))}
                  placeholder=""
                  rows={2}
                />
                {`"
}`}
                <div className="content-counter">
                  {description.length}/{300}
                </div>
              </pre>

              {formError && (
                <div className="json-error-box">
                  {formError.split("\n").map((line, i) => (
                    <div key={i}>• {line}</div>
                  ))}
                </div>
              )}

              {registerSuccess && (
                <div className="json-error-box">
                  {registerSuccess.split("\n").map((line, i) => (
                    <div key={i}>• {line}</div>
                  ))}
                </div>
              )}

              <RootButton keyLabel="Enter" type="submit" className="mt-4 w-100">
                Register
              </RootButton>
            </form>
          </div>
        </div>
      </div>
    </>
  );
}
