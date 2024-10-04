using System.Data;
using Npgsql;
using TelemetryService;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
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

app.MapGet("api/v1/devices/{id:long}/telemetry/latest", async (long id) =>
    {
        var connectionString = "Host=postgresql;Port=5432;Database=smart_home;Username=your_username;Password=your_password;";
        var sql = $@"SELECT id,
              current_temperature,
              last_updated         
            FROM
              temperature_sensors
            WHERE id = {id}
            order by last_updated desc
            Limit 5";
            // Open a connection
            await using var dataSource = NpgsqlDataSource.Create(connectionString);

            // Create a Command
            await using var cmd = dataSource.CreateCommand(sql);

            // Create a new data reader
            using var reader = await cmd.ExecuteReaderAsync();

            List<TelemetryResponse> results = new();
            
            // Read data from the table
            while (await reader.ReadAsync())
            {
                var id_1 = reader.GetInt32(0);
                var current_temperature = reader.GetDouble(1);
                var last_updated = reader.GetDateTime(2);
                
                results.Add(new TelemetryResponse
                {
                    DeviceId = id_1,
                    Temperature = current_temperature,
                    Timestamp = last_updated
                });
            }
            
            return results;
    })
    .WithOpenApi();

app.MapGet("api/v1/devices/{id:long}/telemetry/", async (long id, DateTime start, DateTime end) => 
    {
        var connectionString = "Host=postgresql;Port=5432;Database=smart_home;Username=your_username;Password=your_password;";
        var sql = $@"SELECT id,
              current_temperature,
              last_updated         
            FROM
              temperature_sensors
            WHERE id = {id} and last_updated >= '{start}' and last_updated <= '{end}'";
        
        await using var dataSource = NpgsqlDataSource.Create(connectionString);
        
        await using var cmd = dataSource.CreateCommand(sql);
        
        await using var reader = await cmd.ExecuteReaderAsync();

        List<TelemetryResponse> results = [];
            
        // Read data from the table
        while (await reader.ReadAsync())
        {
            var id_1 = reader.GetInt32(0);
            var current_temperature = reader.GetDouble(1);
            var last_updated = reader.GetDateTime(2);
                
            results.Add(new TelemetryResponse
            {
                DeviceId = id_1,
                Temperature = current_temperature,
                Timestamp = last_updated
            });
        }
            
        return results;
    })
    .WithOpenApi();

app.Run();