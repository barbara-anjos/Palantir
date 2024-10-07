using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using static Palantir.Api.Models.HubSpotTicketModel;
using System.Net.Http.Headers;
using System.Text;
using Palantir.Api.Configurations;
using static Palantir.Api.Models.ClickUpTaskModel;
using Flurl.Http;
using static Palantir.Api.Models.HubSpotTicketModel.HubSpotWebhookRequest;

namespace Palantir.Api.Services
{
    public class ClickUpService
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
        public async Task<ClickUpTaskResponse> CreateTask(string taskName, string status, List<ClickUpCustomField> customFields)
        {
            var requestUrl = $"{_baseUrl}/list/{_listId}/task";

            var newTask = new
            {
                name = taskName,
                status = status,
                custom_fields = customFields,
                team_id = _teamId
            };

            var taskResponse = await requestUrl
                .WithOAuthBearerToken(_apiToken)
                .PostJsonAsync(newTask)
                .ReceiveJson<ClickUpTaskResponse>();

            return taskResponse;
        }

        // Novo método para criar tarefa a partir do tíquete do HubSpot
        public async Task<bool> CreateTaskFromTicket(HubSpotTicketProperties ticketProperties)
        {
            var clickUpTask = new ClickUpTask
            {
                Name = ticketProperties.Name,
                Status = ticketProperties.Status,
                CustomFields = new List<ClickUpCustomField>
            {
                new ClickUpCustomField
                {
                    Name = "HubSpot Ticket ID",
                    Value = ticketProperties.Id
                },
                new ClickUpCustomField
                {
                    Name = "Priority",
                    Value = ticketProperties.Priority
                }
            }
            };

            try
            {
                // Chama o método para criar a tarefa no ClickUp
                var createdTask = await CreateTask(clickUpTask.Name, clickUpTask.Status, clickUpTask.CustomFields);
                return createdTask != null;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error creating task in ClickUp: {ex.Message}");
                return false;
            }
        }

        // Método para obter uma tarefa pelo taskID
        public async Task<ClickUpTaskResponse> GetTaskById(string taskId)
        {
            var requestUrl = $"{_baseUrl}/task/{taskId}";

            var taskResponse = await requestUrl
                .WithOAuthBearerToken(_apiToken)
                .GetJsonAsync<ClickUpTaskResponse>();

            return taskResponse;
        }

        //// Buscar o ID da tarefa associada ao ID do tíquete no HubSpot
        //public async Task<string> GetTaskIdByTicketIdAsync(string ticketId)
        //{
        //	var requestUrl = $"https://api.clickup.com/api/v2/list/{_listId}/task?custom_field={ticketId}";
        //	var response = await _httpClient.GetAsync(requestUrl);

        //	if (response.IsSuccessStatusCode)
        //	{
        //		var content = await response.Content.ReadAsStringAsync();
        //		var taskData = JsonConvert.DeserializeObject<ClickUpTaskResponse>(content);
        //		return taskData.Tasks.FirstOrDefault()?.Id;
        //	}

        //	throw new Exception("Tarefa correspondente não encontrada.");
        //}

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
