import {AppointedRepresentative} from "@/entities/appointed-representatives/AppointedRepresentative";

export declare type CompanyEntity = {
    companyName: string;
    address: string;
    tradingAddress: string;
    postcode: string;
    countryCode: string;
    companyNumber: string;
    firmReferenceNo: string;
    status: string; // FCA
    companyHouseStatus: string;
    type: string;
    isAuthorized: boolean;
    isConfirmedFirmDetails: boolean;
    isVariedFirmPermissions: boolean;
    isSelected: boolean;
    region: string;
    isSoleTrader: boolean;
    // use only name for appointed representative for now.
    appointedRepresentatives: AppointedRepresentative[];
    contactNumber: string | undefined;
    website: string | undefined;
    sicCode: string | undefined;
}