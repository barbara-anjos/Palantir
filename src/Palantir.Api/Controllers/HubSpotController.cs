using Flurl.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Palantir.Api.Configurations;
using Palantir.Api.Enums;
using Palantir.Api.Models;
using Palantir.Api.Services;
using System.Net.Sockets;
using static Palantir.Api.Models.HubSpotTicketModel;

[ApiController]
[Route("api/hubspot")]
public class HubSpotController : ControllerBase
{
	private readonly string _hubSpotApiKey;
	private readonly ClickUpService _clickUpService;
	private readonly HubSpotService _hubSpotService;
	private List<long> lockList;
	private readonly string _pipelineGestao;
	private readonly string _pipelineAutomacao;
	private readonly string _pipelineInfra;
	private readonly string _statusNovoGestao;
	private readonly string _statusNovoAutomacao;
	private readonly string _statusNovoInfra;

	public HubSpotController(IOptions<HubSpotSettings> hubSpotSettings, ClickUpService clickUpService)
	{
		_hubSpotApiKey = hubSpotSettings.Value.ApiKey;
		_clickUpService = clickUpService;
		lockList = new List<long>();
		_pipelineGestao = hubSpotSettings.Value.GestaoPipeline;
		_pipelineAutomacao = hubSpotSettings.Value.AutomacaoPipeline;
		_pipelineInfra = hubSpotSettings.Value.InfraPipeline;
		_statusNovoGestao = hubSpotSettings.Value.GestaoNovoStageId;
		_statusNovoAutomacao = hubSpotSettings.Value.AutomacaoNovoStageId;
		_statusNovoInfra = hubSpotSettings.Value.InfraNovoStageId;
	}

	[HttpPost("ticket")]
	public async Task<IActionResult> CreateTaskFromTicket([FromBody] List<HubSpotNotification> notifications)
	{
		var results = new List<HubSpotNotificationResponse>();

		foreach (var notification in notifications)
		{
			//Tratando envio do mesmo tíquete em mais de uma notificação: create ou update
			if (!Lock(notification.ObjectId))
				continue;

			var result = new HubSpotNotificationResponse();
			result.EventId = notification.EventId.ToString();

			try
			{
				var eventType = !string.IsNullOrEmpty(notification.SubscriptionType) ? notification.SubscriptionType : notification.EventType;

				if (eventType != "ticket.creation" && eventType != "ticket.propertyChange")
					throw new Exception("Somente processa notificações de criação de tíquetes.");

				// Obter detalhes do tíquete criado
				var ticket = await _hubSpotService.GetTicketByIdAsync(notification.ObjectId);
				if (ticket == null)
					return NotFound("Ticket não encontrado ou inválido.");

				// Verificar se o tíquete está na pipeline e status desejados
				//puxar do appsettings.json
				string specificPipeline = "desired-pipeline-id";  // ID da pipeline desejada
				string specificStatus = "desired-status";         // Status desejado

				var hs_priority = HubSpotTicketPrioritySLAConstants.GetPriority(ticket.Priority ?? string.Empty);
				var prioridade = HubSpotTicketPrioritySLAConstants.GetPriority(ticket.Prioridade ?? string.Empty);

				// Tratando a inversão dos valores aqui, pois alterar no Hubspost geraria um caos generelizado.
				if (prioridade == HubSpotTicketSLA.LOW)
				{
					prioridade = HubSpotTicketSLA.HIGH;
				}
				else if (prioridade == HubSpotTicketSLA.HIGH)
				{
					prioridade = HubSpotTicketSLA.LOW;
				}

				var prioridadeMaior = (int)hs_priority > (int)prioridade ? hs_priority : prioridade;
				//string pipeline;

				//Valida a pipeline para criar a task e adicionar a tag
				switch (ticket.Pipeline)
				{
					case var pipeline when pipeline == _pipelineGestao:
						pipeline = "Gestão";
						break;
					case var pipeline when pipeline == _pipelineAutomacao:
						pipeline = "Automação";
						break;
					case var pipeline when pipeline == _pipelineInfra:
						pipeline = "Infra";
						break;
					default:
						throw new Exception("Pipeline do tíquete diferente da esperada: Gestão, Automação ou Infra.");

				}

				//Valida o status para criar a task
				if (ticket.Status != _statusNovoGestao && ticket.Status != _statusNovoAutomacao && ticket.Status != _statusNovoInfra) 
					throw new Exception("Status do tíquete diferente do esperado: Novo.");

				//Valida se a task já não foi criada
				var existTask = await _clickUpService.GetTaskIdByTicketIdAsync(ticket.Id);
				if (existTask != null && existTask.Tasks.Count > 0)
					throw new Exception("Task já criada.");

				#region Propriedades da Task



				#endregion

				if (ticket.Pipeline == specificPipeline &&
					ticket.Status == specificStatus)
				{
					// Chama o serviço para processar o webhook e criar a tarefa no ClickUp
					var task = await _clickUpService.CreateTaskFromTicket(ticket);

					return task ? Ok("Task created successfully in ClickUp.") : StatusCode(500, "Failed to create task in ClickUp.");
				}

				// Se o tíquete não estiver na pipeline ou status desejados, não faz nada
				return Ok("Ticket does not meet the conditions for creating a task.");
			}
			catch (Exception ex)
			{
				result.Message = ex.Message;
			}
			finally
			{
				Unlock(notification.ObjectId);
			}
		}

		return Ok(results);
	}

	#region LockList das Notificações

	private bool Lock(long ticketId)
	{
		lock (lockList)
		{
			if (lockList.Contains(ticketId))
			{
				return false;
			}

			lockList.Add(ticketId);
			return true;
		}
	}

	private void Unlock(long ticketId)
	{
		lock (lockList)
		{
			lockList.Remove(ticketId);
		}
	}

	#endregion

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
