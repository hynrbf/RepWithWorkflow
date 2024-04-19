import { AppointedRepresentative } from "@/entities/appointed-representatives/AppointedRepresentative";
import { AppointedRepresentativeCollection } from "@/entities/appointed-representatives/AppointedRepresentativeCollection";

export declare interface IAppointedRepresentativeService {
  getAppointedRepresentativesAsync(
    customerId?: string,
    params?: Record<string, unknown>,
  ): Promise<AppointedRepresentativeCollection>;

  getAppointedRepresentativeAsync(
    id: string,
  ): Promise<AppointedRepresentative | undefined>;

  getAppointedRepresentativesByEmailAsync(
    email: string,
  ): Promise<AppointedRepresentative>;

  saveOrUpdateAppointedRepresentativeAsync(
    financialPromotion: Partial<AppointedRepresentative>,
  ): Promise<AppointedRepresentative>;

  deleteAppointedRepresentativeAsync(id: string): Promise<boolean>;
}

export const IAppointedRepresentativeServiceInfo = {
  name: "IAppointedRepresentativeService",
};
