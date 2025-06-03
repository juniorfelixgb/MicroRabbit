using MicroRabbit.Banking.Application.Interfaces;
using MicroRabbit.Banking.Application.Services;
using MicroRabbit.Banking.Domain.Interfaces;
using MicroRabbit.Banking.Infras.Contexts;
using MicroRabbit.Banking.Infras.Repositories;
using MicroRabbit.Domain.Core.Bus;
using MicroRabbit.Infra.Bus;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace MicroRabbit.Infra.IoC;

public static class DependencyContainer
{
    public static void RegisterServices(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddTransient<IEventBus, RabbitMQBus>();

        services.AddTransient<IAccountService, AccountService>();
        services.AddTransient<IAccountRepository, AccountRepository>();

        services.AddMediatR(cfg =>
            cfg.RegisterServicesFromAssemblyContaining<BankingDbContext>());
    }
}
