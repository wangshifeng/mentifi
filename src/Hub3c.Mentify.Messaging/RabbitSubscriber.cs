using System;
using System.Collections.Generic;
using System.Text;
using Newtonsoft.Json;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;

namespace Hub3c.ApiMessage
{
    public class RabbitSubscriber
    {
        private IModel channel;
        private EventingBasicConsumer consumer;
        private string queueName;
        private IConnection _connection;
        private ConnectionFactory _factory;

        public RabbitSubscriber(string username, string password, string serverHost, string virtualHost = "/")
        {
            _factory = new ConnectionFactory { UserName = username, Password = password, VirtualHost = virtualHost };

            var list = new List<string>();
            list.Add(serverHost);
            _connection = _factory.CreateConnection(list);
            channel = _connection.CreateModel();
            //channel.ExchangeDeclare(typeof(NotificationAdded).FullName, ExchangeType.Topic);
        }
        public void Subscribe<T>(Action<T> callback)
        {
            try
            {
                consumer = new EventingBasicConsumer(channel);
                consumer.Received += (model, ea) =>
                {
                    var body = ea.Body;
                    var message = Encoding.UTF8.GetString(body);
                    Console.WriteLine(message);
                    var msg = JsonConvert.DeserializeObject<T>(message);
                    callback(msg);
                };

                queueName = channel.QueueDeclare().QueueName;

                channel.QueueBind(queue: queueName,
                   exchange: typeof(T).FullName,
                   routingKey: "*");

                channel.BasicConsume(queue: queueName,
                                     noAck: true,
                                     consumer: consumer);


            }
            catch (Exception)
            {

                throw;
            }
        }
    }
}
