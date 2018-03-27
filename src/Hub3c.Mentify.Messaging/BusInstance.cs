using System;

namespace Hub3c.ApiMessage
{
    public class BusInstance : IBusInstance
    {
        private readonly RabbitBus _instance;

        public BusInstance(RabbitBus instance)
        {
            _instance = instance;
        }

        public void Publish<T>(T message)
        {
            try
            {
                _instance.Publish(message);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public void Declare(Type type)
        {
            _instance.DeclareExchange(type.FullName);
        }
    }

    public interface IBusInstance
    {
        void Publish<T>(T message);
        void Declare(Type type);
    }
}