
using APITask.Data;
using APITask.Services;
using APITask.Services.Implementations;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlServer(
        builder.Configuration.GetConnectionString("DefaultConnection"),
        sqlOptions => sqlOptions.EnableRetryOnFailure()));

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddControllers();

builder.Services.AddScoped<IAssignmentService, AssignmentService>();

builder.Services.AddOpenApi();
builder.Services.AddProblemDetails();

var app = builder.Build();

if (app.Configuration.GetValue<bool>("Database:ApplyMigrations"))
{
    await ApplyDatabaseMigrations(app.Services, app.Logger);
}

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseExceptionHandler();

app.UseAuthorization();

app.MapControllers();

app.Run();

static async Task ApplyDatabaseMigrations(IServiceProvider services, ILogger logger)
{
    const int maximumAttempts = 12;
    var delay = TimeSpan.FromSeconds(5);

    for (var attempt = 1; attempt <= maximumAttempts; attempt++)
    {
        try
        {
            await using var scope = services.CreateAsyncScope();
            var dbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();
            await dbContext.Database.MigrateAsync();
            logger.LogInformation("Database migrations applied successfully");
            return;
        }
        catch (Exception exception) when (attempt < maximumAttempts)
        {
            logger.LogWarning(
                exception,
                "Database is not ready. Migration attempt {Attempt}/{MaximumAttempts} failed; retrying in {DelaySeconds} seconds",
                attempt,
                maximumAttempts,
                delay.TotalSeconds);

            await Task.Delay(delay);
        }
    }

    throw new InvalidOperationException(
        $"Database migrations could not be applied after {maximumAttempts} attempts.");
}
