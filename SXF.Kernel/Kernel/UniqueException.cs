using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SXF.Kernel
{
    public class UniqueException : Exception
    {
        public UniqueException(List<string> fields)
        {
            this._fields = fields;
        }

        private List<string> _fields;
        /// <summary>
        /// 重复字段列表
        /// </summary>
        public List<string> Fields
        {
            get
            {
                return _fields;
            }
        }
    }
}
