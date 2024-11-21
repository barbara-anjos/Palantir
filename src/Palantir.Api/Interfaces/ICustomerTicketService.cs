using Palantir.Api.Models;

namespace Palantir.Api.Interfaces
{
	public interface ICustomerTicketService<T>
	{
		Task<T> GetTicketByIdAsync(long ticketId);

		Task<T> UpdateTicket(T ticket, string ticketId);

		Task<bool> UpdateTicketFromTask(string ticketId, SegfyTask updateData, string pipeline);
	}
}
