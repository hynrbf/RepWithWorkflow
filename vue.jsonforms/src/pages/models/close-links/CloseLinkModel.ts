import { CloseLinkEntity } from "@/entities/CloseLinkEntity";
import { v4 as uuid } from "uuid";

export class CloseLinkModel extends CloseLinkEntity {
  public id: string = uuid();
}