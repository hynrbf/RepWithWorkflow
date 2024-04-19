import { v4 as uuidV4 } from "uuid";
import { ContactNumber } from "@/entities/ContactNumber";
import { CustomerBasic } from "@/entities/CustomerBasic";

export abstract class OnBoardingUserBase implements CustomerBasic {
  public id: string = uuidV4();
  public companyName?: string;
  public email?: string;
  public firstName?: string;
  public lastName?: string;
  public companyNumber?: string;
  public firmReferenceNumber?: string;

  public isFinishedSignUp: boolean = false;
  public isUserPasswordSet: boolean = false;
  public isAuthorised: boolean = false;
  public isCompanyNotApplicable: boolean = false;

  public companyAddress?: string;
  public tempPassword?: string;
  public contactNumber?: ContactNumber;
}
