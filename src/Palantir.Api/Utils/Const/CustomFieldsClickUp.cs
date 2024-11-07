namespace Palantir.Api.Utils.Const
{
	/// <summary>
	/// The ids custom fields in ClickUp. The id name matches the custom field name in ClickUp
	/// </summary>
	public static class CustomFieldsClickUp
	{
		public const string ticketId = "471039af-a966-4bb0-aab3-5fd1cb92f014";
		public const string linkHubSpot = "4b809840-77df-4f7d-8e9a-8bef1000830e";
		public const string urlIntranet = "23a17861-33b0-476a-8ba0-981f3430ea3a";
		public const string tipo = "81250100-87b4-4174-8aad-0e699b856600";
		public const string funcionalidade = "3a672cc9-e65d-4162-8e30-9db1c52d5763";


		/// <summary>
		/// The ids of values for custom field 'Tipo' in ClickUp
		/// </summary>
		public static readonly Dictionary<string, List<string>> TipoValues = new Dictionary<string, List<string>>
		{
			{ "BUG", new List<string> { "9cd5709d-acdd-4eb2-a884-7f7200622664" } },
			{ "AJUSTE", new List<string> { "bf52ac14-42ce-4105-8551-5863dab5adc9" } },
			{ "Não Erro", new List<string> { "7f32912b-0b0d-4a50-9969-5ab45b82167f" } },
			{ "Indevido", new List<string> { "00539e25-1ffa-416a-a021-62af226b9e71" } },
			{ "Solicitação", new List<string> { "05154e22-d405-441c-a9f1-e158a10be81a" } },
			{ "Mudança regra", new List<string> { "c5c438a6-3528-4024-a800-e9238e07ac5b" } }
		};

		/// <summary>
		/// The ids of values for custom field 'Funcionalidade' in ClickUp
		/// </summary>
		public static readonly Dictionary<string, List<string>> FuncionalidadeValues = new Dictionary<string, List<string>>
		{
			{ "Cotação vida - Média", new List<string> { "ca621bbe-d75c-4fd7-81ad-283fcdaf70b6" } },
			{ "Cotação residencial", new List<string> { "4c6c952f-c954-41cd-b6d9-15e956cd4fdf" } },
			{ "Cotação automóvel Carro - Alta (Nova Jornada)", new List<string> { "4a99157f-4b60-40f0-8022-dd7cef3f2310" } },
			{ "Cotação automóvel Caminhão", new List<string> { "4a99157f-4b60-40f0-8022-dd7cef3f2310" } },
			{ "Cotação automóvel Caminhão - Média (Nova Jornada)", new List<string> { "4a99157f-4b60-40f0-8022-dd7cef3f2310" } },
			{ "Cotação automóvel Carro", new List<string> { "4a99157f-4b60-40f0-8022-dd7cef3f2310" } },
			{ "Cotação automóvel Moto", new List<string> { "4a99157f-4b60-40f0-8022-dd7cef3f2310" } },
			{ "Cotação Moto - (Nova Jornada)", new List<string> { "4a99157f-4b60-40f0-8022-dd7cef3f2310" } },
			{ "Renovações não iniciadas", new List<string> { "56e67dc8-50ea-4d16-8dae-6010c1e66770" } },
			{ "Renovações não iniciadas - Média (Nova Jornada)", new List<string> { "56e67dc8-50ea-4d16-8dae-6010c1e66770" } },
			{ "Painel HFy - Baixa (Nova Jornada)", new List<string> { "5185fe6e-c688-414e-81a8-23e3c3134b6d" } },
			{ "Painel HFy - Baixa", new List<string> { "5185fe6e-c688-414e-81a8-23e3c3134b6d" } },
			{ "Cotação empresarial", new List<string> { "5cee1e5b-4719-40a2-97ec-c3edc58c18bf" } },
			{ "Cotação Condomínio - Média", new List<string> { "457aa6ba-2ecb-46dc-83bb-db7613738f6a" } },
			{ "Orçamentos - Baixa (Nova Jornada)", new List<string> { "b23793b1-6b64-4625-883d-902ffaa2650c" } },
			{ "Orçamentos", new List<string> { "b23793b1-6b64-4625-883d-902ffaa2650c" } },
			{ "Relatório de Orçamentos", new List<string> { "6cb51c17-fd5f-437f-987d-105009bc71e1" } },
			{ "Importação de Propostas/Apólices", new List<string> { "bb418178-ca6c-410b-8630-6a22a8076c82" } },
			{ "Busca de propostas/apólices", new List<string> { "88ffc87d-9f59-4061-9030-67ae21b6688f" } },
			{ "Painel de Propostas e Apólices", new List<string> { "a0c1a1b7-0d6e-4803-9ee5-fe99ec9cd3ff" } },
			{ "Controle de itens", new List<string> { "686a89bd-85f1-404f-9689-247e6c80ec8b" } },
			{ "Acesso/senha", new List<string> { "0ddbc307-f6b0-415b-8082-bede788326bb" } },
			{ "Consulta LOG", new List<string> { "19d957ce-40e7-4c1b-bdd6-bd91d1abd8d4" } },
			{ "Corretora", new List<string> { "3ef5ace2-4694-4212-8da3-8a4d31a899bb" } },
			{ "Dashboard - Baixa", new List<string> { "9bec825c-1763-488f-9d52-88aa70c009db" } },
			{ "Devolução de base de dados", new List<string> { "8d9ed496-144f-4f75-9fda-37796b2a240c" } },
			{ "Exclusão de e-mail", new List<string> { "a29e19a8-13e5-4077-a248-7da145daef4b" } },
			{ "Grade Pagamento - Média", new List<string> { "956b3840-325f-474b-9d2c-ea22c5555c39" } },
			{ "Intranet", new List<string> { "af787a6c-47d3-408f-810d-567d4937d535" } },
			{ "Parametrização", new List<string> { "583451c5-4cab-4db8-a63a-d8ac7948ed0f" } },
			{ "Parcelas atrasadas", new List<string> { "84ef69ba-8684-425e-85a9-f9fddb0a8d95" } },
			{ "Parcelas do Segurado", new List<string> { "54bdc4f9-07d5-426d-85ce-74336c5b38fe" } },
			{ "Produtores", new List<string> { "c8af1b7f-1f24-4276-b7f7-ceb4c5e0b80a" } },
			{ "Ramos", new List<string> { "e5ee4f13-9837-4567-96b2-cef5c65c0dab" } },
			{ "Relatório de Pagamento de Comissão (aos produtores)", new List<string> { "ce3dc355-dea7-4dd6-8ca7-186fecb716b0" } },
			{ "Relatório de Comissões da Corretora", new List<string> { "528a231d-2473-48d5-86fe-c3367e670cdd" } },
			{ "Relatório de Análise Financeira", new List<string> { "28e2dc14-13a5-4cb6-939b-981c1392a86e" } },
			{ "Relatório de Renovações", new List<string> { "f0301264-8ebf-4f2a-bab5-2ce4600f9c5e" } },
			{ "Relatório de Pendências de Emissão", new List<string> { "2a40a600-3ad0-4d18-addf-64b6c31a7178" } },
			{ "Relatório de Mix de Carteira", new List<string> { "b56434b0-76f2-45e4-8829-5ef8d9181f37" } },
			{ "Relatório de Produção", new List<string> { "806cec58-eb23-4051-8bcd-3a5b7abd7b49" } },
			{ "Seguradoras", new List<string> { "94275015-12b6-4ad0-820a-7bebacc8cf01" } },
			{ "Segurados", new List<string> { "996c3f33-1851-4b4a-b73b-77966e8c6df3" } },
			{ "Sinistros", new List<string> { "d511a0f7-ad38-482e-bae2-7c5744a651c0" } },
			{ "Solicitação", new List<string> { "4529b0ba-0058-4e24-aa96-558d2949c029" } },
			{ "Tarefas", new List<string> { "5d9bc317-5556-4b65-8c6f-2a4c88f5a67c" } },
			{ "Usuários", new List<string> { "2b01d3d5-04c7-4c72-859e-dcc99a30d6ba" } },
			{ "Comunicador da Corretora", new List<string> { "75cb99e9-2426-4fa0-bebf-c22b5c2bd695" } },
			{ "API Multicálculo", new List<string> { "15a3a8e5-68b0-4a2a-aa47-a0738160f172" } },
			{ "Atualização Tabela FIPE", new List<string> { "75b4bbbf-4e7e-4263-8465-68b918d556e5" } },
			{ "Baixa de comissão", new List<string> { "074905cd-12c8-43a2-8a0b-50fb6c548ea4" } },
			{ "Busca de Extratos", new List<string> { "cd7c5d56-1637-423d-bacd-9733e9396e25" } },
			{ "Busca Parcelas atrasadas", new List<string> { "f4d41b1e-9b7f-453d-bd05-729c03bf4c80" } },
			{ "Calcular renovação - Alta (Nova Jornada)", new List<string> { "a30bfe3e-978c-4792-97b5-dd48bb678a72" } },
			{ "Calcular renovação", new List<string> { "a30bfe3e-978c-4792-97b5-dd48bb678a72" } },
			{ "Controle de endossos", new List<string> { "18d3cead-73a9-4d9d-8bb5-2b025c5ae103" } },
			{ "Controle de Faturas", new List<string> { "65dbf17d-af02-499a-9b8f-12d77fb356f8" } },
			{ "Correções de ambientes - Geral DEVS (Infra)", new List<string> { "11323b34-1ed5-424b-a2dc-c09647838f5f" } },
			{ "Correções de ambientes - Um DEV - Baixo", new List<string> { "64492fb4-5edf-495d-b087-787589299cb7" } },
			{ "Decodificação CPF - Baixa (Nova Jornada)", new List<string> { "abbd9f61-32ad-4c8a-85ef-15a86c8fdfd7" } },
			{ "Decodificação CPF", new List<string> { "abbd9f61-32ad-4c8a-85ef-15a86c8fdfd7" } },
			{ "Decodificação de chassi - Baixa (Nova Jornada)", new List<string> { "741f9d1f-0613-4f35-8a6e-99745ba3074d" } },
			{ "Decodificação Placa - Baixa (Nova Jornada)", new List<string> { "d5e4d64b-158a-4ecc-bb91-4046959fde4c" } },
			{ "Desligamentos (Infra)", new List<string> { "4867ac2e-b727-4844-baf7-83750eb6e01f" } },
			{ "Divergência de Valores - Alta (Nova Jornada)", new List<string> { "ecfb60ca-dac6-4402-aa4e-619004e965d2" } },
			{ "Divergência de Valores", new List<string> { "ecfb60ca-dac6-4402-aa4e-619004e965d2" } },
			{ "E-mail marketing", new List<string> { "103b74e1-58cc-4e84-8874-bde348bb49ad" } },
			{ "Ficha do Seguro - Baixa", new List<string> { "f950f718-724a-4df1-843e-562c3a1492bf" } },
			{ "Fluxo de caixa", new List<string> { "d57e90dd-0f98-48a9-9250-47bbe8f79e76" } },
			{ "Grade de recebimento", new List<string> { "fc6955f2-d0a9-40e1-8c8d-3becc9546c82" } },
			{ "LGPD: Dados - Alto", new List<string> { "0ab99a5b-04f3-4ce9-8c16-0ab15d787e3a" } },
			{ "LGPD: Termos de Uso - Média", new List<string> { "233d44ab-8e24-4987-b803-b4062c7b4adc" } },
			{ "Liberar MC Saúde", new List<string> { "436a9da0-5f63-4142-a66c-44d509a6d222" } },
			{ "Saúde - Baixo", new List<string> { "436a9da0-5f63-4142-a66c-44d509a6d222" } },
			{ "Liberação de Acessos (Infra)", new List<string> { "bbba90c0-c476-4767-a14f-ac9adad87ae4" } },
			{ "Logins das Seguradoras - Alta (Nova Jornada)", new List<string> { "79af074c-9206-429a-923d-f06b3d406138" } },
			{ "Logins das Seguradoras", new List<string> { "79af074c-9206-429a-923d-f06b3d406138" } },
			{ "Migração/Reativação Segfy", new List<string> { "b17e65da-c2c0-44b3-b5af-1181b14430d7" } },
			{ "Reativação", new List<string> { "b17e65da-c2c0-44b3-b5af-1181b14430d7" } },
			{ "Novos Segfyers (Infra)", new List<string> { "dafc9449-87e2-4f27-a597-c92ca9b7f4d3" } },
			{ "Parcelas e Comissões", new List<string> { "0b190ed3-3f1a-47f8-a1b9-56b531594758" } },
			{ "Permissão de documentos (Infra)", new List<string> { "72094f87-a150-4df4-9398-6d113da36958" } },
			{ "Planilhas para importação", new List<string> { "31284856-0206-4de8-9ff4-3e520d6a8f53" } },
			{ "Plugin de acesso ao cálculo nas cias - Média (Nova Jornada)", new List<string> { "8bd04453-2386-4c2a-a236-23a155982b60" } },
			{ "Plugin de acesso ao cálculo nas cias", new List<string> { "8bd04453-2386-4c2a-a236-23a155982b60" } },
			{ "Página pública - Média (Nova Jornada)", new List<string> { "5ea8e37a-b7a4-4ff0-aae9-f86eb93c338e" } },
			{ "Página pública", new List<string> { "5ea8e37a-b7a4-4ff0-aae9-f86eb93c338e" } },
			{ "Repasse aos produtores", new List<string> { "d1e74c92-eb7a-4be7-bdc6-91137f6c81f5" } },
			{ "Reset de senha (Infra)", new List<string> { "cdd2b271-f727-41a6-9c8c-a050ec7e9614" } },
			{ "RoboCote", new List<string> { "b2b066c1-a3be-4219-afac-8d663c3d6c23" } },
			{ "Propostas e Apólices", new List<string> { "2b201504-4ef0-48f9-9287-3d927271a3a3" } },
			{ "Zerar Base", new List<string> { "444cbaab-377b-4c35-96ea-63ce38d32ec9" } },
			{ "Sistema fora do ar/instável", new List<string> { "9e647f3d-b92c-4421-96a8-3c62f1fcb906" } }
		};
	}
}
