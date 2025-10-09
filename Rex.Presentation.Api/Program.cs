using Microsoft.OpenApi;
using Rex.Application;
using Rex.Infrastructure.Persistence;
using Rex.Infrastructure.Shared;
using Rex.Infrastructure.Shared.Services.SignalR.Hubs;
using Rex.Presentation.Api.ServicesExtension;
using Serilog;

var builder = WebApplication.CreateBuilder(args);

builder.Host.UseSerilog((context, LoggerConfiguration) =>
{
    LoggerConfiguration.ReadFrom.Configuration(context.Configuration);
});

builder.Services.AddControllers().AddFilters();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddPersistenceLayer(builder.Configuration);
builder.Services.AddApplicationLayer(builder.Configuration);
builder.Services.AddSharedLayer(builder.Configuration);
builder.Services.AddSwaggerExtension();
builder.Services.AddVersioning();

builder.Services.AddCors(options =>
{
    options.AddPolicy("CorsPolicy", policy =>
    {
        policy.WithOrigins("http://localhost:5500","http://localhost:5501", "http://127.0.0.1:5501", "http://127.0.0.1:5500", "https://localhost:5500")
            .AllowAnyMethod()
            .AllowAnyHeader()
            .AllowCredentials();
    });
});

var app = builder.Build();

app.UseExceptionHandling();

app.UseSerilogRequestLogging();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger(options => options.OpenApiVersion = OpenApiSpecVersion.OpenApi2_0);
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("./v1/swagger.json", "Rex v1");
        c.RoutePrefix = "swagger";
    });
}


app.UseWebSockets();

app.UseRouting(); 

app.UseCors("CorsPolicy");

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();
app.MapHub<AppHub>("/hubs/chat");

app.Run();