import { useEffect, useRef, useCallback } from "react";
import { HubConnection, HubConnectionBuilder } from "@microsoft/signalr";
import type MessageDto from "../Types/message";

export function useSignalR(userId: number, onMessage: (msg: MessageDto) => void) {
    const connectionRef = useRef<HubConnection | null>(null);
    const onMessageRef = useRef(onMessage);

    useEffect(() => {
        onMessageRef.current = onMessage;
    }, [onMessage]);

    useEffect(() => {
        if (!userId) return;

        const connection = new HubConnectionBuilder()
            .withUrl("http://localhost:5174/chatHub", { withCredentials: true })
            .withAutomaticReconnect()
            .build();

        connection.on("ReceiveMessage", (message: MessageDto) => {
            onMessageRef.current(message);
        });

        connection.start().catch(console.error);
        connectionRef.current = connection;

        return () => {
            connection.stop().catch(console.error);
        };
    }, [userId]);

    const sendMessage = useCallback(async (message: { receivingUserId: number; content: string }) => {
        await fetch("http://localhost:5174/api/message", {
            method: "POST",
            credentials: "include",
            headers: { "Content-Type": "application/json" },
            body: JSON.stringify(message),
        });
    }, []);

    return { sendMessage };
}