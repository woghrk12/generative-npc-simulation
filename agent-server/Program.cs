using AgentServer.Services.Observations;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();

builder.Services.AddSingleton<IObservationService, InMemoryObservationService>();

var app = builder.Build();

app.MapControllers();

app.Run();