using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;
using SXF.Utils;
using SXF.RabbitMQ;
using LMSoft.CMS.Models;
using LMSoft.CMS.BLL;
using System.Configuration;

namespace LMSoft.Web.Hubs
{
    /// <summary>
    /// rabbitmq 消息队列
    /// </summary>
    public class RabbitMQInstance
    {
        #region receive message
        /// <summary>
        /// 接收信息监听并处理
        /// </summary>
        public static void Listening()
        {
            //EventLog.Log("star1", "mq");
            RabbitMqClient.Instance.ActionEventMessage += mqClient_ActionEventMessage;
            RabbitMqClient.Instance.OnListening();
        }

        private static void mqClient_ActionEventMessage(EventMessageResult result)
        {
            //EventLog.Log("mqClient_ActionEventMessage", "mq");
            if (result.EventMessageBytes.EventMessageMarkcode == MessageType.Markcode)
            {
                //EventLog.Log("mqClient_ActionEventMessage-->markcode", "mq");
                var message =
                    MessageSerializerFactory.CreateMessageSerializerInstance()
                        .Deserialize<MessageInfo>(result.MessageBytes);

                result.IsOperationOk = true; //处理成功

            }
        }
        #endregion

        #region send message
        /// <summary>
        /// 向消息队列发送有关QuestionsLibraryAndSort添加或删除的消息
        /// </summary>
        /// <param name="mi"></param>
        public static void SendEventMessageByQuestionsLibraryAndSort(MessageInfo mi)
        {

            var sendMessage =
                EventMessageFactory.CreateEventMessageInstance<MessageInfo>(mi, MessageType.Markcode);

            RabbitMqClient.Instance.TriggerEventMessage(sendMessage, "", RabbitMQCMSRedBag);

            //EventLog.Log("添加消息：" + mi.Content, "mq");
        }
        #endregion

        public static string RabbitMQCMSRedBag
        {
            get { return ConfigurationManager.AppSettings["MqListenQueueName"]; }
        }
    }

    public class MessageInfo
    {
        public string Content { get; set; }
        //public QuestionsLibraryAndSort QuestionsLibraryAndSort { get; set; }
    }
    public class MessageType
    {
        public const string Markcode = "";
    }
}
