# AutoReparos.Cadastros - Backend (API e Domínio)

> **Projeto:** AutoReparos - Sistema Integrado de Oficina Mecânica  
> **Fase:** Fase 3 Tech Challenge (13SOAT / 15SOAT FIAP)  
> **Componente:** Serviço de Cadastros da arquitetura multi-repositório
> **Stack:** C# .NET 10, ASP.NET Core Minimal APIs, Entity Framework Core, PostgreSQL 16, OpenTelemetry, Docker.
> **Repositório Oficial:** [Grupo78-PosTech-15SOAT/AutoReparos.Cadastros](https://github.com/Grupo78-PosTech-15SOAT/AutoReparos.Cadastros)

---

## 1. Visão Geral

O **AutoReparos.Cadastros** contém as APIs REST, regras de negócio e persistência do backend. A aplicação Angular da oficina foi extraída para o repositório [AutoReparos.Web](https://github.com/Grupo78-PosTech-15SOAT/AutoReparos.Web), com branch de desenvolvimento e pipeline próprios.

| Projeto | Camada / Descrição |
|---|---|
| `AutoReparos.Cadastros.Domain` | Entidades puras, Value Objects, Enums, regras de negócio e interfaces de repositório (DDD) |
| `AutoReparos.Cadastros.Application` | Casos de uso, DTOs, orquestração de serviços de aplicação e validação |
| `AutoReparos.Cadastros.Infrastructure` | `AppDbContext` EF Core, repositórios PostgreSQL, Identity, geração de JWT e integrações |
| `AutoReparos.Cadastros.Api` | Endpoints Minimal APIs, Swagger, injeção de dependência e OpenTelemetry |
| `AutoReparos.Cadastros.Domain.Tests` | 113 testes unitários de regras de domínio (xUnit + FluentAssertions) |
| `AutoReparos.Cadastros.Application.Tests` | 151 testes unitários de casos de uso e métricas (xUnit + NSubstitute + FluentAssertions) |
| `AutoReparos.Cadastros.IntegrationTests` | 78 testes de integração ponta a ponta com **Testcontainers PostgreSQL** |

---

## 2. Arquitetura e Estrutura de Pastas

A aplicação segue rigorosamente os princípios de **Clean Architecture** e **Vertical Slice**:

```
AutoReparos.Cadastros/
├── AutoReparos.Cadastros.Domain/             # Núcleo de regras de negócio sem dependências externas
├── AutoReparos.Cadastros.Application/        # Orquestração de Use Cases e DTOs
├── AutoReparos.Cadastros.Infrastructure/              # Entity Framework Core, PostgreSQL e Identity
├── AutoReparos.Cadastros.Api/                # Minimal APIs, middlewares e Dockerfile
│   └── Dockerfile                  # Multi-stage build .NET 10 SDK & ASP.NET Runtime
├── AutoReparos.Cadastros.Domain.Tests/       # Suíte de testes unitários do Domínio
├── AutoReparos.Cadastros.Application.Tests/  # Suíte de testes unitários da Aplicação
├── AutoReparos.Cadastros.IntegrationTests/   # Testes de integração com Testcontainers
├── docker/                         # Configurações de containers e observabilidade
│   ├── servers.json                # Pré-configuração do pgAdmin 4
│   └── observability/              # OpenTelemetry Collector, Prometheus, Loki e Grafana
├── docker-compose.yml              # Orquestração local (API + DB + Otel)
├── .github/workflows/ci-cd.yml     # Pipeline CI/CD GitHub Actions
└── AutoReparos.Cadastros.slnx            # Solution .NET 10
```

---

## 3. Execução Local com Docker Compose

O repositório disponibiliza API, banco e observabilidade para execução local via Docker Compose. O frontend é executado a partir de `AutoReparos.Web` ou do Compose do repositório pai.

```bash
# Subir PostgreSQL, API, pgAdmin e Stack de Observabilidade
docker-compose up -d --build
```

### Portas e Serviços Disponíveis

| Serviço | Container | URL de Acesso | Credenciais Padrão |
|---|---|---|---|
| **API REST & Swagger** | `autoreparos-app-api` | [http://localhost:8080/swagger](http://localhost:8080/swagger) | — |
| **pgAdmin 4** | `autoreparos-app-pgadmin` | [http://localhost:5050](http://localhost:5050) | `admin@autoreparos.com` / `Admin@123` |
| **Grafana** | `autoreparos-app-grafana` | [http://localhost:3000](http://localhost:3000) | `admin` / `admin` |
| **Jaeger Traces** | `autoreparos-app-jaeger` | [http://localhost:16686](http://localhost:16686) | — |
| **Prometheus** | `autoreparos-app-prometheus` | [http://localhost:9090](http://localhost:9090) | — |
| **PostgreSQL** | `autoreparos-app-postgres` | `localhost:5432` | `admin` / `Admin@123` |

Para encerrar o ambiente:
```bash
docker-compose down
```

---

## 4. Execução Manual para Desenvolvimento

### Backend (.NET 10)
```bash
# Restaurar e compilar a solution
dotnet build AutoReparos.Cadastros.slnx

# Executar a API localmente
dotnet run --project AutoReparos.Cadastros.Api/AutoReparos.Cadastros.Api.csproj
```

Para desenvolver o frontend, use a branch `develop` do repositório [AutoReparos.Web](https://github.com/Grupo78-PosTech-15SOAT/AutoReparos.Web).

---

## 5. Suíte de Testes Automatizados

Execute as suítes do backend pela solution:

```bash
# Executar todos os testes
dotnet test AutoReparos.Cadastros.slnx

# Executar testes unitários de Domínio (113 testes)
dotnet test AutoReparos.Cadastros.Domain.Tests/AutoReparos.Cadastros.Domain.Tests.csproj

# Executar testes unitários de Aplicação (151 testes)
dotnet test AutoReparos.Cadastros.Application.Tests/AutoReparos.Cadastros.Application.Tests.csproj

# Executar testes de integração com Testcontainers (78 testes)
dotnet test AutoReparos.Cadastros.IntegrationTests/AutoReparos.Cadastros.IntegrationTests.csproj
```

---

## 6. Pipeline de CI/CD

O workflow [`.github/workflows/ci-cd.yml`](.github/workflows/ci-cd.yml) compila e testa a solution .NET, confere a cobertura mínima e executa o quality gate do SonarCloud. A compilação e os testes Angular pertencem ao pipeline de `AutoReparos.Web`.

---

## 7. Integração com o Repositório Pai

Este repositório é registrado como **git submodule** no [repositório pai AutoReparos](https://github.com/Grupo78-PosTech-15SOAT/AutoReparos) no caminho `submodules/AutoReparos.Cadastros/`.

### Migrations de produção

O startup da API continua fazendo migrations por padrão em desenvolvimento. Em produção, `Database__SkipStartupMigration=true` impede que cada réplica execute migrations concorrentes. O workflow do repositório pai cria a migration Job com `Database__RunMigrations=true`, aguarda conclusão e só então instala/atualiza a release Helm. A connection string chega pelo secret Kubernetes criado em runtime a partir do Secrets Manager; ela não deve ser gravada em `values.yaml` nem em artifacts.

## 8. CI/CD e configuração do repositório

O pipeline `.github/workflows/ci-cd.yml` roda em pull request para `main` e `develop` e em push nessas branches, com os jobs `build` (compila e confere `dotnet format`), `test` (testes da solution, cobertura mínima de linhas de 80%) e `quality` (SonarCloud).

Para o job `quality` passar, o repositório precisa destes itens em **Settings → Secrets and variables → Actions**:

| Nome | Onde criar | Obrigatório | Valor |
| --- | --- | --- | --- |
| `SONAR_TOKEN` | aba **Secrets** (do repositório ou da organização, com acesso a este repositório) | sim: sem ele o job `quality` falha de propósito | token gerado no SonarCloud em *My Account → Security*, por quem pode analisar os projetos da organização |
| `SONAR_ORGANIZATION` | aba **Variables** | não | padrão `grupo78-postech-15soat` |
| `SONAR_PROJECT_KEY` | aba **Variables** | não | padrão `Grupo78-PosTech-15SOAT_AutoReparos.Cadastros` |

O projeto `Grupo78-PosTech-15SOAT_AutoReparos.Cadastros` precisa existir no SonarCloud (ou o token precisa poder criá-lo). Nunca versione o token. Depois de criar o secret, reexecute o job `quality` da última execução; não é preciso novo commit.

Quando a proteção da `main` for aplicada (AD-026 do repositório pai), os jobs `build`, `test` e `quality` serão os status checks obrigatórios.
