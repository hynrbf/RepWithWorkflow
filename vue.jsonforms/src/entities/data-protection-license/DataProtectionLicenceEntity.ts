import { DataProtectionOfficer } from "@/entities/data-protection-license/DataProtectionOfficer";
import { useCustomerStore } from "@/stores/useCustomerStore";
import { storeToRefs } from "pinia";

const customStore = useCustomerStore();
const { currentFirmName } = storeToRefs(customStore);

export class DataProtectionLicenceEntity {
  public licenseNumber: string | undefined;
  public endDate: number | undefined;
  public renewalDate: number | undefined;
  public documentUrl: string | undefined;
  public dataProtectionOfficer: DataProtectionOfficer =
    {} as DataProtectionOfficer;

  public isForAnyOtherPurpose: boolean | null = null;
  public isUsedAutomatedProcessing: boolean | null = null;

  public categoriesOfPersonalDataService: Array<{
    title: string;
    details: string[];
  }> = [
    {
      title: "Technological Details (Automatically Collected)",
      details: [
        "IP Address",
        "Web Browser Type and Version",
        "Operating System",
      ],
    },
    {
      title: "Personal Details",
      details: ["Title", "Gender", "Date of Birth", "Profession/Job Title"],
    },
    {
      title: "Payment Details",
      details: ["Bank Details", "Credit or Debit Card Details"],
    },
    {
      title: "Financial Details",
      details: [
        "Income",
        "Expenditure",
        "Assets",
        "Liabilities",
        "Financial Objectives",
      ],
    },
  ];
  public categoriesOfPersonalDataServiceConfirmed: Array<{
    title: string;
    details: string[];
  }> = [];
  public categoriesOfPersonalDataNonService: Array<{
    title: string;
    details: string[];
  }> = [
    {
      title: "Technological Details (Automatically Collected)",
      details: [
        "IP Address",
        "Web Browser Type and Version",
        "Operating System",
      ],
    },
    {
      title: "Personal Details",
      details: ["Title", "Gender", "Date of Birth", "Profession/Job Title"],
    },
    {
      title: "Payment Details",
      details: ["Bank Details", "Credit or Debit Card Details"],
    },
    {
      title: "Financial Details",
      details: [
        "Income",
        "Expenditure",
        "Assets",
        "Liabilities",
        "Financial Objectives",
      ],
    },
  ];
  public categoriesOfPersonalDataNonServiceConfirmed: Array<{
    title: string;
    details: string[];
  }> = [];

  public methodsOfDataCollectionService: Array<{
    title: string;
    text: string;
  }> = [
    {
      title: "General Statistics",
      text: "We collect statistics relating to pages visited, paths through the website, search terms used to find us. This is done to improve the visitor experience, understand our customer’s needs and help us improve site design and layout.",
    },
    {
      title: "Corresponding with Us",
      text: "We may record, use and store any telephone, postal, e-mail or other electronic communications provided by you. This is to ensure that we can refer back to any instruction you may have given to us as well as to ensure that the information we provide you with is accurate.",
    },
  ];
  public methodsOfDataCollectionServiceConfirmed: Array<{
    title: string;
    text: string;
  }> = [];
  public methodsOfDataCollectionNonService: Array<{
    title: string;
    text: string;
  }> = [
    {
      title: "General Statistics",
      text: "We collect statistics relating to pages visited, paths through the website, search terms used to find us. This is done to improve the visitor experience, understand our customer’s needs and help us improve site design and layout.",
    },
    {
      title: "Corresponding with Us",
      text: "We may record, use and store any telephone, postal, e-mail or other electronic communications provided by you. This is to ensure that we can refer back to any instruction you may have given to us as well as to ensure that the information we provide you with is accurate.",
    },
  ];
  public methodsOfDataCollectionNonServiceConfirmed: Array<{
    title: string;
    text: string;
  }> = [];

  public purposeOfDataCollectionService: Array<{ text: string }> = [
    {
      text: "Ensure that our website is compatible with the browsers and operating systems used by most of our visitors",
    },
    {
      text: "Enable us to provide you with information about our products and services",
    },
    {
      text: "Enable us to provide you with our products and services",
    },
    {
      text: "Enable us to contact you regarding general product and service level matters",
    },
    {
      text: "Keep you informed of new features, products and services available from us",
    },
  ];
  public purposeOfDataCollectionServiceConfirmed: Array<{ text: string }> = [];
  public purposeOfDataCollectionNonService: Array<{ text: string }> = [
    {
      text: "Ensure that our website is compatible with the browsers and operating systems used by most of our visitors",
    },
    {
      text: "Enable us to provide you with information about our products and services",
    },
    {
      text: "Enable us to provide you with our products and services",
    },
    {
      text: "Enable us to contact you regarding general product and service level matters",
    },
    {
      text: "Keep you informed of new features, products and services available from us",
    },
  ];
  public purposeOfDataCollectionNonServiceConfirmed: Array<{ text: string }> =
    [];

  public timingOfDataCollectionService: Array<{ text: string }> = [
    {
      text: `Personal data may be collected at various points throughout your interaction with ${currentFirmName.value}. Basic personal and contact details may be collected upon your initial contact with us or upon submission of an email or website contact form, whilst more detailed personal data, including documentation and financial details, may be collected during the process of our service provision. This will usually be when we are collating the personal data required, to submit or manage, an application on your behalf.`,
    },
  ];
  public timingOfDataCollectionServiceConfirmed: Array<{ text: string }> = [];
  public timingOfDataCollectionNonService: Array<{ text: string }> = [
    {
      text: `Personal data may be collected at various points throughout your interaction with ${currentFirmName.value}. Basic personal and contact details may be collected upon your initial contact with us or upon submission of an email or website contact form, whilst more detailed personal data, including documentation and financial details, may be collected during the process of our service provision. This will usually be when we are collating the personal data required, to submit or manage, an application on your behalf.`,
    },
  ];
  public timingOfDataCollectionNonServiceConfirmed: Array<{ text: string }> =
    [];
}
