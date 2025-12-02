import type { JSX } from "react";
import StartPage from "./Pages/StartPage";
import MessagePage from "./Pages/MessagePage";
import LoginPage from "./Pages/LoginPage";
import RegisterPage from "./Pages/Register";

interface Route {
  element: JSX.Element;
  path: string;
  menuLabel?: string;
  loader?: Function;
  children?: Route[];
}

const routes: Route[] = [
  { element: <StartPage />, path: "", menuLabel: "Start" },
  { element: <MessagePage />, path: "messages", menuLabel: "Messages" },
  { element: <LoginPage />, path: "login", menuLabel: "Login" },
  { element: <RegisterPage />, path: "register", menuLabel: "Register" },
];

export default routes;
