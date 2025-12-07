
import React from "react";
import type MessageDto from "../Types/message";

export default function RenderChat({ messages, messagesContainerRef, messageEndRef
}: {
    messages: MessageDto[];
    messagesContainerRef: React.RefObject<HTMLDivElement | null>;
    messageEndRef: React.RefObject<HTMLDivElement | null>;
}) {
    const isSameDay = (a: Date, b: Date) =>
        a.toDateString() === b.toDateString();

    return (
        <div
            ref={messagesContainerRef}
            className="flex-grow-1 overflow-auto border-top border-bottom border-primary p-2"
            style={{ height: "50vh" }}
        >
            {messages.map((msg, i) => {
                const currentDate = new Date(msg.createdAt);
                const prevMsg = messages[i - 1];
                const prevDate = prevMsg ? new Date(prevMsg.createdAt) : null;
                const showDateHeader =
                    !prevMsg || (prevDate && !isSameDay(currentDate, prevDate));

                return (
                    <React.Fragment key={msg.id}>
                        {showDateHeader && (
                            <div className="text-start text-primary my-2">
                                {currentDate.toDateString() === new Date().toDateString()
                                    ? "Today"
                                    : currentDate.toLocaleDateString()}
                            </div>
                        )}
                        {msg.content.split("\n").map((line, index) => (
                            <div key={msg.id + "-" + index} className="message-row text-primary mb-1 ">
                                <div className="text-nowrap">
                                    <span>
                                        [{currentDate.toLocaleTimeString(undefined, { timeStyle: "short" })}]
                                    </span>{" "}
                                    <span className="fw-bold">
                                        {"<"}{msg.sendingUserName}{">"}
                                    </span>
                                </div>
                                <div className="flex-fill">
                                    {line || "\u00A0"}
                                </div>
                            </div>
                        ))}
                    </React.Fragment>
                );
            })}
            <div ref={messageEndRef} />
        </div>
    );
}