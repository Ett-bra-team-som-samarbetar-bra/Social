import React, { useEffect, useState } from "react";
import { AuthContext } from "./AuthContext";
import {
  type User,
  type AuthContextType,
  type LoginRequest,
  type RegisterRequest,
} from "../Types/auth";

const apiUrl = import.meta.env.VITE_API_URL;

export const AuthProvider: React.FC<{ children: React.ReactNode }> = ({
  children,
}) => {
  const [user, setUser] = useState<User | null>(null);
  const [loading, setLoading] = useState(true);

  useEffect(() => {
    fetch(`${apiUrl}/api/auth/me`, { credentials: "include" })
      .then(async (res) => (res.ok ? res.json() : null))
      .then((data) => data && setUser(data))
      .finally(() => setLoading(false));
  }, []);

  const login: AuthContextType["login"] = async (username, password) => {
    const request: LoginRequest = { Username: username, Password: password };
    const res = await fetch(`${apiUrl}/api/auth/login`, {
      method: "POST",
      credentials: "include",
      headers: { "Content-Type": "application/json" },
      body: JSON.stringify(request),
    });

    const data = await res.json();

    if (!res.ok) return { ok: false, error: data?.error || "Login failed" };

    setTimeout(() => 1000);

    setUser(data);
    return { ok: true, user: data };
  };

  const register: AuthContextType["register"] = async (
    username,
    email,
    password,
    description
  ) => {
    const request: RegisterRequest = {
      Username: username,
      Email: email,
      Password: password,
      Description: description,
    };

    const res = await fetch(`${apiUrl}/api/auth/register`, {
      method: "POST",
      credentials: "include",
      headers: { "Content-Type": "application/json" },
      body: JSON.stringify(request),
    });

    const data = await res.json();

    if (!res.ok) {
      return {
        ok: false,
        error: data?.error || "Registration failed.",
      };
    }

    return {
      ok: true,
    };
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
