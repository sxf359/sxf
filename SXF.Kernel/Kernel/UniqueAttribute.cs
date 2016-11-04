using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SXF.Kernel
{
    /// <summary>
    /// 数据不允许重复特性
    /// </summary>
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = false)]
    public class UniqueAttribute : Attribute
    {
    }
}
