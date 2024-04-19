import { defineStore } from "pinia";
import { container } from "tsyringe";
import {
  ICommentService,
  ICommentServiceInfo,
} from "@/infra/dependency-services/rest/comment/ICommentService";
import { CommentEntity } from "@/entities/comment/CommentEntity";
import { AppConstants } from "@/infra/AppConstants";

export interface State {
  comments: CommentEntity[];
}

const commentService = container.resolve<ICommentService>(
  ICommentServiceInfo.name,
);

export const useCommentStore = defineStore(AppConstants.commentStore, {
  state: (): State => ({
    comments: [],
  }),
  actions: {
    async fetchCommentsAsync(
      contentId?: string,
      contentType?: string,
    ): Promise<CommentEntity[]> {
      try {
        this.comments = await commentService.getCommentsAsync(
          contentId,
          contentType,
        );
        return Promise.resolve(this.comments);
      } catch (error) {
        return Promise.reject(error);
      }
    },
    async addOrEditCommentAsync(
      payload: Partial<CommentEntity>,
      cache = true,
    ): Promise<CommentEntity> {
      try {
        const response = await commentService.addOrEditCommentAsync(payload);
        if (cache) {
          this.comments = [...this.comments, response];
        }
        return Promise.resolve(response);
      } catch (error) {
        return Promise.reject(error);
      }
    },
  },
  // ToDo. Remove after api is integrated.
  persist: true,
});
