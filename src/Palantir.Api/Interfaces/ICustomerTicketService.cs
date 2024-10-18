namespace Palantir.Api.Interfaces
{
	public interface ICustomerTicketService<T>
	{
		//buscar tíquete
		Task<T> GetTicketByIdAsync(long ticketId);


		//atualizar tíquete
		//Task UpdateTicketStatusAsync(string ticketId, string newStatus);
	}
}
