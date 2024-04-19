import { CustomerPermission } from "./CustomerPermission";
import { DirectDebitEntity } from "./DirectDebitEntity";
import { SoleTraderDetails } from "./SoleTraderDetails";
import { EmbeddedSigning } from "./EmbeddedSigning";
import { FirmDetail } from "./firm-details/FirmDetail";
import { ProvidersEntity } from "@/entities/providers-and-introducers/ProvidersEntity";
import { IntroducersEntity } from "./providers-and-introducers/IntroducersEntity";
import { ProductDetails } from "./providers-and-introducers/ProductDetails";
import { IntroducerReferalProduct } from "./providers-and-introducers/IntroducerReferalProduct";
import { AppointedRepresentative } from "./appointed-representatives/AppointedRepresentative";
import { ClientMoneyEntity } from "@/entities/client-money/ClientMoneyEntity";
import { ClientMoneyAudit } from "@/entities/client-money/ClientMoneyAudit";
import { DataProtectionLicenceEntity } from "@/entities/data-protection-license/DataProtectionLicenceEntity";
import { DocumentFormatting } from "./stationery/DocumentFormatting";
import { CustomerBase } from "@/entities/CustomerBase";

//This object is Firm Representative who signed up for the first time in behalf of the company
//A company in default is a corp, but this can be a sole trader which acts like a company in FCA
export class CustomerEntity extends CustomerBase {
  public currentFcaPermissions?: CustomerPermission[];
  public customerPermissions?: CustomerPermission[];
  public isDirectDebitEmailSent: boolean = false;
  public isProposalEmailSent: boolean = false;
  public directDebit?: DirectDebitEntity;
  public soleTraderDetails?: SoleTraderDetails;
  public embeddedSigning?: EmbeddedSigning;
  public embeddedDirectDebitSigning?: EmbeddedSigning;
  public isProposalDocumentViewed: boolean = false;
  public isProposalDocumentSigned: boolean = false;
  public dateCreated?: number;
  public firmDetail?: FirmDetail;
  public providersDetails: ProvidersEntity[] = [];
  public introducerDetails: IntroducersEntity[] = [];
  public productDetails?: ProductDetails;
  public IntroducerProductDetails?: IntroducerReferalProduct;
  public appointedRepresentativeDetails: AppointedRepresentative[] = [];
  public isInProgressDataInitializing: boolean = false;
  public isInProgressProposal: boolean = false;
  public isInProgressProposalFollowup: boolean = false;
  public isInProgressDirectDebit: boolean = false;
  public isInProgressDirectDebitFollowup: boolean = false;
  public isGeneratingSigningLink: boolean = false;
  public isGeneratingDirectDebitSigningLink: boolean = false;
  public clientMonies: ClientMoneyEntity[] = [];
  public clientMoneyAudit?: ClientMoneyAudit;
  public dataProtectionLicense?: DataProtectionLicenceEntity;
  public dataProtectionLicenseCache:
    | Partial<DataProtectionLicenceEntity>
    | undefined;
  public documentFormattings: DocumentFormatting[] = [];
}
