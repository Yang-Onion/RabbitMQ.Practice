using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using RabbitMQ.Client;

namespace RabbitMQ.Producer
{
    public class Producer
    {
        static void Main(string[] args)
        {
            var factory = new ConnectionFactory()
            {
                HostName="localhost"
             };
            using (var connection = factory.CreateConnection())
            {
                using (var channel= connection.CreateModel())
                {
                    channel.QueueDeclare(queue: "hello",
                                         durable: false, 
                                         exclusive: false, 
                                         autoDelete: false,
                                         arguments: null);
                    string message = args.Length > 0 ? string.Join(" ", args) : "Hello World !";
                    var body = Encoding.UTF8.GetBytes(message);
                    channel.BasicPublish(exchange:"",routingKey:"hello",basicProperties:null,body:body);
                    Console.WriteLine(" Producer Sent {0}", message);
                }
            }
            Console.WriteLine(" Press [enter] to exit.");
            Console.ReadLine();
        }
    }
}
