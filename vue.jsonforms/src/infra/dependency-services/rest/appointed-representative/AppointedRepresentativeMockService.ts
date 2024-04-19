import { singleton } from "tsyringe";
import { IAppointedRepresentativeService } from "./IAppointedRepresentativeService";
import { AppointedRepresentative } from "@/entities/appointed-representatives/AppointedRepresentative";
import { AppointedRepresentativeCollection } from "@/entities/appointed-representatives/AppointedRepresentativeCollection";
import { Error } from "@progress/kendo-vue-labels";

@singleton()
export default class AppointedRepresentativeMockService
  implements IAppointedRepresentativeService
{
  private _appointedRepresentatives: AppointedRepresentative[] = [];

  public getAppointedRepresentativeAsync(
    id: string,
  ): Promise<AppointedRepresentative | undefined> {
    return Promise.resolve(
      this._appointedRepresentatives.find((item) => item.id === id),
    );
  }

  getAppointedRepresentativesAsync(
    _customerId?: string | undefined,
  ): Promise<AppointedRepresentativeCollection> {
    return Promise.resolve({
      data: this._appointedRepresentatives,
      total: this._appointedRepresentatives.length,
    });
  }

  saveOrUpdateAppointedRepresentativeAsync(
    _appointedRepresentative: Partial<AppointedRepresentative>,
  ): Promise<AppointedRepresentative> {
    throw new Error("Method not implemented.");
  }

  deleteAppointedRepresentativeAsync(_id: string): Promise<boolean> {
    throw new Error("Method not implemented.");
  }

  getAppointedRepresentativesByEmailAsync(
    _email: string,
  ): Promise<AppointedRepresentative> {
    throw new Error("Method not implemented.");
  }
}
