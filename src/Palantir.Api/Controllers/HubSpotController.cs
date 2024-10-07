using Flurl.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Palantir.Api.Configurations;
using Palantir.Api.Services;
using static Palantir.Api.Models.ClickUpTaskUpdateModel;
using static Palantir.Api.Models.HubSpotTicketModel;

[ApiController]
[Route("api/hubspot")]
public class HubSpotController : ControllerBase
{
    private readonly string _hubSpotApiToken;
    private readonly ClickUpService _clickUpService;

    public HubSpotController(IOptions<HubSpotSettings> hubSpotSettings, ClickUpService clickUpService)
    {
        _hubSpotApiToken = hubSpotSettings.Value.ApiToken;
        _clickUpService = clickUpService;
    }    

    //Atualizar tarefa no ClickUp com base em alterações do tíquete
    [HttpPost("update-task-from-ticket")]
    public async Task<IActionResult> UpdateTaskFromHubSpot([FromBody] HubSpotWebhookRequest webhookRequest)
    {
        var ticketId = webhookRequest.ObjectId;
        var updatedProperties = webhookRequest.ChangedProperties;

        // Buscar o ID da tarefa no ClickUp associada ao ID do tíquete no HubSpot
        var taskId = await _clickUpService.GetTaskIdByTicketIdAsync(ticketId);
        if (taskId != null)
        {
            var updatedTaskData = new ClickUpTaskUpdateData
            {
                Status = updatedProperties.Status,
                Priority = updatedProperties.Priority,
                DueDate = updatedProperties.DueDate
            };

            // Atualizar a tarefa no ClickUp
            await _clickUpService.UpdateTaskAsync(taskId, updatedTaskData);
            return Ok("Tarefa atualizada no ClickUp.");
        }

        return NotFound("Tarefa correspondente no ClickUp não encontrada.");
    }

    //Excluir tarefa no ClickUp quando o tíquete for excluído no HubSpot
    [HttpPost("delete-task-from-ticket")]
    public async Task<IActionResult> DeleteTaskFromHubSpot([FromBody] HubSpotWebhookRequest webhookRequest)
    {
        var ticketId = webhookRequest.ObjectId;

        // Buscar o ID da tarefa no ClickUp associada ao ID do tíquete no HubSpot
        var taskId = await _clickUpService.GetTaskIdByTicketIdAsync(ticketId);

        if (taskId != null)
        {
            // Excluir a tarefa no ClickUp
            await _clickUpService.DeleteTaskAsync(taskId);
            return Ok("Tarefa excluída no ClickUp.");
        }

        return NotFound("Tarefa correspondente no ClickUp não encontrada.");
    }

    //Buscar um tíquete no HubSpot usando Flurl
    private async Task<HubSpotTicketProperties> GetHubSpotTicketById(string ticketId)
    {
        var requestUrl = $"https://api.hubapi.com/crm/v3/objects/tickets/{ticketId}";

        var ticketResponse = await requestUrl
            .WithOAuthBearerToken(_hubSpotApiToken)
            .GetJsonAsync<HubSpotTicketResponse>();

        return new HubSpotTicketProperties
        {
            Id = ticketResponse.Id,
            Name = ticketResponse.Properties.Name,
            Status = ticketResponse.Properties.Status,
            Priority = ticketResponse.Properties.Priority,
            CreatedAt = ticketResponse.Properties.CreatedAt
        };
    }
}
