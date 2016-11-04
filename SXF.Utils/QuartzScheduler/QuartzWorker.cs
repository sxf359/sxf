using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Quartz;
using Quartz.Impl;

namespace SXF.Utils.QuartzScheduler
{
    /// <summary>
    /// QuartzWorker自动任务
    /// </summary>
    public class QuartzWorker
    {
        IScheduler scheduler;
        public QuartzWorker()
        {
            // 创建一个工作调度器工场
            ISchedulerFactory schedulerFactory = new StdSchedulerFactory();
            // 获取一个任务调度器
            scheduler = schedulerFactory.GetScheduler();
        }
        public static Dictionary<string, bool> workCache = new Dictionary<string, bool>();
        /// <summary>
        /// 添加一个任务
        /// </summary>
        /// <param name="job"></param>
        public void AddWork(QuartzJob job)
        {
            Type type = job.GetType();
            // 创建一个工作
            string jobName = "JobName_" + type;
            string jobGroup = "JobGroup_" + type;
            JobDetail jobDetail = new JobDetail(jobName, jobGroup, type);
            // 创建一个触发器
            Trigger trigger;
            //使用Cron表达式
            if (!string.IsNullOrEmpty(job.CronExpression))
            {
                CronTrigger cronTrigger = new CronTrigger();
                cronTrigger.CronExpression = new CronExpression(job.CronExpression);
                trigger = cronTrigger;
            }
            else//指定间隔次数
            {
                if (job.RepeatInterval.TotalSeconds == 0)
                    throw new Exception("job.RepeatInterval为0");
                if (job.RepeatCount == 0)
                    throw new Exception("job.RepeatCount为0");
                SimpleTrigger simpleTrigger = new SimpleTrigger();
                simpleTrigger.RepeatInterval = job.RepeatInterval;
                simpleTrigger.RepeatCount = job.RepeatCount;
                trigger = simpleTrigger;
            }
            trigger.Name = "trigger" + jobName;
            trigger.JobName = jobName;
            trigger.JobGroup = jobGroup;
            trigger.Group = "triggergroup" + jobName;
            scheduler.AddJob(jobDetail, true);
            DateTime ft = scheduler.ScheduleJob(trigger);
            workCache.Add(jobDetail.Name, false);
        }
        /// <summary>
        /// 开始运行
        /// </summary>
        public void Start()
        {
            scheduler.Start();
            EventLog.Info("QuartzWorker已启动");
        }
        /// <summary>
        /// 停止运行
        /// </summary>
        public void Stop()
        {
            scheduler.Shutdown(true);
            EventLog.Info("QuartzWorker已停止");
        }
    }
}
