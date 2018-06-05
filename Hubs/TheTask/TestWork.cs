using SXF.Utils;
using SXF.Utils.QuartzScheduler;
using System;

namespace LMSoft.Web.Hubs.TheTask
{
    //[Quartz.DisallowConcurrentExecution]
    public class TestWork : QuartzJob
    {
        public TestWork()
        {
            //CronExpression各参数意义   秒 分 时 日 月 周 年 ，参数有时是6个，有时是7个
            //CronExpression = "0 0/1 * * * ?";
            //每天.1时0分0秒，自动回收到期未回访的客户
            //CronExpression = "0 0 1 * * ?";
            //每隔两分钟执行一次，测试时使用
            //CronExpression = "0 0/5 * * * ?";
            //每隔两小时执行一次
            CronExpression = "0 0/1 * * * ?";
            RepeatInterval = new TimeSpan(0, 0, 0);
            RepeatCount = 0;           
            DoWork += new EventHandler(this_DoWork);
        }

        public void this_DoWork(object sender, EventArgs e)
        {
            try
            {
                EventLog.Log("sxf359", "info");
                //纯话务员

            }
            catch (Exception ex)
            {
                EventLog.Log(ExceptionHelper.GetErrorMessageByLog(ex), "info");
            }
        }
        
    }
}
