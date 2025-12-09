import { Col } from "react-bootstrap";
import { useEffect, useRef, useState } from "react";
import { useLocation, useNavigate } from "react-router-dom";
import { useSignalR } from "../Hooks/useSignalR";
import { useAuth } from "../Hooks/useAuth";
import { useFocus } from "../Context/FocusContext";
import { useHotKey } from "../Hooks/useHotKey";

export interface ConversationDto {
  userId: number;
  username: string;
  hasUnreadMessages: boolean;
  lastMessageAt: string;
}

export default function ConversationList() {
  const { focus } = useFocus();
  const isActiveRegion = focus.region === "right";
  const [conversations, setConversations] = useState<ConversationDto[]>([]);
  const [focused, setFocused] = useState(false);
  const [selectedUserId, setSelectedUserId] = useState<number | null>(null);
  const navigate = useNavigate();
  const { user } = useAuth();
  const listRef = useRef<HTMLDivElement>(null);
  const location = useLocation();
  const activeChatId = Number(location.pathname.split("/").pop());

  const fetchConversations = async () => {
    const res = await fetch("http://localhost:5174/api/message/conversations", {
      credentials: "include",
    });
    if (!res.ok) return;
    const data = await res.json();
    setConversations(data);
  };

  useEffect(() => {
    if (isActiveRegion) {
      setFocused(true);
      listRef.current?.focus();
    } else {
      setFocused(false);
      listRef.current?.blur();
    }
  }, [isActiveRegion]);

  useEffect(() => {
    if (!user) {
      setConversations([]);
      return;
    }
    fetchConversations();
  }, [user]);

  useEffect(() => {
    if (user && activeChatId) {
      fetchConversations();
    }
  }, [activeChatId, user]);

  useSignalR(user?.id ?? 0, () => {
    fetchConversations();
  });

  useEffect(() => {
    if (activeChatId) {
      setSelectedUserId(activeChatId);
    }
  }, [activeChatId]);

  useHotKey(
    "arrowdown",
    () => {
      if (!isActiveRegion) return;
      if (!conversations.length) return;

      const currentIndex =
        selectedUserId === null
          ? -1
          : conversations.findIndex((c) => c.userId === selectedUserId);

      const nextIndex =
        currentIndex < 0
          ? 0
          : Math.min(currentIndex + 1, conversations.length - 1);

      setSelectedUserId(conversations[nextIndex].userId);
    },
    "local",
    "right"
  );

  useHotKey(
    "arrowup",
    () => {
      if (!isActiveRegion) return;
      if (!conversations.length) return;

      const currentIndex =
        selectedUserId === null
          ? -1
          : conversations.findIndex((c) => c.userId === selectedUserId);

      const prevIndex = currentIndex < 0 ? 0 : Math.max(currentIndex - 1, 0);

      setSelectedUserId(conversations[prevIndex].userId);
    },
    "local",
    "right"
  );

  useHotKey(
    " ",
    () => {
      if (!isActiveRegion) return;
      if (!selectedUserId) return;

      navigate(`/messages/${selectedUserId}`);
    },
    "local",
    "right"
  );

  useHotKey(
    "escape",
    () => {
      if (!isActiveRegion) return;

      setFocused(false);
      listRef.current?.blur();
    },
    "local",
    "right"
  );

  const messageHeading = user ? "[M]Messages" : "[░▒▓]Mess■ges̴͊";

  return (
    <Col className="conversation-aside ">
      <h5 className="text-primary mb-3 keybind-header">{messageHeading}</h5>
      <div
        ref={listRef}
        tabIndex={0}
        className={`conversation-list ${focused ? "focused" : ""}`}
        onBlur={() => setFocused(false)}
      >
        {user && (
          <>
            <p className="text-primary">Hit [SPACE] to choose</p>
            <p className="text-primary">Hit [ESC] to escape</p>
          </>
        )}

        {conversations.map((c) => {
          const isSelected = selectedUserId === c.userId;

          return (
            <div
              key={c.userId}
              className={`conversation-item ${isSelected ? "selected" : ""}`}
              onClick={() => {
                setSelectedUserId(c.userId);
                navigate(`/messages/${c.userId}`);
              }}
            >
                {user && (
                    <>
                        <p className="text-primary m-0">[SPACE] to choose</p>
                        <p className="text-primary">[ESC] to escape</p>
                    </>
                )}

                {conversations.map((c) => {
                    const isSelected = selectedUserId === c.userId;

                    return (
                        <div
                            key={c.userId}
                            className={`conversation-item ${isSelected ? "selected" : ""}`}
                            onClick={() => {
                                setSelectedUserId(c.userId);
                                navigate(`/messages/${c.userId}`);
                            }}
                        >
                            {isSelected ? "> " : "  "}
                            @{c.username}{" "}
                            {c.hasUnreadMessages && selectedUserId !== c.userId && (
                                <span className="text-primary">⬤</span>
                            )}
                        </div>
                    );
                })}
              {isSelected ? "> " : "  "}@{c.username}{" "}
              {c.hasUnreadMessages && selectedUserId !== c.userId && (
                <span className="text-primary">⬤</span>
              )}
            </div>
          );
        })}
      </div>
    </Col>
  );
}
