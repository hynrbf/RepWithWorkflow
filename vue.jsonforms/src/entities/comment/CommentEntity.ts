export class CommentEntity {
  public id: string | undefined;
  public parentId?: string;
  public contentId?: string;
  public contentType?: string;
  public content?: string;
  public postId?: string;
  public postType?: string;
  public email?: string;
  public commentType?: string;
  public commentText?: string;
  public isPublic?: boolean = false;
  public createdAt?: number;
}
