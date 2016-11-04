using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SXF.Kernel
{
    public enum PropertyType
    {
        PrimaryKey = 1,
        ForeignKey = 2,
        Property = 3
    }
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = false)]
    public class PropertyAttribute : Attribute
    {
        #region 字段
        private string _columnName;

        private PropertyType _propertyType;

        private string _relationClass;

        private string _relationClassKey;

        #endregion

        #region 构造器
        public PropertyAttribute()
        {
            _propertyType = PropertyType.Property;
            _relationClass = null;
            _relationClassKey = "";
            _columnName = "";
        }

        public PropertyAttribute(PropertyType propertyType)
        {
            _propertyType = propertyType;
            _relationClass = null;
            _relationClassKey = "";
            _columnName = "";
        }
        public PropertyAttribute(PropertyType propertyType, string columnName)
        {
            _propertyType = propertyType;
            _relationClass = null;
            _relationClassKey = "";
            _columnName = columnName;
        }
        public PropertyAttribute(PropertyType propertyType, string relationClass, string relationClassKey)
        {
            _propertyType = propertyType;
            _relationClass = relationClass;
            _relationClassKey = relationClassKey;
            _columnName = "";
        }
        public PropertyAttribute(PropertyType propertyType, string columnName, string relationClass, string relationClassKey)
        {
            _propertyType = propertyType;
            _relationClass = relationClass;
            _relationClassKey = relationClassKey;
            _columnName = columnName;
        }
        #endregion

        #region 属性
        public string ColumnName
        {
            set { _columnName = value; }
            get { return _columnName; }
        }
        public PropertyType PropertyType
        {
            set { _propertyType = value; }
            get { return _propertyType; }
        }
        public string RelationClass
        {
            set { _relationClass = value; }
            get { return _relationClass; }
        }
        public string RelationClassKey
        {
            set { _relationClassKey = value; }
            get { return _relationClassKey; }
        }
        #endregion
    }
}
