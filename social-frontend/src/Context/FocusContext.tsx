import { createContext, useContext, useState } from "react";

type FocusRegion = "left" | "center" | "right" | "none";

interface FocusState {
  region: FocusRegion;
  focusedPostId: number | null;
}

interface FocusContextValue {
  focus: FocusState;
  setRegion: (region: FocusRegion) => void;
  setFocusedPost: (postId: number | null) => void;
  cyclePost: (direction: "next" | "prev", posts: number[]) => void;
}

const FocusContext = createContext<FocusContextValue | null>(null);

export function FocusProvider({ children }: { children: React.ReactNode }) {
  const [focus, setFocus] = useState<FocusState>({
    region: "center",
    focusedPostId: null
  });

  function setRegion(region: FocusRegion) {
    setFocus(prev => ({ ...prev, region }));
  }

  function setFocusedPost(postId: number | null) {
    setFocus(prev => ({ ...prev, focusedPostId: postId }));
  }

  function cyclePost(direction: "next" | "prev", posts: number[]) {
    if (posts.length === 0) return;

    const index = posts.indexOf(focus.focusedPostId ?? posts[0]);
    let newIndex = index;

    if (direction === "next") newIndex = (index + 1) % posts.length;
    if (direction === "prev") newIndex = (index - 1 + posts.length) % posts.length;

    setFocusedPost(posts[newIndex]);
  }

  return (
    <FocusContext.Provider value={{ focus, setRegion, setFocusedPost, cyclePost }}>
      {children}
    </FocusContext.Provider>
  );
}

export function useFocus() {
  const ctx = useContext(FocusContext);
  if (!ctx) throw new Error("useFocus must be used within FocusProvider");
  return ctx;
}
