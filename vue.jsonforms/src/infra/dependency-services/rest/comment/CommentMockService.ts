import { singleton } from "tsyringe";
import { ICommentService } from "./ICommentService";
import { CommentEntity } from "@/entities/comment/CommentEntity";
import { v4 as uuid } from "uuid";
import moment from "moment";

@singleton()
export default class CommentMockService implements ICommentService {
  private _comments: CommentEntity[] = [];

  public getCommentsAsync(
    contentId?: string,
    contentType?: string
  ): Promise<CommentEntity[]> {
    return Promise.resolve(
      this._comments.filter(
        (comment) =>
          comment.contentId === contentId && comment.contentType === contentType
      )
    );
  }

  public addOrEditCommentAsync(
    payload: Partial<CommentEntity>
  ): Promise<CommentEntity> {
    let updatedComment: CommentEntity | undefined;

    this._comments = this._comments.map((comment) => {
      if (payload.id === comment.id) {
        return (updatedComment = {
          ...comment,
          ...payload,
        });
      }
      return comment;
    });

    if (!updatedComment) {
      const newComment: CommentEntity = {
        id: uuid(),
        ...payload,
        createdAt: moment().unix(),
      };
      this._comments = [...this._comments, newComment];
      return Promise.resolve(newComment);
    }

    return Promise.resolve(updatedComment);
  }
}
