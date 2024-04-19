import { CloseLinkEntity } from "@/entities/CloseLinkEntity";
import { ProfessionalIndemnity } from "@/entities/professional-indemnity/ProfessionalIndemnity";
import { IMapperService } from "@/infra/dependency-services/mapper/IMapperService";
import { CloseLinkModel } from "@/pages/models/close-links/CloseLinkModel";
import { ProfessionalIndemnityModel } from "@/pages/models/professional-indemnity-insurance/ProfessionalIndemnityModel";
import { v4 as uuid } from "uuid";
import { FirmBasicInfo } from "@/entities/FirmBasicInfo";

export class MapperService implements IMapperService {
  mapProfessionalIndemnityEntitiesToModels(
    professionalIndemnities: ProfessionalIndemnity[],
  ): ProfessionalIndemnityModel[] {
    const models = [] as ProfessionalIndemnityModel[];

    if (!professionalIndemnities?.length) {
      return models;
    }

    for (const professionalIndemnityItem of professionalIndemnities) {
      const model = new ProfessionalIndemnityModel(professionalIndemnityItem);
      model.piiInsurerInputFirm = {
        firmName: professionalIndemnityItem.insurerName,
        companyNumber: professionalIndemnityItem.companyNumber,
        firmReferenceNumber: professionalIndemnityItem.firmReferenceNumber,
        address: professionalIndemnityItem.insurerRegisteredAddress,
        tradingAddress: professionalIndemnityItem.insurerTradingAddress,
        contactNumber: "",
        website: "",
        fcaStatus: "",
        companyHouseStatus: "",
      };
      model.piiBrokerInputFirm = {
        firmName: professionalIndemnityItem.piiBrokerName,
        companyNumber: professionalIndemnityItem.brokerCompanyNumber,
        firmReferenceNumber:
          professionalIndemnityItem.brokerFirmReferenceNumber,
        address: professionalIndemnityItem.brokerRegisteredAddress,
        tradingAddress: professionalIndemnityItem.brokerTradingAddress,
        contactNumber: "",
        website: "",
        fcaStatus: "",
        companyHouseStatus: "",
      };

      models.push(model);
    }

    return models;
  }

  mapCloseLinkEntitiesToModels(
    closeLinkEntities: CloseLinkEntity[],
  ): CloseLinkModel[] {
    if (!closeLinkEntities?.length) {
      return [] as CloseLinkModel[];
    }

    return closeLinkEntities.map((entity) => {
      entity.firm = this.mapCloseLinkEntityToFirmBasicInfo(entity);

      return {
        id: uuid(),
        ...entity,
      } as CloseLinkModel;
    });
  }

  mapCloseLinkEntityToFirmBasicInfo(
    closeLinkEntity: CloseLinkEntity,
  ): FirmBasicInfo {
    return {
      firmName: closeLinkEntity?.companyName,
      companyNumber: closeLinkEntity?.companyNumber,
      firmReferenceNumber: closeLinkEntity?.firmReferenceNumber,
      address: closeLinkEntity?.registeredAddress,
      tradingAddress: closeLinkEntity?.tradingAddress,
      countryCode: closeLinkEntity?.contactNumber?.countryCode,
      website: closeLinkEntity?.website,
      sicCode: closeLinkEntity?.natureOfBusiness,
      tradingNames: [],
      headOfficeAddress: "",
      contactNumber: "",
      fcaStatus: "",
      companyHouseStatus: "",
    };
  }
}
