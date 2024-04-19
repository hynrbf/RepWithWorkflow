export declare interface ISequenceNoKeeperService {
  setSequenceNo(sequence: number): void;

  incrementSequenceNo(): void;

  getCurrentSequenceNo(): number;
}

export const ISequenceNoKeeperServiceInfo = {
  name: "ISequenceNoKeeperService",
};
