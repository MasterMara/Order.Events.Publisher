﻿using Book.Events.Common;
using Book.Events.Publisher.Logging;
using Book.Events.Publisher.Services;
using Microsoft.Extensions.Hosting;
using Book.Events.V1.Book;
using Book.Events.V1.Book.Props;

namespace Book.Events.Publisher;

public class Worker : BackgroundService
{
    private readonly IConsoleLogger _consoleLogger;
    private readonly IMessageBrokerService _messageBrokerService;
    private readonly Action<object> _action = Action;


    public Worker(IConsoleLogger consoleLogger, IMessageBrokerService messageBrokerService)
    {
        _consoleLogger = consoleLogger;
        _messageBrokerService = messageBrokerService;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        for (var i = 0; i < 1; i++)
            _action(new Context(_consoleLogger, _messageBrokerService, stoppingToken));

        await Task.CompletedTask;
    }


    private static async void Action(object c)
    {
        if (!(c is IContext context)) return;

        var i = 0;


        try
        {
            var bookCreated = new Created
            {
                Id = "1",
                BookNumber = "123456",
                BookName = "Outliers",
                Writer = new Writer
                {
                    Id = "10",
                    Name = "Mustafa KARACABEY"
                },
                TotalAmount = new Money
                {
                    CurrencyCode = 949,
                    Value = 100
                }
            };

            await context.MessageBrokerService.Publish(bookCreated, Guid.NewGuid());
            context.Logger.LogInformation($"Event Published, OrderNumber:{bookCreated.BookNumber} ");
        }
        catch (Exception e)
        {
            //context.Logger.LogError(e.Message, e);
        }
    }
}