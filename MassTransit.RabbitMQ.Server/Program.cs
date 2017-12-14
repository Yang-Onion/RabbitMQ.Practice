using MassTransit.RabbitMQ.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MassTransit.RabbitMQ.Server
{
    class Program
    {
        static void Main(string[] args)
        {
            var bus = BusCreator.CreateBus((cfg,host)=>
            {
                cfg.ReceiveEndpoint(host, RabbitMqConstants.GreetingQueue, e =>
                {
                    e.Consumer<GreetingConsumer>();
                });
            });

            bus.Start();
        }

    }

    public class GreetingConsumer : IConsumer<GreetingCommand>
    {
        public async Task Consume(ConsumeContext<GreetingCommand> context)
        {
            await Console.Out.WriteLineAsync($"receive greeting command:{context.Message.Content},{context.Message.Id},{context.Message.DateTime}");
        }
    }
}
