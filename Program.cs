using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using RoyaAi.Layers.Infrastructure.AgentProtocol.Interfaces;
using RoyaAi.Layers.Infrastructure.AgentProtocol.Services;
using RoyaAi.Layers.Kernel;
using RoyaAi.Layers.Kernel.Interfaces;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Register RoyaAI services
builder.Services.AddSingleton<IKernelService, KernelService>();
builder.Services.AddS 