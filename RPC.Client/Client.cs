using System;
using System.Text;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System.Collections.Concurrent;

namespace RPC.Client
{
    public class Client
    {
        private readonly IModel _channel;
        private readonly IConnection _connection;
        private readonly EventingBasicConsumer  _consumer;
        private readonly string _replyQueueName;
        private readonly BlockingCollection<string> respQueue = new BlockingCollection<string>();
        private readonly IBasicProperties props;

        public Client()
        {
            var factory = new ConnectionFactory
            {
                HostName="localhost"
            };
            _connection = factory.CreateConnection();
            _channel = _connection.CreateModel();
            var _consumer = new EventingBasicConsumer(_channel);
            _replyQueueName = _channel.QueueDeclare().QueueName;

            var correlationId = Guid.NewGuid().ToString();

            props = _channel.CreateBasicProperties();
            props.CorrelationId = correlationId;
            props.ReplyTo = _replyQueueName;

            _consumer.Received += (model, ea) =>
            {
                var body = ea.Body;
                var response = Encoding.UTF8.GetString(body);
                if (ea.BasicProperties.CorrelationId == correlationId)
                {
                    respQueue.Add(response);
                }
            };

            
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

            _channel.BasicConsume(consumer: _consumer,
                                   queue: _replyQueueName,
                                   autoAck: true
            );

            return respQueue.Take();
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