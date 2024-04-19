import { FileEntity } from "@/entities/FileEntity";

export class FileModel extends FileEntity {
    public uid?: string; 
    public uploadedUrl?: string;
}