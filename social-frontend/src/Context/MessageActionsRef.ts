
export const messageActionsRef: {
    scrollToTop: (() => void) | null;
    scrollToBottom: (() => void) | null;
    loadOlderMessages: (() => void) | null;
} = {
    scrollToTop: null,
    scrollToBottom: null,
    loadOlderMessages: null,
};
