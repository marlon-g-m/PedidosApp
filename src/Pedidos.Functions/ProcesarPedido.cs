using System.Text.Json;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;

namespace Pedidos.Functions;

public record PedidoCreado(int Id, string Cliente, decimal Total, string Estado, DateTime CreadoEn);

public class ProcesarPedido(ILogger<ProcesarPedido> logger)
{
    [Function(nameof(ProcesarPedido))]
    public async Task Run(
        [QueueTrigger("pedidos-creados", Connection = "AzureWebJobsStorage")] string mensaje)
    {
        var pedido = JsonSerializer.Deserialize<PedidoCreado>(mensaje,
            new JsonSerializerOptions { PropertyNameCaseInsensitive = true })!;

        logger.LogInformation("Procesando pedido {Id} de {Cliente} por ${Total}",
            pedido.Id, pedido.Cliente, pedido.Total);

        await Task.Delay(500);

        logger.LogInformation("Pedido {Id} procesado OK", pedido.Id);
    }
}