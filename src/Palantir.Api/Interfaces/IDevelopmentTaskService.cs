using Palantir.Api.Models;
using static Palantir.Api.Models.ClickUpTaskModel;
using static Palantir.Api.Models.HubSpotTicketModel.HubSpotWebhookRequest;

namespace Palantir.Api.Interfaces
{
	public interface IDevelopmentTaskService
	{
		Task<ClickUpTask> CreateTask(ClickUpTask newTask);

		//criar tarefa
		Task<bool> CreateTaskFromTicket(SegfyTask ticketProperties, string s);

        Task<ClickUpTask> GetTaskById(string id);

		Task<ClickUpTask> UpdateTask(ClickUpTask updatedTask, string taskId);

        //atualizar tarefa
        Task <bool>UpdateTaskFromTicket(string taskId, SegfyTask updatedData);

        //buscar tarefa
        Task<TaskList> GetTaskIdByTicketIdAsync(string ticketId);

		//excluir tarefa
		//Task DeleteTaskAsync(string taskId);
	}
}
