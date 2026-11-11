using System.Collections.Generic;
using AutoReparos.Cadastros.Application.OrdensServicos.DTOs.Response;
using AutoReparos.Cadastros.Application.OrdensServicos.Strategies.Interfaces;
using AutoReparos.Cadastros.Domain.OrdensServicos.Entities;
using AutoReparos.Cadastros.Domain.OrdensServicos.Enums;

namespace AutoReparos.Cadastros.Application.OrdensServicos.Strategies
{
    public class ApprovalKanbanCardStrategy : IKanbanCardStrategy
    {
        public bool CanHandle(EStatusOrdemServico status) => status == EStatusOrdemServico.AguardandoAprovacao;
        public string ColumnKey => "Aprovacao";

        public KanbanCardDto CreateCard(OrdemServico os, IDictionary<string, string> usuarioDict)
        {
            var clienteNome = os.Cliente?.Nome ?? "Cliente não encontrado";
            var placaVeiculo = os.Veiculo?.Placa?.Valor ?? "Placa não encontrada";
            var modeloVeiculo = os.Veiculo?.Modelo ?? "Modelo não encontrado";

            return new ApprovalKanbanCardDto(
                os.Id, clienteNome, placaVeiculo, modeloVeiculo,
                os.Status.ToString(), os.ValorTotal, os.Servicos.Count, os.EnvioAprovacaoEm);
        }
    }
}
