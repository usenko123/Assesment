using Assessment.Api.Extensions;
using Assessment.Application;
using Assessment.Infrastructure;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddApiLayer();
builder.Services.AddApplication();
builder.Services.AddInfrastructure(builder.Configuration);

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseExceptionHandler();
app.UseStatusCodePages();

await app.UseDevelopmentSeedingAsync();

app.UseCors(CorsExtensions.FrontendPolicy);
app.MapControllers();

app.Run();

public partial class Program;
