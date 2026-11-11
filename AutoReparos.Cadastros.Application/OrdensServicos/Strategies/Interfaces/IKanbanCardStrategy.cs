using System;
using System.Collections.Generic;
using AutoReparos.Cadastros.Application.OrdensServicos.DTOs.Response;
using AutoReparos.Cadastros.Domain.OrdensServicos.Entities;
using AutoReparos.Cadastros.Domain.OrdensServicos.Enums;

namespace AutoReparos.Cadastros.Application.OrdensServicos.Strategies.Interfaces
{
    public interface IKanbanCardStrategy
    {
        bool CanHandle(EStatusOrdemServico status);
        string ColumnKey { get; }
        KanbanCardDto CreateCard(OrdemServico os, IDictionary<string, string> usuarioDict);
    }
}
