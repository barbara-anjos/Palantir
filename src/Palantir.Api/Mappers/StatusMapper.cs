using Microsoft.Extensions.Options;
using Palantir.Api.Configurations;

namespace Palantir.Api.Mappers
{
	public class StatusMapper
	{
		private readonly Dictionary<string, string> _hubSpotToClickUpStatus;
		private readonly Dictionary<string, string> _clickUpToHubSpotStatus;

		public StatusMapper(IOptions<ClickUpSettings> clickUpSettings, IOptions<HubSpotSettings> hubSpotSettings)
		{
			_hubSpotToClickUpStatus = new Dictionary<string, string>
			{
				{ hubSpotSettings.Value.GestaoNovoStageId, clickUpSettings.Value.NovoStatus },
				{ hubSpotSettings.Value.GestaoExecutandoStageId, clickUpSettings.Value.ExecutandoStatus },
				{ hubSpotSettings.Value.GestaoBloqueadoStageId, clickUpSettings.Value.BloqueadoStatus },
				{ hubSpotSettings.Value.GestaoComunicaoStageId, clickUpSettings.Value.ComunicaoStatus },
				{ hubSpotSettings.Value.GestaoTestarStageId, clickUpSettings.Value.TestarStatus },
				{ hubSpotSettings.Value.GestaoTestandoStageId, clickUpSettings.Value.TestandoStatus },
				{ hubSpotSettings.Value.GestaoReprovadoStageId, clickUpSettings.Value.ReprovadoStatus },
				{ hubSpotSettings.Value.GestaoAprovadoStageId, clickUpSettings.Value.AprovadoStatus },
				{ hubSpotSettings.Value.GestaoConcluidoStageId, clickUpSettings.Value.ConcluidoStatus },
				{ hubSpotSettings.Value.AutomacaoNovoStageId, clickUpSettings.Value.NovoStatus },
				{ hubSpotSettings.Value.AutomacaoExecutandoStageId, clickUpSettings.Value.ExecutandoStatus },
				{ hubSpotSettings.Value.AutomacaoBloqueadoStageId, clickUpSettings.Value.BloqueadoStatus },
				{ hubSpotSettings.Value.AutomacaoComunicaoStageId, clickUpSettings.Value.ComunicaoStatus },
				{ hubSpotSettings.Value.AutomacaoTestarStageId, clickUpSettings.Value.TestarStatus },
				{ hubSpotSettings.Value.AutomacaoTestandoStageId, clickUpSettings.Value.TestandoStatus },
				{ hubSpotSettings.Value.AutomacaoReprovadoStageId, clickUpSettings.Value.ReprovadoStatus },
				{ hubSpotSettings.Value.AutomacaoAprovadoStageId, clickUpSettings.Value.AprovadoStatus },
				{ hubSpotSettings.Value.AutomacaoConcluidoStageId, clickUpSettings.Value.ConcluidoStatus },
				{ hubSpotSettings.Value.InfraNovoStageId, clickUpSettings.Value.NovoStatus },
				{ hubSpotSettings.Value.InfraEmTratativaStageId, clickUpSettings.Value.ExecutandoStatus },
				{ hubSpotSettings.Value.InfraFechadoStageId, clickUpSettings.Value.ConcluidoStatus }
			};

			_clickUpToHubSpotStatus = _hubSpotToClickUpStatus.ToDictionary(kvp => kvp.Value, kvp => kvp.Key);
		}

		/// <summary>
		/// Converts HubSpot status to ClickUp status
		/// </summary>
		/// <param name="hubSpotStageId"></param>
		/// <returns></returns>
		public string GetClickUpStatus(string hubSpotStageId)
		{
			if (_hubSpotToClickUpStatus.TryGetValue(hubSpotStageId, out var clickUpStatus))
			{
				return clickUpStatus;
			}
			return "Status not mapped";
		}

		/// <summary>
		/// Converts ClickUp status to HubSpot status
		/// </summary>
		/// <param name="clickUpStatus"></param>
		/// <returns></returns>
		public string GetHubSpotStatus(string clickUpStatus)
		{
			if (_clickUpToHubSpotStatus.TryGetValue(clickUpStatus, out var hubSpotStageId))
			{
				return hubSpotStageId;
			}
			return "Status not mapped";
		}
	}
}
