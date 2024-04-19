import { FirmBasicInfo } from "@/entities/FirmBasicInfo";
import { Money } from "@/entities/Money";
import { ContactNumber } from "../ContactNumber";
import {FileEntity} from "@/entities/FileEntity";

export class EmployersLiability {
  public paymentFrequency?: string;
  public retroactiveStartDate?: number;
  public startDate?: number;
  public endDate?: number;
  public policyExclusions?: string;
  public insurer: FirmBasicInfo = new FirmBasicInfo();
  public broker: FirmBasicInfo = new FirmBasicInfo();
  public premiumAmount: Money = new Money();
  public singleIndemnityLimit: Money = new Money();
  public aggregateIndemnityLimit: Money = new Money();
  public policyExcess: Money = new Money();
  public proposalFormFile: FileEntity[] = [];
  public schedOfInsurance: FileEntity[] = [];
  public insurerRegisteredAddress?: string;
  public insurerTradingAddress?: string;
  public insurerIsTradingSameAsRegisteredAddress: boolean = false;
  public insurerEmailAddress?: string;
  public insurerContactNumber?: ContactNumber;
  public insurerWebsite?: string;
  public isBrokerResponsible: boolean | null = null;
  public isBrokerCompany: boolean | null = true;
  public brokerRegisteredAddress: string | undefined;
  public brokerTradingAddress: string | undefined;
  public brokerIsTradingSameAsRegisteredAddress: boolean = false;
  public brokerEmailAddress?: string;
  public brokerContactNumber?: ContactNumber;
  public brokerWebsite?: string;
}