import { Row } from "react-bootstrap";
import RootButton from "./RootButton";

interface InfoModalProps {
  show: boolean;
  onClose: () => void;
}

const asciiLogo = `
██████   ██████   ██████  ████████     █████   ██████  ██████ ███████ ███████ ███████
██   ██ ██    ██ ██    ██    ██       ██   ██ ██      ██      ██      ██      ██
██████  ██    ██ ██    ██    ██       ███████ ██      ██      █████   ███████ ███████
██   ██ ██    ██ ██    ██    ██       ██   ██ ██      ██      ██           ██      ██
██   ██  ██████   ██████     ██   ██  ██   ██  ██████  ██████ ███████ ███████ ███████
`;

// TODO
const keybinds = [
  { key: "I", desc: "Open information" },
  { key: "U", desc: "Target user" },
  { key: "M", desc: "Target messages" },
  { key: "P", desc: "Target posts? TODO" },
  { key: "X", desc: "Close" },
];

export default function InfoModal({ show, onClose }: InfoModalProps) {
  if (!show) return null;

  return (
    <>
      <div className="info-modal-overlay" onClick={onClose} />

      <div className="info-modal p-4">
        <pre className="ascii-logo info-modal-ascii-logo">{asciiLogo}</pre>

        <Row className="w-100 info-modal-spacing flex-column">
          <div className="m-0 p-0 fw-bold mb-2">Keybinds:</div>

          <ul style={{ listStyle: "none", padding: 0, margin: 0 }}>
            {keybinds.map((kb, i) => (
              <li key={i} style={{ display: "flex", marginBottom: 8 }}>
                <span className="info-modal-key">
                  {kb.key}
                </span>
                <span>{kb.desc}</span>
              </li>
            ))}
          </ul>
        </Row>

        <div style={{ marginTop: "auto", width: "100%", display: "flex", justifyContent: "center" }}>
          <RootButton keyLabel="X" onClick={onClose} className="pt-2">
            Close
          </RootButton>
        </div>
      </div>
    </>
  );
}