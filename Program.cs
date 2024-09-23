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

app.MapGet("api/v1/devices/{id:long}/telemetry/latest", (long id) => new TelemetryResponse
    {
        DeviceId = id,
        Temperature = 22,
        Timestamp = DateTime.Now
    })
    .WithOpenApi();

app.MapGet("api/v1/devices/{id:long}/telemetry/", (long id, DateTime start, DateTime end) => new List<TelemetryResponse>
    {
        new()
        {
            DeviceId = id,
            Temperature = 22,
            Timestamp = DateTime.Now
        },
        new()
        {
            DeviceId = id,
            Temperature = 21,
            Timestamp = DateTime.Now
        }
    })
    .WithOpenApi();

app.Run();