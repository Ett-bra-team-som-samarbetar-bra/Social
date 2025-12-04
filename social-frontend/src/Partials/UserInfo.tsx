import RootButton from "../Components/RootButton";
import { useAuth } from "../Hooks/useAuth";

export default function UserInfo() {
  const { user, logout } = useAuth();
  function handleLogout() {
    logout();
  }
  return <>{user && <RootButton onClick={handleLogout}>Logout</RootButton>}</>;
}
