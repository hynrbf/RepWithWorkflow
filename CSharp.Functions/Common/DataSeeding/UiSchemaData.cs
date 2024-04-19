using Common.Entities;

namespace Common
{
    [Obsolete]
    public static class UiSchemaData
    {
        public static List<UiSchemaModel> GetInitialUiSchemaData()
        {
            return new List<UiSchemaModel>
            {
                new UiSchemaModel
                {
                    Id = "cb975da2-69dd-48bc-9b63-1f0f0ffacdcb",
                    Page = "SignUpPage",
                    FormNameKey = "SignUpPage.YourDetailsForm",
                    UiSchema = "{\"type\":\"Group\",\"elements\":[{\"type\":\"HorizontalLayout\",\"elements\":[{\"type\":\"Control\",\"scope\":\"#/properties/firstName\",\"options\":{\"placeholder\":\"Type ...\"}},{\"type\":\"Control\",\"scope\":\"#/properties/lastName\",\"options\":{\"placeholder\":\"Type ...\"}}]},{\"type\":\"Control\",\"scope\":\"#/properties/companyName\",\"options\":{\"placeholder\":\"Type ...\"},\"rule\":{\"effect\":\"DISABLE\",\"condition\":{\"scope\":\"#/properties/companyNotApplicable\",\"schema\":{\"const\":true}}}},{\"type\":\"Control\",\"scope\":\"#/properties/companyNotApplicable\"},{\"type\":\"VerticalLayout\",\"elements\":[{\"type\":\"HorizontalLayout\",\"elements\":[{\"type\":\"Control\",\"scope\":\"#/properties/homeAddress\"},{\"type\":\"Control\",\"scope\":\"#/properties/dateOfBirth\",\"options\":{\"format\":\"date\"}}],\"rule\":{\"effect\":\"HIDE\",\"condition\":{\"scope\":\"#/properties/isManualAddress\",\"schema\":{\"const\":true}}}},{\"type\":\"VerticalLayout\",\"elements\":[{\"type\":\"Control\",\"scope\":\"#/properties/empty\"},{\"type\":\"HorizontalLayout\",\"elements\":[{\"type\":\"Control\",\"scope\":\"#/properties/lineOne\"},{\"type\":\"Control\",\"scope\":\"#/properties/lineTwo\"},{\"type\":\"Control\",\"scope\":\"#/properties/city\"}]}],\"rule\":{\"effect\":\"SHOW\",\"condition\":{\"scope\":\"#/properties/isManualAddress\",\"schema\":{\"const\":true}}}},{\"type\":\"HorizontalLayout\",\"elements\":[{\"type\":\"Control\",\"scope\":\"#/properties/postCode\"},{\"type\":\"Control\",\"scope\":\"#/properties/country\"},{\"type\":\"Control\",\"scope\":\"#/properties/dateOfBirth\",\"options\":{\"format\":\"date\"}}],\"rule\":{\"effect\":\"SHOW\",\"condition\":{\"scope\":\"#/properties/isManualAddress\",\"schema\":{\"const\":true}}}}],\"rule\":{\"effect\":\"SHOW\",\"condition\":{\"scope\":\"#/properties/companyNotApplicable\",\"schema\":{\"const\":true}}}},{\"type\":\"Control\",\"scope\":\"#/properties/companies\"},{\"type\":\"Control\",\"scope\":\"#/properties/email\",\"options\":{\"placeholder\":\"Type ...\"}},{\"type\":\"Control\",\"scope\":\"#/properties/contactNumber\",\"options\":{\"placeholder\":\"07777777777\"}}]}"
                },
                new UiSchemaModel
                {
                    Id = "82a7248e-428b-421a-b42f-fc40143eae6a",
                    Page = "SignUpPage",
                    FormNameKey = "SignUpPage.FcaStatusForm",
                    UiSchema = "{\"type\":\"VerticalLayout\",\"elements\":[{\"type\":\"Control\",\"scope\":\"#/properties/radioGroup\",\"options\":{\"format\":\"radio\"}}]}"
                },
                new UiSchemaModel
                {
                    Id = "8811ae19-88c5-4437-9395-32983e763c48",
                    Page = "SignUpPage",
                    FormNameKey = "SignUpPage.DirectDebit",
                    UiSchema = "{\"type\":\"Group\",\"elements\":[{\"elements\":[{\"type\":\"Control\",\"scope\":\"#/properties/directDebitName\"},{\"type\":\"Control\",\"scope\":\"#/properties/directDebitPosition\"},{\"type\":\"Control\",\"scope\":\"#/properties/directDebitEmail\"},{\"type\":\"Control\",\"scope\":\"#/properties/bankSortCode\"},{\"type\":\"Control\",\"scope\":\"#/properties/accountNumber\"},{\"type\":\"Control\",\"scope\":\"#/properties/accountName\"},{\"type\":\"Control\",\"scope\":\"#/properties/collectionRef\"}]}]}"
                }
            };
        }
    }
}
