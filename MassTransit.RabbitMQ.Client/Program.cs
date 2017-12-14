using MassTransit.RabbitMQ.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MassTransit.RabbitMQ.Client
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Press Enter to  send a message.To exit ,Ctrl+C");
            var bus = BusCreator.CreateBus();
            var sendToUri = new Uri($"{RabbitMqConstants.RabbitMqUri}{RabbitMqConstants.GreetingQueue}");
            while (Console.ReadLine() != null)
            {
                var content = Console.ReadLine();
                Task.Run(() => SendCommand(bus, sendToUri, content)).Wait();
            }
            Console.ReadLine();
        }

        public static async void SendCommand(IBusControl bus,Uri sendToUri,string content)
        {
            var endPoint = await bus.GetSendEndpoint(sendToUri);
            var command = new GreetingCommand()
            {
                Id = Guid.NewGuid().ToString(),
                Content=content,
                DateTime = DateTime.Now
            };

            await endPoint.Send(command);

            Console.WriteLine($"send command :id{command.Id},{command.DateTime}");
        }
    }

   
}
