using System.Text;
using Newtonsoft.Json;
using RabbitMQ.Client;

namespace Hub3c.ApiMessage
{
    public class RabbitBus
    {
        private ConnectionFactory _connectionFactory;
        private IConnection _connection;
        private IModel _channel;

        public RabbitBus() { }

        public RabbitBus(string hostName, string userName, string passsword, string virtualHost = "/")
        {
            CreateConnection(hostName, userName, passsword, virtualHost);
        }


        private void CreateConnection(string hostName, string userName, string passsword, string virtualHost)
        {
            _connectionFactory = new ConnectionFactory() { HostName = hostName, UserName = userName, Password = passsword, VirtualHost = virtualHost };
            _connection = _connectionFactory.CreateConnection();
            _channel = _connection.CreateModel();
        }

        public void DeclareExchange(string name)
        {
            _channel.ExchangeDeclare(exchange: name, type: "topic");
        }

        public string GetConnection()
        {
            return _connectionFactory.HostName;
        }

        public void Publish<T>(T message)
        {
            DeclareExchange(typeof(T).FullName);

            var body = Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(message));
            _channel.BasicPublish(exchange: typeof(T).FullName, routingKey: "*", basicProperties: null, body: body);
        }
    }
}