import { AppointedRepresentative } from "@/entities/appointed-representatives/AppointedRepresentative";

export declare interface IAppointedRepresentativeService {
  getCustomerAppointedRepresentativeByEmailAsync(
    email: string,
  ): Promise<AppointedRepresentative>;
}

export const IAppointedRepresentativeServiceInfo = {
  name: "IAppointedRepresentativeService",
};
