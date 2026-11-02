namespace Pedidos.Api.Models;

public class Pedido
{
    public int Id { get; set; }
    public string Cliente { get; set; } = string.Empty;
    public decimal Total { get; set; }
    public string Estado { get; set; } = "Pendiente";
    public DateTime CreadoEn { get; set; } = DateTime.UtcNow;
}

public record CrearPedidoDto(string Cliente, decimal Total);
public record PedidoDto(int Id, string Cliente, decimal Total, string Estado, DateTime CreadoEn);