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

/*public record PostCreateDto
{
    public required string Title { get; set; }
    public required string Content { get; set; }
}

public record PostEditDto
{
    public int Id { get; set; }
    public required string Content { get; set; }
} */
