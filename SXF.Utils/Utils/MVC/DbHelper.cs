using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Transactions;

namespace SXF.Utils.MVC
{
    public static class DbHelper
    {
        /// <summary>
        /// with(nolock)
        /// 调用方法
        /// NoLockInvokeDB(() =>
        ///{
        ///using (var db = new TicketDB())
        ///{
        ///   lst = db.User.ToList();
        ///}
        ///});
        /// </summary>
        /// <param name="action"></param>
        public static void NoLockInvokeDB(Action action)
        {
            var transactionOptions = new TransactionOptions();
            transactionOptions.IsolationLevel = IsolationLevel.ReadUncommitted;
            using (var transactionScope = new TransactionScope(TransactionScopeOption.Required, transactionOptions))
            {
                try
                {
                    action();
                }
                finally
                {
                    transactionScope.Complete();
                }
            }
        }
    }
}
