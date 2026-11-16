namespace AutoReparos.Cadastros.Application.Clientes.UseCases.Interfaces
{
    public interface IExcluirClienteUseCase
    {
        Task ExecuteAsync(Guid id);
    }
}
