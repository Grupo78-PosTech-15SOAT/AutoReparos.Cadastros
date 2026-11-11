namespace AutoReparos.Cadastros.Application.OrdensServicos.UseCases.Aprovacao.Interfaces
{
    public interface IAprovarOrdemServicoUseCase
    {
        Task ExecuteAsync(string token);
    }
}
