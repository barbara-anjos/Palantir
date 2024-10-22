using Microsoft.EntityFrameworkCore.Storage.ValueConversion.Internal;

namespace Palantir.Api.Utils.Const
{
	/// <summary>
	/// Represents the rules for ticket tags
	/// </summary>
	public static class TicketTagRulesDictionary
    {
        public const string PORTO = "porto";
		public const string AZUL = "azul";
		public const string AZULPORASSINATURA = "azul por assinatura";
		public const string BLLU = "bllu"; //BLLU and AZULPORASSINATURA are the same, but the ticket name can contain one or the other
		public const string ITAU = "itau";
		public const string MITSUI = "mitsui";
		public const string ZERARBASE = "zerar base";
		public const string DEVOLUCAOBASE = "devolução de base";
	}
}
