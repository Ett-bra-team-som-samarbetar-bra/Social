import { createContext, useContext, useEffect, useRef } from "react";

type Callback = () => void;

interface HotkeyEntry {
  key: string;
  callback: Callback;
  scope: "global" | "local";
  region?: "left" | "center" | "right";
  id: string;
}

interface HotkeyContextValue {
  registerHotkey: (
    key: string,
    callback: Callback,
    scope: "global" | "local",
    region?: "left" | "center" | "right"
  ) => string;
  unregisterHotkey: (id: string) => void;
}

const HotkeyContext = createContext<HotkeyContextValue | null>(null);

export function HotkeyProvider({ children }: { children: React.ReactNode }) {
  const hotkeys = useRef<HotkeyEntry[]>([]);

  const registerHotkey = (
    key: string,
    callback: Callback,
    scope: "global" | "local",
    region?: "left" | "center" | "right"
  ) => {
    if (!key) return "";
    const id = crypto.randomUUID();
    hotkeys.current.push({
      id,
      key: key.toLowerCase(),
      callback,
      scope,
      region,
    });
    return id;
  };

  const unregisterHotkey = (id: string) => {
    hotkeys.current = hotkeys.current.filter((h) => h.id !== id);
  };

  useEffect(() => {
    const handler = (e: KeyboardEvent) => {
      const key = e.key.toLowerCase();

      const active = document.activeElement as HTMLElement | null;
      const isTyping =
        active &&
        (active.tagName === "INPUT" ||
          active.tagName === "TEXTAREA" ||
          active.isContentEditable);

      for (const hotkey of hotkeys.current) {
        if (hotkey.key !== key) continue;

        // Block all hotkeys while typing
        if (isTyping) continue;

        e.preventDefault();
        hotkey.callback();
      }
    };

    window.addEventListener("keydown", handler);
    return () => window.removeEventListener("keydown", handler);
  }, []);

  return (
    <HotkeyContext.Provider value={{ registerHotkey, unregisterHotkey }}>
      {children}
    </HotkeyContext.Provider>
  );
}

export function useHotkeyManager() {
  const ctx = useContext(HotkeyContext);
  if (!ctx)
    throw new Error("useHotkeyManager must be used inside <HotkeyProvider>");
  return ctx;
}
