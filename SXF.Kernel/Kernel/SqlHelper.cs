using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data.Common;
using System.Data.SqlClient;
using System.Data;

namespace SXF.Kernel
{
    public class SqlHelper : DataBaseHelper
    {
        public SqlHelper()
            : base()
        {
        }
        /// <summary>
        /// 根据参数类型实例化
        /// </summary>
        /// <param name="content">内容，链接字符串本身或链接结点的名称</param>
        /// <param name="type">ConnectionStringType枚举</param>
        public SqlHelper(string content, ConnectionStringType type)
            : base(content, type)
        { }
        /// <summary>
        /// 根据参数类型实例化
        /// </summary>
        /// <param name="conntent">内容，链接字符串本身或链接结点的名称</param>
        /// <param name="type">connname或connstring</param>
        public SqlHelper(string conntent, string type)
            : base(conntent, type)
        { }
        /// <summary>
        /// 用web.config文件里链接结点实例化
        /// </summary>
        /// <param name="connName">web.config文件里链接结点的名称</param>
        public SqlHelper(string connName)
            : base(connName)
        { }
        /// <summary>
        /// 用委托实例化，委托方法返回链接字符串
        /// </summary>
        /// <param name="handler">委托方法</param>
        public SqlHelper(GetConnStringHandler handler)
            : base(handler)
        { }


        protected override void fillCmdParams_(DbCommand cmd)
        {
            foreach (KeyValuePair<string, object> kv in _params)
            {
                DbParameter p = new SqlParameter(kv.Key, kv.Value);
                cmd.Parameters.Add(p);
            }
            foreach (SqlParameter outp in _outParams)
            {
                outp.Direction = ParameterDirection.Output;
                cmd.Parameters.Add(outp);
            }
        }
        protected override DbCommand createCmd_(string cmdText, DbConnection conn)
        {
            return new SqlCommand(cmdText, (SqlConnection)conn);
        }
        protected override DbCommand createCmd_()
        {
            return new SqlCommand();
        }
        protected override DbDataAdapter createDa_(string cmdText, DbConnection conn)
        {
            return new SqlDataAdapter(cmdText, (SqlConnection)conn);
        }
        protected override DbConnection createConn_()
        {
            return new SqlConnection(_connectionString);
        }

      

        /// <summary>
        /// 新的分页存储过程，更改原来查询结果排序错误
        /// 以前传入排序参数可能不兼容，会导致语法错误
        /// </summary>
        /// <param name="tableName">要显示的表或多个表的连接</param>
        /// <param name="fields">要显示的字段列表</param>
        /// <param name="sortfield">排序字段</param>
        /// <param name="singleSortType">排序方法，false为升序，true为降序</param>
        /// <param name="pageSize">每页显示的记录个数</param>
        /// <param name="pageIndex">要显示那一页的记录</param>
        /// <param name="condition">查询条件,不需where</param>
        /// <param name="count">查询到的记录数</param>
        /// <returns></returns>
        public DataTable TablesPage(string tableName, string fields, string sortfield, bool singleSortType, int pageSize, int pageIndex, string condition, out int count)
        {
            this.Params.Clear();
            this.Params.Add("tblName", tableName);

            this.Params.Add("fields", fields);
            this.Params.Add("sortfields", sortfield);
            this.Params.Add("singleSortType", singleSortType ? "1" : "0");
            this.Params.Add("pageSize", pageSize);
            this.Params.Add("pageIndex", pageIndex);
            this.Params.Add("strCondition", condition);
            SqlParameter p = new SqlParameter("@Counts", SqlDbType.Int);
            this.OutParams.Add(p);
            DataTable dt = this.RunDataTable("sp_TablesPage");
            count = (int)p.Value;
            return dt;
        }

      
        /// <summary>
        /// 根据表插入记录,dataTable需按查询生成结构
        /// </summary>
        /// <param name="dataTable"></param>
        public void InsertFromDataTable(DataTable dataTable, string tableName)
        {
            SqlBulkCopy sqlBulkCopy = new SqlBulkCopy(base.ConnectionString);
            sqlBulkCopy.DestinationTableName = tableName;
            sqlBulkCopy.BatchSize = dataTable.Rows.Count;
            if (dataTable != null && dataTable.Rows.Count != 0)
            {
                sqlBulkCopy.WriteToServer(dataTable);
            }
            sqlBulkCopy.Close();
        }
      
    }
}
