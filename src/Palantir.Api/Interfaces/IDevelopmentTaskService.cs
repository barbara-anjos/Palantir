using static Palantir.Api.Models.HubSpotTicketModel.HubSpotWebhookRequest;

namespace Palantir.Api.Interfaces
{
	public interface IDevelopmentTaskService<T>
	{
		//criar tarefa
		Task<bool> CreateTaskFromTicket(T ticketProperties);

		//atualizar tarefa

		//buscar tarefa
	}
}
