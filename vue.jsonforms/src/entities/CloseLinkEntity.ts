import { ContactNumber } from "@/entities/ContactNumber";
import { CompanyDetailsBase } from "@/entities/firm-details/CompanyDetailsBase";
import { IShareholder } from "@/entities/firm-details/IShareholder";

export class CloseLinkEntity
  extends CompanyDetailsBase
  implements IShareholder
{
  public natureOfCloseLink: string | undefined;
  public registeredAddress: string | undefined;
  public tradingAddress: string | undefined;
  public isTradingAddressChanged: boolean = false;
  public isTradingSameAsRegisteredAddress: boolean = false;
  public emailAddress: string | undefined;
  public contactNumber: ContactNumber = new ContactNumber();
  public website: string | undefined;
  public percentageOfCapital: number | null = null;
  public percentageOfVotingRights: number | null = null;
}
