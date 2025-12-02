import React, { useEffect, useState } from "react";
import { AuthContext } from "./AuthContext";
import { type User, type AuthContextType } from "../Types/auth";

export const AuthProvider: React.FC<{ children: React.ReactNode }> = ({ children }) => {
  const [user, setUser] = useState<User | null>(null);
  const [loading, setLoading] = useState(true);

  useEffect(() => {
    fetch("/api/auth/me", { credentials: "include" })
      .then(async (res) => (res.ok ? res.json() : null))
      .then((data) => data && setUser(data))
      .finally(() => setLoading(false));
  }, []);

  const login: AuthContextType["login"] = async (username, password) => {
    const res = await fetch("/api/auth/login", {
      method: "POST",
      credentials: "include",
      headers: { "Content-Type": "application/json" },
      body: JSON.stringify({ username, password }),
    });

    if (!res.ok) return false;

    const loggedInuser = await res.json();
    setUser(loggedInuser);
    return true;
  };

  const register: AuthContextType["register"] = async (username, email, password, description) => {
    const res = await fetch("/api/auth/register", {
      method: "POST",
      credentials: "include",
      headers: { "Content-Type": "application/json" },
      body: JSON.stringify({ username, email, password, description }),
    });

    if (!res.ok) return false;

    const newUser = await res.json();
    setUser(newUser);
    return true;
  };

  const logout = async () => {
    await fetch("/api/auth/logout", {
      method: "POST",
      credentials: "include",
    });

    setUser(null);
  };

  return (
    <AuthContext.Provider value={{ user, loading, login, register, logout }}>
      {children}
    </AuthContext.Provider>
  );
};
