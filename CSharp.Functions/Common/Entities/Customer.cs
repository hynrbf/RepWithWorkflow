namespace Common.Entities
{
    //This object is Firm Representative who signed up for the first time in behalf of the company
    //A company in default is a corp, but this can be a sole trader which acts like a company in FCA
    public class Customer : CustomerBase
    {
        public bool IsDirectDebitEmailSent { get; set; }
        public bool IsProposalEmailSent { get; set; }
        public bool IsLockProposalDocument { get; set; }
        public bool IsProposalDocumentViewed { get; set; }
        public bool IsProposalDocumentSigned { get; set; }
        public bool IsProposalDocumentRejected { get; set; }
        public bool IsProposal10MinsRuleSent { get; set; }
        public bool IsProposal1HourRuleSent { get; set; }
        public bool IsProposal2DaysRuleSent { get; set; }
        public bool IsProposal7DaysRuleSent { get; set; }
        public bool IsProposal14DaysRuleSent { get; set; }
        public bool IsDirectDebitDocumentSigned { get; set; }
        public bool IsDirectDebitFollowupSent { get; set; }
        public long DateCreated { get; set; }
        public string? LastPasswordReset { get; set; }
        public SchemaAnswer? SchemaAnswer { get; set; }
        public DirectDebit? DirectDebit { get; set; }
        public List<CustomerPermission> CurrentFcaPermissions { get; set; } = new();
        public List<CustomerPermission> CustomerPermissions { get; set; } = new();
        public SoleTraderDetails? SoleTraderDetails { get; set; }
        public EmbeddedSigning? EmbeddedSigning { get; set; }
        public EmbeddedSigning? EmbeddedDirectDebitSigning { get; set; }
        public FirmDetail? FirmDetail { get; set; }
        public string? FirmProfileEditStatus { get; set; } = FirmProfileStatus.Incomplete.ToString();

        //ToDo.to recheck if bad data only because this gets error 
        //Unexpected character encountered while parsing value: [. Path '[3].ProvidersDetails[0].Products', line 1, position 63618.
        public List<Providers> ProvidersDetails { get; set; } = new();

        public List<Introducer> IntroducerDetails { get; set; } = new();
        public List<AppointedRepresentative> AppointedRepresentativeDetails { get; set; } = new();
        public List<ClientMoney> ClientMonies { get; set; } = new();
        public ClientMoneyAudit? ClientMoneyAudit { get; set; }
        public ProductDetails? ProductDetails { get; set; }
        public IntroducerReferalProduct? IntroducerProductDetails { get; set; }
        public DataProtectionLicense? DataProtectionLicenseCache { get; set; }
        public List<DocumentFormatting>? DocumentFormattings { get; set; }

        public bool IsInProgressProposal { get; set; }
        public bool IsInProgressProposalFollowup { get; set; }
        public bool IsInProgressDirectDebit { get; set; }
        public bool IsInProgressDirectDebitFollowup { get; set; }
        public bool IsGeneratingSigningLink { get; set; }
        public bool IsGeneratingDirectDebitSigningLink { get; set; }
    }
}