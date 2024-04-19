import {CompanyEntity} from "../CompanyEntity";

export declare type FormKeysSignUpPageYourDetails = {
    firstName: string;
    lastName: string;
    companyName: string;
    companyNotApplicable: string;
    homeAddress: string;
    dateOfBirth: string;
    lineOne: string;
    lineTwo: string;
    city: string;
    postCode: string;
    country: string;
    companies: CompanyEntity[] | undefined;
    email: string;
    contactNumber: string;
    isManualAddress: boolean;
}