import { useContext } from "react";
import { type AuthContextType } from "../Types/auth";
import { AuthContext } from "../Auth/AuthContext";

export const useAuth = (): AuthContextType => {
  const ctx = useContext(AuthContext);
  if (!ctx) throw new Error("useAuth must be used inside a AuthProvider");
  return ctx;
};
