import { useArCustomerStore } from "@/stores/useArCustomerStore";
import { storeToRefs } from "pinia";
import { DataProtectionLicenceEntity } from "@/entities/data-protection-license/DataProtectionLicenceEntity";

const customerArStore = useArCustomerStore();
const { currentArCustomer } = storeToRefs(customerArStore);

export class ArDataProtectionLicenceEntity extends DataProtectionLicenceEntity{
  public timingOfDataCollectionService: Array<{ text: string }> = [
    {
      text: `Personal data may be collected at various points throughout your interaction with ${currentArCustomer.value}. Basic personal and contact details may be collected upon your initial contact with us or upon submission of an email or website contact form, whilst more detailed personal data, including documentation and financial details, may be collected during the process of our service provision. This will usually be when we are collating the personal data required, to submit or manage, an application on your behalf.`,
    },
  ];
  public timingOfDataCollectionNonService: Array<{ text: string }> = [
    {
      text: `Personal data may be collected at various points throughout your interaction with ${currentArCustomer.value}. Basic personal and contact details may be collected upon your initial contact with us or upon submission of an email or website contact form, whilst more detailed personal data, including documentation and financial details, may be collected during the process of our service provision. This will usually be when we are collating the personal data required, to submit or manage, an application on your behalf.`,
    },
  ];
}
