import { CommentEntity } from "@/entities/comment/CommentEntity";

export declare interface ICommentService {
  getCommentsAsync(
    contentId?: string,
    contentType?: string
  ): Promise<CommentEntity[]>;

  addOrEditCommentAsync(payload: Partial<CommentEntity>): Promise<CommentEntity>;
}

export const ICommentServiceInfo = {
  name: "ICommentService",
};
