import { CloseLinkEntity } from "@/entities/CloseLinkEntity";
import { CloseLinkModel } from "@/pages/models/close-links/CloseLinkModel";
import { ProfessionalIndemnity } from "@/entities/professional-indemnity/ProfessionalIndemnity";
import { ProfessionalIndemnityModel } from "@/pages/models/professional-indemnity-insurance/ProfessionalIndemnityModel";
import { FirmBasicInfo } from "@/entities/FirmBasicInfo";

export interface IMapperService {
  mapCloseLinkEntitiesToModels(
    closeLinkEntities: CloseLinkEntity[],
  ): CloseLinkModel[];

  mapProfessionalIndemnityEntitiesToModels(
    professionalIndemnities: ProfessionalIndemnity[],
  ): ProfessionalIndemnityModel[];

  mapCloseLinkEntityToFirmBasicInfo(
    closeLinkEntity: CloseLinkEntity,
  ): FirmBasicInfo;
}

export const IMapperServiceInfo = {
  name: "IMapperService",
};
