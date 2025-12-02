import type { Message } from "./message";
import type { Post } from "./post";

export interface User {
  Id: string;
  Username: string;
  Email: string;
  Description: string;
  CreatedAt: Date;
  Following: User[];
  Followers: User[];
  Messages: Message[];
  MessaagesReceived: Message[];
  LikedPosts: Post[];
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
  login: (email: string, password: string) => Promise<boolean>;
  register: (
    username: string,
    email: string,
    password: string,
    description: string
  ) => Promise<boolean>;
  logout: () => Promise<void>;
}
