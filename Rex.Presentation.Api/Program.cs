using Microsoft.OpenApi;
using Rex.Application;
using Rex.Infrastructure.Persistence;
using Rex.Infrastructure.Shared;
using Rex.Presentation.Api.ServicesExtension;
using Serilog;

var builder = WebApplication.CreateBuilder(args);

builder.Host.UseSerilog((context, LoggerConfiguration) =>
{
    LoggerConfiguration.ReadFrom.Configuration(context.Configuration);
});

// Add services to the container.

builder.Services.AddControllers().AddFilters();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddPersistenceLayer(builder.Configuration);
builder.Services.AddApplicationLayer(builder.Configuration);
builder.Services.AddSharedLayer(builder.Configuration);
builder.Services.AddSwaggerExtension();
builder.Services.AddVersioning();

var app = builder.Build();

app.UseSerilogRequestLogging();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger(options => options.OpenApiVersion =
        OpenApiSpecVersion.OpenApi2_0);
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("./v1/swagger.json", "Rex v1");
        c.RoutePrefix = "swagger";
    });
}

app.UseHttpsRedirection();
app.UseExceptionHandling();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();