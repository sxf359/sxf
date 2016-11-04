using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Data;
using System.Linq;
using System.Data.SqlClient;
using System.Data.Common;
using System.Runtime.InteropServices;
using System.Reflection;
using System.Text;
using SXF.Utils;


namespace SXF.Kernel
{

    /// <summary>
    /// 数据访问助手类
    /// </summary>
    public class DbHelper
    {
        #region 属性
        private object lockHelper = new object();
        protected string m_connectionstring = null;
        private DbProviderFactory m_factory = null;
        private Hashtable m_paramcache = Hashtable.Synchronized(new Hashtable());// Parameters缓存哈希表
        private IDbProvider m_provider = null;
        private int m_querycount = 0;
        private string m_querydetail = "";

        private DbConnection _conn;        //数据连接
        private DbTransaction _trans;      //事务连接





        /// <summary>
        /// 数据库连接字符串
        /// </summary>
        protected internal string ConnectionString
        {
            get
            {
                return this.m_connectionstring;
            }
            set
            {
                this.m_connectionstring = value;
            }
        }
        /// <summary>
        /// DbFactory实例
        /// </summary>
        public DbProviderFactory Factory
        {
            get
            {
                if (this.m_factory == null)
                {
                    this.m_factory = this.Provider.Instance();
                }
                return this.m_factory;
            }
        }
        /// <summary>
        /// IDbProvider接口
        /// </summary>
        public IDbProvider Provider
        {
            get
            {
                if (this.m_provider == null)
                {
                    lock (this.lockHelper)
                    {
                        if (this.m_provider == null)
                        {
                            try
                            {
                                this.m_provider = (IDbProvider)Activator.CreateInstance(Type.GetType("SXF.Kernel.SqlServerProvider, SXF.Kernel", false, true));
                            }
                            catch
                            {
                                new Terminator().Throw("SqlServerProvider 数据库访问器创建失败！");
                            }
                        }
                    }
                }
                return this.m_provider;
            }
        }
        /// <summary>
        /// 查询次数统计
        /// </summary>
        public int QueryCount
        {
            get
            {
                return this.m_querycount;
            }
            set
            {
                this.m_querycount = value;
            }
        }
        /// <summary>
        /// 查询详情
        /// </summary>
        public string QueryDetail
        {
            get
            {
                return m_querydetail;
            }
            set
            {
                m_querydetail = value;
            }
        }
        #endregion

        #region 私有方法
        protected void fillCmdParams_(DbCommand cmd, params DbParameter[] param)
        {
            foreach (SqlParameter p in param)
            {
                cmd.Parameters.Add(p);
            }

        }
        protected DbCommand createCmd_(string cmdText, DbConnection conn)
        {
            return new SqlCommand(cmdText, (SqlConnection)conn);
        }
        protected DbCommand createCmd_()
        {
            return new SqlCommand();
        }
        protected DbDataAdapter createDa_(string cmdText, DbConnection conn)
        {
            return new SqlDataAdapter(cmdText, (SqlConnection)conn);
        }






        private int doTrans_(string text, CommandType type, params DbParameter[] param)
        {
            //EventLog.WriteLog("doTrans_:"+_conn.ToString());
            if (_conn == null)
            {
                throw new Exception("无效连接");
            }
            DbCommand cmd = createCmd_(text, _conn);
            cmd.CommandTimeout = 180;   //操作超时时间180秒
            cmd.CommandType = type;
            if (_trans == null)
            {
                throw new Exception("事务没有开启");
            }
            cmd.Transaction = _trans;
            fillCmdParams_(cmd, param);
            return cmd.ExecuteNonQuery();
        }

        private DataSet doDataSetTrans_(string text, CommandType type, params DbParameter[] param)
        {
            //EventLog.WriteLog("doDataSetTrans_:" + _conn.ToString());
            DbDataAdapter da = createDa_(text, _conn);
            da.SelectCommand.CommandType = type;
            if (_trans == null)
            {
                throw new Exception("事务没有开启");
            }
            da.SelectCommand.Transaction = _trans;
            fillCmdParams_(da.SelectCommand, param);
            DataSet ds = new DataSet();
            da.Fill(ds);
            return ds;
        }

        private object doScalarTrans_(string text, CommandType type, params DbParameter[] param)
        {
            //EventLog.WriteLog("doScalarTrans_:" + _conn.ToString());
            DbCommand cmd = createCmd_(text, _conn);
            cmd.CommandTimeout = 180;   //操作超时时间180秒
            cmd.CommandType = type;
            if (_trans == null)
            {
                throw new Exception("事务没有开启");
            }
            cmd.Transaction = _trans;
            fillCmdParams_(cmd, param);
            return cmd.ExecuteScalar();
        }

        private DbDataReader doDataReaderTrans_(string text, CommandType type, params DbParameter[] param)
        {
            DbCommand cmd = createCmd_(text, _conn);
            cmd.CommandTimeout = 180;   //操作超时时间180秒
            cmd.CommandType = type;
            if (_trans == null)
            {
                throw new Exception("事务没有开启");
            }
            cmd.Transaction = _trans;
            fillCmdParams_(cmd, param);
            return cmd.ExecuteReader(CommandBehavior.CloseConnection);

        }


        /// <summary>
        ///  将DataRow类型的列值分配到DbParameter参数数组
        /// </summary>
        /// <param name="commandParameters">要分配值的DbParameter参数数组</param>
        /// <param name="dataRow">将要分配给存储过程参数的DataRow</param>
        private void AssignParameterValues(DbParameter[] commandParameters, DataRow dataRow)
        {
            if ((commandParameters != null) && (dataRow != null))
            {
                int num = 0;
                foreach (DbParameter parameter in commandParameters)
                {
                    if ((parameter.ParameterName == null) || (parameter.ParameterName.Length <= 1))
                    {
                        new Terminator().Throw(string.Format("请提供参数{0}一个有效的名称{1}.", num, parameter.ParameterName));
                    }
                    if (dataRow.Table.Columns.IndexOf(parameter.ParameterName.Substring(1)) != -1)
                    {
                        parameter.Value = dataRow[parameter.ParameterName.Substring(1)];
                    }
                    num++;
                }
            }
        }
        /// <summary>
        /// 将一个对象数组分配给DbParameter参数数组
        /// </summary>
        /// <param name="commandParameters">要分配值的DbParameter参数数组</param>
        /// <param name="parameterValues">将要分配给存储过程参数的对象数组</param>
        private void AssignParameterValues(DbParameter[] commandParameters, object[] parameterValues)
        {
            if ((commandParameters != null) && (parameterValues != null))
            {
                if (commandParameters.Length != parameterValues.Length)
                {
                    new Terminator().Throw("参数值个数与参数不匹配。");
                }
                int index = 0;
                int length = commandParameters.Length;
                while (index < length)
                {
                    if (parameterValues[index] is IDbDataParameter)
                    {
                        IDbDataParameter parameter = (IDbDataParameter)parameterValues[index];
                        if (parameter.Value == null)
                        {
                            commandParameters[index].Value = DBNull.Value;
                        }
                        else
                        {
                            commandParameters[index].Value = parameter.Value;
                        }
                    }
                    else if (parameterValues[index] == null)
                    {
                        commandParameters[index].Value = DBNull.Value;
                    }
                    else
                    {
                        commandParameters[index].Value = parameterValues[index];
                    }
                    index++;
                }
            }
        }
        /// <summary>
        /// 将DbParameter参数数组(参数值)分配给DbCommand命令.
        /// 这个方法将给任何一个参数分配DBNull.Value;
        /// 该操作将阻止默认值的使用.
        /// </summary>
        /// <param name="command">命令名</param>
        /// <param name="commandParameters">DbParameters数组</param>
        private void AttachParameters(DbCommand command, DbParameter[] commandParameters)
        {
            if (command == null)
            {
                throw new ArgumentNullException("command");
            }
            if (commandParameters != null)
            {
                foreach (DbParameter parameter in commandParameters)
                {
                    if (parameter != null)
                    {
                        if (((parameter.Direction == ParameterDirection.InputOutput) || (parameter.Direction == ParameterDirection.Input)) && (parameter.Value == null))
                        {
                            parameter.Value = DBNull.Value;
                        }
                        command.Parameters.Add(parameter);
                    }
                }
            }
        }

        /// <summary>
        /// DbParameter参数数组的深层拷贝
        /// </summary>
        /// <param name="originalParameters">原始参数数组</param>
        /// <returns>返回一个同样的参数数组</returns>
        private DbParameter[] CloneParameters(DbParameter[] originalParameters)
        {
            DbParameter[] parameterArray = new DbParameter[originalParameters.Length];
            int index = 0;
            int length = originalParameters.Length;
            while (index < length)
            {
                parameterArray[index] = (DbParameter)((ICloneable)originalParameters[index]).Clone();
                index++;
            }
            return parameterArray;
        }

        /// <summary>
        /// 探索运行时的存储过程,返回DbParameter参数数组.
        /// 初始化参数值为 DBNull.Value.
        /// </summary>
        /// <param name="connection">一个有效的数据库连接</param>
        /// <param name="spName">存储过程名称</param>
        /// <param name="includeReturnValueParameter">是否包含返回值参数</param>
        /// <returns>返回DbParameter参数数组</returns>
        private DbParameter[] DiscoverSpParameterSet(DbConnection connection, string spName, bool includeReturnValueParameter)
        {
            if (connection == null)
            {
                throw new ArgumentNullException("connection");
            }
            if ((spName == null) || (spName.Length == 0))
            {
                throw new ArgumentNullException("spName");
            }
            DbCommand cmd = connection.CreateCommand();
            cmd.CommandText = spName;
            cmd.CommandType = CommandType.StoredProcedure;
            connection.Open();
            this.Provider.DeriveParameters(cmd);
            connection.Close();
            if (!includeReturnValueParameter)
            {
                cmd.Parameters.RemoveAt(0);
            }
            DbParameter[] array = new DbParameter[cmd.Parameters.Count];
            cmd.Parameters.CopyTo(array, 0);
            foreach (DbParameter parameter in array)
            {
                parameter.Value = DBNull.Value;
            }
            return array;
        }

        private static string GetQueryDetail(string commandText, DateTime dtStart, DateTime dtEnd, DbParameter[] cmdParams)
        {
            string str = "<tr style=\"background: rgb(255, 255, 255) none repeat scroll 0%; -moz-background-clip: -moz-initial; -moz-background-origin: -moz-initial; -moz-background-inline-policy: -moz-initial;\">";
            string str2 = "";
            string str3 = "";
            string str4 = "";
            string str5 = "";
            if ((cmdParams != null) && (cmdParams.Length > 0))
            {
                foreach (DbParameter parameter in cmdParams)
                {
                    if (parameter != null)
                    {
                        str2 = str2 + "<td>" + parameter.ParameterName + "</td>";
                        str3 = str3 + "<td>" + parameter.DbType.ToString() + "</td>";
                        str4 = str4 + "<td>" + parameter.Value.ToString() + "</td>";
                    }
                }
                str5 = string.Format("<table width=\"100%\" cellspacing=\"1\" cellpadding=\"0\" style=\"background: rgb(255, 255, 255) none repeat scroll 0%; margin-top: 5px; font-size: 12px; display: block; -moz-background-clip: -moz-initial; -moz-background-origin: -moz-initial; -moz-background-inline-policy: -moz-initial;\">{0}{1}</tr>{0}{2}</tr>{0}{3}</tr></table>", new object[] { str, str2, str3, str4 });
            }
            return string.Format("<center><div style=\"border: 1px solid black; margin: 2px; padding: 1em; text-align: left; width: 96%; clear: both;\"><div style=\"font-size: 12px; float: right; width: 100px; margin-bottom: 5px;\"><b>TIME:</b> {0}</div><span style=\"font-size: 12px;\">{1}{2}</span></div><br /></center>", dtEnd.Subtract(dtStart).TotalMilliseconds / 1000.0, commandText, str5);
        }

        /// <summary>
        /// 预处理dbcommand
        /// </summary>
        /// <param name="command">DbCommand</param>
        /// <param name="connection">DbConnection</param>
        /// <param name="transaction">DbTransaction</param>
        /// <param name="commandType">CommandType</param>
        /// <param name="commandText">commandText</param>
        /// <param name="commandParameters">commandParameters</param>
        /// <param name="mustCloseConnection">bool</param>
        private void PrepareCommand(DbCommand command, DbConnection connection, DbTransaction transaction, CommandType commandType, string commandText, DbParameter[] commandParameters, out bool mustCloseConnection)
        {
            if (command == null)
            {
                throw new ArgumentNullException("command");
            }
            if ((commandText == null) || (commandText.Length == 0))
            {
                throw new ArgumentNullException("commandText");
            }
            if (connection.State != ConnectionState.Open)
            {
                mustCloseConnection = true;
                connection.Open();
            }
            else
            {
                mustCloseConnection = false;
            }
            command.Connection = connection;
            command.CommandText = commandText;
            if (transaction != null)
            {
                if (transaction.Connection == null)
                {
                    throw new ArgumentException("The transaction was rollbacked or commited, please provide an open transaction.", "transaction");
                }
                command.Transaction = transaction;
            }
            command.CommandType = commandType;
            if (commandParameters != null)
            {
                this.AttachParameters(command, commandParameters);
            }
        }

        private enum DbConnectionOwnership
        {
            Internal,
            External
        }
        #endregion

        #region 构造函数
        public DbHelper(string connString)
        {

            this.BuildConnection(connString);

        }
        #endregion

        #region 公用方法
        /// <summary>
        /// 构建数据库连接
        /// </summary>
        /// <param name="connectionString">数据库连接字符串</param>
        public void BuildConnection(string connectionString)
        {
            if (string.IsNullOrEmpty(connectionString))
            {
                new Terminator().Throw("请检查数据库连接信息，当前数据库连接信息为空。");
            }
            this.m_connectionstring = connectionString;
            this.m_querycount = 0;
        }


        /// <summary>
        /// 追加参数数组到缓存
        /// </summary>
        /// <param name="commandText">存储过程名或SQL语句</param>
        /// <param name="commandParameters">要缓存的参数数组</param>
        public void CacheParameterSet(string commandText, params DbParameter[] commandParameters)
        {
            if ((this.ConnectionString == null) || (this.ConnectionString.Length == 0))
            {
                throw new ArgumentNullException("ConnectionString");
            }
            if ((commandText == null) || (commandText.Length == 0))
            {
                throw new ArgumentNullException("commandText");
            }
            string str = this.ConnectionString + ":" + commandText;
            this.m_paramcache[str] = commandParameters;
        }

        /// <summary>
        /// 创建DbCommand命令,指定数据库连接对象,存储过程名和参数
        /// </summary>
        /// <remarks>
        /// 示例:  
        /// DbCommand command = CreateCommand( conn , "AddCustomer" , "CustomerID" , "CustomerName" );
        /// </remarks>
        /// <param name="connection">一个有效的数据库连接对象</param>
        /// <param name="spName">存储过程名称</param>
        /// <param name="sourceColumns">源表的列名称数组</param>
        /// <returns>返回DbCommand命令</returns>
        public DbCommand CreateCommand(DbConnection connection, string spName, params string[] sourceColumns)
        {
            if (connection == null)
            {
                throw new ArgumentNullException("connection");
            }
            if ((spName == null) || (spName.Length == 0))
            {
                throw new ArgumentNullException("spName");
            }
            DbCommand command = this.Factory.CreateCommand();
            command.CommandText = spName;
            command.Connection = connection;
            command.CommandType = CommandType.StoredProcedure;
            if ((sourceColumns != null) && (sourceColumns.Length > 0))
            {
                DbParameter[] spParameterSet = this.GetSpParameterSet(connection, spName);
                for (int i = 0; i < sourceColumns.Length; i++)
                {
                    spParameterSet[i].SourceColumn = sourceColumns[i];
                }
                this.AttachParameters(command, spParameterSet);
            }
            return command;
        }




        #region dataset相关方法
        /// <summary>
        /// 执行不带参数的SQL语句,返回DataSet.
        /// </summary>
        /// <remarks>
        ///    示例:  
        ///     DataSet ds = ExecuteDataset("SELECT * FROM [table1]");
        /// </remarks> 
        /// <param name="commandText">SQL语句</param>
        /// <returns>返回一个包含结果集的DataSet</returns>
        public DataSet ExecuteDataset(string commandText)
        {
            return this.ExecuteDataset(CommandType.Text, commandText, null);
        }
        /// <summary>
        /// 执行不带参数的存储过程名或SQL语句,返回DataSet.
        /// </summary>
        /// <remarks>
        /// 示例:  
        /// DataSet ds = ExecuteDataset( CommandType.StoredProcedure, "GetOrders");
        /// </remarks>
        /// <param name="commandType">命令类型 (存储过程,命令文本或其它)</param>
        /// <param name="commandText">存储过程名称或SQL语句</param>
        /// <returns>返回一个包含结果集的DataSet</returns>
        public DataSet ExecuteDataset(CommandType commandType, string commandText)
        {
            return this.ExecuteDataset(commandType, commandText, null);
        }
        /// <summary>
        /// 执行带参数的存储过程名或SQL语句,返回DataSet.
        /// </summary>
        /// <remarks>
        /// 示例: 
        /// DataSet ds = ExecuteDataset( CommandType.StoredProcedure, "GetOrders", new DbParameter("@prodid", 24));
        /// </remarks>
        /// <param name="commandType">命令类型 (存储过程,命令文本或其它)</param>
        /// <param name="commandText">存储过程名或SQL语句</param>
        /// <param name="commandParameters">SqlParamter参数数组</param>
        /// <returns>返回一个包含结果集的DataSet</returns>
        public DataSet ExecuteDataset(CommandType commandType, string commandText, params DbParameter[] commandParameters)
        {
            if ((this.ConnectionString == null) || (this.ConnectionString.Length == 0))
            {
                throw new ArgumentNullException("ConnectionString");
            }
            using (DbConnection connection = this.Factory.CreateConnection())
            {
                //EventLog.WriteLog("ConnectionString:" + this.ConnectionString);
                connection.ConnectionString = this.ConnectionString;
                connection.Open();
                return this.ExecuteDataset(connection, commandType, commandText, commandParameters);

            }
        }
        /// <summary>
        /// 执行指定数据库连接对象的命令,返回DataSet.
        /// </summary>
        /// <remarks>
        /// 示例:  
        /// DataSet ds = ExecuteDataset( conn , CommandType.StoredProcedure , "GetOrders" );
        /// </remarks>
        /// <param name="connection">一个有效的数据库连接对象</param>
        /// <param name="commandType">命令类型 (存储过程,命令文本或其它)</param>
        /// <param name="commandText">存储过程名或SQL语句</param>
        /// <returns>返回一个包含结果集的DataSet</returns>
        public DataSet ExecuteDataset(DbConnection connection, CommandType commandType, string commandText)
        {
            return this.ExecuteDataset(connection, commandType, commandText, null);
        }
        /// <summary>
        ///  执行指定数据库连接对象的命令,指定存储过程参数,返回DataSet.
        /// </summary>
        /// <remarks>
        /// 此方法不提供访问存储过程输入参数和返回值.
        ///    示例.:  
        ///    DataSet ds = ExecuteDataset(conn, "GetOrders", 24, 36);
        /// </remarks>
        /// <param name="connection">一个有效的数据库连接对象</param>
        /// <param name="spName">存储过程名</param>
        /// <param name="parameterValues">分配给存储过程输入参数的对象数组</param>
        /// <returns>返回一个包含结果集的DataSet</returns>
        public DataSet ExecuteDataset(DbConnection connection, string spName, params object[] parameterValues)
        {
            if (connection == null)
            {
                throw new ArgumentNullException("connection");
            }
            if ((spName == null) || (spName.Length == 0))
            {
                throw new ArgumentNullException("spName");
            }
            if ((parameterValues != null) && (parameterValues.Length > 0))
            {
                DbParameter[] spParameterSet = this.GetSpParameterSet(connection, spName);
                this.AssignParameterValues(spParameterSet, parameterValues);
                return this.ExecuteDataset(connection, CommandType.StoredProcedure, spName, spParameterSet);
            }
            return this.ExecuteDataset(connection, CommandType.StoredProcedure, spName);
        }
        /// <summary>
        /// 执行指定数据库连接对象的命令,返回DataSet.
        /// </summary>
        /// <remarks>
        ///  示例: 
        /// DataSet ds = ExecuteDataset( conn, CommandType.StoredProcedure, "GetOrders", new DbParameter("@prodid", 24));
        /// </remarks>
        /// <param name="connection">一个有效的数据库连接对象</param>
        /// <param name="commandType">命令类型 (存储过程,命令文本或其它)</param>
        /// <param name="commandText">存储过程名或SQL语句</param>
        /// <param name="commandParameters">SqlParamter参数数组</param>
        /// <returns></returns>
        public DataSet ExecuteDataset(DbConnection connection, CommandType commandType, string commandText, params DbParameter[] commandParameters)
        {
            //EventLog.WriteLog(commandText);
            if (connection == null)
            {
                throw new ArgumentNullException("connection");
            }
            using (DbCommand command = this.Factory.CreateCommand())
            {
                bool mustCloseConnection = false;
                this.PrepareCommand(command, connection, null, commandType, commandText, commandParameters, out mustCloseConnection);
                //EventLog.WriteLog(commandText);
                using (DbDataAdapter adapter = this.Factory.CreateDataAdapter())
                {
                    adapter.SelectCommand = command;
                    DataSet dataSet = new DataSet();
                    DateTime now = DateTime.Now;
                    adapter.Fill(dataSet);
                    DateTime dtEnd = DateTime.Now;
                    m_querydetail = GetQueryDetail(command.CommandText, now, dtEnd, commandParameters);
                    this.m_querycount++;
                    command.Parameters.Clear();
                    if (mustCloseConnection)
                    {
                        connection.Close();
                    }
                    return dataSet;
                }
            }
        }

        /// <summary>
        /// 执行指定连接数据库连接字符串的存储过程,使用DataRow做为参数值,返回DataSet.
        /// </summary>
        /// <param name="spName">存储过程名称</param>
        /// <param name="dataRow">使用DataRow作为参数值</param>
        /// <returns>返回一个包含结果集的DataSet.</returns>
        public DataSet ExecuteDatasetTypedParams(string spName, DataRow dataRow)
        {
            if ((this.ConnectionString == null) || (this.ConnectionString.Length == 0))
            {
                throw new ArgumentNullException("ConnectionString");
            }
            if ((spName == null) || (spName.Length == 0))
            {
                throw new ArgumentNullException("spName");
            }
            if ((dataRow != null) && (dataRow.ItemArray.Length > 0))
            {
                DbParameter[] spParameterSet = this.GetSpParameterSet(spName);
                this.AssignParameterValues(spParameterSet, dataRow);
                return this.ExecuteDataset(CommandType.StoredProcedure, spName, spParameterSet);
            }
            return this.ExecuteDataset(CommandType.StoredProcedure, spName);
        }
        /// <summary>
        /// 执行指定连接数据库连接对象的存储过程,使用DataRow做为参数值,返回DataSet.
        /// </summary>
        /// <param name="connection">一个有效的数据库连接对象</param>
        /// <param name="spName">存储过程名称</param>
        /// <param name="dataRow">使用DataRow作为参数值</param>
        /// <returns>返回一个包含结果集的DataSet.</returns>
        public DataSet ExecuteDatasetTypedParams(DbConnection connection, string spName, DataRow dataRow)
        {
            if (connection == null)
            {
                throw new ArgumentNullException("connection");
            }
            if ((spName == null) || (spName.Length == 0))
            {
                throw new ArgumentNullException("spName");
            }
            if ((dataRow != null) && (dataRow.ItemArray.Length > 0))
            {
                DbParameter[] spParameterSet = this.GetSpParameterSet(connection, spName);
                this.AssignParameterValues(spParameterSet, dataRow);
                return this.ExecuteDataset(connection, CommandType.StoredProcedure, spName, spParameterSet);
            }
            return this.ExecuteDataset(connection, CommandType.StoredProcedure, spName);
        }
        /// <summary>
        ///  执行指定连接数据库事务的存储过程,使用DataRow做为参数值,返回DataSet.
        /// </summary>
        /// <param name="transaction">一个有效的连接事务</param>
        /// <param name="spName">存储过程名称</param>
        /// <param name="dataRow">使用DataRow作为参数值</param>
        /// <returns>返回一个包含结果集的DataSet.</returns>
        public DataSet ExecuteDatasetTypedParams(DbTransaction transaction, string spName, DataRow dataRow)
        {
            if (transaction == null)
            {
                throw new ArgumentNullException("transaction");
            }
            if ((transaction != null) && (transaction.Connection == null))
            {
                throw new ArgumentException("The transaction was rollbacked or commited, please provide an open transaction.", "transaction");
            }
            if ((spName == null) || (spName.Length == 0))
            {
                throw new ArgumentNullException("spName");
            }
            if ((dataRow != null) && (dataRow.ItemArray.Length > 0))
            {
                DbParameter[] spParameterSet = this.GetSpParameterSet(transaction.Connection, spName);
                this.AssignParameterValues(spParameterSet, dataRow);
                return this.ExecuteDataset(transaction, CommandType.StoredProcedure, spName, spParameterSet);
            }
            return this.ExecuteDataset(transaction, CommandType.StoredProcedure, spName);
        }

        public void UpdateDataSet(DataSet dataSet, string tableName)
        {
            string str = string.Format("Select * from {0} where 1=0", tableName);
            DbCommandBuilder builder = this.Factory.CreateCommandBuilder();
            builder.DataAdapter = this.Factory.CreateDataAdapter();
            builder.DataAdapter.SelectCommand = this.Factory.CreateCommand();
            builder.DataAdapter.DeleteCommand = this.Factory.CreateCommand();
            builder.DataAdapter.InsertCommand = this.Factory.CreateCommand();
            builder.DataAdapter.UpdateCommand = this.Factory.CreateCommand();
            builder.DataAdapter.SelectCommand.CommandText = str;
            builder.DataAdapter.SelectCommand.Connection = this.Factory.CreateConnection();
            builder.DataAdapter.DeleteCommand.Connection = this.Factory.CreateConnection();
            builder.DataAdapter.InsertCommand.Connection = this.Factory.CreateConnection();
            builder.DataAdapter.UpdateCommand.Connection = this.Factory.CreateConnection();
            builder.DataAdapter.SelectCommand.Connection.ConnectionString = this.ConnectionString;
            builder.DataAdapter.DeleteCommand.Connection.ConnectionString = this.ConnectionString;
            builder.DataAdapter.InsertCommand.Connection.ConnectionString = this.ConnectionString;
            builder.DataAdapter.UpdateCommand.Connection.ConnectionString = this.ConnectionString;
            this.UpdateDataSet(builder.GetInsertCommand(), builder.GetDeleteCommand(), builder.GetUpdateCommand(), dataSet, tableName);
        }

        public void UpdateDataSet(DbCommand insertCommand, DbCommand deleteCommand, DbCommand updateCommand, DataSet dataSet, string tableName)
        {
            if (insertCommand == null)
            {
                throw new ArgumentNullException("insertCommand");
            }
            if (deleteCommand == null)
            {
                throw new ArgumentNullException("deleteCommand");
            }
            if (updateCommand == null)
            {
                throw new ArgumentNullException("updateCommand");
            }
            if ((tableName == null) || (tableName.Length == 0))
            {
                throw new ArgumentNullException("tableName");
            }
            using (DbDataAdapter adapter = this.Factory.CreateDataAdapter())
            {
                adapter.UpdateCommand = updateCommand;
                adapter.InsertCommand = insertCommand;
                adapter.DeleteCommand = deleteCommand;
                adapter.Update(dataSet, tableName);
                dataSet.AcceptChanges();
            }
        }
        #endregion


        #region 执行事务
        /// <summary>
        /// 执行指定事务的命令,返回DataSet.
        /// </summary>
        /// <param name="transaction">事务</param>
        /// <param name="commandType">>命令类型 (存储过程,命令文本或其它)</param>
        /// <param name="commandText">存储过程名或SQL语句</param>
        /// <returns>返回一个包含结果集的DataSet</returns>
        public DataSet ExecuteDataset(DbTransaction transaction, CommandType commandType, string commandText)
        {
            return this.ExecuteDataset(transaction, commandType, commandText, null);
        }
        /// <summary>
        /// 执行指定事务的命令,指定参数值,返回DataSet.
        /// </summary>
        /// <remarks>
        /// 此方法不提供访问存储过程输入参数和返回值.
        ///    示例.:  
        ///    DataSet ds = ExecuteDataset(trans, "GetOrders", 24, 36);
        /// </remarks>
        /// <param name="transaction">事务</param>
        /// <param name="spName">存储过程名</param>
        /// <param name="parameterValues">分配给存储过程输入参数的对象数组</param>
        /// <returns>返回一个包含结果集的DataSet</returns>
        public DataSet ExecuteDataset(DbTransaction transaction, string spName, params object[] parameterValues)
        {
            if (transaction == null)
            {
                throw new ArgumentNullException("transaction");
            }
            if ((transaction != null) && (transaction.Connection == null))
            {
                throw new ArgumentException("The transaction was rollbacked or commited, please provide an open transaction.", "transaction");
            }
            if ((spName == null) || (spName.Length == 0))
            {
                throw new ArgumentNullException("spName");
            }
            if ((parameterValues != null) && (parameterValues.Length > 0))
            {
                DbParameter[] spParameterSet = this.GetSpParameterSet(transaction.Connection, spName);
                this.AssignParameterValues(spParameterSet, parameterValues);
                return this.ExecuteDataset(transaction, CommandType.StoredProcedure, spName, spParameterSet);
            }
            return this.ExecuteDataset(transaction, CommandType.StoredProcedure, spName);
        }
        /// <summary>
        /// 执行指定事务的命令,指定参数,返回DataSet.
        /// </summary>
        /// <remarks>
        /// 示例:  
        /// DataSet ds = ExecuteDataset( trans , CommandType.StoredProcedure , "GetOrders" , new DbParameter( "@prodid" , 24 ) );
        /// </remarks>
        /// <param name="transaction">事务</param>
        /// <param name="commandType">命令类型 (存储过程,命令文本或其它)</param>
        /// <param name="commandText">存储过程名或SQL语句</param>
        /// <param name="commandParameters">SqlParamter参数数组</param>
        /// <returns>返回一个包含结果集的DataSet</returns>
        public DataSet ExecuteDataset(DbTransaction transaction, CommandType commandType, string commandText, params DbParameter[] commandParameters)
        {
            if (transaction == null)
            {
                throw new ArgumentNullException("transaction");
            }
            if ((transaction != null) && (transaction.Connection == null))
            {
                throw new ArgumentException("The transaction was rollbacked or commited, please provide an open transaction.", "transaction");
            }
            DbCommand command = this.Factory.CreateCommand();
            bool mustCloseConnection = false;
            this.PrepareCommand(command, transaction.Connection, transaction, commandType, commandText, commandParameters, out mustCloseConnection);
            using (DbDataAdapter adapter = this.Factory.CreateDataAdapter())
            {
                adapter.SelectCommand = command;
                DataSet dataSet = new DataSet();
                adapter.Fill(dataSet);
                command.Parameters.Clear();
                return dataSet;
            }
        }
        #endregion


        #region ExecuteNonQuery方法

        /// <summary>
        /// 执行不带参数的SQL语句,返回受影响的行数.
        /// </summary>
        /// <remarks>
        /// 示例:  
        /// int result = ExecuteNonQuery( "SELECT * FROM [table123]" );
        /// </remarks>
        /// <param name="commandText">SQL语句</param>
        /// <returns>返回受影响的行数</returns>
        public int ExecuteNonQuery(string commandText)
        {
            return this.ExecuteNonQuery(CommandType.Text, commandText, null);
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="commandType"></param>
        /// <param name="commandText"></param>
        /// <returns>返回受影响的行数</returns>
        public int ExecuteNonQuery(CommandType commandType, string commandText)
        {
            return this.ExecuteNonQuery(commandType, commandText, null);
        }

        /// <summary>
        /// 执行指定存储过程名称或SQL语句
        /// </summary>
        /// <param name="commandType">命令类型(存储过程,命令文本或其它.)</param>
        /// <param name="commandText">存储过程名称或SQL语句</param>
        /// <param name="commandParameters">SqlParamter参数数组</param>
        /// <returns>返回受影响的行数</returns>
        public int ExecuteNonQuery(CommandType commandType, string commandText, params DbParameter[] commandParameters)
        {
            if ((this.ConnectionString == null) || (this.ConnectionString.Length == 0))
            {
                throw new ArgumentNullException("ConnectionString");
            }
            using (DbConnection connection = this.Factory.CreateConnection())
            {
                connection.ConnectionString = this.ConnectionString;
                connection.Open();
                return this.ExecuteNonQuery(connection, commandType, commandText, commandParameters);
            }
        }


        /// <summary>
        /// 执行指定数据库连接对象的命令,将对象数组的值赋给存储过程参数
        /// </summary>
        /// <remarks>
        /// 此方法不提供访问存储过程输出参数和返回值
        /// </remarks>
        /// <param name="connection">一个有效的数据库连接对象</param>
        /// <param name="spName">存储过程名</param>
        /// <param name="parameterValues">分配给存储过程输入参数的对象数组</param>
        /// <returns>返回受影响的行数</returns>
        public int ExecuteNonQuery(DbConnection connection, string spName, params object[] parameterValues)
        {
            if (connection == null)
            {
                throw new ArgumentNullException("connection");
            }
            if ((spName == null) || (spName.Length == 0))
            {
                throw new ArgumentNullException("spName");
            }
            if ((parameterValues != null) && (parameterValues.Length > 0))
            {
                DbParameter[] spParameterSet = this.GetSpParameterSet(connection, spName);
                this.AssignParameterValues(spParameterSet, parameterValues);
                return this.ExecuteNonQuery(connection, CommandType.StoredProcedure, spName, spParameterSet);
            }
            return this.ExecuteNonQuery(connection, CommandType.StoredProcedure, spName);
        }
        /// <summary>
        /// 执行指定数据库连接对象的命令
        /// </summary>
        /// <param name="connection">一个有效的数据库连接对象</param>
        /// <param name="commandType">命令类型(存储过程,命令文本或其它.)</param>
        /// <param name="commandText">存储过程名称或SQL语句</param>
        /// <returns>返回受影响的行数</returns>
        public int ExecuteNonQuery(DbConnection connection, CommandType commandType, string commandText)
        {
            return this.ExecuteNonQuery(connection, commandType, commandText, null);
        }
        /// <summary>
        /// 执行指定数据库连接对象的命令
        /// </summary>
        /// <param name="connection">一个有效的数据库连接对象</param>
        /// <param name="commandType">命令类型(存储过程,命令文本或其它.)</param>
        /// <param name="commandText">存储过程名称或SQL语句</param>
        /// <param name="commandParameters">SqlParamter参数数组</param>
        /// <returns>返回受影响的行数</returns>
        public int ExecuteNonQuery(DbConnection connection, CommandType commandType, string commandText, params DbParameter[] commandParameters)
        {
            if (connection == null)
            {
                throw new ArgumentNullException("connection");
            }
            DbCommand command = this.Factory.CreateCommand();
            bool mustCloseConnection = false;
            this.PrepareCommand(command, connection, null, commandType, commandText, commandParameters, out mustCloseConnection);
            DateTime now = DateTime.Now;
            int num = command.ExecuteNonQuery();
            DateTime dtEnd = DateTime.Now;
            m_querydetail = GetQueryDetail(command.CommandText, now, dtEnd, commandParameters);
            this.m_querycount++;
            command.Parameters.Clear();
            if (mustCloseConnection)
            {
                connection.Close();
            }
            return num;
        }

        public int ExecuteNonQuery(out int id, CommandType commandType, string commandText)
        {
            return this.ExecuteNonQuery(out id, commandType, commandText, (DbParameter[])null);
        }
        public int ExecuteNonQuery(out int id, CommandType commandType, string commandText, params DbParameter[] commandParameters)
        {
            if ((this.ConnectionString == null) || (this.ConnectionString.Length == 0))
            {
                throw new ArgumentNullException("ConnectionString");
            }
            using (DbConnection connection = this.Factory.CreateConnection())
            {
                connection.ConnectionString = this.ConnectionString;
                connection.Open();
                return this.ExecuteNonQuery(out id, connection, commandType, commandText, commandParameters);
            }
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="id"></param>
        /// <param name="commandText"></param>
        /// <returns>返回受影响的行数</returns>
        public int ExecuteNonQuery(out int id, string commandText)
        {
            return this.ExecuteNonQuery(out id, CommandType.Text, commandText, (DbParameter[])null);
        }
        public int ExecuteNonQuery(out int id, DbConnection connection, CommandType commandType, string commandText)
        {
            return this.ExecuteNonQuery(out id, connection, commandType, commandText, null);
        }

        public int ExecuteNonQuery(out int id, DbTransaction transaction, CommandType commandType, string commandText)
        {
            return this.ExecuteNonQuery(out id, transaction, commandType, commandText, null);
        }

        public int ExecuteNonQuery(out int id, DbConnection connection, CommandType commandType, string commandText, params DbParameter[] commandParameters)
        {
            if (connection == null)
            {
                throw new ArgumentNullException("connection");
            }
            if (this.Provider.GetLastIdSql().Trim() == "")
            {
                throw new ArgumentNullException("GetLastIdSql is \"\"");
            }
            DbCommand command = this.Factory.CreateCommand();
            bool mustCloseConnection = false;
            this.PrepareCommand(command, connection, null, commandType, commandText, commandParameters, out mustCloseConnection);
            int num = command.ExecuteNonQuery();
            command.Parameters.Clear();
            command.CommandType = CommandType.Text;
            command.CommandText = this.Provider.GetLastIdSql();
            id = int.Parse(command.ExecuteScalar().ToString());
            DateTime now = DateTime.Now;
            id = int.Parse(command.ExecuteScalar().ToString());
            DateTime dtEnd = DateTime.Now;
            m_querydetail = GetQueryDetail(command.CommandText, now, dtEnd, commandParameters);
            this.m_querycount++;
            if (mustCloseConnection)
            {
                connection.Close();
            }
            return num;
        }

        public int ExecuteNonQuery(out int id, DbTransaction transaction, CommandType commandType, string commandText, params DbParameter[] commandParameters)
        {
            if (transaction == null)
            {
                throw new ArgumentNullException("transaction");
            }
            if ((transaction != null) && (transaction.Connection == null))
            {
                throw new ArgumentException("The transaction was rollbacked or commited, please provide an open transaction.", "transaction");
            }
            DbCommand command = this.Factory.CreateCommand();
            bool mustCloseConnection = false;
            this.PrepareCommand(command, transaction.Connection, transaction, commandType, commandText, commandParameters, out mustCloseConnection);
            int num = command.ExecuteNonQuery();
            command.Parameters.Clear();
            command.CommandType = CommandType.Text;
            command.CommandText = this.Provider.GetLastIdSql();
            id = int.Parse(command.ExecuteScalar().ToString());
            return num;
        }
        /// <summary>
        ///  执行指定连接数据库连接字符串的存储过程,使用DataRow做为参数值,返回受影响的行数.
        /// </summary>
        /// <param name="spName">存储过程名称</param>
        /// <param name="dataRow">使用DataRow作为参数值</param>
        /// <returns>返回受影响的行数</returns>
        public int ExecuteNonQueryTypedParams(string spName, DataRow dataRow)
        {
            if ((this.ConnectionString == null) || (this.ConnectionString.Length == 0))
            {
                throw new ArgumentNullException("ConnectionString");
            }
            if ((spName == null) || (spName.Length == 0))
            {
                throw new ArgumentNullException("spName");
            }
            if ((dataRow != null) && (dataRow.ItemArray.Length > 0))
            {
                DbParameter[] spParameterSet = this.GetSpParameterSet(spName);
                this.AssignParameterValues(spParameterSet, dataRow);
                return this.ExecuteNonQuery(CommandType.StoredProcedure, spName, spParameterSet);
            }
            return this.ExecuteNonQuery(CommandType.StoredProcedure, spName);
        }
        /// <summary>
        /// 执行指定连接数据库连接对象的存储过程,使用DataRow做为参数值,返回受影响的行数.
        /// </summary>
        /// <param name="connection"></param>
        /// <param name="spName"></param>
        /// <param name="dataRow"></param>
        /// <returns></returns>
        public int ExecuteNonQueryTypedParams(DbConnection connection, string spName, DataRow dataRow)
        {
            if (connection == null)
            {
                throw new ArgumentNullException("connection");
            }
            if ((spName == null) || (spName.Length == 0))
            {
                throw new ArgumentNullException("spName");
            }
            if ((dataRow != null) && (dataRow.ItemArray.Length > 0))
            {
                DbParameter[] spParameterSet = this.GetSpParameterSet(connection, spName);
                this.AssignParameterValues(spParameterSet, dataRow);
                return this.ExecuteNonQuery(connection, CommandType.StoredProcedure, spName, spParameterSet);
            }
            return this.ExecuteNonQuery(connection, CommandType.StoredProcedure, spName);
        }

        public int ExecuteNonQueryTypedParams(DbTransaction transaction, string spName, DataRow dataRow)
        {
            if (transaction == null)
            {
                throw new ArgumentNullException("transaction");
            }
            if ((transaction != null) && (transaction.Connection == null))
            {
                throw new ArgumentException("The transaction was rollbacked or commited, please provide an open transaction.", "transaction");
            }
            if ((spName == null) || (spName.Length == 0))
            {
                throw new ArgumentNullException("spName");
            }
            if ((dataRow != null) && (dataRow.ItemArray.Length > 0))
            {
                DbParameter[] spParameterSet = this.GetSpParameterSet(transaction.Connection, spName);
                this.AssignParameterValues(spParameterSet, dataRow);
                return this.ExecuteNonQuery(transaction, CommandType.StoredProcedure, spName, spParameterSet);
            }
            return this.ExecuteNonQuery(transaction, CommandType.StoredProcedure, spName);
        }

        public int ExecuteNonQuery(DbTransaction transaction, CommandType commandType, string commandText)
        {
            return this.ExecuteNonQuery(transaction, commandType, commandText, null);
        }

        public int ExecuteNonQuery(DbTransaction transaction, string spName, params object[] parameterValues)
        {
            if (transaction == null)
            {
                throw new ArgumentNullException("transaction");
            }
            if ((transaction != null) && (transaction.Connection == null))
            {
                throw new ArgumentException("The transaction was rollbacked or commited, please provide an open transaction.", "transaction");
            }
            if ((spName == null) || (spName.Length == 0))
            {
                throw new ArgumentNullException("spName");
            }
            if ((parameterValues != null) && (parameterValues.Length > 0))
            {
                DbParameter[] spParameterSet = this.GetSpParameterSet(transaction.Connection, spName);
                this.AssignParameterValues(spParameterSet, parameterValues);
                return this.ExecuteNonQuery(transaction, CommandType.StoredProcedure, spName, spParameterSet);
            }
            return this.ExecuteNonQuery(transaction, CommandType.StoredProcedure, spName);
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="transaction"></param>
        /// <param name="commandType"></param>
        /// <param name="commandText"></param>
        /// <param name="commandParameters"></param>
        /// <returns>返回命令影响的行数</returns>
        public int ExecuteNonQuery(DbTransaction transaction, CommandType commandType, string commandText, params DbParameter[] commandParameters)
        {
            if (transaction == null)
            {
                throw new ArgumentNullException("transaction");
            }
            if ((transaction != null) && (transaction.Connection == null))
            {
                throw new ArgumentException("The transaction was rollbacked or commited, please provide an open transaction.", "transaction");
            }
            DbCommand command = this.Factory.CreateCommand();
            bool mustCloseConnection = false;
            this.PrepareCommand(command, transaction.Connection, transaction, commandType, commandText, commandParameters, out mustCloseConnection);
            int num = command.ExecuteNonQuery();
            command.Parameters.Clear();
            return num;
        }

        #endregion


        #region 获取object对象方法
        public T ExecuteObject<T>(string commandText)
        {
            DataSet set = this.ExecuteDataset(commandText);
            if (Validate.CheckedDataSet(set))
            {
                return DataHelper.ConvertRowToObject<T>(set.Tables[0].Rows[0]);
            }
            return default(T);
        }

        public T ExecuteObjectTrans<T>(string commandText)
        {
            DataSet set = this.ExecDataSetTrans(commandText);
            if (Validate.CheckedDataSet(set))
            {
                return DataHelper.ConvertRowToObject<T>(set.Tables[0].Rows[0]);
            }
            return default(T);
        }

        public T ExecuteObject<T>(string commandText, List<DbParameter> prams)
        {
            DataSet set = this.ExecuteDataset(CommandType.Text, commandText, prams.ToArray());
            if (Validate.CheckedDataSet(set))
            {
                return DataHelper.ConvertRowToObject<T>(set.Tables[0].Rows[0]);
            }
            return default(T);
        }

        public IList<T> ExecuteObjectList<T>(string commandText)
        {
            DataSet set = this.ExecuteDataset(commandText);
            if (Validate.CheckedDataSet(set))
            {
                return DataHelper.ConvertDataTableToObjects<T>(set.Tables[0]);
            }
            return null;
        }

        public IList<T> ExecuteObjectList<T>(string commandText, List<DbParameter> prams)
        {
            DataSet set = this.ExecuteDataset(CommandType.Text, commandText, prams.ToArray());
            if (Validate.CheckedDataSet(set))
            {
                return DataHelper.ConvertDataTableToObjects<T>(set.Tables[0]);
            }
            return null;
        }
        #endregion


        #region 获取DbDataReader对象方法
        public DbDataReader ExecuteReader(CommandType commandType, string commandText)
        {
            return this.ExecuteReader(commandType, commandText, null);
        }

        public DbDataReader ExecuteReader(string spName, params object[] parameterValues)
        {
            if ((this.ConnectionString == null) || (this.ConnectionString.Length == 0))
            {
                throw new ArgumentNullException("ConnectionString");
            }
            if ((spName == null) || (spName.Length == 0))
            {
                throw new ArgumentNullException("spName");
            }
            if ((parameterValues != null) && (parameterValues.Length > 0))
            {
                DbParameter[] spParameterSet = this.GetSpParameterSet(spName);
                this.AssignParameterValues(spParameterSet, parameterValues);
                return this.ExecuteReader(this.ConnectionString, new object[] { CommandType.StoredProcedure, spName, spParameterSet });
            }
            return this.ExecuteReader(this.ConnectionString, new object[] { CommandType.StoredProcedure, spName });
        }

        public DbDataReader ExecuteReader(CommandType commandType, string commandText, params DbParameter[] commandParameters)
        {
            DbDataReader reader;
            if ((this.ConnectionString == null) || (this.ConnectionString.Length == 0))
            {
                throw new ArgumentNullException("ConnectionString");
            }
            DbConnection connection = null;
            try
            {
                connection = this.Factory.CreateConnection();
                connection.ConnectionString = this.ConnectionString;
                connection.Open();
                reader = this.ExecuteReader(connection, null, commandType, commandText, commandParameters, DbConnectionOwnership.Internal);
            }
            catch
            {
                if (connection != null)
                {
                    connection.Close();
                }
                throw;
            }
            return reader;
        }

        public DbDataReader ExecuteReader(DbConnection connection, CommandType commandType, string commandText)
        {
            return this.ExecuteReader(connection, commandType, commandText, null);
        }

        public DbDataReader ExecuteReader(DbConnection connection, string spName, params object[] parameterValues)
        {
            if (connection == null)
            {
                throw new ArgumentNullException("connection");
            }
            if ((spName == null) || (spName.Length == 0))
            {
                throw new ArgumentNullException("spName");
            }
            if ((parameterValues != null) && (parameterValues.Length > 0))
            {
                DbParameter[] spParameterSet = this.GetSpParameterSet(connection, spName);
                this.AssignParameterValues(spParameterSet, parameterValues);
                return this.ExecuteReader(connection, CommandType.StoredProcedure, spName, spParameterSet);
            }
            return this.ExecuteReader(connection, CommandType.StoredProcedure, spName);
        }

        public DbDataReader ExecuteReader(DbTransaction transaction, CommandType commandType, string commandText)
        {
            return this.ExecuteReader(transaction, commandType, commandText, null);
        }

        public DbDataReader ExecuteReader(DbTransaction transaction, string spName, params object[] parameterValues)
        {
            if (transaction == null)
            {
                throw new ArgumentNullException("transaction");
            }
            if ((transaction != null) && (transaction.Connection == null))
            {
                throw new ArgumentException("The transaction was rollbacked or commited, please provide an open transaction.", "transaction");
            }
            if ((spName == null) || (spName.Length == 0))
            {
                throw new ArgumentNullException("spName");
            }
            if ((parameterValues != null) && (parameterValues.Length > 0))
            {
                DbParameter[] spParameterSet = this.GetSpParameterSet(transaction.Connection, spName);
                this.AssignParameterValues(spParameterSet, parameterValues);
                return this.ExecuteReader(transaction, CommandType.StoredProcedure, spName, spParameterSet);
            }
            return this.ExecuteReader(transaction, CommandType.StoredProcedure, spName);
        }

        public DbDataReader ExecuteReader(DbConnection connection, CommandType commandType, string commandText, params DbParameter[] commandParameters)
        {
            return this.ExecuteReader(connection, null, commandType, commandText, commandParameters, DbConnectionOwnership.External);
        }

        public DbDataReader ExecuteReader(DbTransaction transaction, CommandType commandType, string commandText, params DbParameter[] commandParameters)
        {
            if (transaction == null)
            {
                throw new ArgumentNullException("transaction");
            }
            if ((transaction != null) && (transaction.Connection == null))
            {
                throw new ArgumentException("The transaction was rollbacked or commited, please provide an open transaction.", "transaction");
            }
            return this.ExecuteReader(transaction.Connection, transaction, commandType, commandText, commandParameters, DbConnectionOwnership.External);
        }

        private DbDataReader ExecuteReader(DbConnection connection, DbTransaction transaction, CommandType commandType, string commandText, DbParameter[] commandParameters, DbConnectionOwnership connectionOwnership)
        {
            DbDataReader reader2;
            if (connection == null)
            {
                throw new ArgumentNullException("connection");
            }
            bool mustCloseConnection = false;
            DbCommand command = this.Factory.CreateCommand();
            try
            {
                DbDataReader reader;
                this.PrepareCommand(command, connection, transaction, commandType, commandText, commandParameters, out mustCloseConnection);
                DateTime now = DateTime.Now;
                if (connectionOwnership == DbConnectionOwnership.External)
                {
                    reader = command.ExecuteReader();
                }
                else
                {
                    reader = command.ExecuteReader(CommandBehavior.CloseConnection);
                }
                DateTime dtEnd = DateTime.Now;
                m_querydetail = GetQueryDetail(command.CommandText, now, dtEnd, commandParameters);
                this.m_querycount++;
                bool flag2 = true;
                foreach (DbParameter parameter in command.Parameters)
                {
                    if (parameter.Direction != ParameterDirection.Input)
                    {
                        flag2 = false;
                    }
                }
                if (flag2)
                {
                    command.Parameters.Clear();
                }
                reader2 = reader;
            }
            catch
            {
                if (mustCloseConnection)
                {
                    connection.Close();
                }
                throw;
            }
            return reader2;
        }

        public DbDataReader ExecuteReaderTypedParams(string spName, DataRow dataRow)
        {
            if ((this.ConnectionString == null) || (this.ConnectionString.Length == 0))
            {
                throw new ArgumentNullException("ConnectionString");
            }
            if ((spName == null) || (spName.Length == 0))
            {
                throw new ArgumentNullException("spName");
            }
            if ((dataRow != null) && (dataRow.ItemArray.Length > 0))
            {
                DbParameter[] spParameterSet = this.GetSpParameterSet(spName);
                this.AssignParameterValues(spParameterSet, dataRow);
                return this.ExecuteReader(this.ConnectionString, new object[] { CommandType.StoredProcedure, spName, spParameterSet });
            }
            return this.ExecuteReader(this.ConnectionString, new object[] { CommandType.StoredProcedure, spName });
        }

        public DbDataReader ExecuteReaderTypedParams(DbConnection connection, string spName, DataRow dataRow)
        {
            if (connection == null)
            {
                throw new ArgumentNullException("connection");
            }
            if ((spName == null) || (spName.Length == 0))
            {
                throw new ArgumentNullException("spName");
            }
            if ((dataRow != null) && (dataRow.ItemArray.Length > 0))
            {
                DbParameter[] spParameterSet = this.GetSpParameterSet(connection, spName);
                this.AssignParameterValues(spParameterSet, dataRow);
                return this.ExecuteReader(connection, CommandType.StoredProcedure, spName, spParameterSet);
            }
            return this.ExecuteReader(connection, CommandType.StoredProcedure, spName);
        }

        public DbDataReader ExecuteReaderTypedParams(DbTransaction transaction, string spName, DataRow dataRow)
        {
            if (transaction == null)
            {
                throw new ArgumentNullException("transaction");
            }
            if ((transaction != null) && (transaction.Connection == null))
            {
                throw new ArgumentException("The transaction was rollbacked or commited, please provide an open transaction.", "transaction");
            }
            if ((spName == null) || (spName.Length == 0))
            {
                throw new ArgumentNullException("spName");
            }
            if ((dataRow != null) && (dataRow.ItemArray.Length > 0))
            {
                DbParameter[] spParameterSet = this.GetSpParameterSet(transaction.Connection, spName);
                this.AssignParameterValues(spParameterSet, dataRow);
                return this.ExecuteReader(transaction, CommandType.StoredProcedure, spName, spParameterSet);
            }
            return this.ExecuteReader(transaction, CommandType.StoredProcedure, spName);
        }
        #endregion


        #region Scalar方法
        /// <summary>
        /// 执行查询，并返回查询所返回的结果集中第一行的第一列。所有其他的列和行将被忽略，是一个对象，可能为null
        /// </summary>
        /// <param name="commandType"></param>
        /// <param name="commandText"></param>
        /// <returns></returns>
        public object ExecuteScalar(CommandType commandType, string commandText)
        {
            return this.ExecuteScalar(commandType, commandText, null);
        }

        public object ExecuteScalar(CommandType commandType, string commandText, params DbParameter[] commandParameters)
        {
            if ((this.ConnectionString == null) || (this.ConnectionString.Length == 0))
            {
                throw new ArgumentNullException("ConnectionString");
            }
            using (DbConnection connection = this.Factory.CreateConnection())
            {
                connection.ConnectionString = this.ConnectionString;
                connection.Open();
                return this.ExecuteScalar(connection, commandType, commandText, commandParameters);
            }
        }

        public object ExecuteScalar(DbConnection connection, CommandType commandType, string commandText)
        {
            return this.ExecuteScalar(connection, commandType, commandText, null);
        }

        public object ExecuteScalar(DbConnection connection, string spName, params object[] parameterValues)
        {
            if (connection == null)
            {
                throw new ArgumentNullException("connection");
            }
            if ((spName == null) || (spName.Length == 0))
            {
                throw new ArgumentNullException("spName");
            }
            if ((parameterValues != null) && (parameterValues.Length > 0))
            {
                DbParameter[] spParameterSet = this.GetSpParameterSet(connection, spName);
                this.AssignParameterValues(spParameterSet, parameterValues);
                return this.ExecuteScalar(connection, CommandType.StoredProcedure, spName, spParameterSet);
            }
            return this.ExecuteScalar(connection, CommandType.StoredProcedure, spName);
        }

        public object ExecuteScalar(DbTransaction transaction, CommandType commandType, string commandText)
        {
            return this.ExecuteScalar(transaction, commandType, commandText, null);
        }

        public object ExecuteScalar(DbTransaction transaction, string spName, params object[] parameterValues)
        {
            if (transaction == null)
            {
                throw new ArgumentNullException("transaction");
            }
            if ((transaction != null) && (transaction.Connection == null))
            {
                throw new ArgumentException("The transaction was rollbacked or commited, please provide an open transaction.", "transaction");
            }
            if ((spName == null) || (spName.Length == 0))
            {
                throw new ArgumentNullException("spName");
            }
            if ((parameterValues != null) && (parameterValues.Length > 0))
            {
                DbParameter[] spParameterSet = this.GetSpParameterSet(transaction.Connection, spName);
                this.AssignParameterValues(spParameterSet, parameterValues);
                return this.ExecuteScalar(transaction, CommandType.StoredProcedure, spName, spParameterSet);
            }
            return this.ExecuteScalar(transaction, CommandType.StoredProcedure, spName);
        }

        public object ExecuteScalar(DbConnection connection, CommandType commandType, string commandText, params DbParameter[] commandParameters)
        {
            if (connection == null)
            {
                throw new ArgumentNullException("connection");
            }
            DbCommand command = this.Factory.CreateCommand();
            bool mustCloseConnection = false;
            this.PrepareCommand(command, connection, null, commandType, commandText, commandParameters, out mustCloseConnection);
            object obj2 = command.ExecuteScalar();
            command.Parameters.Clear();
            if (mustCloseConnection)
            {
                connection.Close();
            }
            return obj2;
        }

        public object ExecuteScalar(DbTransaction transaction, CommandType commandType, string commandText, params DbParameter[] commandParameters)
        {
            if (transaction == null)
            {
                throw new ArgumentNullException("transaction");
            }
            if ((transaction != null) && (transaction.Connection == null))
            {
                throw new ArgumentException("The transaction was rollbacked or commited, please provide an open transaction.", "transaction");
            }
            DbCommand command = this.Factory.CreateCommand();
            bool mustCloseConnection = false;
            this.PrepareCommand(command, transaction.Connection, transaction, commandType, commandText, commandParameters, out mustCloseConnection);
            DateTime now = DateTime.Now;
            object obj2 = command.ExecuteScalar();
            DateTime dtEnd = DateTime.Now;
            m_querydetail = GetQueryDetail(command.CommandText, now, dtEnd, commandParameters);
            this.m_querycount++;
            command.Parameters.Clear();
            return obj2;
        }

        /// <summary>
        /// 执行查询，并返回查询所返回的结果集中第一行的第一列。所有其他的列和行将被忽略,如果为null，则返回空字符串
        /// </summary>
        /// <param name="commandType"></param>
        /// <param name="commandText"></param>
        /// <returns></returns>
        public string ExecuteScalarToStr(CommandType commandType, string commandText)
        {
            object obj2 = this.ExecuteScalar(commandType, commandText);
            if (obj2 == null)
            {
                return "";
            }
            return obj2.ToString();
        }

        public string ExecuteScalarToStr(CommandType commandType, string commandText, params DbParameter[] commandParameters)
        {
            object obj2 = this.ExecuteScalar(commandType, commandText, commandParameters);
            if (obj2 == null)
            {
                return "";
            }
            return obj2.ToString();
        }

        public object ExecuteScalarTypedParams(string spName, DataRow dataRow)
        {
            if ((this.ConnectionString == null) || (this.ConnectionString.Length == 0))
            {
                throw new ArgumentNullException("ConnectionString");
            }
            if ((spName == null) || (spName.Length == 0))
            {
                throw new ArgumentNullException("spName");
            }
            if ((dataRow != null) && (dataRow.ItemArray.Length > 0))
            {
                DbParameter[] spParameterSet = this.GetSpParameterSet(spName);
                this.AssignParameterValues(spParameterSet, dataRow);
                return this.ExecuteScalar(CommandType.StoredProcedure, spName, spParameterSet);
            }
            return this.ExecuteScalar(CommandType.StoredProcedure, spName);
        }

        public object ExecuteScalarTypedParams(DbConnection connection, string spName, DataRow dataRow)
        {
            if (connection == null)
            {
                throw new ArgumentNullException("connection");
            }
            if ((spName == null) || (spName.Length == 0))
            {
                throw new ArgumentNullException("spName");
            }
            if ((dataRow != null) && (dataRow.ItemArray.Length > 0))
            {
                DbParameter[] spParameterSet = this.GetSpParameterSet(connection, spName);
                this.AssignParameterValues(spParameterSet, dataRow);
                return this.ExecuteScalar(connection, CommandType.StoredProcedure, spName, spParameterSet);
            }
            return this.ExecuteScalar(connection, CommandType.StoredProcedure, spName);
        }

        public object ExecuteScalarTypedParams(DbTransaction transaction, string spName, DataRow dataRow)
        {
            if (transaction == null)
            {
                throw new ArgumentNullException("transaction");
            }
            if ((transaction != null) && (transaction.Connection == null))
            {
                throw new ArgumentException("The transaction was rollbacked or commited, please provide an open transaction.", "transaction");
            }
            if ((spName == null) || (spName.Length == 0))
            {
                throw new ArgumentNullException("spName");
            }
            if ((dataRow != null) && (dataRow.ItemArray.Length > 0))
            {
                DbParameter[] spParameterSet = this.GetSpParameterSet(transaction.Connection, spName);
                this.AssignParameterValues(spParameterSet, dataRow);
                return this.ExecuteScalar(transaction, CommandType.StoredProcedure, spName, spParameterSet);
            }
            return this.ExecuteScalar(transaction, CommandType.StoredProcedure, spName);
        }
        #endregion


        #region  FillDataset方法
        public void FillDataset(CommandType commandType, string commandText, DataSet dataSet, string[] tableNames)
        {
            if ((this.ConnectionString == null) || (this.ConnectionString.Length == 0))
            {
                throw new ArgumentNullException("ConnectionString");
            }
            if (dataSet == null)
            {
                throw new ArgumentNullException("dataSet");
            }
            using (DbConnection connection = this.Factory.CreateConnection())
            {
                connection.ConnectionString = this.ConnectionString;
                connection.Open();
                this.FillDataset(connection, commandType, commandText, dataSet, tableNames);
            }
        }

        public void FillDataset(string spName, DataSet dataSet, string[] tableNames, params object[] parameterValues)
        {
            if ((this.ConnectionString == null) || (this.ConnectionString.Length == 0))
            {
                throw new ArgumentNullException("ConnectionString");
            }
            if (dataSet == null)
            {
                throw new ArgumentNullException("dataSet");
            }
            using (DbConnection connection = this.Factory.CreateConnection())
            {
                connection.ConnectionString = this.ConnectionString;
                connection.Open();
                this.FillDataset(connection, spName, dataSet, tableNames, parameterValues);
            }
        }

        public void FillDataset(CommandType commandType, string commandText, DataSet dataSet, string[] tableNames, params DbParameter[] commandParameters)
        {
            if ((this.ConnectionString == null) || (this.ConnectionString.Length == 0))
            {
                throw new ArgumentNullException("ConnectionString");
            }
            if (dataSet == null)
            {
                throw new ArgumentNullException("dataSet");
            }
            using (DbConnection connection = this.Factory.CreateConnection())
            {
                connection.ConnectionString = this.ConnectionString;
                connection.Open();
                this.FillDataset(connection, commandType, commandText, dataSet, tableNames, commandParameters);
            }
        }

        public void FillDataset(DbConnection connection, CommandType commandType, string commandText, DataSet dataSet, string[] tableNames)
        {
            this.FillDataset(connection, commandType, commandText, dataSet, tableNames, null);
        }

        public void FillDataset(DbConnection connection, string spName, DataSet dataSet, string[] tableNames, params object[] parameterValues)
        {
            if (connection == null)
            {
                throw new ArgumentNullException("connection");
            }
            if (dataSet == null)
            {
                throw new ArgumentNullException("dataSet");
            }
            if ((spName == null) || (spName.Length == 0))
            {
                throw new ArgumentNullException("spName");
            }
            if ((parameterValues != null) && (parameterValues.Length > 0))
            {
                DbParameter[] spParameterSet = this.GetSpParameterSet(connection, spName);
                this.AssignParameterValues(spParameterSet, parameterValues);
                this.FillDataset(connection, CommandType.StoredProcedure, spName, dataSet, tableNames, spParameterSet);
            }
            else
            {
                this.FillDataset(connection, CommandType.StoredProcedure, spName, dataSet, tableNames);
            }
        }

        public void FillDataset(DbTransaction transaction, CommandType commandType, string commandText, DataSet dataSet, string[] tableNames)
        {
            this.FillDataset(transaction, commandType, commandText, dataSet, tableNames, null);
        }

        public void FillDataset(DbTransaction transaction, string spName, DataSet dataSet, string[] tableNames, params object[] parameterValues)
        {
            if (transaction == null)
            {
                throw new ArgumentNullException("transaction");
            }
            if ((transaction != null) && (transaction.Connection == null))
            {
                throw new ArgumentException("The transaction was rollbacked or commited, please provide an open transaction.", "transaction");
            }
            if (dataSet == null)
            {
                throw new ArgumentNullException("dataSet");
            }
            if ((spName == null) || (spName.Length == 0))
            {
                throw new ArgumentNullException("spName");
            }
            if ((parameterValues != null) && (parameterValues.Length > 0))
            {
                DbParameter[] spParameterSet = this.GetSpParameterSet(transaction.Connection, spName);
                this.AssignParameterValues(spParameterSet, parameterValues);
                this.FillDataset(transaction, CommandType.StoredProcedure, spName, dataSet, tableNames, spParameterSet);
            }
            else
            {
                this.FillDataset(transaction, CommandType.StoredProcedure, spName, dataSet, tableNames);
            }
        }

        public void FillDataset(DbConnection connection, CommandType commandType, string commandText, DataSet dataSet, string[] tableNames, params DbParameter[] commandParameters)
        {
            this.FillDataset(connection, null, commandType, commandText, dataSet, tableNames, commandParameters);
        }

        public void FillDataset(DbTransaction transaction, CommandType commandType, string commandText, DataSet dataSet, string[] tableNames, params DbParameter[] commandParameters)
        {
            this.FillDataset(transaction.Connection, transaction, commandType, commandText, dataSet, tableNames, commandParameters);
        }

        private void FillDataset(DbConnection connection, DbTransaction transaction, CommandType commandType, string commandText, DataSet dataSet, string[] tableNames, params DbParameter[] commandParameters)
        {
            if (connection == null)
            {
                throw new ArgumentNullException("connection");
            }
            if (dataSet == null)
            {
                throw new ArgumentNullException("dataSet");
            }
            DbCommand command = this.Factory.CreateCommand();
            bool mustCloseConnection = false;
            this.PrepareCommand(command, connection, transaction, commandType, commandText, commandParameters, out mustCloseConnection);
            using (DbDataAdapter adapter = this.Factory.CreateDataAdapter())
            {
                adapter.SelectCommand = command;
                if ((tableNames != null) && (tableNames.Length > 0))
                {
                    string sourceTable = "Table";
                    for (int i = 0; i < tableNames.Length; i++)
                    {
                        if ((tableNames[i] == null) || (tableNames[i].Length == 0))
                        {
                            throw new ArgumentException("The tableNames parameter must contain a list of tables, a value was provided as null or empty string.", "tableNames");
                        }
                        adapter.TableMappings.Add(sourceTable, tableNames[i]);
                        sourceTable = sourceTable + ((i + 1)).ToString();
                    }
                }
                adapter.Fill(dataSet);
                command.Parameters.Clear();
            }
            if (mustCloseConnection)
            {
                connection.Close();
            }
        }
        #endregion


        #region DbParameter方法
        public DbParameter[] GetCachedParameterSet(string commandText)
        {
            if ((this.ConnectionString == null) || (this.ConnectionString.Length == 0))
            {
                throw new ArgumentNullException("ConnectionString");
            }
            if ((commandText == null) || (commandText.Length == 0))
            {
                throw new ArgumentNullException("commandText");
            }
            string str = this.ConnectionString + ":" + commandText;
            DbParameter[] originalParameters = this.m_paramcache[str] as DbParameter[];
            if (originalParameters == null)
            {
                return null;
            }
            return this.CloneParameters(originalParameters);
        }
        /// <summary>
        /// 返回指定的存储过程的参数集
        /// </summary>
        /// <param name="spName">存储过程名</param>
        /// <returns>返回DbParameter参数数组</returns>
        public DbParameter[] GetSpParameterSet(string spName)
        {
            return this.GetSpParameterSet(spName, false);
        }
        /// <summary>
        /// 返回指定的存储过程的参数集
        /// </summary>
        /// <param name="spName">存储过程名</param>
        /// <param name="includeReturnValueParameter">是否包含返回值参数</param>
        /// <returns>返回DbParameter参数数组</returns>
        public DbParameter[] GetSpParameterSet(string spName, bool includeReturnValueParameter)
        {
            if ((this.ConnectionString == null) || (this.ConnectionString.Length == 0))
            {
                throw new ArgumentNullException("ConnectionString");
            }
            if ((spName == null) || (spName.Length == 0))
            {
                throw new ArgumentNullException("spName");
            }
            using (DbConnection connection = this.Factory.CreateConnection())
            {
                connection.ConnectionString = this.ConnectionString;
                return this.GetSpParameterSetInternal(connection, spName, includeReturnValueParameter);
            }
        }

        /// <summary>
        /// [内部]返回指定的存储过程的参数集(使用连接对象).
        /// </summary>
        /// <param name="connection">一个有效的数据库连接对象</param>
        /// <param name="spName">存储过程名</param>
        /// <returns>返回DbParameter参数数组</returns>
        internal DbParameter[] GetSpParameterSet(DbConnection connection, string spName)
        {
            return this.GetSpParameterSet(connection, spName, false);
        }
        /// <summary>
        /// [内部]返回指定的存储过程的参数集(使用连接对象).
        /// </summary>
        /// <param name="connection">一个有效的数据库连接对象</param>
        /// <param name="spName">存储过程名</param>
        /// <param name="includeReturnValueParameter">是否包含返回值参数</param>
        /// <returns>返回DbParameter参数数组</returns>
        internal DbParameter[] GetSpParameterSet(DbConnection connection, string spName, bool includeReturnValueParameter)
        {
            if (connection == null)
            {
                throw new ArgumentNullException("connection");
            }
            using (DbConnection connection2 = (DbConnection)((ICloneable)connection).Clone())
            {
                return this.GetSpParameterSetInternal(connection2, spName, includeReturnValueParameter);
            }
        }
        /// <summary>
        /// [私有]返回指定的存储过程的参数集(使用连接对象)
        /// </summary>
        /// <param name="connection">一个有效的数据库连接对象</param>
        /// <param name="spName">存储过程名</param>
        /// <param name="includeReturnValueParameter">是否包含返回值参数</param>
        /// <returns>返回DbParameter参数数组</returns>
        private DbParameter[] GetSpParameterSetInternal(DbConnection connection, string spName, bool includeReturnValueParameter)
        {
            if (connection == null)
            {
                throw new ArgumentNullException("connection");
            }
            if ((spName == null) || (spName.Length == 0))
            {
                throw new ArgumentNullException("spName");
            }
            string str = connection.ConnectionString + ":" + spName + (includeReturnValueParameter ? ":include ReturnValue Parameter" : "");
            DbParameter[] originalParameters = this.m_paramcache[str] as DbParameter[];
            if (originalParameters == null)
            {
                DbParameter[] parameterArray2 = this.DiscoverSpParameterSet(connection, spName, includeReturnValueParameter);
                this.m_paramcache[str] = parameterArray2;
                originalParameters = parameterArray2;
            }
            return this.CloneParameters(originalParameters);
        }

        public DbParameter MakeInParam(string paraName, object paraValue)
        {
            return this.MakeParam(paraName, paraValue, ParameterDirection.Input);
        }

        public DbParameter MakeOutParam(string paraName, Type paraType)
        {
            return this.MakeParam(paraName, null, ParameterDirection.Output, paraType, "");
        }

        public DbParameter MakeOutParam(string paraName, Type paraType, int size)
        {
            return this.MakeParam(paraName, null, ParameterDirection.Output, paraType, "", size);
        }

        public DbParameter MakeOutParam(string paraName, object paraValue, Type paraType, int size)
        {
            return this.MakeParam(paraName, paraValue, ParameterDirection.Output, paraType, "", size);
        }

        public DbParameter MakeParam(string paraName, object paraValue, ParameterDirection direction)
        {
            return this.Provider.MakeParam(paraName, paraValue, direction);
        }

        public DbParameter MakeParam(string paraName, object paraValue, ParameterDirection direction, Type paraType, string sourceColumn)
        {
            return this.Provider.MakeParam(paraName, paraValue, direction, paraType, sourceColumn);
        }

        public DbParameter MakeParam(string paraName, object paraValue, ParameterDirection direction, Type paraType, string sourceColumn, int size)
        {
            return this.Provider.MakeParam(paraName, paraValue, direction, paraType, sourceColumn, size);
        }

        public DbParameter MakeReturnParam()
        {
            return this.MakeReturnParam("ReturnValue");
        }

        public DbParameter MakeReturnParam(string paraName)
        {
            return this.MakeParam(paraName, 0, ParameterDirection.ReturnValue);
        }
        #endregion


        #region 执行存储过程方法
        public int RunProc(string procName)
        {
            return this.ExecuteNonQuery(CommandType.StoredProcedure, procName, null);
        }

        public void RunProc(string procName, out DbDataReader reader)
        {
            reader = this.ExecuteReader(CommandType.StoredProcedure, procName, null);
        }

        public void RunProc(string procName, out DataSet ds)
        {
            ds = this.ExecuteDataset(CommandType.StoredProcedure, procName, null);
        }

        public void RunProc(string procName, out object obj)
        {
            obj = this.ExecuteScalar(CommandType.StoredProcedure, procName, null);
        }

        public int RunProc(string procName, List<DbParameter> prams)
        {
            prams.Add(this.MakeReturnParam());
            return this.ExecuteNonQuery(CommandType.StoredProcedure, procName, prams.ToArray());
        }

        public void RunProc(string procName, List<DbParameter> prams, out DbDataReader reader)
        {
            prams.Add(this.MakeReturnParam());
            reader = this.ExecuteReader(CommandType.StoredProcedure, procName, prams.ToArray());
        }

        public void RunProc(string procName, List<DbParameter> prams, out DataSet ds)
        {
            prams.Add(this.MakeReturnParam());
            ds = this.ExecuteDataset(CommandType.StoredProcedure, procName, prams.ToArray());
        }

        public void RunProc(string procName, List<DbParameter> prams, out object obj)
        {
            prams.Add(this.MakeReturnParam());
            obj = this.ExecuteScalar(CommandType.StoredProcedure, procName, prams.ToArray());
        }

        public T RunProcObject<T>(string procName)
        {
            DataSet ds = null;
            this.RunProc(procName, out ds);
            if (Validate.CheckedDataSet(ds))
            {
                return DataHelper.ConvertRowToObject<T>(ds.Tables[0].Rows[0]);
            }
            return default(T);
        }

        /// <summary>
        /// 运行存储过程，返回对象
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="procName"></param>
        /// <param name="prams"></param>
        /// <returns></returns>
        public T RunProcObject<T>(string procName, List<DbParameter> prams)
        {
            DataSet ds = null;
            this.RunProc(procName, prams, out ds);
            if (Validate.CheckedDataSet(ds))
            {
                return DataHelper.ConvertRowToObject<T>(ds.Tables[0].Rows[0]);
            }
            return default(T);
        }

        /// <summary>
        /// 事务运行存储过程，返回对象
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="procName"></param>
        /// <param name="prams"></param>
        /// <returns></returns>
        public T RunProcObjectTrans<T>(string procName, List<DbParameter> prams)
        {
            DataSet ds = null;
            ds = RunDataSetTrans(procName, prams.ToArray());
            if (Validate.CheckedDataSet(ds))
            {
                return DataHelper.ConvertRowToObject<T>(ds.Tables[0].Rows[0]);
            }
            return default(T);
        }


        /// <summary>
        /// 运行存储过程返回列表
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="procName"></param>
        /// <returns></returns>
        public IList<T> RunProcObjectList<T>(string procName)
        {
            DataSet ds = null;
            this.RunProc(procName, out ds);
            if (Validate.CheckedDataSet(ds))
            {
                return DataHelper.ConvertDataTableToObjects<T>(ds.Tables[0]);
            }
            return null;
        }
        /// <summary>
        /// 运行存储过程返回列表
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="procName"></param>
        /// <param name="prams"></param>
        /// <returns></returns>
        public IList<T> RunProcObjectList<T>(string procName, List<DbParameter> prams)
        {
            DataSet ds = null;
            this.RunProc(procName, prams, out ds);
            if (Validate.CheckedDataSet(ds))
            {
                return DataHelper.ConvertDataTableToObjects<T>(ds.Tables[0]);
            }
            return null;
        }
        #endregion

        #region 基本的增删改查操作

        /// <summary>
        /// 插入操作，返回刚插入的ID编号,舍弃SELECT @@IDENTITY方法，使用更合理的SELECT SCOPE_IDENTITY()，当没有自增列的时候SELECT SCOPE_IDENTITY()返回null，此时经过转化使其返回-1
        /// </summary>
        /// <param name="_tableName"></param>
        /// <returns></returns>
        public int Insert(string _tableName, params DbParameter[] commandParameters)
        {
            //EventLog.WriteLog("cc");
            StringBuilder k = new StringBuilder();
            StringBuilder v = new StringBuilder();
            k.AppendFormat("insert into [{0}] (", _tableName);
            v.Append(") values(");
            //foreach (string name in Params.Keys)
            for (int i = 0; i < commandParameters.Length; i++)
            {
                string key = commandParameters[i].ParameterName;
                if (key.IndexOf("@") == 0)
                {
                    key = key.Substring(1);
                }

                k.AppendFormat("[{0}],", key);
                v.AppendFormat("@{0},", key);
            }
            k.Remove(k.Length - 1, 1);
            v.Remove(v.Length - 1, 1);
            string sql = k.ToString() + v.ToString() + ");SELECT SCOPE_IDENTITY()";
            //EventLog.WriteLog(sql);
            object obj = ExecuteScalar(CommandType.Text, sql, commandParameters);
            if (obj == null)   //表示没有自增的ID字段
            {
                return -1;    //返回-1
            }
            return Convert.ToInt32(obj);
        }
        /// <summary>
        /// 更新表,单字段
        /// </summary>
        /// <param name="tableName">表名</param>
        /// <param name="key">更新根据的条件字段</param>
        /// <returns></returns>
        public int Update(string tableName, string key, params DbParameter[] commandParameters)
        {
            StringBuilder k = new StringBuilder();
            k.AppendFormat("update [{0}] set ", tableName);
            for (int i = 0; i < commandParameters.Length; i++)
            {
                string _key = commandParameters[i].ParameterName;
                if (_key.IndexOf("@") == 0)
                {
                    _key = _key.Substring(1);
                }
                if (_key.ToUpper() != key.ToUpper())
                    k.AppendFormat("[{0}]=@{0},", _key);
            }
            //删除update table字符串中的[{0}]=@{0},最后面的逗号字符
            k.Remove(k.Length - 1, 1);
            k.AppendFormat(" where [{0}]=@{0}", key);
            //EventLog.WriteLog(k.ToString());
            return ExecuteNonQuery(CommandType.Text, k.ToString(), commandParameters);
        }

        /// <summary>
        /// 更新表,多列,条件字段不能与更新字段有重复,如有重复,请调用Execute(string text);
        /// </summary>
        /// <param name="tableName">表名</param>
        /// <param name="keys">where条件参数数组</param>
        /// <param name="commandParameters">set后面被更新字段的集合</param>
        /// <returns></returns>
        public int Update(string tableName, string[] keys, params DbParameter[] commandParameters)
        {
            StringBuilder k = new StringBuilder();
            StringBuilder v = new StringBuilder();
            StringBuilder t = new StringBuilder();
            StringCollection col = new StringCollection();
            col.AddRange(keys);
            k.AppendFormat("update [{0}] set ", tableName);
            t.Append(" where ");
            for (int i = 0; i < commandParameters.Length; i++)
            {
                string _key = commandParameters[i].ParameterName;
                if (_key.IndexOf("@") == 0)
                {
                    _key = _key.Remove(0, 1);
                }
                if (!keys.Contains(_key))
                {
                    k.AppendFormat("[{0}]=@{0},", _key);
                }
            }
            foreach (string key in keys)
            {
                v.AppendFormat(" and [{0}]=@{0}", key);
            }
            //删除update table字符串中的[{0}]=@{0},最后面的逗号字符
            k.Remove(k.Length - 1, 1);
            //EventLog.WriteLog(v.ToString());
            //删除where条件紧跟的空格和and字符
            v.Remove(0, 5);
            string sql = k.ToString() + t.ToString() + v.ToString();
            //EventLog.WriteLog(sql);
            return ExecuteNonQuery(CommandType.Text, sql, commandParameters);
        }

        /// <summary>
        /// 删除,非参数类
        /// </summary>
        /// <param name="tableName"></param>
        /// <param name="condition"></param>
        /// <returns></returns>
        public int Delete(string tableName, string condition)
        {
            string sql = "delete from [" + tableName + "] where " + condition;
            return ExecuteNonQuery(sql);
        }
        /// <summary>
        /// 根据where参数删除
        /// </summary>
        /// <param name="tableName"></param>
        /// <returns></returns>
        public int DeleteByParam(string tableName, params DbParameter[] commandParameters)
        {
            StringBuilder k = new StringBuilder();
            StringBuilder f = new StringBuilder();
            k.AppendFormat("delete from [{0}] where ", tableName);
            for (int i = 0; i < commandParameters.Length; i++)
            {
                string _key = commandParameters[i].ParameterName;
                if (_key.IndexOf("@") == 0)
                {
                    _key = _key.Remove(0, 1);
                }
                f.AppendFormat(" and [{0}]=@{0}", _key);
            }
            f.Remove(0, 5);
            k.Append(f.ToString());
            return ExecuteNonQuery(CommandType.Text, k.ToString(), commandParameters);
        }

        /// <summary>
        /// 根据where参数返回数据集
        /// </summary>
        /// <param name="tableName"></param>
        /// <returns></returns>
        public DataTable Get(string tableName, params DbParameter[] commandParameters)
        {
            StringBuilder k = new StringBuilder();
            StringBuilder f = new StringBuilder();
            k.AppendFormat("select * from [{0}] where ", tableName);
            for (int i = 0; i < commandParameters.Length; i++)
            {
                string _key = commandParameters[i].ParameterName;
                if (_key.IndexOf("@") == 0)
                {
                    _key = _key.Remove(0, 1);
                }
                f.AppendFormat(" and [{0}]=@{0}", _key);
            }
            f.Remove(0, 5);
            k.Append(f.ToString());
            return this.ExecuteDataset(CommandType.Text, k.ToString(), commandParameters).Tables[0];
        }

        /// <summary>
        /// 根据条件判断记录是否存在
        /// </summary>
        /// <param name="tableName"></param>
        /// <param name="commandParameters"></param>
        /// <returns></returns>
        public bool Exists(string tableName, params DbParameter[] commandParameters)
        {
            StringBuilder k = new StringBuilder();
            StringBuilder f = new StringBuilder();
            k.AppendFormat("select count(1)  from [{0}] where ", tableName);
            for (int i = 0; i < commandParameters.Length; i++)
            {
                string _key = commandParameters[i].ParameterName;
                if (_key.IndexOf("@") == 0)
                {
                    _key = _key.Remove(0, 1);
                }
                f.AppendFormat(" and [{0}]=@{0}", _key);
            }
            f.Remove(0, 5);
            k.Append(f.ToString());
            object o = this.ExecuteScalar(CommandType.Text, k.ToString(), commandParameters);
            if (o.ToInteger() == 0)   //不存在记录
            {
                return false;
            }
            return true;             //存在记录
        }

        /// <summary>
        /// 根据条件判断记录是否存在
        /// </summary>
        /// <param name="tableName">表名</param>
        /// <param name="where">条件</param>
        /// <returns></returns>
        public bool Exists(string tableName, string where)
        {
            string sql = string.Format("select count(1) from [{0}] where {1}", tableName, where);
            //EventLog.WriteLog(sql);
            object o = this.ExecuteScalar(CommandType.Text, sql);
            if (o.ToInteger() == 0)   //不存在记录
            {
                return false;
            }
            return true;
        }

        /// <summary>
        /// 根据条件判断记录是否存在
        /// </summary>
        /// <param name="tableName">表名</param>
        /// <param name="id">主键字段</param>
        /// <returns></returns>
        public bool Exists(string tableName, int id)
        {
            string sql = string.Format("select count(1) from [{0}] where id={1}", tableName, id);
            object o = this.ExecuteScalar(CommandType.Text, sql);
            if (o.ToInteger() == 0)   //不存在记录
            {
                return false;
            }
            return true;
        }

        #endregion


        #region 利用反射进行的添加和更新及数据校验操作

        /// <summary>
        /// 插入数据时判断数据在表中是否存在重复，where条件只判断一个UniqueAttribute字段 true：表示存在 false:不存在
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="t"></param>
        /// <returns></returns>
        public bool ExistsByInsert<T>(T t)
        {
            Type type = t.GetType();
            PropertyInfo[] properties = type.GetProperties();
            StringBuilder sb = new StringBuilder();
            List<string> fields = new List<string>();

            foreach (var item in properties)
            {
                UniqueAttribute attr = Mapping.GetProperty<UniqueAttribute>(item);
                if (attr != null)
                {
                    var param = new List<DbParameter>();
                    sb.Append("SELECT COUNT(1) FROM [");
                    sb.Append(type.Name);
                    sb.Append("] WHERE ");
                    sb.AppendFormat(" {0}=@{0} ", item.Name);
                    param.Add(this.MakeInParam(item.Name, item.GetValue(t, null)));
                    int n = (int)ExecuteScalar(CommandType.Text, sb.ToString(), param.ToArray());
                    sb.Length = 0;   //清除StringBuilder对象
                    if (n > 0)
                    {
                        fields.Add(item.Name);
                    }
                }
            }
            if (fields.Count > 0)
            {
                //throw new UniqueException(fields);
                return true;
            }
            return false;
        }

        /// <summary>
        /// 更新数据时判断数据在表中是否存在重复，where条件只判断一个UniqueAttribute字段 true：表示存在 false:不存在
        /// 更新的时候要排除自身这条记录，然后看是否有重复
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="t"></param>
        /// <returns></returns>
        public bool ExistsByUpdate<T>(T t)
        {
            //获取自增列名
            string identity = Mapping.GetIdentity<T>();

            Type type = t.GetType();
            PropertyInfo[] properties = type.GetProperties();
            StringBuilder sb = new StringBuilder();
            List<string> fields = new List<string>();

            //properties

            foreach (var item in properties)
            {
                UniqueAttribute attr = Mapping.GetProperty<UniqueAttribute>(item);
                if (attr != null)
                {
                    var param = new List<DbParameter>();
                    sb.Append("SELECT COUNT(1) FROM [");
                    sb.Append(type.Name);
                    sb.Append("] WHERE ");
                    sb.AppendFormat(" {0}=@{0} and {1}<>@{1} ", item.Name, identity);
                    param.Add(this.MakeInParam(item.Name, item.GetValue(t, null)));
                    //获取实体自增列的值赋值给自增列字段
                    param.Add(this.MakeInParam(identity, Mapping.GetIdentityValue(t)));
                    //EventLog.WriteLog(sb.ToString());
                    int n = (int)ExecuteScalar(CommandType.Text, sb.ToString(), param.ToArray());

                    //EventLog.WriteLog(n.ToString());
                    //sb.Clear();
                    sb.Length = 0;  //清除StringBuilder对象
                    if (n > 0)
                    {
                        fields.Add(item.Name);
                    }
                }

            }

            if (fields.Count > 0)
            {
                //throw new UniqueException(fields);
                return true;
            }
            return false;
        }

        /// <summary>
        /// 利用反射单表保存
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public int Save(object obj)
        {
            var parameter = new List<DbParameter>();

            PropertyInfo[] infos = obj.GetType().GetProperties();
            PropertyInfo identityInfo = null;
            foreach (PropertyInfo info in infos)
            {
                Type type = info.PropertyType;
                PropertyAttribute property = Mapping.GetProperty(info);
                string columnName = property.ColumnName == "" ? info.Name : property.ColumnName;
                //该属性为自增字段则跳过
                if (Mapping.FieldIsIndentity(info))
                {
                    identityInfo = info;
                    continue;
                }
                //该属性为外键对应引用类型则直接跳过
                if (property.PropertyType == PropertyType.ForeignKey)
                    continue;
                //如果该类型未List<T>则直接跳过
                if (type.GetInterface("System.Collections.ICollection") != null)
                    continue;

                object objValue;
                objValue = info.GetValue(obj, null);
                //引用类型未赋值直接跳过
                if (objValue == null)
                    continue;

                parameter.Add(this.MakeInParam(columnName, objValue));
            }
            int identityId = this.Insert(obj.GetType().Name, parameter.ToArray());
            //保存数据库后，为实体的自增属性赋值
            if (identityInfo != null)
            {
                identityInfo.SetValue(obj, identityId, null);
            }
            return identityId;
        }

        /// <summary>
        /// 根据实体保存至数据库,存在重复数据不保存,返回-1
        /// </summary>
        /// <param name="t"></param>
        /// <param name="fields">验证重复字段</param>
        /// <returns></returns>
        public int SaveUnique<T>(T t)
        {
            if (t == null)
                return 0;
            if (ExistsByInsert<T>(t))
                return -1;
            return Save(t);
        }

        /// <summary>
        /// 利用反射单表更新
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public int Update(object obj)
        {
            var parameter = new List<DbParameter>();
            List<string> keys = new List<string>();
            PropertyInfo[] infos = obj.GetType().GetProperties();
            foreach (PropertyInfo info in infos)
            {
                Type type = info.PropertyType;

                if (type.GetInterface("System.Collections.ICollection") != null)
                    continue;

                PropertyAttribute property = Mapping.GetProperty(info);
                string columnName = property.ColumnName == "" ? info.Name : property.ColumnName;
                //EventLog.WriteLog(info.Name + ":" + info.GetValue(obj, null));
                //判断是否有自增列，并且自增列有值
                if (Mapping.FieldIsIndentity(info) && info.GetValue(obj, null).ToInteger() <= 0)
                {

                    continue;
                }
                else if (Mapping.FieldIsIndentity(info) && info.GetValue(obj, null).ToInteger() > 0)
                {
                    keys.Add(columnName);
                }
                else if (Mapping.FieldIsUnique(info) && info.GetValue(obj, null).IsNotNullAndEmpty())   //否则判断是否有唯一约束的列
                {
                    keys.Add(columnName);
                }


                object objValue;
                objValue = info.GetValue(obj, null);
                if (objValue == null)
                    continue;
                //object o = Mapping.DefaultForType(info.PropertyType);
                //if (objValue.Equals(o))
                //    continue;
                if (property.PropertyType == PropertyType.ForeignKey)
                    continue;



                parameter.Add(this.MakeInParam(columnName, objValue));
            }
            return this.Update(obj.GetType().Name, keys.ToArray(), parameter.ToArray());
        }

        /// <summary>
        /// 根据实体更新至数据库,存在重复数据不保存,返回-1
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="t"></param>
        /// <returns></returns>
        public int UpdateUnique<T>(T t)
        {
            if (t == null)
                return 0;
            if (ExistsByUpdate<T>(t))
                return -1;
            return Update(t);
        }

        #endregion


        #region 事务处理
        /// <summary>
        /// 开始事务,调用事务必须调用CommitTran()提交事务或者调用RollbackTran()回滚事务
        /// </summary>
        public void BeginTran()
        {
            _conn = this.Factory.CreateConnection();
            _conn.ConnectionString = this.ConnectionString;
            //EventLog.WriteLog(_conn.ConnectionString);
            try
            {

                _conn.Open();
                _trans = _conn.BeginTransaction();

            }
            catch (DbException ee)
            {
                throw new Exception(ee.Message);
            }
        }
        /// <summary>
        /// 提交事务
        /// </summary>
        public void CommitTran()
        {
            if (_conn == null)
            {
                throw new Exception("数据连接意外关闭");
            }
            try
            {
                _trans.Commit();
                _conn.Close();
            }
            catch (InvalidOperationException ex)
            {
                _conn.Close();
            }
            catch (DbException ee)
            {
                _conn.Close();
                throw new Exception(ee.Message);
            }
        }
        /// <summary>
        /// 回滚事务事务
        /// </summary>
        public void RollbackTran()
        {
            if (_conn == null)
            {
                throw new Exception("数据连接意外关闭");
            }
            try
            {
                _trans.Rollback();
                _conn.Close();
            }
            catch (InvalidOperationException ex)
            {
                _conn.Close();
            }
            catch (DbException ee)
            {
                _conn.Close();
                throw new Exception(ee.Message);
            }
        }



        /// <summary>
        /// 事务执行一个sql语句
        /// </summary>
        /// <param name="sql"></param>
        /// <returns></returns>
        public int ExecuteTrans(string sql, params DbParameter[] param)
        {
            return doTrans_(sql, CommandType.Text, param);
        }
        /// <summary>
        /// 事务执行一个存储过程
        /// </summary>
        /// <param name="sp"></param>
        /// <returns></returns>
        public int RunTrans(string sp, params DbParameter[] param)
        {
            return doTrans_(sp, CommandType.StoredProcedure, param);
        }
        /// <summary>
        /// 事务执行一个sql语句，返回datatable表
        /// </summary>
        /// <param name="sql"></param>
        /// <returns></returns>
        public DataTable ExecDataTableTrans(string sql, params DbParameter[] param)
        {
            return ExecDataSetTrans(sql, param).Tables[0];
        }

        /// <summary>
        /// 事务执行一个存储过程，返回DataTable
        /// </summary>
        /// <param name="sp">存储过程</param>
        /// <returns></returns>
        public DataTable RunDataTableTrans(string sp, params DbParameter[] param)
        {
            return RunDataSetTrans(sp, param).Tables[0];
        }
        /// <summary>
        /// 事务执行一条sql语句，返回DataSet
        /// </summary>
        /// <param name="sql"></param>
        /// <returns></returns>
        public DataSet ExecDataSetTrans(string sql, params DbParameter[] param)
        {
            return doDataSetTrans_(sql, CommandType.Text, param);
        }

        /// <summary>
        /// 事务执行一个存储过程，返回DataSet
        /// </summary>
        /// <param name="sp">存储过程</param>
        /// <returns></returns>
        public DataSet RunDataSetTrans(string sp, params DbParameter[] param)
        {
            return doDataSetTrans_(sp, CommandType.StoredProcedure, param);
        }
        /// <summary>
        /// 事务执行一条sql语句，返回首行首列
        /// </summary>
        /// <param name="sql">sql语句</param>
        /// <returns></returns>
        public object ExecScalarTrans(string sql, params DbParameter[] param)
        {
            return doScalarTrans_(sql, CommandType.Text, param);
        }
        /// <summary>
        /// 事务执行一个存储过程，返回首行首列
        /// </summary>
        /// <param name="sp">存储过程</param>
        /// <returns></returns>
        public object RunScalarTrans(string sp, params DbParameter[] param)
        {
            return doScalarTrans_(sp, CommandType.StoredProcedure, param);
        }
        /// <summary>
        /// 事务执行一条sql语句，返回DbDataReader
        /// </summary>
        /// <param name="sql">sql语句</param>
        /// <returns></returns>
        public DbDataReader ExecDataReaderTrans(string sql, params DbParameter[] param)
        {
            return doDataReaderTrans_(sql, CommandType.Text, param);
        }
        /// <summary>
        /// 事务执行一个存储过程，返回DbDataReader
        /// </summary>
        /// <param name="sp">存储过程</param>
        /// <returns></returns>
        public DbDataReader RunDataReaderTrans(string sp, params DbParameter[] param)
        {
            return doDataReaderTrans_(sp, CommandType.StoredProcedure, param);
        }

        #endregion

        #region 其他辅助方法
        public DataTable GetEmptyTable(string tableName)
        {
            string commandText = string.Format("SELECT * FROM {0} WHERE 1=0", tableName);
            return this.ExecuteDataset(commandText).Tables[0];
        }
        /// <summary>
        /// 得到表中行数，不支持参数化
        /// </summary>
        /// <param name="tableName">表名</param>
        /// <param name="condition">条件，不用加where</param>
        /// <returns></returns>
        public int GetCount(string tableName, string condition)
        {
            string strsql = string.Empty;
            if (string.IsNullOrEmpty(condition))
            {
                strsql = "select count(1) from [" + tableName + "]";
            }
            else
            {
                strsql = "select count(1) from [" + tableName + "] where " + condition;
            }
            try
            {
                return Convert.ToInt32(ExecuteScalar(CommandType.Text, strsql));
            }
            catch (DbException e)
            {
                return -1;
            }
        }
        /// <summary>
        /// 得到表中某行的最大值
        /// </summary>
        /// <param name="tableName">表名</param>
        /// <param name="fieldName">字段名</param>
        /// <returns></returns>
        public int GetMaxValue(string tableName, string fieldName)
        {
            string strsql = "select max(" + fieldName + ") from [" + tableName + "]";
            return Convert.ToInt32(ExecuteScalar(CommandType.Text, strsql));
        }
        /// <summary>
        /// 根据表名得到数据，不支持参数化
        /// </summary>
        /// <param name="tableName">表名</param>
        /// <returns></returns>
        public DataTable GetTable(string tableName)
        {
            return ExecuteDataset("select * from [" + tableName + "]").Tables[0];
        }

        /// <summary>
        /// 运行含有GO命令的多条SQL命令
        /// </summary>
        /// <param name="commandText">SQL命令字符串</param>
        public void ExecuteCommandWithSplitter(string commandText)
        {
            this.ExecuteCommandWithSplitter(commandText, "\r\nGO\r\n");
        }
        /// <summary>
        /// 运行含有GO命令的多条SQL命令
        /// </summary>
        /// <param name="commandText">SQL命令字符串</param>
        /// <param name="splitter">分割字符串</param>
        public void ExecuteCommandWithSplitter(string commandText, string splitter)
        {
            int num2;
            int startIndex = 0;
        Label_0003:
            num2 = commandText.IndexOf(splitter, startIndex);
            int length = ((num2 > startIndex) ? num2 : commandText.Length) - startIndex;
            string str = commandText.Substring(startIndex, length);
            if (str.Trim().Length > 0)
            {
                this.ExecuteNonQuery(CommandType.Text, str);
            }
            if (num2 != -1)
            {
                startIndex = num2 + splitter.Length;
                if (startIndex < commandText.Length)
                {
                    goto Label_0003;
                }
            }
        }
        #endregion


        #endregion



    }
}

