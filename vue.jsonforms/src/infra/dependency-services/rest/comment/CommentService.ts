import { singleton } from "tsyringe";
import RestBase from "../RestBase";
import { REMOTE_API } from "@/config";
import { ICommentService } from "./ICommentService";
import { CommentEntity } from "@/entities/comment/CommentEntity";

@singleton()
export default class CommentService
  extends RestBase
  implements ICommentService
{
  async getCommentsAsync(
    contentId?: string,
    contentType?: string
  ): Promise<CommentEntity[]> {
    try {
      return await this.getRemoteAsync<CommentEntity[]>(
        `${REMOTE_API}/GetCommentsAsync?contentId=${contentId}&contentType=${contentType}`
      );
    } catch (error) {
      return Promise.reject(error);
    }
  }

  async addOrEditCommentAsync(
    payload: Partial<CommentEntity>
  ): Promise<CommentEntity> {
    try {
      return await this.postRemoteAsync<CommentEntity>(
        `${REMOTE_API}/AddOrEditCommentAsync`,
        payload
      );
    } catch (error) {
      return Promise.reject(error);
    }
  }
}
