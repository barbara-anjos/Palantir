using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using static Palantir.Api.Models.HubSpotTicketModel;
using System.Net.Http.Headers;
using System.Text;
using Palantir.Api.Configurations;
using static Palantir.Api.Models.ClickUpTaskModel;
using Flurl.Http;
using static Palantir.Api.Models.HubSpotTicketModel.HubSpotWebhookRequest;
using Palantir.Api.Interfaces;
using System.Net;
using Palantir.Api.Enums;
using System.Net.Sockets;
using Palantir.Api.Utils;

namespace Palantir.Api.Services
{
	public class ClickUpService : IDevelopmentTaskService<HubSpotTicketProperties>
	{
		private readonly HttpClient _httpClient;
		private readonly string _apiToken;
		private readonly string _baseUrl;
		private readonly string _listId;
		private readonly string _teamId;
		private object _clickUpSettings;

		public ClickUpService(HttpClient httpClient, IOptions<ClickUpSettings> clickUpSettings)
		{
			_httpClient = httpClient;
			_apiToken = clickUpSettings.Value.ApiToken;
			_baseUrl = clickUpSettings.Value.BaseUrl;
			_listId = clickUpSettings.Value.ListId;
			_teamId = clickUpSettings.Value.TeamId;
		}

		// Método para criar uma tarefa
		public async Task<ClickUpTaskResponse> CreateTask(ClickUpTask task)
		{
			var requestUrl = $"{_baseUrl}/list/{_listId}/task";

			var taskResponse = await requestUrl
				.WithOAuthBearerToken(_apiToken)
				.PostJsonAsync(task)
				.ReceiveJson<ClickUpTaskResponse>();

			return taskResponse;
		}

		// Novo método para criar tarefa a partir do tíquete do HubSpot
		public async Task<bool> CreateTaskFromTicket(HubSpotTicketProperties ticket, string ticketPipeline)
		{
			var priority = HubSpotTicketPrioritySLAConstants.GetPriority(ticket.Priority ?? string.Empty);
			var prioritySegfy = HubSpotTicketPrioritySLAConstants.GetPriority(ticket.PrioritySegfy ?? string.Empty);

			//Dealing with inverted priorities values, as changing them in HubSpot would be a hassle
			if (prioritySegfy == HubSpotTicketSLA.LOW)
				prioritySegfy = HubSpotTicketSLA.HIGH;
			else if (prioritySegfy == HubSpotTicketSLA.HIGH)
				prioritySegfy = HubSpotTicketSLA.LOW;

			var timeEstimate = (int)prioritySegfy;
			var startDate = ticket.SendAt ?? ticket.CreatedAt;
			var dueDate = startDate.WorkingHours(timeEstimate);

			//Check if the ticket name contains a specific text to set a tag
			string tagRules = string.Empty;
			string tagRulesGrupoPorto = string.Empty;
			string tagRulesGrupoPortoAno = string.Empty;
			string anoAtual = DateTime.Now.Year.ToString();
			switch (ticket.Name)
			{
				case var t when t.Contains(TicketTagRules.ZERARBASE.ToString()):
					tagRules = "zerar base";
					break;

				case var t when t.Contains(TicketTagRules.DEVOLUCAOBASE.ToString()):
					tagRules = "devolutiva-basededados";
					break;

				case var t when t.Contains(TicketTagRulesGrupoPorto.PORTO.ToString()):
					tagRulesGrupoPorto = "porto-tickets";
					tagRulesGrupoPortoAno = $"porto-{anoAtual}";
					break;

				case var t when t.Contains(TicketTagRulesGrupoPorto.AZUL.ToString()):
					tagRulesGrupoPorto = "porto-tickets";
					tagRulesGrupoPortoAno = $"porto-{anoAtual}";
					break;

				case var t when t.Contains(TicketTagRulesGrupoPorto.ITAU.ToString()):
					tagRulesGrupoPorto = "porto-tickets";
					tagRulesGrupoPortoAno = $"porto-{anoAtual}";
					break;

				case var t when t.Contains(TicketTagRulesGrupoPorto.MITSUI.ToString()):
					tagRulesGrupoPorto = "porto-tickets";
					tagRulesGrupoPortoAno = $"porto-{anoAtual}";
					break;

				case var t when t.Contains(TicketTagRulesGrupoPorto.AZULPORASSINATURA.ToString()):
					tagRulesGrupoPorto = "porto-tickets";
					tagRulesGrupoPortoAno = $"porto-{anoAtual}";
					break;

				case var t when t.Contains(TicketTagRulesGrupoPorto.BLLU.ToString()):
					tagRulesGrupoPorto = "porto-tickets";
					tagRulesGrupoPortoAno = $"porto-{anoAtual}";
					break;
			}

			var clickUpTask = new ClickUpTask
			{
				Name = $"{ticket.Id} - {ticket.Name}",
				Description = ticket.Content,
				StartDate = new DateTimeOffset(startDate).ToUnixTimeMilliseconds(),
				DueDate = new DateTimeOffset(dueDate).ToUnixTimeMilliseconds(),
				TimeEstimate = timeEstimate,
				Priority = HubSpotTicketPrioritySLAConstants.PriorityMap[priority],
				Tags = new List<Tags>
				{
					new Tags
					{
						Name = ticketPipeline
					},
					new Tags
					{
						Name = "ticket"
					},
					new Tags
					{
						Name = tagRulesGrupoPorto
					},
					new Tags
					{
						Name = tagRulesGrupoPortoAno
					},
					new Tags
					{
						Name = tagRules
					}
				},
				CustomFields = new List<ClickUpCustomField>
				{
					new ClickUpCustomField
					{
						Id = "471039af-a966-4bb0-aab3-5fd1cb92f014",
						Value = ticket.Id
					},
					new ClickUpCustomField
					{
						Id = "4b809840-77df-4f7d-8e9a-8bef1000830e",
						Value = $"https://app.hubspot.com/contacts/6828248/record/0-5/{ticket.Id}"
					},
					new ClickUpCustomField
					{
						Id = "", //custom_field Link Intranet
						Value = ticket.LinkIntranet
					},
					new ClickUpCustomField
					{
						Id = "", //custom_field Tipo
						Value = ticket.Category
					},
					new ClickUpCustomField
					{
						Id = "", //custom_field Funcionalidade
						Value = ticket.Services
					},
				}
			};

			try
			{
				var createdTask = await CreateTask(clickUpTask);
				return createdTask != null;
			}
			catch (Exception ex)
			{
				//Colocar log na aws?
				Console.WriteLine($"Error creating task in ClickUp: {ex.Message}");
				return false;
			}
		}

		/// <summary>
		/// Get a task by ID
		/// </summary>
		/// <param name="taskId"></param>
		/// <returns></returns>
		public async Task<ClickUpTaskResponse> GetTaskById(string taskId)
		{
			var requestUrl = $"{_baseUrl}/task/{taskId}";

			var taskResponse = await requestUrl
				.WithOAuthBearerToken(_apiToken)
				.GetJsonAsync<ClickUpTaskResponse>();

			return taskResponse;
		}

		/// <summary>
		/// Get the task ID associated with the ticket ID in HubSpot
		/// </summary>
		/// <param name="ticketId"></param>
		/// <returns></returns>
		/// <exception cref="Exception"></exception>
		public async Task<TaskList> GetTaskIdByTicketIdAsync(string ticketId)
		{
			try
			{
				//Corrigir url base do clickup
				var requestUrl = $"https://api.clickup.com/api/v2/list/{_listId}/task?custom_field={ticketId}";
				var response = await _httpClient.GetAsync(requestUrl);

				if (response.StatusCode != HttpStatusCode.OK)
					return null;

				var content = await response.Content.ReadAsStringAsync();
				return JsonConvert.DeserializeObject<TaskList>(content);
			}
			catch (Exception ex)
			{

				throw new Exception("Tarefa correspondente não encontrada.");
			}
		}

		//// Atualizar uma tarefa existente no ClickUp
		//public async Task UpdateTaskAsync(string taskId, ClickUpTaskUpdateData updatedData)
		//      {
		//          var requestUrl = $"https://api.clickup.com/api/v2/task/{taskId}";

		//          var taskUpdateData = new
		//          {
		//              status = updatedData.Status,
		//              priority = MapPriority(updatedData.Priority),
		//              due_date = new DateTimeOffset(updatedData.DueDate).ToUnixTimeMilliseconds()
		//          };

		//          var content = new StringContent(JsonConvert.SerializeObject(taskUpdateData), Encoding.UTF8, "application/json");
		//          _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", _apiToken);

		//          var response = await _httpClient.PutAsync(requestUrl, content);

		//          if (!response.IsSuccessStatusCode)
		//          {
		//              throw new Exception("Erro ao atualizar tarefa no ClickUp.");
		//          }
		//      }

		//      // Excluir uma tarefa no ClickUp
		//      public async Task DeleteTaskAsync(string taskId)
		//      {
		//          var requestUrl = $"https://api.clickup.com/api/v2/task/{taskId}";
		//          _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", _apiToken);

		//          var response = await _httpClient.DeleteAsync(requestUrl);

		//          if (!response.IsSuccessStatusCode)
		//          {
		//              throw new Exception("Erro ao excluir a tarefa no ClickUp.");
		//          }
		//      }
	}
}
