using MicroRabbit.Banking.Application.Services;
using MicroRabbit.Banking.Infras.Contexts;
using MicroRabbit.Infra.IoC;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddMediatR(cfg => cfg.RegisterServicesFromAssemblyContaining<AccountService>());

var connection = builder.Configuration.GetConnectionString("BankingConnection");

builder.Services.AddDbContext<BankingDbContext>(options =>
    options.UseSqlServer(connection,
        b => b.MigrationsAssembly("MicroRabbit.Banking.Api")));

// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddControllers();
builder.Services
    .AddOpenApi()
    .RegisterServices(builder.Configuration);

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseHttpsRedirection();
app.MapControllers();

app.Run();
