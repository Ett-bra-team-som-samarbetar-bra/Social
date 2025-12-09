import { useState } from "react";
import { useHotKey } from "../Hooks/useHotKey";
import { useFocus } from "./FocusContext.tsx";
import InfoModal from "../Components/InfoModal";
import LogoutModal from "../Components/LogoutModal";
import { useAuth } from "../Hooks/useAuth";
import { messageActionsRef } from "./MessageActionsRef";

export function GlobalHotkeys() {
    const { setRegion } = useFocus();
    const { logout } = useAuth();
    const [showInfoModal, setShowInfoModal] = useState(false);
    const [showLogoutModal, setShowLogoutModal] = useState(false);

    useHotKey("Escape", () => setRegion("none"), "global");
    useHotKey("u", () => setRegion("left"), "global");
    useHotKey("p", () => setRegion("center"), "global");
    useHotKey("m", () => setRegion("right"), "global");
    useHotKey("i", () => setShowInfoModal(true), "global");
    useHotKey("l", () => setShowLogoutModal(true), "global");

    useHotKey("o", () => messageActionsRef.loadOlderMessages?.(), "global");

    useHotKey("k", () => messageActionsRef.scrollToTop?.(), "global");

    useHotKey("n", () => messageActionsRef.scrollToBottom?.(), "global");

    return (
        <>
            <InfoModal
                show={showInfoModal}
                setShow={setShowInfoModal}
                onClose={() => setShowInfoModal(false)}
            />
            <LogoutModal
                show={showLogoutModal}
                setShow={setShowLogoutModal}
                onClose={() => setShowLogoutModal(false)}
                onLogout={logout}
            />
        </>
    );
}
