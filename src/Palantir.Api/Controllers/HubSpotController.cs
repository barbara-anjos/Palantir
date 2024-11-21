using Flurl.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Microsoft.VisualBasic;
using Palantir.Api.Configurations;
using Palantir.Api.Enums;
using Palantir.Api.Interfaces;
using Palantir.Api.Mappers;
using Palantir.Api.Models;
using Palantir.Api.Services;
using Palantir.Api.Utils;
using System.Diagnostics.Eventing.Reader;
using System.Net.Sockets;
using static Palantir.Api.Models.ClickUpTaskModel;
using static Palantir.Api.Models.HubSpotTicketModel;
using static Palantir.Api.Models.HubSpotTicketModel.HubSpotWebhookRequest;

[ApiController]
[Route("api/hubspot")]
public class HubSpotController : ControllerBase
{
	private readonly string _hubSpotApiKey;
	private readonly IDevelopmentTaskService _clickUpService;
	private readonly ICustomerTicketService<HubSpotTicketResponse> _hubSpotService;
	private List<long> _lockList;
	private readonly string _pipelineGestao;
	private readonly string _pipelineAutomacao;
	private readonly string _pipelineInfra;
	private readonly ILogger<HubSpotController> _logger;
	private readonly StatusMapper _statusMapper;

	public HubSpotController(IOptions<HubSpotSettings> hubSpotSettings, IDevelopmentTaskService clickUpService, ICustomerTicketService<HubSpotTicketResponse> hubSpotService, StatusMapper statusMapper)
	{
		_hubSpotApiKey = hubSpotSettings.Value.ApiKey;
		_clickUpService = clickUpService;
		_hubSpotService = hubSpotService;
		_lockList = new List<long>();
		_pipelineGestao = hubSpotSettings.Value.GestaoPipelineId;
		_pipelineAutomacao = hubSpotSettings.Value.AutomacaoPipelineId;
		_pipelineInfra = hubSpotSettings.Value.InfraPipelineId;
		_statusMapper = statusMapper;
}

	/// <summary>
	/// Create or Update a task in ClickUp from a ticket in HubSpot
	/// </summary>
	/// <param name="notifications"></param>
	/// <returns></returns>
	[HttpPost("ticket")]
	public async Task<IActionResult> CreateOrUpdateTaskFromTicket([FromBody] List<HubSpotNotification> notifications)
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
				var eventType = !string.IsNullOrEmpty(notification.SubscriptionType) ? notification.SubscriptionType : "";

				if (eventType != "ticket.creation" && eventType != "ticket.propertyChange")
					throw new Exception("Only processes ticket creation or update notifications.");

				//Get created ticket details
				var ticket = await _hubSpotService.GetTicketByIdAsync(notification.ObjectId);
				if (ticket == null)
					return NotFound("Ticket not found or invalid.");

				//Check the ticket status to set task status
				var ticketStatus = ticket.Properties.Status;

				//Check the ticket pipeline to create the task with the correct pipeline tag
				var ticketPipeline = ticket.Properties.Pipeline;
				if (ticketPipeline == _pipelineGestao)
					ticketPipeline = "Gestão";
				if (ticketPipeline == _pipelineAutomacao)
					ticketPipeline = "Automação";
				if (ticketPipeline == _pipelineInfra)
					ticketPipeline = "Infra";

				var priority = HubSpotTicketPrioritySLAConstants.GetPriority(ticket.Properties.Priority ?? string.Empty);
				var prioritySegfy = HubSpotTicketPrioritySLAConstants.GetPriority(ticket.Properties.PrioritySegfy ?? string.Empty);

				//Dealing with inverted priorities values, as changing them in HubSpot would be a hassle
				if (prioritySegfy == HubSpotTicketSLA.LOW)
					prioritySegfy = HubSpotTicketSLA.HIGH;
				else if (prioritySegfy == HubSpotTicketSLA.HIGH)
					prioritySegfy = HubSpotTicketSLA.LOW;

				var timeEstimate = (int)prioritySegfy;
				var startDate = ticket.Properties.SendAt ?? DateTime.Now;
				var dueDate = startDate.WorkingHours(timeEstimate);

				//Check if the task already exists
				var existTask = await _clickUpService.GetTaskIdByTicketIdAsync(ticket.Id);

				//Check if the ticket is in the expected pipeline
				if (ticketPipeline == "Gestão" || ticketPipeline == "Automação" || ticketPipeline == "Infra")
				{
					var segfyTask = new SegfyTask()
					{
						Name = ticket.Properties.Name,
						Description = ticket.Properties.Content,
						StartDate = new DateTimeOffset(startDate).ToUnixTimeMilliseconds(),
						DueDate = new DateTimeOffset(dueDate).ToUnixTimeMilliseconds(),
						TimeEstimate = timeEstimate * 3600000,
						PriorityId = HubSpotTicketPrioritySLAConstants.PriorityMap[prioritySegfy],
						PriorityName = prioritySegfy.ToString(),
						Category = ticket.Properties.Category,
						Services = ticket.Properties.Services,
						LinkIntranet = ticket.Properties.LinkIntranet,
						TicketId = ticket.Properties.Id,
						Status = _statusMapper.GetHubSpotStatus(ticketStatus, ticketPipeline)
					};

					//Notification type is ticket.creation and the task does not exist -> create task
					//Notification type is ticket.propertyChange and the does not exist -> create task
					if (existTask == null || existTask.Tasks.Count <= 0)
					{
						var task = await _clickUpService.CreateTaskFromTicket(segfyTask, ticketPipeline);
						return task ? Ok("Task created successfully in ClickUp.") : StatusCode(500, "Failed to create task in ClickUp.");
					}
					//Notification type is ticket.creation and the task exists -> update task				
					//Notification type is ticket.propertyChange and the task exist -> update task
					else
					{
						var updatedPropertiesName = notification.PropertyName;
						var updatedPropertiesValue = notification.PropertyValue;
						var taskId = existTask.Tasks[0].Id;

						switch (updatedPropertiesName)
						{
							case "hs_pipeline_stage":
								segfyTask.Status = updatedPropertiesValue;
								break;
							case "subject":
								segfyTask.Name = updatedPropertiesValue;
								break;
							case "content":
								segfyTask.Description = updatedPropertiesValue;
								break;
							case "data_envio___dev":
								if (DateTimeOffset.TryParse(updatedPropertiesValue, out var parsedDate))
								{
									var startDateUpdated = parsedDate.DateTime;
									var dueDateUpdated = startDateUpdated.WorkingHours(timeEstimate);
									segfyTask.StartDate = new DateTimeOffset(startDateUpdated).ToUnixTimeMilliseconds();
									segfyTask.DueDate = new DateTimeOffset(dueDateUpdated).ToUnixTimeMilliseconds();
								}
								break;
							case "hs_ticket_priority":
								if (HubSpotTicketPrioritySLAConstants.PriorityDictionary.TryGetValue(updatedPropertiesValue, out var priorityId))
								{
									segfyTask.PriorityId = (int)priorityId;
									timeEstimate = (int)priorityId * 3600000;
								}
								break;
							case "link_intranet":
								segfyTask.LinkIntranet = updatedPropertiesValue;
								break;
							case "categoria":
								segfyTask.Category = updatedPropertiesValue;
								break;
							case "servicos__clonado_":
								segfyTask.Services = updatedPropertiesValue;
								break;
							default:
								return Ok("Updated property is not tracked.");
						}

						var taskUpdated = await _clickUpService.UpdateTaskFromTicket(taskId, segfyTask, ticketPipeline);
						results.Add(result);
						return taskUpdated ? Ok("Task updated successfully in ClickUp.") : StatusCode(500, "Failed to update task in ClickUp.");
					}
				}
				else
				{
					results.Add(result);
					return Ok("Ticket pipeline different than expected: Gestão, Automação, Infra.");
				}
			}
			catch (Exception ex)
			{
				return StatusCode(500, $"Failed to update the ticket in HubSpot: {ex.Message}");
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
		lock (_lockList)
		{
			if (_lockList.Contains(ticketId))
			{
				return false;
			}

			_lockList.Add(ticketId);
			return true;
		}
	}

	private void Unlock(long ticketId)
	{
		lock (_lockList)
		{
			_lockList.Remove(ticketId);
		}
	}

	#endregion

	[HttpGet("{ticketId}")]
	public async Task<IActionResult> GetTicket(long ticketId)
	{
		try
		{
			var ticket = await _hubSpotService.GetTicketByIdAsync(ticketId);

			if (ticket == null)
				return NotFound();

			return Ok(ticket);
		}
		catch (Exception ex)
		{
			throw new Exception($"Error searching ticket in HubSpot: {ex.Message}");
		}
	}

}
