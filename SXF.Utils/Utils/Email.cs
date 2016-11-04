using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net;
using System.Net.Mail;
using System.Text.RegularExpressions;

namespace SXF.Utils
{
    /// <summary>
    /// 邮件发送类
    /// </summary>
    public class Email
    {

        public Email()
        {
        }


        #region 属性
        /// <summary>
        /// 邮件主题
        /// </summary>
        public virtual string Title { get; set; }

        /// <summary>
        /// 邮件内容
        /// </summary>
        public virtual string Cont { get; set; }

        /// <summary>
        /// 发邮件地址
        /// </summary>
        public virtual string SendEMail { get; set; }

        /// <summary>
        /// 发邮件密码
        /// </summary>
        public virtual string SendPwd { get; set; }

        /// <summary>
        /// 收邮件地址
        /// </summary>
        public string ReceiveEmail { get; set; }
        #endregion

        #region 方法
        /// <summary>
        /// 发邮件
        /// </summary>
        /// <param name="email"></param>
        public void SendMail(Email email)
        {
            //发邮件地址和密码
            string email_sender = System.Configuration.ConfigurationManager.AppSettings["emailSender"].ToString();
            email.SendEMail = email_sender.Split(',')[0];
            email.SendPwd = email_sender.Split(',')[1];

            var smtpServer = GetSmtpServer(email.SendEMail);
            MailAddress addressFrom = new MailAddress(email.SendEMail);
            MailMessage mailMsg = new MailMessage();
            mailMsg.From = addressFrom;
            mailMsg.To.Add(email.ReceiveEmail);
            mailMsg.Subject = email.Title;
            mailMsg.Body = email.Cont;
            mailMsg.IsBodyHtml = true;
            mailMsg.BodyEncoding = Encoding.Default;
            mailMsg.Priority = MailPriority.High;
            SmtpClient smtpClient = new SmtpClient();
            smtpClient.Host = smtpServer;
            smtpClient.UseDefaultCredentials = false;
            smtpClient.Credentials = new NetworkCredential(email.SendEMail, email.SendPwd);
            smtpClient.DeliveryMethod = SmtpDeliveryMethod.Network;
            smtpClient.Send(mailMsg);
        }


        /// <summary>
        /// 发邮件给管理员
        /// </summary>
        /// <param name="email"></param>
        public void SendMailToAdmin(Email email)
        {
            //发邮件地址和密码
            string email_sender = System.Configuration.ConfigurationManager.AppSettings["emailSender"].ToString();
            string sendEmail = email_sender.Split(',')[0];      //发邮件地址
            string sendPwd = email_sender.Split(',')[1];        //发邮件密码
            string receiveEmail = ReceiveEmail;       //收邮件的管理员
            var smtpServer = GetSmtpServer(sendEmail);
            MailAddress addressFrom = new MailAddress(sendEmail);
            MailMessage mailMsg = new MailMessage();
            mailMsg.From = addressFrom;
            mailMsg.To.Add(receiveEmail);
            //同时发送多个邮件
            //mailMsg.To.Add("626056064@qq.com,718083192@qq.com");
            mailMsg.Subject = email.Title;    //邮件标题
            mailMsg.Body = email.Cont;        //邮件内容
            mailMsg.IsBodyHtml = true;
            mailMsg.BodyEncoding = Encoding.Default;
            mailMsg.Priority = MailPriority.High;
            SmtpClient smtpClient = new SmtpClient();
            smtpClient.Host = smtpServer;
            smtpClient.UseDefaultCredentials = false;
            smtpClient.Credentials = new NetworkCredential(sendEmail, sendPwd);
            smtpClient.DeliveryMethod = SmtpDeliveryMethod.Network;
            smtpClient.Send(mailMsg);
        }

        public string GetSmtpServer(string sendEmail)
        {
            var smtpServer = "smtp.exmail.qq.com";
            //var addrM = Regex.Match(sendEmail, @"[^@]+@(?<host>.+)",
            //    RegexOptions.IgnoreCase | RegexOptions.Singleline);
            //var host = addrM.Groups["host"].Value;
            //EventLog.WriteLog(host);
            //smtpServer = "smtp." + host;
            return smtpServer;
        }



        /// <summary>
        /// 添加空格
        /// </summary>
        /// <param name="t"></param>
        /// <param name="count"></param>
        public static void AppendNbsp(ref StringBuilder t, int count)
        {
            while (count > 0)
            {
                t.Append("&nbsp;");
                count--;
            }
        }


        /// <summary>
        ///格式化的邮件内容
        /// </summary>
        /// <param name="email_customer"></param>
        /// <returns></returns>
        public string GetAdminBody(string emailContent)
        {
            StringBuilder bodyInfo = new StringBuilder("<html><head><title></title></head><body>");
            bodyInfo.Append(string.Format("管理员您好："));
            //bodyInfo.Append("<br /><br />");
            bodyInfo.Append(string.Format("{0}", emailContent));
            SXF.Utils.Email.AppendNbsp(ref bodyInfo, 77);
            bodyInfo.Append(string.Format("<p>{0}<br/>", "牛买网"));
            SXF.Utils.Email.AppendNbsp(ref bodyInfo, 77);
            bodyInfo.Append(string.Format("{0}</body></html>", DateTime.Now.ToShortDateString()));
            return bodyInfo.ToString();
        }

    }
        #endregion





}
