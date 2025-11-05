using Registrations.Models;
using Microsoft.EntityFrameworkCore;

var config = new ConfigurationBuilder()
			.AddEnvironmentVariables()
			.Build();

var builder = WebApplication.CreateBuilder(args);
string connectionString = config["TIMESCALE_CONN_STRING"] ?? throw new InvalidDataException("TIMESCALE_CONN_STRING ne obstaja");
builder.Services.AddDbContextFactory<PostgresContext>(options => options.UseNpgsql(connectionString));



builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

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
