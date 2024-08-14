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

builder.Services.AddCors(options => options.AddPolicy(name: "Origin", policy =>
{
    policy.AllowAnyOrigin().AllowAnyMethod().AllowAnyHeader();
}));

var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    await ServiceExtension.ApplyMigrationsExtension(services);
    await ServiceExtension.SeedDataExtension(services);
}

app.MapDefaultEndpoints();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseCors("Origin");

app.UseAuthentication();

app.UseAuthorization();

app.MapControllers();

app.Run();

app.AddErrorHanldeMiddlewareExtension();