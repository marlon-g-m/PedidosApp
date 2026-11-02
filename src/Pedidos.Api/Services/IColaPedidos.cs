using System.Text.Json;
using Azure.Storage.Queues;
using Pedidos.Api.Models;

namespace Pedidos.Api.Services;

public interface IColaPedidos
{
    Task PublicarPedidoCreadoAsync(PedidoDto pedido, CancellationToken ct = default);
}

public class ColaPedidosAzure : IColaPedidos
{
    private readonly QueueClient _queue;

    public ColaPedidosAzure(IConfiguration config)
    {
        var conn = config.GetConnectionString("Storage")!;
        // Base64: es el formato que espera el QueueTrigger de Functions por defecto
        _queue = new QueueClient(conn, "pedidos-creados",
            new QueueClientOptions { MessageEncoding = QueueMessageEncoding.Base64 });
    }

    public async Task PublicarPedidoCreadoAsync(PedidoDto pedido, CancellationToken ct = default)
    {
        await _queue.CreateIfNotExistsAsync(cancellationToken: ct);
        await _queue.SendMessageAsync(JsonSerializer.Serialize(pedido), ct);
    }
}