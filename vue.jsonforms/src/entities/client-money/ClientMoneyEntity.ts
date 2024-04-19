import { FileEntity } from "@/entities/FileEntity";
import { ClassificationEnum } from "./ClassificationEnum";
import { FirmRepresentative } from "@/entities/FirmRepresentative";
import { ContactNumber } from "@/entities/ContactNumber";
import { InsuranceProvider } from "./InsuranceProvider";

export class ClientMoneyEntity {
  public id: string | undefined;
  public classification: ClassificationEnum | undefined;
  public insurerRating: string | undefined;
  public providerName: string | undefined;
  public companyNumber: number | undefined;
  public firmReferenceNumber: number | undefined;
  public isPraAuthorized: boolean = false;
  public registeredAddress: string | undefined;
  public tradingAddress: string | undefined;
  public emailAddress: string | undefined;
  public contactNumber: ContactNumber | undefined;
  public website: string | undefined;
  public withBindingAuthority: boolean | null = null;
  public dateAgreed: number | undefined;

  public insuranceProviders: InsuranceProvider[] = [new InsuranceProvider()];

  public haveRiskTransferAgreement: boolean | null = null;
  public allowComminglingOfRiskTransferFunds: boolean | null = null;
  public accountType: string | undefined;

  public riskTransferAgreement: FileEntity[] | undefined;
  public StatutoryTrustLetter: FileEntity[] | undefined;
  public firmRepresentative: FirmRepresentative | undefined;

  public createdAt: number | undefined;
  public updatedAt: number | undefined;
}
