using System.ComponentModel.DataAnnotations;

namespace AutoReparos.Cadastros.Application.Insumos.DTOs.Request
{
    public record AtualizarEstoqueDto(
        [Range(1, int.MaxValue, ErrorMessage = "Quantidade deve ser maior que zero.")]
        int Quantidade
    );
}
