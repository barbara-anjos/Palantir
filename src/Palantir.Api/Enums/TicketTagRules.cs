namespace Palantir.Api.Enums
{
	/// <summary>
	/// Represents the Grupo Porto rules for ticket tags
	/// </summary>
	public enum TicketTagRulesGrupoPorto
	{ 
		PORTO,
		AZUL,
		ITAU,
		MITSUI,
		AZULPORASSINATURA, //BLLU and AZULPORASSINATURA are the same, but the ticket name can contain one or the other
		BLLU
	}

	/// <summary>
	/// Represents the rules for ticket tags
	/// </summary>
	public enum TicketTagRules
    {
        ZERARBASE,
		DEVOLUCAOBASE
    }
}
