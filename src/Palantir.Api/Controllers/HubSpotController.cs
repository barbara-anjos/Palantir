using Flurl.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Microsoft.VisualBasic;
using Palantir.Api.Configurations;
using Palantir.Api.Enums;
using Palantir.Api.Interfaces;
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
    private List<long> lockList;
    private readonly string _pipelineGestao;
    private readonly string _pipelineAutomacao;
    private readonly string _pipelineInfra;
    private readonly string _statusNovoGestao;
    private readonly string _statusNovoAutomacao;
    private readonly string _statusNovoInfra;

    public HubSpotController(IOptions<HubSpotSettings> hubSpotSettings, IDevelopmentTaskService clickUpService, ICustomerTicketService<HubSpotTicketResponse> hubSpotService)
    {
        _hubSpotApiKey = hubSpotSettings.Value.ApiKey;
        _clickUpService = clickUpService;
        _hubSpotService = hubSpotService;
        lockList = new List<long>();
        _pipelineGestao = hubSpotSettings.Value.GestaoPipelineId;
        _pipelineAutomacao = hubSpotSettings.Value.AutomacaoPipelineId;
        _pipelineInfra = hubSpotSettings.Value.InfraPipelineId;
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
                var eventType = !string.IsNullOrEmpty(notification.SubscriptionType) ? notification.SubscriptionType : ""; // notification.EventType;

                if (eventType != "ticket.creation" && eventType != "ticket.propertyChange")
                    throw new Exception("Only processes ticket creation notifications.");

                //Get created ticket details
                var ticket = await _hubSpotService.GetTicketByIdAsync(notification.ObjectId);
                if (ticket == null)
                    return NotFound("Ticket not found or invalid.");

                //Check the status to create the task
                var ticketStatus = ticket.Properties.Status;
                if (ticketStatus != _statusNovoGestao && ticketStatus != _statusNovoAutomacao && ticketStatus != _statusNovoInfra)
                    throw new Exception("Ticket status different than expected: Novo.");

                //Check if the task already exists
                var existTask = await _clickUpService.GetTaskIdByTicketIdAsync(ticket.Id);
                if (existTask != null && existTask.Tasks.Count > 0)
                    throw new Exception("Task alredy created.");

                //Check the ticket pipeline to create the task with the correct pipeline tag
                var ticketPipeline = ticket.Properties.Pipeline;
                if (ticketPipeline == _pipelineGestao)
                    ticketPipeline = "Gestão";
                if (ticketPipeline == _pipelineAutomacao)
                    ticketPipeline = "Automação";
                if (ticketPipeline == _pipelineInfra)
                    ticketPipeline = "Infra";

                var priority = HubSpotTicketPrioritySLAConstants.GetPriority(ticket.Properties.Priority?? string.Empty);
                var prioritySegfy = HubSpotTicketPrioritySLAConstants.GetPriority(ticket.Properties.PrioritySegfy ?? string.Empty);

                //Dealing with inverted priorities values, as changing them in HubSpot would be a hassle
                if (prioritySegfy == HubSpotTicketSLA.LOW)
                    prioritySegfy = HubSpotTicketSLA.HIGH;
                else if (prioritySegfy == HubSpotTicketSLA.HIGH)
                    prioritySegfy = HubSpotTicketSLA.LOW;

                var timeEstimate = (int)prioritySegfy;
                var startDate = ticket.Properties.SendAt ?? ticket.Properties.CreateDate;
                var dueDate = startDate.WorkingHours(timeEstimate);

                //Only create the task if the ticket is in the expected pipeline and status
                if ((ticketPipeline == "Gestão" && ticketStatus == _statusNovoGestao)
                    || (ticketPipeline == "Automação" && ticketStatus == _statusNovoAutomacao)
                    || (ticketPipeline == "Infra" && ticketStatus == _statusNovoInfra))
                {
                    var segfyTask = new SegfyTask()
                    {
                        Name = ticket.Properties.Name,
                        Description = ticket.Properties.Content,
                        StartDate = new DateTimeOffset(startDate).ToUnixTimeMilliseconds(),
                        DueDate = new DateTimeOffset(dueDate).ToUnixTimeMilliseconds(),
                        TimeEstimate = timeEstimate,
                        PriorityId = HubSpotTicketPrioritySLAConstants.PriorityMap[prioritySegfy],
                        PriorityName = prioritySegfy.ToString(),
                        Category = ticket.Properties.Category,
                        Services = ticket.Properties.Services,
                        LinkIntranet = ticket.Properties.LinkIntranet,
                        TicketId = ticket.Properties.Id
					};
                    var task = await _clickUpService.CreateTaskFromTicket(segfyTask, ticketPipeline);
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

			results.Add(result);
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

    ////Atualizar tarefa no ClickUp com base em alterações do tíquete
   // [HttpPost]
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
