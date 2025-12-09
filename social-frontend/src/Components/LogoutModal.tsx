import RootButton from "./RootButton";
import { useHotKey } from "../Hooks/useHotKey";

interface LogoutModalProps {
    show: boolean;
    onClose: () => void;
    setShow: (show: boolean) => void;
    onLogout: () => void;
}

const asciiLogo = `
██████   ██████   ██████  ████████     █████   ██████  ██████ ███████ ███████ ███████
██   ██ ██    ██ ██    ██    ██       ██   ██ ██      ██      ██      ██      ██
██████  ██    ██ ██    ██    ██       ███████ ██      ██      █████   ███████ ███████
██   ██ ██    ██ ██    ██    ██       ██   ██ ██      ██      ██           ██      ██
██   ██  ██████   ██████     ██   ██  ██   ██  ██████  ██████ ███████ ███████ ███████
`;

export default function LogoutModal({ show, onClose, setShow, onLogout }: LogoutModalProps) {
    
    useHotKey(show ? "Escape" : undefined, () => {
        setShow(false);
    });

    useHotKey(show ? "Enter" : undefined, () => {
        onLogout();
        setShow(false);
    });

    const handleLogout = () => {
        onLogout();
        setShow(false);
    };

    if (!show) return null;

    return (
        <>
            <div className="info-modal-overlay" onClick={onClose} />
            <div className="logout-modal p-4">
                <div className="w-100 d-flex justify-content-start">
                    <pre className="ascii-logo logout-modal-ascii-logo">{asciiLogo}</pre>
                </div>
                <div className="w-100 d-flex flex-grow-1 align-items-center justify-content-center">
                    <div className="fw-bold text-center">[Do you want to logout?]</div>
                </div>
                <div className="w-100 d-flex gap-3 justify-content-center">
                    <RootButton
                        keyLabel="Escape"
                        onClick={onClose}
                        className="pt-2"
                        textColor="primary"
                        backgroundColor="transparent">
                        Cancel
                    </RootButton>
                    <RootButton keyLabel="Enter" onClick={handleLogout} className="pt-2">
                        Logout
                    </RootButton>
                </div>
            </div>
        </>
    );
}