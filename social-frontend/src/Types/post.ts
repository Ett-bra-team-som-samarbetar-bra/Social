export interface Post {
  username: string;
  createdAt: Date;
  updatedAt: Date;
  title: string;
  content: string;
  likeCount: number;
  comments: Comment[];
  isEdited: boolean;
}

/*public int UserId { get; set; }
    public required string UserName { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
    public required string Title { get; set; }
    public required string Content { get; set; }
    public int LikeCount { get; set; } = 0;
    public ICollection<CommentResponseDto> Comments { get; set; } = [];
    public bool IsEdited => CreatedAt != UpdatedAt; */
