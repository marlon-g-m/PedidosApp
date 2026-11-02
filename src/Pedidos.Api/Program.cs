using Microsoft.EntityFrameworkCore;
using Pedidos.Api.Data;
using Pedidos.Api.Services;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddOpenApi();
builder.Services.AddHealthChecks();

builder.Services.AddDbContext<AppDbContext>(opt =>
    opt.UseSqlite(builder.Configuration.GetConnectionString("Default")));

builder.Services.AddSingleton<IColaPedidos, ColaPedidosAzure>();

// ---- CORS: qué sitios web pueden llamar a esta API desde el navegador ----
// Cors:Origins = lista separada por comas. Si está vacía, se permite cualquier origen (solo para pruebas).
var origenes = (builder.Configuration["Cors:Origins"] ?? "")
    .Split(',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);

builder.Services.AddCors(opt => opt.AddDefaultPolicy(policy =>
{
    if (origenes.Length > 0) policy.WithOrigins(origenes);
    else policy.AllowAnyOrigin();
    policy.AllowAnyHeader().AllowAnyMethod();
}));

var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    scope.ServiceProvider.GetRequiredService<AppDbContext>().Database.Migrate();
}

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseCors();
app.MapHealthChecks("/health");
app.MapControllers();

app.Run();