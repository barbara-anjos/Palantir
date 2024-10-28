using Flurl.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Palantir.Api.Configurations;
using Palantir.Api.Interfaces;
using Palantir.Api.Models;
using Palantir.Api.Services;
using static Palantir.Api.Models.ClickUpTaskModel;
using static Palantir.Api.Models.HubSpotTicketModel.HubSpotWebhookRequest;

[ApiController]
[Route("api/clickup")]
public class ClickUpController : ControllerBase
{
    private readonly IDevelopmentTaskService _clickUpService;
	private readonly ICustomerTicketService<HubSpotTicketResponse> _hubSpotService;
	private readonly string _clickUpApiToken;

    public ClickUpController(IOptions<ClickUpSettings> clickUpSettings, IDevelopmentTaskService clickUpService, ICustomerTicketService<HubSpotTicketResponse> hubSpotService)
    {
        _hubSpotService = hubSpotService;
        _clickUpApiToken = clickUpSettings.Value.ApiToken;
        _clickUpService = clickUpService;
    }

	/// <summary>
	/// Endpoint to create a new task in ClickUp
	/// </summary>
	/// <param name="newTask"></param>
	/// <returns></returns>
	[HttpPost("task")]
    public async Task<IActionResult> CreateTask([FromBody] SegfyTask newTask)
    {
        if (newTask == null || string.IsNullOrEmpty(newTask.Name))
        {
            return BadRequest("Invalid task data.");
        }

        var createdTask = await _clickUpService.CreateTask(newTask);
        return CreatedAtAction(nameof(GetTask), new { id = createdTask.TaskId }, createdTask);
    }

	/// <summary>
	/// Endpoint to get a task by ID
	/// </summary>
	/// <param name="id"></param>
	/// <returns></returns>
	[HttpGet("task/{id}")]
    public async Task<IActionResult> GetTask(string id)
    {
        var task = await _clickUpService.GetTaskById(id);
        return Ok(task);
    }

    //// Webhook do ClickUp: Atualizar status do tíquete no HubSpot quando o status da tarefa for alterado no ClickUp
    //[HttpPost("update-ticket-from-task")]
    //public async Task<IActionResult> UpdateTicketFromClickUp([FromBody] ClickUpWebhookRequest webhookRequest)
    //{
    //    var taskId = webhookRequest.ObjectId;
    //    var updatedProperties = webhookRequest.ChangedProperties;

    //    // Buscar o tíquete no HubSpot associado à tarefa no ClickUp
    //    var hubSpotTicketId = await GetHubSpotTicketIdByTaskId(taskId);

    //    if (hubSpotTicketId != null)
    //    {
    //        // Atualizar o status do tíquete no HubSpot
    //        await _hubSpotService.UpdateTicketStatusAsync(hubSpotTicketId, updatedProperties.Status);
    //        return Ok("Tíquete no HubSpot atualizado com sucesso.");
    //    }

    //    return NotFound("Tíquete correspondente no HubSpot não encontrado.");
    //}

    //Método para buscar o ID do tíquete do HubSpot associado a uma tarefa no ClickUp
    //private async Task<string> GetHubSpotTicketIdByTaskId(string taskId)
    //{
    //    var requestUrl = $"https://api.clickup.com/api/v2/task/{taskId}";

    //    var taskResponse = await requestUrl
    //        .WithOAuthBearerToken(_clickUpApiToken)
    //        .GetJsonAsync<ClickUpTaskResponse>();

    //    return taskResponse.CustomFields
    //        .FirstOrDefault(field => field.Name == "HubSpot Ticket ID")?.Value;
    //}
}
