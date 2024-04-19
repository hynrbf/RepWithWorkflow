using Common;
using Common.Infra;

namespace Api.Infra
{
    public class GuideSchemaAndUiRepository : IGuideSchemaAndUiRepository
    {
        public string GetSchema()
            => SchemaData.GetGuideInitialSchemaData();

        public string GetUiSchema()
             => SchemaData.GetGuideInitialUiSchemaData();
    }
}