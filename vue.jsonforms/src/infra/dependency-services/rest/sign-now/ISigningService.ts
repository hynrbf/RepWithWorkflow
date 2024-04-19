export declare interface ISigningService {
  sendInviteToSignDocument(receiverEmail: string): Promise<boolean>;
}

export const ISigningServiceInfo = {
  name: "ISigningService",
};