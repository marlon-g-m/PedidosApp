using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Pedidos.Api.Data;
using Pedidos.Api.Models;
using Pedidos.Api.Services;

namespace Pedidos.Api.Controllers;

[ApiController]
[Route("api/[controller]")]   // => /api/pedidos
public class PedidosController(AppDbContext db, IColaPedidos cola, ILogger<PedidosController> logger) : ControllerBase
{
    [HttpGet]                                                   // 200
    public async Task<ActionResult<IEnumerable<PedidoDto>>> GetAll(CancellationToken ct)
    {
        var pedidos = await db.Pedidos
            .AsNoTracking()                      // solo lectura: sin change tracking
            .OrderByDescending(p => p.CreadoEn)
            .Select(p => new PedidoDto(p.Id, p.Cliente, p.Total, p.Estado, p.CreadoEn))
            .ToListAsync(ct);                    // aquí recién se ejecuta el SQL
        return Ok(pedidos);
    }

    [HttpGet("{id:int}")]                                       // 200 o 404
    public async Task<ActionResult<PedidoDto>> GetById(int id, CancellationToken ct)
    {
        var p = await db.Pedidos.FindAsync([id], ct);
        if (p is null) return NotFound();
        return new PedidoDto(p.Id, p.Cliente, p.Total, p.Estado, p.CreadoEn);
    }

    [HttpPost]                                                  // 201 o 400
    public async Task<ActionResult<PedidoDto>> Create(CrearPedidoDto dto, CancellationToken ct)
    {
        if (string.IsNullOrWhiteSpace(dto.Cliente) || dto.Total <= 0)
            return BadRequest("Cliente es obligatorio y Total debe ser mayor a 0.");

        var pedido = new Pedido { Cliente = dto.Cliente, Total = dto.Total };
        db.Pedidos.Add(pedido);
        await db.SaveChangesAsync(ct);                          // INSERT

        var result = new PedidoDto(pedido.Id, pedido.Cliente, pedido.Total, pedido.Estado, pedido.CreadoEn);
        await cola.PublicarPedidoCreadoAsync(result, ct);       // evento a la cola
        logger.LogInformation("Pedido {Id} creado y enviado a la cola", pedido.Id);

        return CreatedAtAction(nameof(GetById), new { id = pedido.Id }, result);
    }

    [HttpDelete("{id:int}")]                                    // 204 o 404
    public async Task<IActionResult> Delete(int id, CancellationToken ct)
    {
        var p = await db.Pedidos.FindAsync([id], ct);
        if (p is null) return NotFound();
        db.Pedidos.Remove(p);
        await db.SaveChangesAsync(ct);
        return NoContent();
    }
}