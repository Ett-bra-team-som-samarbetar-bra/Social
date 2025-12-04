import type { RouteObject } from "react-router-dom";
import { createBrowserRouter, RouterProvider } from "react-router-dom";
import { StrictMode } from "react";
import { createRoot } from "react-dom/client";
import "../sass/index.scss";
import App from "./App";
import routes from "./routes";
import { AuthProvider } from "./Auth/AuthProvider";
import ProtectedRoute from "./Auth/ProtectedRoute";
import GuestRoute from "./Auth/GuestRoute";

const mappedRoutes: RouteObject[] = routes.map((r) => {
  let element = r.element;

  if (r.requiresAuth) {
    element = <ProtectedRoute>{element}</ProtectedRoute>;
  }

  if (r.guestOnly) {
    element = <GuestRoute>{element}</GuestRoute>;
  }

  return {
    path: r.path,
    element,
    loader: r.loader,
    children: r.children as RouteObject[] | undefined,
  };
});

const router = createBrowserRouter([
  {
    path: "/",
    element: <App />,
    children: mappedRoutes,
  },
]);

createRoot(document.querySelector("#root")!).render(
  <StrictMode>
    <AuthProvider>
      <RouterProvider router={router} />
    </AuthProvider>
  </StrictMode>
);
