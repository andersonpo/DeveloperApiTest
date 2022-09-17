using DeveloperApiTest.Infraestrutura;
using DeveloperApiTest.Infraestrutura.Logs;
using Serilog;

var builder = WebApplication.CreateBuilder(args);

#region Log
builder.Logging.ClearProviders();
Log.Logger = LogExtensao.CriarLogger(builder.Configuration);
builder.Logging.AddSerilog(Log.Logger);
builder.Services.AdicionarSerilog();
#endregion

builder.Services.AdicionarDependencias(builder.Configuration);

var app = builder.Build();
app.UsarDependencias()
    .Run();
