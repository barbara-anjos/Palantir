using static Palantir.Api.Models.ClickUpTaskModel;
using static Palantir.Api.Models.HubSpotTicketModel.HubSpotWebhookRequest;

namespace Palantir.Api.Interfaces
{
	public interface IDevelopmentTaskService<T, R>
	{
		//criar tarefa
		Task<bool> CreateTaskFromTicket(T ticketProperties, string s);

		//atualizar tarefa
		//Task UpdateTaskAsync(string taskId, T updatedData);

		//buscar tarefa
		Task<R> GetTaskIdByTicketIdAsync(string ticketId);

		//excluir tarefa
		//Task DeleteTaskAsync(string taskId);
	}
}
