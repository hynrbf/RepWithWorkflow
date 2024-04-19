import { CompanyDetailsBase } from "./CompanyDetailsBase";

export class CompanyDetails {
    
    //Company Details
    public companyDetails: CompanyDetailsBase[] | undefined;

    //Controlling Interests
    public controllingDetails: CompanyDetailsBase[] | undefined;

    //Financial Status
    public annualIncome: string | undefined; 
    public sourceOfIncome: string | undefined;
    public totalAssets: string | undefined;
    public totalLiabilities: string | undefined;
    public totalAmountOfWhichYouActAsAGuarantor: string | undefined;
    public financialAdditionalInformation : string | undefined;
}