using AgentServer.Services.Decisions;
using AgentServer.Services.Memories;
using AgentServer.Services.Observations;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();

builder.Services.AddSingleton<IObservationService, InMemoryObservationService>();
builder.Services.AddSingleton<IImportanceScorer, RuleBasedImportanceScorer>();
builder.Services.AddSingleton<IMemoryStreamService, InMemoryMemoryStreamService>();
builder.Services.AddSingleton<IMemoryRetrievalService, MemoryRetrievalService>();
builder.Services.AddSingleton<IDecisionService, RuleBasedDecisionService>();

var app = builder.Build();

app.MapControllers();

app.Run();