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

        Task<SegfyTask> GetTaskById(string id);

        //atualizar tarefa
        //Task UpdateTaskAsync(string taskId, T updatedData);

        //buscar tarefa
        Task<SegfyTaskList> GetTaskIdByTicketIdAsync(string ticketId);

		//excluir tarefa
		//Task DeleteTaskAsync(string taskId);
	}
}
