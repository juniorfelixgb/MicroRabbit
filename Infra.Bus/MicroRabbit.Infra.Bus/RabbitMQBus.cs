using System.Text;
using System.Text.Json;
using MediatR;
using MicroRabbit.Domain.Core.Bus;
using MicroRabbit.Domain.Core.Commands;
using MicroRabbit.Domain.Core.Events;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;

namespace MicroRabbit.Infra.Bus;

public sealed class RabbitMQBus : IEventBus
{
    private readonly ISender _sender;
    private readonly Dictionary<string, List<Type>> _handlers;
    private readonly List<Type> _eventTypes;
    public RabbitMQBus(ISender sender)
    {
        _sender = sender;
        _handlers = new Dictionary<string, List<Type>>();
        _eventTypes = new List<Type>();
    }

    public async Task Publish<T>(T @event) where T : Event
    {
        var factory = new ConnectionFactory
        {
            HostName = "localhost",
            Port = 15672,
            UserName = "guest",
            Password = "guest"
        };

        using var connection = await factory.CreateConnectionAsync();
        using var channel = await connection.CreateChannelAsync();

        try
        {

            var eventName = @event.GetType().Name;
            await channel.QueueDeclareAsync(eventName, false, false, false, null);

            var message = JsonSerializer.Serialize(@event);
            await channel.BasicPublishAsync(
                "",
                eventName,
                false,
                Encoding.UTF8.GetBytes(message)
            );
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error publishing event: {ex.Message}");
        }
        finally
        {
            await connection.CloseAsync();
            await channel.CloseAsync();
        }
    }

    public Task SendCommand<T>(T command) where T : Command
    {
        return _sender.Send(command);
    }

    public async Task Susbcribe<T, TH>()
        where T : Event
        where TH : IEventHandler<T>
    {
        var eventName = typeof(T).Name;
        var handlerType = typeof(TH);

        if (!_eventTypes.Contains(typeof(T)))
        {
            _eventTypes.Add(typeof(T));
        }

        if (!_handlers.ContainsKey(eventName))
        {
            _handlers.Add(eventName, new List<Type>());
        }

        if (_handlers[eventName].Any(h => h == handlerType))
        {
            throw new ArgumentException($"Handler type {handlerType.Name} is already registered for event {eventName}");
        }

        _handlers[eventName].Add(handlerType);

        await StartBasicConsume<T>();
    }

    private async Task StartBasicConsume<T>() where T : Event
    {
        var factory = new ConnectionFactory
        {
            HostName = "localhost",
            Port = 15672,
            UserName = "guest",
            Password = "guest",
        };

        var connection = await factory.CreateConnectionAsync();
        var channel = await connection.CreateChannelAsync();

        var eventName = typeof(T).Name;
        await channel.QueueDeclareAsync(eventName, false, false, false, null);
        var consumer = new AsyncEventingBasicConsumer(channel);

        consumer.ReceivedAsync += Consumer_ReceivedAsync;

        await channel.BasicConsumeAsync(eventName, true, consumer);
    }

    private async Task Consumer_ReceivedAsync(object sender, BasicDeliverEventArgs @event)
    {
        var eventName = @event.RoutingKey;
        var message = Encoding.UTF8.GetString(@event.Body.ToArray());

        try
        {
            await ProcessEvent(eventName, message).ConfigureAwait(false);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error processing event {eventName}: {ex.Message}");
            return;
        }
    }

    private async Task ProcessEvent(string eventName, string message)
    {
        if (_handlers.ContainsKey(eventName))
        {
            var susbcriptions = _handlers[eventName];
            foreach (var suscription in susbcriptions)
            {
                var handler = Activator.CreateInstance(suscription);
                if (handler == null)
                {
                    continue;
                }

                var eventType = _eventTypes.SingleOrDefault(t => t.Name == eventName);
                if (eventType == null)
                {
                    Console.WriteLine($"Event type {eventName} not found.");
                    continue;
                }

                var @event = JsonSerializer.Deserialize(message, eventType);
                if (@event == null)
                {
                    Console.WriteLine($"Deserialization of event {eventName} returned null.");
                    continue;
                }
                
                var concreteType = typeof(IEventHandler<>).MakeGenericType(eventType);
                if (concreteType == null)
                {
                    Console.WriteLine($"Concrete type for event handler not found for {eventName}.");
                    continue;
                }

                var methodInfo = concreteType.GetMethod("Handle");
                if (methodInfo == null)
                {
                    throw new InvalidOperationException($"Handle method not found in handler for event {eventName}.");
                }

                var taskObj = methodInfo.Invoke(handler, new object[] { @event });
                if (taskObj is Task task)
                {
                    await task;
                }
                else
                {
                    throw new InvalidOperationException($"Handler for event {eventName} did not return a Task.");
                }
            }
        }
    }
}
