namespace AutoReparos.Cadastros.IntegrationTests.Infrastructure.Config;

public record DbContainerSettings(
    string Image = "postgres:16-alpine",
    string Database = "cadastros_test",
    string Username = "admin",
    string Password = "admin123"
);
