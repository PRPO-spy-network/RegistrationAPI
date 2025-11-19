using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using RegistrationAPI.Classes;
using Registrations.Models;

var builder = WebApplication.CreateBuilder(args);
string connectionString = builder.Configuration["TIMESCALE_CONN_STRING"] ?? throw new InvalidDataException("TIMESCALE_CONN_STRING ne obstaja");
builder.Services.AddDbContextFactory<PostgresContext>(options => options.UseNpgsql(connectionString));


builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddHealthChecks()
	.AddNpgSql(connectionString, name: "timescale");

builder.Services.AddHttpClient();

builder.Services.AddSingleton<DiscordService>();

var app = builder.Build();

app.MapHealthChecks("/health");

var logger = app.Logger;
if (app.Environment.IsDevelopment())
{
	logger.LogInformation("Running in dev mode");
}


using (var scope = app.Services.CreateScope())
{
	var dbContextFactory = scope.ServiceProvider.GetRequiredService<IDbContextFactory<PostgresContext>>();
	using var dbContext = dbContextFactory.CreateDbContext();
	try
	{
		if (await dbContext.Database.CanConnectAsync())
		{
			logger.LogInformation("Connected to timescale.");
		}
		else
		{
			logger.LogWarning("Can't connect to timescale. ");
		}
	}
	catch (Exception ex)
	{
		logger.LogError($"Error connecting to timescale: {ex.Message}");
	}
}

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}
else
{
	app.UseHttpsRedirection();
}

//app.UseAuthorization();

app.MapControllers();

app.Run();
