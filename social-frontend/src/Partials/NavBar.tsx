import { useState } from "react";
import { Nav, Navbar } from "react-bootstrap";
import { NavLink } from "react-router-dom";
import routes from "../routes";
import { useAuth } from "../Hooks/useAuth";
export default function NavBar() {
  const { user } = useAuth();
  const [expanded, setExpanded] = useState(false);

  return (
    <div className="nav-nav text-start mt-0">
      <Navbar
        expanded={expanded}
        expand="lg"
        className="w-100 d-flex align-items-center mt-0 pt-0"
      >
        <Navbar.Toggle
          aria-controls="root-navbar"
          className="bg-primary border-0"
          onClick={() => setExpanded((prev) => !prev)}
        />

        <Navbar.Collapse id="root-navbar">
          <Nav>
            {routes
              .filter((r) => {
                if (!r.menuLabel) return false;

                if (r.requiresAuth && !user) return false;

                if (r.guestOnly && user) return false;

                return true;
              })
              .map((r, i) => (
                <Nav.Link
                  key={i}
                  as={NavLink}
                  to={r.path}
                  className="nav-link px-lg-4 text-uppercase pt-0"
                  onClick={() => setTimeout(() => setExpanded(false), 200)}
                >
                  [{r.menuLabel}]
                </Nav.Link>
              ))}
          </Nav>
        </Navbar.Collapse>
      </Navbar>
    </div>
  );
}
