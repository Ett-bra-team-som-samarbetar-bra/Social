import { useState } from "react";
import { Nav, Navbar } from "react-bootstrap";
import { NavLink } from "react-router-dom";
import routes from "../routes";

export default function Header() {
    const [expanded, setExpanded] = useState(false);

    return (
        <header className="app-header">
            <Navbar expanded={expanded} expand="lg" className="w-100 d-flex align-items-center">
                
                <Navbar.Brand className="me-auto">
                    <h1 className="m-0">[ROOT ACCESS]</h1>
                </Navbar.Brand>

                <Navbar.Toggle 
                    aria-controls="root-navbar" 
                    className="bg-primary border-0"
                    onClick={() => setExpanded(prev => !prev)}
                />

                <Navbar.Collapse id="root-navbar">
                    <Nav className="ms-auto">
                        {routes
                            .filter(r => r.menuLabel)
                            .map((r, i) => (
                                <Nav.Link
                                    key={i}
                                    as={NavLink}
                                    to={r.path}
                                    className="nav-link px-lg-4 text-uppercase"
                                    onClick={() => setTimeout(() => setExpanded(false), 200)}
                                >
                                    [{r.menuLabel}]
                                </Nav.Link>
                            ))}
                    </Nav>
                </Navbar.Collapse>
            </Navbar>
        </header>
    );
}
