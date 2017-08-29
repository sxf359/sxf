using System;

namespace SXF.Utils
{
    /// <summary>
    /// 垃圾回收基类
    /// </summary>
    public class DisposableBase : IDisposable
    {
        #region 垃圾回收
        //是否回收完毕
        bool _disposed;
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }
        ~DisposableBase()
        {
            Dispose(false);
        }

        //这里的参数表示示是否需要释放那些实现IDisposable接口的托管对象
        protected virtual void Dispose(bool disposing)
        {
            if (_disposed) return; //如果已经被回收，就中断执行
            if (disposing)
            {
                //TODO:释放那些实现IDisposable接口的托管对象
                Dispose();
            }
            //TODO:释放非托管资源，设置对象为null
            _disposed = true;

        }
        #endregion
    }
}
