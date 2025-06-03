using System;
using MediatR;
using MicroRabbit.Banking.Domain.Events;
using MicroRabbit.Domain.Core.Bus;

namespace MicroRabbit.Banking.Domain.Commands;

public class CreateTransferCommand : TransferCommand
{
    public CreateTransferCommand(int from, int to, decimal amount)
    {
        From = from;
        To = to;
        Amount = amount;
    }
}

public class TransferCommandHandler : IRequestHandler<CreateTransferCommand, bool>
{
    private readonly IEventBus _eventBus;
    public TransferCommandHandler(IEventBus eventBus)
    {
        _eventBus = eventBus ?? throw new ArgumentNullException(nameof(eventBus));
    }
    public async Task<bool> Handle(CreateTransferCommand request, CancellationToken cancellationToken)
    {
        await _eventBus.Publish(new TransferCreatedEvent(request.From, request.To, request.Amount));
        return true;
    }
}
