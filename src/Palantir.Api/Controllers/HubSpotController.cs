using Flurl.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Palantir.Api.Configurations;
using Palantir.Api.Services;
using static Palantir.Api.Models.HubSpotTicketModel;

[ApiController]
[Route("api/hubspot")]
public class HubSpotController : ControllerBase
{
    private readonly string _hubSpotApiKey;
    private readonly ClickUpService _clickUpService;
	private readonly HubSpotService _hubSpotService;

	public HubSpotController(IOptions<HubSpotSettings> hubSpotSettings, ClickUpService clickUpService)
    {
        _hubSpotApiKey = hubSpotSettings.Value.ApiKey;
        _clickUpService = clickUpService;
    }

	[HttpPost("ticket")]
	public async Task<IActionResult> CreateTicket([FromBody] HubSpotWebhookRequest request)
	{
		// Validação do request
		if (request == null || string.IsNullOrEmpty(request.ObjectId))
		{
			return BadRequest("Invalid request.");
		}

		// Obter detalhes do tíquete criado
		var ticketProperties = await _hubSpotService.GetTicketByIdAsync(request.ObjectId);
		if (ticketProperties == null)
		{
			return NotFound("Ticket not found.");
		}

		// Verificar se o tíquete está na pipeline e status desejados
		string specificPipeline = "desired-pipeline-id";  // ID da pipeline desejada
		string specificStatus = "desired-status";         // Status desejado

		if (ticketProperties.Pipeline == specificPipeline &&
			ticketProperties.Status == specificStatus)
		{
			// Chama o serviço para processar o webhook e criar a tarefa no ClickUp
			var result = await _clickUpService.CreateTaskFromTicket(ticketProperties);

			return result ? Ok("Task created successfully in ClickUp.") : StatusCode(500, "Failed to create task in ClickUp.");
		}

		// Se o tíquete não estiver na pipeline ou status desejados, não faz nada
		return Ok("Ticket does not meet the conditions for creating a task.");
	}

	////Atualizar tarefa no ClickUp com base em alterações do tíquete
	//[HttpPost("update-task-from-ticket")]
	//public async Task<IActionResult> UpdateTaskFromHubSpot([FromBody] HubSpotWebhookRequest webhookRequest)
	//{
	//    var ticketId = webhookRequest.ObjectId;
	//    var updatedProperties = webhookRequest.ChangedProperties;

	//    // Buscar o ID da tarefa no ClickUp associada ao ID do tíquete no HubSpot
	//    var taskId = await _clickUpService.GetTaskIdByTicketIdAsync(ticketId);
	//    if (taskId != null)
	//    {
	//        var updatedTaskData = new ClickUpTaskUpdateData
	//        {
	//            Status = updatedProperties.Status,
	//            Priority = updatedProperties.Priority,
	//            DueDate = updatedProperties.DueDate
	//        };

	//        // Atualizar a tarefa no ClickUp
	//        await _clickUpService.UpdateTaskAsync(taskId, updatedTaskData);
	//        return Ok("Tarefa atualizada no ClickUp.");
	//    }

	//    return NotFound("Tarefa correspondente no ClickUp não encontrada.");
	//}

	////Excluir tarefa no ClickUp quando o tíquete for excluído no HubSpot
	//[HttpPost("delete-task-from-ticket")]
	//public async Task<IActionResult> DeleteTaskFromHubSpot([FromBody] HubSpotWebhookRequest webhookRequest)
	//{
	//    var ticketId = webhookRequest.ObjectId;

	//    // Buscar o ID da tarefa no ClickUp associada ao ID do tíquete no HubSpot
	//    var taskId = await _clickUpService.GetTaskIdByTicketIdAsync(ticketId);

	//    if (taskId != null)
	//    {
	//        // Excluir a tarefa no ClickUp
	//        await _clickUpService.DeleteTaskAsync(taskId);
	//        return Ok("Tarefa excluída no ClickUp.");
	//    }

	//    return NotFound("Tarefa correspondente no ClickUp não encontrada.");
	//}
}
