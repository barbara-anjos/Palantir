using System.Runtime.CompilerServices;

namespace Palantir.Api.Configurations
{
    public class HubSpotSettings
    {
        public string ApiToken { get; set; }
        public string BaseUrl { get; set; }
        public string GestaoPipeline { get; set; }
        public string AutomacaoPipeline { get; set; }
		public string InfraPipeline { get; set; }
	}
}
