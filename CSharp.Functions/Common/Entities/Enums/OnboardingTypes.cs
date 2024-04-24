using System.ComponentModel;

namespace Common.Entities
{
    public enum OnboardingTypes
    {
        Firm, //or this is called (Directly Authorised) DA
        Ar,
        Employee,
        Provider,
        Introducer,
        Affiliate
    }

    public enum ProfileStatuses
    {
        [Description("As firm's AR only")] Basic,

        [Description("Onboarding with user account")]
        Full
    }
}