using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Palantir.Api.Configurations;
using Palantir.Api.Interfaces;
using Palantir.Api.Models;
using Palantir.Api.Utils;
using Palantir.Api.Utils.Const;
using static Palantir.Api.Models.ClickUpTaskModel;
using static Palantir.Api.Models.HubSpotTicketModel.HubSpotWebhookRequest;

[ApiController]
[Route("api/clickup")]
public class ClickUpController : ControllerBase
{
	private readonly IDevelopmentTaskService _clickUpService;
	private readonly ICustomerTicketService<HubSpotTicketResponse> _hubSpotService;
	private readonly LockManager<string> _lockManager;

	public ClickUpController(IOptions<ClickUpSettings> clickUpSettings, IDevelopmentTaskService clickUpService, ICustomerTicketService<HubSpotTicketResponse> hubSpotService)
	{
		_hubSpotService = hubSpotService;
		_clickUpService = clickUpService;
		_lockManager = new LockManager<string>();
	}

	/// <summary>
	/// Endpoint to create a new task in ClickUp
	/// </summary>
	/// <param name="newTask"></param>
	/// <returns></returns>
	[HttpPost("create-task")]
	public async Task<IActionResult> CreateTask([FromBody] ClickUpTask newTask)
	{
		if (newTask == null || string.IsNullOrEmpty(newTask.Name))
		{
			return BadRequest("Invalid task data.");
		}

		var createdTask = await _clickUpService.CreateTask(newTask);
		return Ok(createdTask);
	}

	/// <summary>
	/// Endpoint to get a task by ID
	/// </summary>
	/// <param name="id"></param>
	/// <returns></returns>
	[HttpGet("get-task/{id}")]
	public async Task<IActionResult> GetTask(string id)
	{
		var task = await _clickUpService.GetTaskById(id);
		return Ok(task);
	}

	/// <summary>
	/// Endpoint to get a task by the custom field TicketId
	/// </summary>
	/// <param name="id"></param>
	/// <returns></returns>
	[HttpGet("get-task-ticketId")]
	public async Task<IActionResult> GetTaskByTicketId(string id)
	{
		var task = await _clickUpService.GetTaskIdByTicketIdAsync(id);
		return Ok(task);
	}

	[HttpPost("update-ticket")]
	public async Task<IActionResult> UpdateTicketFromTask([FromBody] ClickUpWebhookPayload payload)
	{
		string taskId = payload.Task_id;

		if (!_lockManager.Lock(taskId))
			return Ok("Task already been processed");

		try
		{
			//Get task details
			var task = await _clickUpService.GetTaskById(taskId);
			if (task == null)
				return NotFound("Task not found in ClickUp.");

			//Check if the task corresponds to a ticket in HubSpot
			var ticketIdField = task.CustomFields?.FirstOrDefault(cf => cf.Id == CustomFieldsClickUp.ticketId);
			if (ticketIdField == null || string.IsNullOrEmpty((string?)ticketIdField.Value))
				return NotFound("Task does not corresponds to a ticket.");

			//Get ticket pipeline
			var ticketPipelineName = task.Tags?.FirstOrDefault(t => t.Name == "gestão" || t.Name == "automação" || t.Name == "infra")?.Name;
			if (string.IsNullOrEmpty(ticketPipelineName))
				return NotFound("Task does not corresponds to a ticket.");

			string ticketId = (string)ticketIdField.Value;
			string ticketPipeline = (string)ticketPipelineName;
			var updatedData = new SegfyTask();

			//Check if the ticket exists in HubSpot
			var ticket = await _hubSpotService.GetTicketByIdAsync(long.Parse(ticketId));
			if (ticket == null)
				return NotFound("Ticket not found in HubSpot.");

			// Update the status or priority of the ticket in HubSpot based on the payload
			if (payload.History_items.Any(hi => hi.Field == "status"))
			{
				updatedData.Status = payload.History_items.First(hi => hi.Field == "status").After.Status;
			}
			else if (payload.History_items.Any(hi => hi.Field == "priority"))
			{
				updatedData.PriorityName = payload.History_items.First(hi => hi.Field == "priority").After.Priority;
			}

			var updateResult = await _hubSpotService.UpdateTicketFromTask(ticketId, updatedData, ticketPipeline);
			return updateResult ? Ok("Ticket status or priority updated successfully.") : StatusCode(500, "Failed to update the ticket in HubSpot.");
		}
		catch (Exception ex)
		{
			return StatusCode(500, $"Failed to update the ticket in HubSpot: {ex.Message}");
		}
		finally
		{
			_lockManager.Unlock(taskId);
		}
	}
}
