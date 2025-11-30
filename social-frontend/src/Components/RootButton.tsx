import React from "react";
import { Button } from "react-bootstrap";

interface RootButtonProps {
    onClick?: () => void;
    children?: React.ReactNode;
    className?: string;
    color?: string; 
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
    color = "primary",
    type = "button",
    keyLabel,
    disabled = false,
}: RootButtonProps) {
    return (
        <Button
            type={type}
            disabled={disabled}
            onClick={onClick}
            className={`
                ${className}
                root-btn
                bg-${color}
                text-dark
                border-0
                rounded-0      
                d-flex
                align-items-center
                justify-content-center
                gap-2
            `}
        >
            {keyLabel && (
                <span className="root-btn-key">[{keyLabel}]</span>
            )}
            <span className="root-btn-text">{children}</span>
        </Button>
    );
}
