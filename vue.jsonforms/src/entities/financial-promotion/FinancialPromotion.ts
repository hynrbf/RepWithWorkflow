import { FileEntity } from "@/entities/FileEntity";
import { EditorContent } from "./EditorContent";
import { FinancialPromotionStatus } from "./FinancialPromotionStatus";

export class FinancialPromotion {
  id: string = "";
  mediaOutlet: string | undefined;
  contentUrl: string | undefined;
  structuredContent: string | undefined;
  content:
    | {
        textContent: Record<string, unknown>; // ToDo. Create exact type
        documents: unknown[]; // ToDo. Create exact type
        images: unknown[]; // ToDo. Create exact type
        videos: unknown[]; // ToDo. Create exact type
      }
    | undefined;
  editorContent: EditorContent | undefined;
  owner: string | undefined;
  moderator: string | undefined;
  approvalStatus: FinancialPromotionStatus = FinancialPromotionStatus.Pending;
  approvalDays: number | undefined;
  isLive: boolean | undefined;
  media: Array<FileEntity> | undefined;
  type: string | undefined;
  productType: unknown[] = []; // ToDo. create type for product type list
  remunerationMethod: string | undefined;
  provider: string | undefined;
  createdAt: number | undefined;
  updatedAt: number | undefined;
  approvedAt: number | undefined;
  rejectedAt: number | undefined;
  isDisclosureConfirmed: boolean = false;
  customerId: string | undefined;
}
