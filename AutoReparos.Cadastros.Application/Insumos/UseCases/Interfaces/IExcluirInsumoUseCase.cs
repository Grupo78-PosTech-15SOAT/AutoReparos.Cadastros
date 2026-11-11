namespace AutoReparos.Cadastros.Application.Insumos.UseCases.Interfaces
{
    public interface IExcluirInsumoUseCase
    {
        Task ExecuteAsync(Guid id);
    }
}
