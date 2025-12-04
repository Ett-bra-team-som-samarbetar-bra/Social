import React from "react";
import { Button } from "react-bootstrap";
import { useHotKey } from "../Hooks/useHotKey";

interface RootButtonProps {
    onClick?: () => void;
    children?: React.ReactNode;
    backgroundColor?: string;
    textColor?: string;
    fontsize?: number;
    className?: string;
    keyLabel?: string;
    rounded?: string;
    disabled?: boolean;
    size?: "sm" | "md" | "lg";
    type?: "button" | "submit" | "reset";
}

export default function RootButton({
    onClick,
    children,
    className = "",
    textColor = "dark",
    backgroundColor = "primary",
    fontsize = 0.6,
    type = "button",
    keyLabel,
    disabled = false,
}: RootButtonProps) {

    useHotKey(keyLabel, onClick || (() => { }));

    return (
        <Button
            type={type}
            disabled={disabled}
            onClick={onClick}
            style={{ fontSize: `${fontsize}rem` }}
            className={`
                ${className}
                root-btn
                bg-${backgroundColor}
                text-${textColor}
                border-0
                rounded-0      
                d-flex
                align-items-center
                justify-content-center
                gap-2
            `}
        >
            {keyLabel && (
                <span className="fw-bold">[{keyLabel}]</span>
            )}
            <span className="fw-bold">{children}</span>
        </Button>
    );
}
