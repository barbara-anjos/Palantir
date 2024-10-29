using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using Palantir.Api.Configurations;
using static Palantir.Api.Models.ClickUpTaskModel;
using Flurl.Http;
using static Palantir.Api.Models.HubSpotTicketModel.HubSpotWebhookRequest;
using Palantir.Api.Interfaces;
using System.Net;
using Palantir.Api.Enums;
using Palantir.Api.Utils;
using Palantir.Api.Utils.Const;
using Palantir.Api.Models;
using System.Net.Http.Headers;
using System.Threading.Tasks;

namespace Palantir.Api.Services
{
    public class ClickUpService : IDevelopmentTaskService
    {
        private readonly HttpClient _httpClient;
        private readonly string _apiToken;
        private readonly string _baseUrl;
        private readonly string _listId;
        private readonly string _teamId;
        private readonly ILogger<ClickUpService> _logger;

		public ClickUpService(HttpClient httpClient, IOptions<ClickUpSettings> clickUpSettings, ILogger<ClickUpService> logger)
        {
            _httpClient = httpClient;
            _apiToken = clickUpSettings.Value.ApiToken;
            _httpClient.DefaultRequestHeaders.Clear();
            _httpClient.DefaultRequestHeaders.Add("Authorization", _apiToken);
			_baseUrl = clickUpSettings.Value.BaseUrl;
            _listId = clickUpSettings.Value.ListId;
            _teamId = clickUpSettings.Value.TeamId;
            _logger = logger;
        }

        /// <summary>
        /// Create a task
        /// </summary>
        /// <param name="task"></param>
        /// <returns></returns>
        public async Task<ClickUpTask> CreateTask(ClickUpTask task)
        {
            var requestUrl = $"{_baseUrl}/list/{_listId}/task?custom_task_ids=true&team_id={_teamId}";

            var taskResponse = await requestUrl
                .WithOAuthBearerToken(_apiToken)
                .PostJsonAsync(task)
                .ReceiveJson<ClickUpTask>();

            return taskResponse;
        }

        /// <summary>
        /// Create a task in ClickUp from a ticket in HubSpot
        /// </summary>
        /// <param name="ticket"></param>
        /// <param name="ticketPipeline"></param>
        /// <returns></returns>
        public async Task<bool> CreateTaskFromTicket(SegfyTask ticket, string s)
        {
            var clickUpTask = new ClickUpTask
            {
                Name = $"{ticket.TicketId} - {ticket.Name}",
                Description = ticket.Description,
                StartDate = ticket.StartDate,
                DueDate = ticket.DueDate,
                TimeEstimate = ticket.TimeEstimate,
                Priority = ticket.Priority,
                Tags = GetTagsFromTicketName(ticket.Name, ticket.Pipeline),
                CustomFields = new List<ClickUpCustomField>
                {
                    new ClickUpCustomField
                    {
                        Id = CustomFieldsClickUp.ticketId,
                        Value = ticket.TicketId
                    },
                    new ClickUpCustomField
                    {
                        Id = CustomFieldsClickUp.linkHubSpot,
                        Value = $"https://app.hubspot.com/contacts/6828248/record/0-5/{ticket.TicketId}"
                    },
                    new ClickUpCustomField
                    {
                        Id = CustomFieldsClickUp.urlIntranet,
                        Value = ticket.LinkIntranet
                    },
                    new ClickUpCustomField
                    {
                        Id = CustomFieldsClickUp.tipo,
                        Value = ticket.Category
                    },
                    new ClickUpCustomField
                    {
                        Id = CustomFieldsClickUp.funcionalidade,
                        Value = ticket.Services
                    },
                }
            };

            var createdTask = await CreateTask(clickUpTask);
            return createdTask != null;
        }

        /// <summary>
        /// Checks the ticket name and returns the tags to be added to the task
        /// </summary>
        /// <param name="ticketName"></param>
        /// <returns></returns>
        private List<Tags> GetTagsFromTicketName(string ticketName, string pipeline)
        {
            var tags = new List<Tags>();
            string ticketNameLower = ticketName.ToLower();
            string anoAtual = DateTime.Now.Year.ToString();

            if (ticketNameLower.Contains(TicketTagRulesDictionary.ZERARBASE))
            {
                tags.Add(new Tags { Name = "zerar base" });
            }
            else if (ticketNameLower.Contains(TicketTagRulesDictionary.DEVOLUCAOBASE))
            {
                tags.Add(new Tags { Name = "devolutiva-basededados" });
            }
            else if (ticketNameLower.Contains(TicketTagRulesDictionary.PORTO)
                || ticketNameLower.Contains(TicketTagRulesDictionary.AZUL)
                || ticketNameLower.Contains(TicketTagRulesDictionary.AZULPORASSINATURA)
                || ticketNameLower.Contains(TicketTagRulesDictionary.BLLU)
                || ticketNameLower.Contains(TicketTagRulesDictionary.ITAU)
                || ticketNameLower.Contains(TicketTagRulesDictionary.MITSUI))
            {
                tags.Add(new Tags { Name = "porto-tickets" });
                tags.Add(new Tags { Name = $"porto-{anoAtual}" });
            }

            tags.Add(new Tags { Name = "ticket" });
            tags.Add(new Tags { Name = pipeline });

            return tags;
        }

        /// <summary>
        /// Get a task by ID
        /// </summary>
        /// <param name="taskId"></param>
        /// <returns></returns>
        public async Task<ClickUpTask> GetTaskById(string taskId)
        {
            try
            {
				var requestUrl = $"{_baseUrl}/task/{taskId}?custom_task_ids=true&team_id={_teamId}";

				_logger.LogInformation($"Requesting URL: {requestUrl}");

				var response = await _httpClient.GetAsync(requestUrl); 
                var content = await response.Content.ReadAsStringAsync();

				_logger.LogInformation($"Response Status Code: {response.StatusCode}");
				_logger.LogInformation($"Response Content: {content}");

				if (response.IsSuccessStatusCode && !string.IsNullOrEmpty(content))
				{
					var taskData = JsonConvert.DeserializeObject<ClickUpTask>(content);

					return taskData;
				}

				return null;

			}
            catch (Exception ex)
            {
				_logger.LogError(ex, "An error occurred while getting the task by ID.");
				throw;
            }
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
                var requestUrl = $"{_baseUrl}/list/{_listId}/task?custom_field=[{{\"field_id\":\"{CustomFieldsClickUp.ticketId}\",\"operator\":\"=\",\"value\":\"" + ticketId + "\"}]";
                var response = await _httpClient.GetAsync(requestUrl);

                if (response.StatusCode != HttpStatusCode.OK)
                    return null;

                var content = await response.Content.ReadAsStringAsync();
                var taskList = JsonConvert.DeserializeObject<TaskList>(content);

                return taskList;
            }

			catch (Exception ex)
            {
                throw new Exception($"Erro ao buscar tarefas: {ex.Message}");
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
