namespace Common.Entities
{
    public class ARAdditionalInformation
    {
        public bool? HasAnyInsurerEverDeclined { get; set; }
        public string? DeclinedInfo { get; set; }
        public List<string> DeclinedSupportingDocumentsUrls { get; set; } = new();
        public bool? HasBeenRemovedOrRefused { get; set; }
        public List<RemoveRefusedItems> HasBeenRemovedOrRefusedItems { get; set; } = new ();
        public bool? HasPreviouslyBeenAnAR { get; set; }
        public List<PreviouslyBeenAnARItems> HasPreviouslyBeenAnARItems { get; set; } = new();
        public List<PrimaryMarketCovered> PrimaryMarketCovered { get; set; } = new();
        public string? PermittedToUndertakeRegulatedActivities { get; set; }
        public bool? ContractedToPortfolioManagement { get; set; }
        public string? PortfolioManagementInfo { get; set; }
        public List<string> PortfolioManagementSupportingDocumentsUrls { get; set; } = new();
        public bool? WillBeTiedAgent { get; set; }
        public string? WillBeTiedAgentInfo { get; set; }
        public List<string> WillBeTiedAgentSupportingDocumentsUrls { get; set; } = new();
        public bool? ProvideAnyRegulatedServices { get; set; }
        public string? ProvideAnyRegulatedServicesInfo { get; set; }
        public List<string> ProvideAnyRegulatedServicesSupportingDocumentsUrls { get; set; } = new();
        public bool? ConductAnyNonRegulatedActivities { get; set; }
        public string? ConductAnyNonRegulatedActivitiesInfo { get; set; }
        public List<string> ConductAnyNonRegulatedActivitiesSupportingDocumentsUrls { get; set; } = new();
        public bool? IncludeAnyNonRegulatedFinancialServices { get; set; }
        public string? NonRegulatedFinancialServicesInfo { get; set; }
        public List<string> NonRegulatedFinancialServicesSupportingDocumentsUrls { get; set; } = new();
        public List<NonRegulatedFinancialServicesItems> NonRegulatedFinancialServicesItems { get; set; } = new();
    }

    public class RemoveRefusedItems
    {
        public string? Status { get; set; }
        public string? ProviderName { get; set; }
        public string? PartyName { get; set; }
        public long RefusedDate { get; set; }
        public string? RefusedInfo { get; set; }
        public List<string> RefusedSupportingDocumentsUrls { get; set; } = new List<string>();
    }

    public class PreviouslyBeenAnARItems
    {
        public bool? PrincipalFirmName { get; set; } 
        public string? FirmReferenceNumber { get; set; }
        public long? StartDate { get; set; }
        public long? EndDate { get; set; }
        public string? ReasonForTermination { get; set; }
    }

    public class PrimaryMarketCovered 
    {
        public string? Label { get; set; }
        public string? Value { get; set; }
        public List<PrimaryMarketCovered> Items { get; set; } = new List<PrimaryMarketCovered>();
        public string? Parent { get; set; }
    }

    public class NonRegulatedFinancialServicesItems
    {
        public string? ProductOrService { get; set; }
        public Money? TotalProjectAnnualIncome { get; set; }
    }
}
