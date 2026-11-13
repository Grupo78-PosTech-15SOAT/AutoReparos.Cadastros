using Xunit;

namespace AutoReparos.Cadastros.IntegrationTests.Infrastructure
{
    [CollectionDefinition("IntegrationTests")]
    public class IntegrationTestCollection : ICollectionFixture<CustomWebApplicationFactory<Program>>
    {
        // Esta classe não contém código. 
        // Ela serve para o xUnit compartilhar a Factory (e o banco Docker).
    }
}
