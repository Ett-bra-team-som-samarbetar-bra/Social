import type { JSX } from "react";
import type { LoaderFunction } from "react-router-dom";
import StartPage from "./Pages/StartPage";
import MessagePage from "./Pages/MessagePage";
import LoginPage from "./Pages/LoginPage";
import RegisterPage from "./Pages/Register";

interface Route {
  element: JSX.Element;
  path: string;
  menuLabel?: string;
  loader?: LoaderFunction;
  children?: Route[];
  requiresAuth?: boolean;
  guestOnly?: boolean;
}

const routes: Route[] = [
  { element: <StartPage />, path: "", menuLabel: "Start", requiresAuth: true },
  {
    element: <MessagePage />,
    path: "messages",
    menuLabel: "Messages",
    requiresAuth: true,
  },
  {
    element: <LoginPage />,
    path: "login",
    menuLabel: "Login",
    guestOnly: true,
  },
  {
    element: <RegisterPage />,
    path: "register",
    menuLabel: "Register",
    guestOnly: true,
  },
];

export default routes;
