using Microsoft.Extensions.Options;
using Palantir.Api.Configurations;

namespace Palantir.Api.Mappers
{
	/// <summary>
	/// Provides methods to map statuses between HubSpot and ClickUp.
	/// </summary>
	public class StatusMapper
	{
		private readonly Dictionary<string, Dictionary<string, string>> _hubSpotToClickUpStatus;
		private readonly Dictionary<string, Dictionary<string, string>> _clickUpToHubSpotStatus;

		public StatusMapper(IOptions<ClickUpSettings> clickUpSettings, IOptions<HubSpotSettings> hubSpotSettings)
		{
			_hubSpotToClickUpStatus = new Dictionary<string, Dictionary<string, string>>
			{
				{
					"gestão", new Dictionary<string, string>
					{
						{ hubSpotSettings.Value.GestaoNovoStageId, clickUpSettings.Value.NovoStatus },
						{ hubSpotSettings.Value.GestaoExecutandoStageId, clickUpSettings.Value.ExecutandoStatus },
						{ hubSpotSettings.Value.GestaoBloqueadoStageId, clickUpSettings.Value.BloqueadoStatus },
						{ hubSpotSettings.Value.GestaoComunicacaoStageId, clickUpSettings.Value.ComunicacaoStatus },
						{ hubSpotSettings.Value.GestaoTestarStageId, clickUpSettings.Value.TestarStatus },
						{ hubSpotSettings.Value.GestaoTestandoStageId, clickUpSettings.Value.TestandoStatus },
						{ hubSpotSettings.Value.GestaoReprovadoStageId, clickUpSettings.Value.ReprovadoStatus },
						{ hubSpotSettings.Value.GestaoAprovadoStageId, clickUpSettings.Value.AprovadoStatus },
						{ hubSpotSettings.Value.GestaoConcluidoStageId, clickUpSettings.Value.ConcluidoStatus }
					}
				},
				{
					"automação", new Dictionary<string, string>
					{
						{ hubSpotSettings.Value.AutomacaoNovoStageId, clickUpSettings.Value.NovoStatus },
						{ hubSpotSettings.Value.AutomacaoExecutandoStageId, clickUpSettings.Value.ExecutandoStatus },
						{ hubSpotSettings.Value.AutomacaoBloqueadoStageId, clickUpSettings.Value.BloqueadoStatus },
						{ hubSpotSettings.Value.AutomacaoComunicacaoStageId, clickUpSettings.Value.ComunicacaoStatus },
						{ hubSpotSettings.Value.AutomacaoTestarStageId, clickUpSettings.Value.TestarStatus },
						{ hubSpotSettings.Value.AutomacaoTestandoStageId, clickUpSettings.Value.TestandoStatus },
						{ hubSpotSettings.Value.AutomacaoReprovadoStageId, clickUpSettings.Value.ReprovadoStatus },
						{ hubSpotSettings.Value.AutomacaoAprovadoStageId, clickUpSettings.Value.AprovadoStatus },
						{ hubSpotSettings.Value.AutomacaoConcluidoStageId, clickUpSettings.Value.ConcluidoStatus }
					}
				},
				{
					"infra", new Dictionary<string, string>
					{
						{ hubSpotSettings.Value.InfraNovoStageId, clickUpSettings.Value.NovoStatus },
						{ hubSpotSettings.Value.InfraEmTratativaStageId, clickUpSettings.Value.ExecutandoStatus },
						{ hubSpotSettings.Value.InfraFechadoStageId, clickUpSettings.Value.ConcluidoStatus }
					}
				}
			};

			_clickUpToHubSpotStatus = _hubSpotToClickUpStatus
			.ToDictionary(
				pipeline => pipeline.Key,
				pipeline => pipeline.Value.GroupBy(kvp => kvp.Value)
										 .ToDictionary(g => g.Key, g => g.First().Key) // Seleciona um ID padrão para cada status do ClickUp
			);
		}

		/// <summary>
		/// Converts HubSpot status to ClickUp status
		/// </summary>
		/// <param name="hubSpotStageId"></param>
		/// <returns></returns>
		public string GetClickUpStatus(string hubSpotStageId, string pipeline)
		{
			if (_hubSpotToClickUpStatus.TryGetValue(pipeline, out var stageMappings)
				&& stageMappings.TryGetValue(hubSpotStageId, out var clickUpStatus))
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
		public string GetHubSpotStatus(string clickUpStatus, string pipeline)
		{
			if (_clickUpToHubSpotStatus.TryGetValue(pipeline, out var stageMappings)
				&& stageMappings.TryGetValue(clickUpStatus, out var hubSpotStageId))
			{
				return hubSpotStageId;
			}
			return "Status not mapped";
		}
	}
}
