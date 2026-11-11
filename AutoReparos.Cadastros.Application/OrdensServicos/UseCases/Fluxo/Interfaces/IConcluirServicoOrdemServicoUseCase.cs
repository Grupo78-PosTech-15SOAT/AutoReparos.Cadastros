namespace AutoReparos.Cadastros.Application.OrdensServicos.UseCases.Fluxo.Interfaces
{
    public interface IConcluirServicoOrdemServicoUseCase
    {
        Task ExecuteAsync(Guid ordemServicoId, Guid ordemServicoServicoId);
    }
}
