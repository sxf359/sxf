using System;
using System.Linq;
using SXF.Utils;
using SXF.Utils.QuartzScheduler;
using LMSoft.CMS.BLL;
using LMSoft.CMS.Models;
using System.Linq.Expressions;
using LMSoft.FrameWork.Identity;
using SXF.Utils.DynamicLambda;
using System.Data.Entity.SqlServer;
using Microsoft.AspNet.Identity.EntityFramework; 

namespace LMSoft.Web.Hubs.TheTask
{
    public class RecycleCustomerWork : QuartzJob, IDisposable
    {
        private ApplicationUserManager _userManager;
        private LMIdentityDbContext _db = new LMIdentityDbContext();
        public RecycleCustomerWork()
        {
            //CronExpression各参数意义   秒 分 时 日 月 周 年 ，参数有时是6个，有时是7个
            //CronExpression = "0 0/1 * * * ?";          
            //每隔两分钟执行一次，测试时使用
            //CronExpression = "0 0/5 * * * ?";
            //每隔两小时执行一次
            CronExpression = "0 0 0/1 * * ?";
            RepeatInterval = new TimeSpan(0, 0, 0);
            RepeatCount = 0;
            DoWork += new EventHandler(this_DoWork);
        }
        public void this_DoWork(object sender, EventArgs e)
        {
            //EventLog.WriteLog("star1");
            try
            {

                //纯话务员
                var list = userManager.Users.Where(m => m.RoleLevel == 0).Select(m => m.IID).ToList();
                //有最近回访日期并且从属于某个话务员的客户
                Expression<Func<Customer, bool>> lambda = m => m.AdminId > 0 && m.LastReviewTime != null && m.Frozen == false && m.Dealed == 0;
                lambda = lambda.And(m => list.Any(x => x == m.AdminId));
                using (var customerService = new CustomerService())
                {
                    var query = customerService.GetCustomer(lambda);
                    //回访日期超过了规定的天数
                    var vQuery = query.Where(m => SqlFunctions.DateDiff("dd", DateTime.Now, SqlFunctions.DateAdd("dd", m.Days, m.LastReviewTime)) <= 0);
                    //EventLog.Log("" + vQuery, "info");
                    EventLog.Log("回访记录开始", "info");
                    foreach (var item in vQuery)
                    {

                        //int operatorId = item.AdminId;
                        var operatorModel = userManager.Users.FirstOrDefault(m => m.IID == item.AdminId);
                        if (operatorModel == null)  //表示话务员不存在
                        {
                            continue;
                        }
                        using (var customerService1 = new CustomerService())
                        {
                            var model = customerService1.Get(item.GID);
                            model.AdminId = 0;                      //话务员编号变为0
                            model.Recycle = 1;                      //标记客户为回收状态
                            model.LastReviewTime = null;
                            customerService1.Update(model);   //更新客户资料
                            EventLog.Log("回收客户编号：" + item.IID, "info");
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                EventLog.Log(ExceptionHelper.GetErrorMessageByLog(ex), "info");
            }


        }

        protected ApplicationUserManager userManager
        {
            get
            {
                var store = new UserStore<ApplicationUser>(_db);
                return _userManager ?? new ApplicationUserManager(store);

            }
            private set
            {
                _userManager = value;
            }
        }

       

        public void Dispose()
        {
            _db.Dispose();
            GC.SuppressFinalize(this);
        }
    }
}
