using Common.Entities;

namespace Common
{
    public static class SchemaData
    {
        public static List<SchemaModel> GetInitialSchemaData()
        {
            return new List<SchemaModel>
            {
                new()
                {
                    Id = "200071b9-7577-4488-a2cd-487cf5baf746",
                    Page = "SignUpPage",
                    FormNameKey = "SignUpPage.YourDetailsForm",
                    Schema = "{\"properties\":{\"isManualAddress\":{\"title\":\"Home Address\",\"type\":\"boolean\"},\"firstName\":{\"title\":\"Forename(s)\",\"type\":\"string\",\"tooltip\":\"Tooltip your name\"},\"lastName\":{\"title\":\"Surname\",\"type\":\"string\"},\"companyName\":{\"title\":\"Company Name\",\"type\":\"string\",\"enum\":[\"Axa Aofas (Postcode KT2 5RG)\",\"Axa Financial Services LTD (Postcode: EC3V 3ND)\"]},\"companyNotApplicable\":{\"title\":\"Not Applicable e.g. Sole Trader\",\"type\":\"boolean\"},\"homeAddress\":{\"title\":\"Home Address\",\"type\":\"string\",\"enum\":[\"SW1A 1AA\",\"EC2N 2DB\"]},\"dateOfBirth\":{\"title\":\"Date of Birth\",\"type\":\"string\"},\"empty\":{\"title\":\"Home Address\"},\"lineOne\":{\"title\":\"Line 1\",\"type\":\"string\"},\"lineTwo\":{\"title\":\"Line 2\",\"type\":\"string\"},\"city\":{\"title\":\"City\",\"type\":\"string\",\"enum\":[\"Abbey Wood\",\"Barnes\"]},\"postCode\":{\"title\":\"Postcode\",\"type\":\"string\"},\"country\":{\"title\":\"Country\",\"type\":\"string\",\"enum\":[\"UK\",\"PH\"]},\"companies\":{\"type\":\"array\",\"items\":{\"type\":\"object\",\"properties\":{\"companyName\":{\"type\":\"string\"},\"postcode\":{\"type\":\"string\"},\"companyNumber\":{\"type\":\"string\"},\"firmReferenceNo\":{\"type\":\"string\"},\"role\":{\"type\":\"string\"},\"isConfirmedFirmDetails\":{\"type\":\"boolean\"},\"isVariedFirmPermissions\":{\"type\":\"boolean\"},\"isSelected\":{\"type\":\"boolean\"}}}},\"email\":{\"title\":\"Email Address\",\"type\":\"string\"},\"contactNumber\":{\"title\":\"Contact Number\",\"type\":\"string\"}},\"required\":[\"firstName\",\"lastName\",\"email\",\"contactNumber\"]}"
                },
                new()
                {
                    Id = "66ae74de-2b13-46f1-9462-2821e096e60b",
                    Page = "SignUpPage",
                    FormNameKey = "SignUpPage.FcaStatusForm",
                    Schema =
                        "{\"type\":\"object\",\"properties\":{\"radioGroup\":{\"title\":\"My Radio\",\"type\":\"string\",\"enum\":[\"Richdale LTD (Postcode KT2 5RG)\",\"Richdale Broker and Financial Services LTD (Postcode: EC3V 3ND)\",\"Richdale Consultants LTD (Postcode: EC3V 3ND)\",\"Richdale Health Centres Limited (Postcode: UB6 7AZ)\",\"Richdale Holdings Limited (Postcode: N3 1DH)\",\"Richdale Management Company Limited (Postcode: BT1 1FH)\"]}}}"
                },
                new()
                {
                    Id = "7b731cce-8b47-4a00-8f5d-0b30f104be02",
                    Page = "SignUpPage",
                    FormNameKey = "SignUpPage.DirectDebit",
                    Schema =
                        "{\"properties\":{\"directDebitName\":{\"title\":\"Name\",\"type\":\"string\",\"tooltip\":\"\"},\"directDebitPosition\":{\"title\":\"Position\",\"type\":\"string\",\"tooltip\":\"\"},\"directDebitEmail\":{\"title\":\"Email\",\"type\":\"string\",\"tooltip\":\"\"},\"bankSortCode\":{\"title\":\"Bank Sort Code\",\"type\":\"string\",\"tooltip\":\"\"},\"accountNumber\":{\"title\":\"Account Number\",\"type\":\"string\",\"tooltip\":\"\"},\"accountName\":{\"title\":\"Account Name\",\"type\":\"string\",\"tooltip\":\"\"},\"collectionRef\":{\"title\":\"Collections will be made using this Reference\",\"type\":\"string\",\"tooltip\":\"\"}},\"required\":[\"directDebitName\",\"directDebitPosition\",\"directDebitEmail\",\"bankSortCode\",\"accountNumber\",\"accountName\",\"collectionRef\"]}"
                }
            };
        }

        public static string GetGuideInitialSchemaData()
        {
            return
                "{\"properties\":{\"name\":{\"title\":\"Full Name\",\"type\":\"string\",\"minLength\":1},\"nationality\":{\"title\":\"Nationality\",\"type\":\"string\",\"enum\":[\"Filipino\",\"Egyptian\",\"French\",\"Hungarian\",\"German\",\"English\"]},\"address\":{\"title\":\"Address\",\"type\":\"string\"},\"age\":{\"title\":\"Age\",\"type\":\"integer\",\"minimum\":0},\"birthday\":{\"title\":\"Birthday\",\"type\":\"string\",\"format\":\"date\"},\"question\":{\"title\":\"Question\",\"type\":\"string\"}},\"required\":[\"name\",\"nationality\",\"address\",\"age\",\"birthday\"]}";
        }

        public static string GetGuideInitialUiSchemaData()
        {
            return
                "{\"type\":\"Group\",\"elements\":[{\"elements\":[{\"type\":\"Control\",\"scope\":\"#/properties/name\"},{\"type\":\"Control\",\"scope\":\"#/properties/nationality\"},{\"type\":\"Control\",\"scope\":\"#/properties/address\",\"options\":{\"multi\":true}}]},{\"type\":\"VerticalLayout\",\"elements\":[{\"type\":\"Control\",\"scope\":\"#/properties/age\"},{\"type\":\"Control\",\"scope\":\"#/properties/birthday\",\"options\":{\"yearsRange\":[2020,2021]}},{\"type\":\"Control\",\"scope\":\"#/properties/question\",\"rule\":{\"effect\":\"SHOW\",\"condition\":{\"scope\":\"#/properties/age\",\"schema\":{\"minimum\":24}}}}]}]}";

        }
    }
}