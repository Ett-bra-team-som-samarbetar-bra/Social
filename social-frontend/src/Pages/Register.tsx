import { useState } from "react";
import RootButton from "../Components/RootButton";
import { useAuth } from "../Hooks/useAuth";
import { registerSchema } from "../Validation/registerSchema";

export default function RegisterPage() {
  const [username, setUsername] = useState("");
  const [email, setEmail] = useState("");
  const [password, setPassword] = useState("");
  const [description, setDescription] = useState("");
  const [formError, setFormError] = useState<string | null>(null);
  const { register } = useAuth();

  function onSubmit(e: React.FormEvent) {
    e.preventDefault();

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

    register(username, email, password, description);
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
          {`",
  "email": "`}
          <input
            className="json-input"
            value={email}
            onChange={(e) => setEmail(e.target.value)}
            placeholder="your email"
          />
          {`",
  "description": "`}
          <textarea
            className="json-input json-textarea"
            value={description}
            onChange={(e) => setDescription(e.target.value.slice(0, 300))}
            placeholder="your description"
            rows={2}
          />
          {`"
}`}
          <div className="char-counter">
            {300 - description.length} characters left for description
          </div>
        </pre>

        {formError && (
          <div className="json-error-box">
            {formError.split("\n").map((line, i) => (
              <div key={i}>• {line}</div>
            ))}
          </div>
        )}

        <RootButton keyLabel="Enter" type="submit" className="mt-3 w-100">
          Register
        </RootButton>
      </form>
    </div>
  );
}
