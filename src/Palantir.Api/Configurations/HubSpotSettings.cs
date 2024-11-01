using System.Runtime.CompilerServices;

namespace Palantir.Api.Configurations
{
    public class HubSpotSettings
    {
        public string ApiKey { get; set; }
        public string BaseUrl { get; set; }
        public string PropertiesUrl { get; set; }
		public string GestaoPipelineId { get; set; }
		public string GestaoNovoStageId { get; set; }
		public string GestaoExecutandoStageId { get; set; }
		public string GestaoBloqueadoStageId { get; set; }
		public string GestaoComunicaoStageId { get; set; }
		public string GestaoTestarStageId { get; set; }
		public string GestaoTestandoStageId { get; set; }
		public string GestaoReprovadoStageId { get; set; }
		public string GestaoAprovadoStageId { get; set; }
		public string GestaoConcluidoStageId { get; set; }
		public string AutomacaoPipelineId { get; set; }
		public string AutomacaoExecutandoStageId { get; set; }
		public string AutomacaoBloqueadoStageId { get; set; }
		public string AutomacaoComunicaoStageId { get; set; }
		public string AutomacaoTestarStageId { get; set; }
		public string AutomacaoTestandoStageId { get; set; }
		public string AutomacaoReprovadoStageId { get; set; }
		public string AutomacaoAprovadoStageId { get; set; }
		public string AutomacaoConcluidoStageId { get; set; }
		public string AutomacaoNovoStageId { get; set; }
		public string InfraPipelineId { get; set; }
		public string InfraNovoStageId { get; set; }
		public string InfraEmTratativaStageId { get; set; }
		public string InfraFechadoStageId { get; set; }
	}
}
