using TaskManagement.AppServices.Extensions;
using TaskManagement.Infrastructure.Extensions;
using TaskManagement.WebAPI.Extensions;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddWebApiServices();
builder.Services.AddAppServices();
builder.Services.AddInfrastructure(builder.Configuration);

var app = builder.Build();

app.UseAuthorization();

app.MapControllers();

app.Run();

public partial class Program;
