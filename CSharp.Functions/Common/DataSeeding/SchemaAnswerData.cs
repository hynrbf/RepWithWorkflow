using Common.Entities;

namespace Common
{
    [Obsolete]
    public static class SchemaAnswerData
    {
        public static List<SchemaAnswer> GetInitialSchemaAnswersData()
        {
            return new List<SchemaAnswer>
            {
                new()
                {
                    Id = "default",
                    Page = "SignUpPage",
                    FormNameKey = "SignUpPage.YourDetailsForm",
                    Answers =
                        "{\"firstName\":\"\",\"lastName\":\"\",\"companyName\":\"\",\"companyNotApplicable\":false,\"homeAddress\":\"\",\"isManualAddress\":false,\"dateOfBirth\":\"\",\"lineOne\":\"\",\"lineTwo\":\"\",\"city\":\"\",\"postCode\":\"\",\"country\":\"\",\"companies\":[],\"email\":\"\",\"contactNumber\":\"\"}"
                }
            };
        }
    }
}