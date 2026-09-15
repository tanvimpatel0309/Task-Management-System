using TaskManagement.AppServices.Extensions;
using TaskManagement.Command.Extensions;
using TaskManagement.Infrastructure.Extensions;
using TaskManagement.Queries.Extensions;
using TaskManagement.WebAPI.Extensions;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddWebApiServices(builder.Configuration);
builder.Services.AddAppServices(builder.Configuration);
builder.Services.AddCommandServices();
builder.Services.AddQueryServices();
builder.Services.AddInfrastructure(builder.Configuration);

var app = builder.Build();

app.UseWebApiPipeline();

await app.SeedDevelopmentDataAsync();

app.MapControllers();

app.Run();

public partial class Program;
