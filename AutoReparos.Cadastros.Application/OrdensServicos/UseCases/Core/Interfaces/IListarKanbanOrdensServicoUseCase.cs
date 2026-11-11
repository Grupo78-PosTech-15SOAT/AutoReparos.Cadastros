using System.Collections.Generic;
using System.Threading.Tasks;
using AutoReparos.Cadastros.Application.OrdensServicos.DTOs.Response;

namespace AutoReparos.Cadastros.Application.OrdensServicos.UseCases.Core.Interfaces
{
    public interface IListarKanbanOrdensServicoUseCase
    {
        Task<IEnumerable<KanbanColumnDto>> ExecuteAsync();
    }
}
