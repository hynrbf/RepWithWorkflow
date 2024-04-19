export interface EditorContent {
  content?: string;
  rawContent?: string;
  suggestions?: {
    attributes?: Record<string, unknown>;
    authorId: string;
    createdAt: number;
    data: unknown;
    hasComments: boolean;
    id: string;
    type: string;
  }[];
  commentThreads?: unknown[]; // ToDo. create type on ckeditor comment threads
}
