import JsonDisplay from "../Components/JsonDisplay";
import RootButton from "../Components/RootButton";
import { useAuth } from "../Hooks/useAuth";
import { useState } from "react";
import { passwordSchema } from "../Validation/passwordSchema";
import { descriptionSchema } from "../Validation/descriptionSchema";

const apiUrl = import.meta.env.VITE_API_URL;

export default function UserInfo() {
  const [editMode, setEditMode] = useState<"password" | "description" | null>(
    null
  );
  const [newPassword, setNewPassword] = useState("");
  const [newDescription, setNewDescription] = useState("");
  const [error, setError] = useState<string | null>(null);
  const [success, setSuccess] = useState<string | null>(null);
  const { user, logout, updateUser } = useAuth();

  function handleLogout() {
    logout();
  }

  async function handleChangePassword() {
    setError(null);
    setSuccess(null);

    const result = passwordSchema.safeParse({
      newPassword,
    });

    if (!result.success) {
      const message = result.error.issues
        .map((issue) => issue.message)
        .join("\n");

      setError(message);
      return;
    }

    const res = await fetch(`${apiUrl}/api/user/update-password`, {
      method: "PUT",
      credentials: "include",
      headers: { "Content-Type": "application/json" },
      body: JSON.stringify({ newPassword }),
    });

    const data = await res.json();

    if (!res.ok) {
      setError(data.error || "Password update failed.");
      return;
    }

    setSuccess("Password updated successfully.");
    setNewPassword("");
    setEditMode(null);
  }

  async function handleChangeDescription() {
    setError(null);
    setSuccess(null);

    const result = descriptionSchema.safeParse({
      newDescription,
    });

    if (!result.success) {
      const message = result.error.issues
        .map((issue) => issue.message)
        .join("\n");

      setError(message);
      return;
    }

    const res = await fetch(`${apiUrl}/api/user/update-description`, {
      method: "PUT",
      credentials: "include",
      headers: { "Content-Type": "application/json" },
      body: JSON.stringify({ newDescription }),
    });

    const data = await res.json();

    if (!res.ok) {
      setError(data.error || "Description update failed.");
      return;
    }

    updateUser({ description: newDescription });

    setSuccess("Description updated successfully.");
    setNewDescription("");
    setEditMode(null);
  }

  function cancelEdit() {
    setError(null);
    setSuccess(null);
    setEditMode(null);
  }

  const prefilledData = {
    name: "null",
    email: "undefined",
    description:
      "a̶̭̦̹̲̟̟͑͌͊ẃ̵̫͙̪̰̤̭́͌̈́̏͗l̴̞̼͇͛͑̌̎͑͜ơ̵̹̖̖̈d̴͓̜̰͉̆͘ḱ̶ͅl̶̢̤̞̝̭͙̄͆̽͊ơ̴͕̠̈̎͂̿̐̂j̷̤͔͔̞̪̜͇͌̊̊̋̉l̷̘̣̗̋̂͑v̴̯̣̻̰̯̘̓͌͜ņ̸̛̗̙̳͌̈́͌̈́̈́̕á̸͕̺̫̏̌w̶̹͕̩̽̊l̵̥̰̟̄̊̽͐̉͘͝ḳ̵͕̉̐d̶̛̹̮̭̺̯̼̾̋͂j̶̧̞͙̖͈̰̙̎͘͠a̸̡͈̳͕͍̎͒͗̅̀͝l̷͔̲̫̯̑ͅd̵͉̮̯͚̖̙́̓͐͐̄́w̴̬͔̋̌͑̔̔̚͜ḏ̶̢͇̋́j̷͙̪͙̒̈́k̴̠̹͖͎̄̒a̴̯͗͘͠w̵͙̫̲̿j̶̫̼̇̃̇d̸̨̮̠̝̗͜͠k̸͎̺̊̅̍̍͘w̸̳̆̇a̵̢̢̯̥̜̖̓̅̆̍͋j̷͓̐̄̍͆̕d̶̺̝̥͕͈̀̍́ķ̵̛̠̳̱͉͎̈̀̀̀å̵̜̱͙̲̣̙̥͆̃͐w̵̛̩͔͙̥̎͛͂̓̊̽l̴̟͐d̶̢̛̯͔̄͠k̸̡̿ã̵̖̠̜͚̈́̽́̕͜l̷̦͔͎͙͓͇͔͑͊ḳ̶͈̭̘͛̆́͝͝͝l̶̨̛̲͓̰̘̓̿̋̉̂͝n̵̙̬͑̓̎̌̚͝b̷͔̍̈́ͅķ̶̛̥̣̻̍̉n̸̛͇̘̓͠ͅk̶͎̘̐͊̒ĺ̸̺̟̖̺͌̀j̸̧̤̣̒͋̄̏̈̏d̶̳̑̈́̚f̴͔̍̕ȧ̶̞͈͕̝̪̭̉l̵͕̭̺͊̔̉̈́̄̕k̴̡̛̦̠̪̭̪̄̊̍͗̐͂w̶̟̜͒̋̃d̷̞̟̈́a̸̠̳̓̇̑̋̐̐ḻ̸̨̳̰̩̅ḱ̸̖̗̯̣̫͍̦d̴͚̠̥̥̱͑̓̊̚ͅa̶̻͈̩͗͗̏͘͝ͅl̶̘̟̙̥̹̽͋͊̅̕d̸̥͌͋̓k̷̻͑͐ă̴̦̹͍͉̿͌̔ṋ̶͚͓͓̻̱̫̍̈̄͆͘j̶͇͇͈͉̹́̀̅̓n̸͚̬̰̪̿̓a̵͚̹̖̿̓̾͠͝ĵ̵̳͇̼̱̭̿͜k̷̹͕̯͎̗̹̽̆͌̿̽ͅl̶̩͍̤̰̏͗̈́̓̎̆͜d̶̞̮̳̿̂",
  };

  const profileData = user
    ? {
      username: user.username,
      email: user.email,
      description: user.description,
    }
    : prefilledData;

  return (
    <div className="user-info-wrapper">
      <h5 className="text-primary mb-3">[U]User</h5>

      <JsonDisplay data={profileData} />

      {editMode === "password" && user && (
        <div className=" mt-3">
          <pre className="json-pre">
            {`{
  "newPassword": "`}
            <input
              type="password"
              className="json-input"
              value={newPassword}
              onChange={(e) => setNewPassword(e.target.value)}
            />
            {`"
}`}
          </pre>

          {error && (
            <div className="json-error-box">
              {error.split("\n").map((line, i) => (
                <div key={i}>• {line}</div>
              ))}
            </div>
          )}

          {success && (
            <div className="json-error-box">
              {success.split("\n").map((line, i) => (
                <div key={i}>• {line}</div>
              ))}
            </div>
          )}
          <div className="d-flex gap-3">
            <RootButton
              keyLabel="S"
              className="mt-2 flex-grow-1"
              onClick={handleChangePassword}
            >
              Save
            </RootButton>
            <RootButton
              keyLabel="C"
              className="mt-2 flex-grow-1"
              onClick={cancelEdit}
            >
              Cancel
            </RootButton>
          </div>
        </div>
      )}

      {editMode === "description" && user && (
        <div className="mt-3">
          <pre className="json-pre">
            {`{
  "newDescription": "`}
            <textarea
              className="json-input json-textarea"
              rows={2}
              value={newDescription}
              onChange={(e) => setNewDescription(e.target.value.slice(0, 300))}
            />
            {`"
}`}
          </pre>

          {error && (
            <div className="json-error-box">
              {error.split("\n").map((line, i) => (
                <div key={i}>• {line}</div>
              ))}
            </div>
          )}

          {success && (
            <div className="json-error-box">
              {success.split("\n").map((line, i) => (
                <div key={i}>• {line}</div>
              ))}
            </div>
          )}
          <div className="d-flex gap-3">
            <RootButton
              keyLabel="S"
              className="mt-2 flex-grow-1"
              onClick={handleChangeDescription}
            >
              Save
            </RootButton>
            <RootButton
              keyLabel="C"
              className="mt-2 flex-grow-1"
              onClick={cancelEdit}
            >
              Cancel
            </RootButton>
          </div>
        </div>
      )}

      {!editMode && user && (
        <div className="d-flex flex-column gap-2 mt-3">
          <RootButton
            keyLabel="P"
            onClick={() => {
              setEditMode("password");
              setSuccess(null);
              setError(null);
            }}
          >
            Change Password
          </RootButton>

          <RootButton
            keyLabel="D"
            onClick={() => {
              setEditMode("description");
              setSuccess(null);
              setError(null);
            }}
          >
            Change Description
          </RootButton>
        </div>
      )}
      {user && (
        <RootButton
          keyLabel="L"
          onClick={handleLogout}
          className="logout-button"
        >
          Logout
        </RootButton>
      )}
    </div>
  );
}
