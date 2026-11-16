using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AutoReparos.Cadastros.Infrastructure.Data.Migrations
{
    /// <inheritdoc />
    public partial class v10_notificacoes_enviadas : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "NotificacoesEnviadas",
                columns: table => new
                {
                    EventId = table.Column<Guid>(type: "uuid", nullable: false),
                    Tipo = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    ClienteId = table.Column<Guid>(type: "uuid", nullable: false),
                    EnviadoEm = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_NotificacoesEnviadas", x => x.EventId);
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "NotificacoesEnviadas");
        }
    }
}
