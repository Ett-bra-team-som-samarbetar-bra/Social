import type { Message } from "./message";
import type { Post } from "./post";

export interface User {
  id: string;
  username: string;
  email: string;
  description: string;
  createdAt: Date;
  following: User[];
  followers: User[];
  messages: Message[];
  messaagesReceived: Message[];
  likedPosts: Post[];
}

export interface LoginRequest {
  Username: string;
  Password: string;
}

export interface RegisterRequest {
  Username: string;
  Email: string;
  Password: string;
  Description: string;
}

export interface UpdatePasswordRequest {
  NewPassword: string;
}

export interface UserIdRequest {
  UserId: number;
}

export interface AuthContextType {
  user: User | null;
  loading: boolean;
  login: (
    email: string,
    password: string
  ) => Promise<{ ok: boolean; error?: string }>;
  register: (
    username: string,
    email: string,
    password: string,
    description: string
  ) => Promise<{ ok: boolean; error?: string }>;
  logout: () => Promise<void>;
}
