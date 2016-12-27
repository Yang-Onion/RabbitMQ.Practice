using System;
using System.Text;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;

namespace RPC.Client
{
    public class Client
    {
        private readonly IModel _channel;
        private readonly IConnection _connection;
        private readonly QueueingBasicConsumer _consumer;
        private readonly string _replyQueueName;

        public Client()
        {
            var factory = new ConnectionFactory
            {
                HostName = "192.168.2.122",
                UserName = "Yang",
                Password = "cms2016...",
                Port = AmqpTcpEndpoint.UseDefaultPort
            };
            _connection = factory.CreateConnection();
            _channel = _connection.CreateModel();
            _consumer = new QueueingBasicConsumer(_channel);
            _replyQueueName = _channel.QueueDeclare().QueueName;
            _channel.BasicConsume(queue:_replyQueueName,
                                  noAck:true,
                                  consumer: _consumer);
        }

        private string Call(string message)
        {
            var correlationId = Guid.NewGuid().ToString();
            var props = _channel.CreateBasicProperties();
            props.ReplyTo = _replyQueueName;
            props.CorrelationId = correlationId;

            var requestBody = Encoding.UTF8.GetBytes(message);
            _channel.BasicPublish(exchange:"",
                                  routingKey:"rpc_queue",
                                  basicProperties: props,
                                  body: requestBody);

            while (true)
            {
                var ea = (BasicDeliverEventArgs)_consumer.Queue.Dequeue();
                if (ea.BasicProperties.CorrelationId.Equals(correlationId))
                {
                    return Encoding.UTF8.GetString(ea.Body);
                }
            }
        }

        public void Close()
        {
            _connection.Close();
        }

        private static void Main(string[] args)
        {
            var client = new Client();
            var num = "10";
            Console.WriteLine($" client Requesting fib({num})");
            var rpcResult = client.Call(num);
            Console.WriteLine($"rpc_server_Fib{num},got {rpcResult}");
            client.Close();
        }
    }
}