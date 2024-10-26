using Palantir.Api.Models;
using static Palantir.Api.Models.ClickUpTaskModel;
using static Palantir.Api.Models.HubSpotTicketModel.HubSpotWebhookRequest;

namespace Palantir.Api.Interfaces
{
	public interface IDevelopmentTaskService
	{
		Task<SegfyTask> CreateTask(SegfyTask newTask);

		//criar tarefa
		Task<bool> CreateTaskFromTicket(SegfyTask ticketProperties, string s);

		//atualizar tarefa
		//Task UpdateTaskAsync(string taskId, T updatedData);

		//buscar tarefa
		Task<SegfyTask> GetTaskIdByTicketIdAsync(string ticketId);

		//excluir tarefa
		//Task DeleteTaskAsync(string taskId);
	}
}
