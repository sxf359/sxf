using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SXF.Kernel
{
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = false)]
    public class IdentityAttribute : Attribute
    {
        public IdentityAttribute()
        {
        }
    }
}
