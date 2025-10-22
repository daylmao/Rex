using Microsoft.AspNetCore.Authentication.JwtBearer;
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
builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
});

builder.Services.AddAuthorization();

builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAllOrigins", policy =>
    {
        policy
            .AllowAnyHeader()
            .AllowAnyMethod()
            .AllowAnyOrigin(); 
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

app.UseCors("AllowAllOrigins");

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();
app.MapHub<AppHub>("/hubs/app");

app.Run();