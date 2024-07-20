using Papyrus.Docs.AuthApi.Extensions;

var builder = WebApplication.CreateBuilder(args);

builder.AddServiceDefaults();

// environment specific configuration
builder.Configuration
    .SetBasePath(builder.Environment.ContentRootPath)
    .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
    .AddJsonFile($"appsettings.{builder.Environment.EnvironmentName}.json", optional: true)
    .AddJsonFile("appsettings.Development.override.json", optional: true)
    .AddEnvironmentVariables();

// Add services to the container.
builder.Services.AddAuthenticationExtension(builder.Configuration);

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Extension methods for adding services to the container
builder.Services.AddDbContextExtension(builder.Configuration);
builder.Services.AddRepositoriesExtension();

var app = builder.Build();

app.MapDefaultEndpoints();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();

app.ApplyMigrationsExtension();

app.SeedDataExtension();