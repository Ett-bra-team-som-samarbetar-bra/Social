import { useHotKey } from "../Hooks/useHotKey";
import { useFocus } from "./FocusContext.tsx";

export function GlobalHotkeys() {
    const { setRegion } = useFocus();

    useHotKey("u", () => setRegion("left"), "global");

    useHotKey("p", () => setRegion("center"), "global");

    useHotKey("m", () => setRegion("right"), "global");

    return null;
}
