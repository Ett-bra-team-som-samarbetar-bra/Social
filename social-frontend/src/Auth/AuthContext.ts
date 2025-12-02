import { createContext } from "react";
import { type AuthContextType } from "../Types/auth";

export const AuthContext = createContext<AuthContextType | undefined>(undefined);
