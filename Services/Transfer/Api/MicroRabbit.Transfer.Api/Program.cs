using MicroRabbit.Infra.IoC;
using MicroRabbit.Transfer.Infra.Contexts;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
var connection = builder.Configuration.GetConnectionString("BankingConnection");
builder.Services.AddDbContext<TransferDbContext>(options =>
    options.UseSqlServer(connection,
        b => b.MigrationsAssembly("MicroRabbit.Transfer.Api")));
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

