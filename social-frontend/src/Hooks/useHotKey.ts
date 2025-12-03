import { useEffect } from "react";

export function useHotKey(keyLabel: string | undefined, callback?: () => void) {
    useEffect(() => {
        if (!keyLabel || !callback) return;

        const targetKey = keyLabel.toLowerCase();

        const handleKeyPress = (e: KeyboardEvent) => {
            const activeEl = document.activeElement as HTMLElement | null;

            if (
                activeEl &&
                (
                    activeEl.tagName === "INPUT" ||
                    activeEl.tagName === "TEXTAREA" ||
                    activeEl.isContentEditable ||
                    activeEl.closest("input, textarea, [contenteditable]") 
                )
            ) {
                return;
            }

            if (e.key.toLowerCase() === targetKey) {
                e.preventDefault();
                callback();
            }
        };

        window.addEventListener("keydown", handleKeyPress);
        return () => window.removeEventListener("keydown", handleKeyPress);
    }, [keyLabel, callback]);
}
