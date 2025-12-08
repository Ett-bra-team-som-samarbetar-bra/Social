export interface Post {
  id: number;
  userId: number;
  username: string;
  createdAt: Date;
  updatedAt: Date;
  title: string;
  content: string;
  likeCount: number;
  comments: Comment[];
  isEdited: boolean;
}

export interface PostCreateDto {
  title: string;
  content: string;
}

export interface Comment {
  userId: number;
  username: string;
  content: string;
  createdAt: string;
}
