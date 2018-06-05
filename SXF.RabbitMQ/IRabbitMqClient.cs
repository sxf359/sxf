﻿using System;

namespace SXF.RabbitMQ
{
    /// <summary>
    /// RabbitMq client 接口。
    /// </summary>
    public interface IRabbitMqClient : IDisposable
    {
        /// <summary>
        /// RabbitMqClient 数据上下文。
        /// </summary>
        RabbitMqClientContext Context { get; set; }

        /// <summary>
        /// 消息被本地激活事件。通过绑定该事件来获取消息队列推送过来的消息。只能绑定一个事件处理程序。
        /// </summary>
        event ActionEvent ActionEventMessage;

        /// <summary>
        /// 当侦听的队列中有消息到达时触发的可多次执行事件
        /// </summary>
        event ActionEventMultipleHandler ActionEventMultiple;

        /// <summary>
        /// 触发一个事件，向队列推送一个事件消息。
        /// </summary>
        /// <param name="eventMessage">消息类型实例</param>
        /// <param name="exChange">Exchange名称</param>
        /// <param name="queue">队列名称</param>
        void TriggerEventMessage(EventMessage eventMessage, string exChange, string queue);

        /// <summary>
        /// 开始消息队列的默认监听。
        /// </summary>
        void OnListening();
        /// <summary>
        /// 指定监听某一消息队列
        /// </summary>
        /// <param name="queueName">指定监听的消息队列名</param>
        void OnListening(string queueName);
    }
}