using AgentServer.Services.Memories;
using AgentServer.Services.Observations;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();

builder.Services.AddSingleton<IObservationService, InMemoryObservationService>();
builder.Services.AddSingleton<IMemoryStreamService, InMemoryMemoryStreamService>();

var app = builder.Build();

app.MapControllers();

app.Run();