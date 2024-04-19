using Aspose.Words;

namespace Common.Infra
{
    public abstract class AsposeLicensedServiceBase
    {
        protected AsposeLicensedServiceBase()
        {
            SetAsposeLicence();
        }

        private static void SetAsposeLicence()
        {
            var license = new License();

            try
            {
                license.SetLicense("Aspose.Total.NET.lic");
                Console.WriteLine("License set successfully.");
            }
            catch (Exception e)
            {
                // We do not ship any license with this example, visit the Aspose site to obtain either a temporary or permanent license. 
                Console.WriteLine("\nThere was an error setting the license: " + e.Message);
            }
        }
    }
}