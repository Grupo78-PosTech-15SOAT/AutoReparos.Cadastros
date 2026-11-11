using System;
using System.Collections.Generic;
using AutoReparos.Cadastros.Application.OrdensServicos.DTOs.Response;
using AutoReparos.Cadastros.Application.OrdensServicos.Strategies.Interfaces;
using AutoReparos.Cadastros.Domain.OrdensServicos.Entities;
using AutoReparos.Cadastros.Domain.OrdensServicos.Enums;

namespace AutoReparos.Cadastros.Application.OrdensServicos.Strategies
{
    public class FinishedKanbanCardStrategy : IKanbanCardStrategy
    {
        public bool CanHandle(EStatusOrdemServico status)
            => status == EStatusOrdemServico.Finalizada || status == EStatusOrdemServico.Entregue;

        public string ColumnKey => "Finalizada";

        public KanbanCardDto CreateCard(OrdemServico os, IDictionary<string, string> usuarioDict)
        {
            var clienteNome = os.Cliente?.Nome ?? "Cliente não encontrado";
            var placaVeiculo = os.Veiculo?.Placa?.Valor ?? "Placa não encontrada";
            var modeloVeiculo = os.Veiculo?.Modelo ?? "Modelo não encontrado";

            var isEntregue = os.Status == EStatusOrdemServico.Entregue;
            var dataConclusaoOuEntrega = isEntregue
                ? (os.EntregueEm ?? os.FinalizadoEm ?? DateTime.UtcNow)
                : (os.FinalizadoEm ?? DateTime.UtcNow);

            return new FinishedKanbanCardDto(
                os.Id, clienteNome, placaVeiculo, modeloVeiculo,
                os.Status.ToString(), os.ValorTotal,
                dataConclusaoOuEntrega.ToLocalTime(),
                isEntregue ? "Entregue" : "Aguardando Entrega");
        }
    }
}
