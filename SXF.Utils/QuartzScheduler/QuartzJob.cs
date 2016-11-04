using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Quartz;

namespace SXF.Utils.QuartzScheduler
{
    /// <summary>
    /// 任务接口
    /// 优先使用Cron表达式,如果为空,则使用重复规则
    /// </summary>
    public class QuartzJob : IJob
    {
        //string JobName { get; }
        //string JobGroup { get; }
        /// <summary>
        /// Cron表达式
        /// </summary>
        public string CronExpression;
        /// <summary>
        /// 重复间隔
        /// </summary>
        public TimeSpan RepeatInterval;
        /// <summary>
        /// 重复次数
        /// </summary>
        public int RepeatCount;

        /// <summary>
        /// 执行的任务委托
        /// </summary>
        public event EventHandler DoWork;
        static object lockObj = new object();
        public void Execute(Quartz.JobExecutionContext context)
        {
            string name = context.JobDetail.Name;
            if (QuartzWorker.workCache[name])
            {
                EventLog.Log(DoWork.Target.GetType() + " 没运行完又自动触发,被忽略", "info");
                return;
            }
            QuartzWorker.workCache[name] = true;
            try
            {
                DoWork(null, null);
            }
            catch (Exception ero)
            {
                EventLog.Log(DoWork.Target.GetType() + " 执行出错:" + ero, true);
            }
            QuartzWorker.workCache[name] = false;
        }
    }
}
