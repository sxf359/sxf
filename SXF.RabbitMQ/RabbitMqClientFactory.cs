using RabbitMQ.Client;
using SXF.RabbitMQ.Config;
using System;

namespace SXF.RabbitMQ
{
    /// <summary>
    /// RabbitMQClient创建工厂。
    /// </summary>
    public class RabbitMqClientFactory
    {
        /// <summary>
        /// 创建一个单例的RabbitMqClient实例。
        /// </summary>
        /// <returns>IRabbitMqClient</returns>
        public static IRabbitMqClient CreateRabbitMqClientInstance()
        {
            var rabbitMqClientContext = new RabbitMqClientContext
            {
                ListenQueueName = MqConfigDomFactory.CreateConfigDomInstance().MqListenQueueName,
                InstanceCode = Guid.NewGuid().ToString()
            };

            RabbitMqClient.Instance = new RabbitMqClient
            {
                Context = rabbitMqClientContext
            };

            return RabbitMqClient.Instance;
        }
        public static IRabbitMqClient CreateRabbitMqClientInstance(string queueName)
        {
            var rabbitMqClientContext = new RabbitMqClientContext
            {
                ListenQueueName = MqConfigDomFactory.CreateConfigDomInstance(queueName).MqListenQueueName,
                InstanceCode = Guid.NewGuid().ToString()
            };

            var rabbitclient = new RabbitMqClient
            {
                Context = rabbitMqClientContext
            };

            return rabbitclient;
        }

        /// <summary>
        /// 创建一个IConnection
        /// </summary>
        /// <returns></returns>
        internal static IConnection CreateConnection()
        {
            var mqConfigDom = MqConfigDomFactory.CreateConfigDomInstance(); //获取MQ的配置

            const ushort heartbeat = 60;
            var factory = new ConnectionFactory()
            {
                HostName = mqConfigDom.MqHost,
                UserName = mqConfigDom.MqUserName,
                Password = mqConfigDom.MqPassword,
                RequestedHeartbeat = heartbeat, //心跳超时时间
                AutomaticRecoveryEnabled = true //自动重连
            };

            return factory.CreateConnection(); //创建连接对象
        }
        /// <summary>
        /// 创建一个IConnection，此链接的队列名是自定义的
        /// </summary>
        /// <param name="queueName"></param>
        /// <returns></returns>
        internal static IConnection CreateConnection(string queueName)
        {
            var mqConfigDom = MqConfigDomFactory.CreateConfigDomInstance(queueName); //获取MQ的配置

            const ushort heartbeat = 60;
            var factory = new ConnectionFactory()
            {
                HostName = mqConfigDom.MqHost,
                UserName = mqConfigDom.MqUserName,
                Password = mqConfigDom.MqPassword,
                RequestedHeartbeat = heartbeat, //心跳超时时间
                AutomaticRecoveryEnabled = true //自动重连
            };

            return factory.CreateConnection(); //创建连接对象
        }

        /// <summary>
        /// 创建一个IModel。
        /// </summary>
        /// <param name="connection">IConnection.</param>
        /// <returns></returns>
        internal static IModel CreateModel(IConnection connection)
        {
            return connection.CreateModel(); //创建通道
        }
    }
}