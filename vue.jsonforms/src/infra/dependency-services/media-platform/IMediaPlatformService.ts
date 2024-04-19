import { MediaPlatform } from "@/entities/MediaPlatform";

export declare interface IMediaPlatformService {
  getMediaPlatforms(): MediaPlatform[];
  getMediaPlatform(id: string): MediaPlatform | undefined;
  validateUrl(string: string, id?: string): boolean;
}

export const IMediaPlatformServiceInfo = {
  name: "IMediaPlatformService",
};
