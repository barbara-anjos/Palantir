namespace Palantir.Api.Interfaces
{
	public interface ICustomerTicketService<T>
	{
		Task<T> GetTicketByIdAsync(long ticketId);

		Task<bool> UpdateTicketFromTask(string ticketId, T updateData);
	}
}
