import { AppConstants } from "@/infra/AppConstants";
import { ISequenceNoKeeperService } from "./ISequenceNoKeeperService";

export default class SequenceNoKeeperService
  implements ISequenceNoKeeperService
{
  setSequenceNo(sequence: number): void {
    localStorage.setItem(
      AppConstants.saveOrNextComponentSequenceKey,
      sequence.toString(),
    );
  }

  incrementSequenceNo(): void {
    const newNo = this.getCurrentSequenceNo() + 1;
    this.setSequenceNo(newNo);
  }

  getCurrentSequenceNo(): number {
    const sequence = localStorage.getItem(
      AppConstants.saveOrNextComponentSequenceKey,
    );

    if (!sequence) {
      this.resetSequenceNoToZero();
      return 0;
    }

    return parseInt(sequence);
  }

  resetSequenceNoToZero(): void {
    localStorage.setItem(
      AppConstants.saveOrNextComponentSequenceKey,
      "0".toString(),
    );
  }
}
