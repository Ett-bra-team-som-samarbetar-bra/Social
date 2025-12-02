import type { RouteObject } from "react-router-dom";
import { createBrowserRouter, RouterProvider } from "react-router-dom";
import { StrictMode } from "react";
import { createRoot } from "react-dom/client";
import "../sass/index.scss";
import App from "./App";
import routes from "./routes";
import { AuthProvider } from "./Auth/AuthProvider";

const router = createBrowserRouter([
  {
    path: "/",
    element: <App />,
    children: routes as RouteObject[],
  },
]);

createRoot(document.querySelector("#root")!).render(
  <StrictMode>
    <AuthProvider>
      <RouterProvider router={router} />
    </AuthProvider>
  </StrictMode>
);
