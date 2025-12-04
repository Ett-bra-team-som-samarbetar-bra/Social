import type MessageDto from "./message";
import type { Post } from "./post";

export interface User {
  id: number;
  username: string;
  email: string;
  description: string;
  createdAt: Date;
  following: User[];
  followers: User[];
  messages: MessageDto[];
  messaagesReceived: MessageDto[];
  likedPosts: Post[];
}

export interface LoginRequest {
  username: string;
  password: string;
}

export interface RegisterRequest {
  username: string;
  email: string;
  password: string;
  description: string;
}

export interface UpdatePasswordRequest {
  newPassword: string;
}

export interface UserIdRequest {
  userId: number;
}

export interface AuthContextType {
  user: User | null;
  loading: boolean;
  login: (email: string, password: string) => Promise<boolean>;
  register: (
    username: string,
    email: string,
    password: string,
    description: string
  ) => Promise<boolean>;
  logout: () => Promise<void>;
}
