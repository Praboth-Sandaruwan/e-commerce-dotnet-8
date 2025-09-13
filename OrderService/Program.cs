using OrderService;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddOrderServices(
    builder.Configuration,
    builder.Environment
    );

var app = builder.Build();


app.UseHttpsRedirection();

app.MapGet("/health", () =>
{
    return Results.Ok("Healthy");
})
.WithName("GetHealthCheck")
.WithTags("Test");

app.UseOrderServices();

app.Run();

