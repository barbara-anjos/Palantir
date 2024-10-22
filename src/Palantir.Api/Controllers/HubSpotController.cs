using Flurl.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Palantir.Api.Configurations;
using Palantir.Api.Enums;
using Palantir.Api.Interfaces;
using Palantir.Api.Models;
using Palantir.Api.Services;
using Palantir.Api.Utils;
using System.Net.Sockets;
using static Palantir.Api.Models.ClickUpTaskModel;
using static Palantir.Api.Models.HubSpotTicketModel;
using static Palantir.Api.Models.HubSpotTicketModel.HubSpotWebhookRequest;

[ApiController]
[Route("api/hubspot")]
public class HubSpotController : ControllerBase
{
	private readonly string _hubSpotApiKey;
	private readonly IDevelopmentTaskService<HubSpotTicketProperties, TaskList> _clickUpService;
	private readonly HubSpotService _hubSpotService;
	private List<long> lockList;
	private readonly string _pipelineGestao;
	private readonly string _pipelineAutomacao;
	private readonly string _pipelineInfra;
	private readonly string _statusNovoGestao;
	private readonly string _statusNovoAutomacao;
	private readonly string _statusNovoInfra;

	public HubSpotController(IOptions<HubSpotSettings> hubSpotSettings, IDevelopmentTaskService<HubSpotTicketProperties, TaskList> clickUpService)
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

	/// <summary>
	/// Create a task in ClickUp from a ticket in HubSpot
	/// </summary>
	/// <param name="notifications"></param>
	/// <returns></returns>
	[HttpPost("ticket")]
	public async Task<IActionResult> CreateTaskFromTicket([FromBody] List<HubSpotNotification> notifications)
	{
		var results = new List<HubSpotNotificationResponse>();

		foreach (var notification in notifications)
		{
			//Dealing with the same ticket in more than one notification: create or update
			if (!Lock(notification.ObjectId))
				continue;

			var result = new HubSpotNotificationResponse();
			result.EventId = notification.EventId.ToString();

			try
			{
				var eventType = !string.IsNullOrEmpty(notification.SubscriptionType) ? notification.SubscriptionType : notification.EventType;

				if (eventType != "ticket.creation" && eventType != "ticket.propertyChange")
					throw new Exception("Only processes ticket creation notifications.");

				//Get created ticket details
				var ticket = await _hubSpotService.GetTicketByIdAsync(notification.ObjectId);
				if (ticket == null)
					return NotFound("Ticket not found or invalid.");

				//Check the status to create the task
				if (ticket.Status != _statusNovoGestao && ticket.Status != _statusNovoAutomacao && ticket.Status != _statusNovoInfra)
					throw new Exception("Ticket status different than expected: Novo.");

				//Check if the task already exists
				var existTask = await _clickUpService.GetTaskIdByTicketIdAsync(ticket.Id);
				if (existTask != null && existTask.Tasks.Count > 0)
					throw new Exception("Task alredy created.");

                //Check the ticket pipeline to create the task with the correct pipeline tag
                var ticketPipeline = ticket.Pipeline;
                switch (ticketPipeline)
				{
					case var t when t == _pipelineGestao:
                        ticketPipeline = "Gestão";
                        break;

                    case var t when t == _pipelineAutomacao:
                        ticketPipeline = "Automação";
                        break;

                    case var t when t == _pipelineInfra:
                        ticketPipeline = "Infra";
                        break;
                }

                //Only create the task if the ticket is in the expected pipeline and status
                if ((ticket.Pipeline == _pipelineGestao && ticket.Status == _statusNovoGestao)
					|| (ticket.Pipeline == _pipelineAutomacao && ticket.Status == _statusNovoAutomacao)
					|| (ticket.Pipeline == _pipelineInfra && ticket.Status == _statusNovoInfra))
				{
					var task = await _clickUpService.CreateTaskFromTicket(ticket, ticketPipeline);
					return task ? Ok("Task created successfully in ClickUp.") : StatusCode(500, "Failed to create task in ClickUp.");
				}
				else
					return Ok("Ticket does not meet the conditions for creating a task.");
			}
			catch (Exception ex)
			{
				//colocar na aws
				result.Message = ex.Message;
			}
			finally
			{
				Unlock(notification.ObjectId);
			}
		}

		return Ok(results);
	}

	#region Notifications LockList

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
