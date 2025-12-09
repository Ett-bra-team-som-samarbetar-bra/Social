import { useEffect } from "react";
import { useHotkeyManager } from "../Context/HotkeyContext";

export function useHotKey(
    key: string | undefined,
    callback: (() => void) | undefined,
    scope: "global" | "local" = "local",
    region?: "left" | "center" | "right"
) {
    const { registerHotkey, unregisterHotkey } = useHotkeyManager();

    useEffect(() => {
        if (!key || !callback) return;

        const id = registerHotkey(key, callback, scope, region);

        return () => unregisterHotkey(id);
    }, [key, callback, scope, region]);
}
