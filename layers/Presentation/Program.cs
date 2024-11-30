var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("v1", new Microsoft.OpenApi.Models.OpenApiInfo
    {
        Title = "RoyaAI Agent Protocol API",
        Version = "v1",
        Description = "API endpoints implementing the Agent Protocol specification"
    });
    
    var xmlFile = $"{System.Reflection.Assembly.GetExecutingAssembly().GetName().Name}.xml";
    var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
    if (File.Exists(xmlPath))
    {
        options.IncludeXmlComments(xmlPath);
    }
});

// Add Controllers support
builder.Services.AddControllers();

// Add CORS
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll", 
        builder => builder
            .AllowAnyOrigin()
            .AllowAnyMethod()
            .AllowAnyHeader());
});

// Register services from Kernel layer
builder.Services.AddKernelServices();

// Register repositories and other infrastructure services
builder.Services.AddInfrastructureServices();

// Register Agent Protocol services
builder.Services.AddSingleton<IAgentService, AgentService>();
builder.Services.AddSingleton<ITaskService, TaskService>();
builder.Services.AddSingleton<IStepService, StepService>();
builder.Services.AddSingleton<IArtifactService, ArtifactService>();
var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(c => 
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "RoyaAI Agent Protocol API v1");
        c.RoutePrefix = string.Empty; // Set Swagger UI at app root
    });
}

app.UseHttpsRedirection();

// Enable CORS
app.UseCors("AllowAll");

// Add authentication middleware if needed
// app.UseAuthentication();

app.UseAuthorization();
// Map Controllers
app.MapControllers();

// Configure API versioning and routing
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

// Add basic health check endpoint
app.MapGet("/health", () => "Healthy")
    .WithName("GetHealth")
    .WithOpenApi();

app.Run();

// Add namespace imports at the top
// These are placed at the bottom but would be at the top of the actual file
// using Microsoft.AspNetCore.Builder;
// using Microsoft.Extensions.DependencyInjection;
// using Microsoft.Extensions.Hosting;
// using RoyaAi.Layers.Kernel;
// using RoyaAi.Layers.Infrastructure;
// using RoyaAi.Layers.Presentation.Services;
// using RoyaAi.Layers.Presentation.Services.Interfaces;
