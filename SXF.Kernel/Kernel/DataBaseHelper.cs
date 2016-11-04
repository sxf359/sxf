using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data.Common;
using System.Xml;
using System.Web;
using System.Data;
using System.Web.UI.WebControls;
using System.Web.UI;
using System.Reflection;
using System.Collections;
using System.Web.UI.HtmlControls;
using System.Web.Caching;
using SXF.Utils;

namespace SXF.Kernel
{
    public delegate string GetConnStringHandler();
    public abstract class DataBaseHelper
    {
        #region 字段和属性
        protected string _connectionString;
        protected Dictionary<string, object> _params;   //输入参数
        protected List<DbParameter> _outParams;         //输出参数
       

        protected DbConnection _conn;
        protected DbTransaction _trans;

        protected string ConnectionString
        {
            get { return _connectionString; }
            set { _connectionString = value; }
        }
        /// <summary>
        /// 输入参数
        /// </summary>
        public Dictionary<string, object> Params
        {
            get { return _params; }
            set { _params = value; }
        }

        /// <summary>
        /// 输出参数
        /// </summary>
        public List<DbParameter> OutParams
        {
            get { return _outParams; }
            set { _outParams = value; }
        }

       

      
        #endregion

        #region 结构函数
        public DataBaseHelper()
        {
            XmlDocument xmldoc = new XmlDocument();
            string configpath = System.Web.Hosting.HostingEnvironment.MapPath("~/Web.config");
            xmldoc.Load(configpath);
            XmlNode xn = null;
            if (xmldoc.SelectNodes("/configuration/connectionStrings/add")[0] != null)
            {
                xn = xmldoc.SelectNodes("/configuration/connectionStrings/add")[0];
                this.ConnectionString = xn.Attributes["connectionString"].Value;
            }
            else
            {
                xn = xmldoc.SelectNodes("/configration/appSettings/add")[0];
                this.connString = xn.Attributes["value"].Value;
            }

            _params = new Dictionary<string, object>();
            _outParams = new List<DbParameter>();
           
             
        }

        /// <summary>
        /// 连接字符串
        /// </summary>
        private string connString = string.Empty;

        /// <summary>
        /// 获得连接字符串
        /// </summary>
        public string GetConnString
        {
            get
            {
                return connString;
            }
        }


        public DataBaseHelper(string content, ConnectionStringType type)
        {
            _params = new Dictionary<string, object>();
            _outParams = new List<DbParameter>();
            
            if (type == ConnectionStringType.ConnString)
            {
                _connectionString = content;
            }
            else if (type == ConnectionStringType.ConnName)
            {
                XmlDocument xmldoc = new XmlDocument();
                string configpath = System.Web.Hosting.HostingEnvironment.MapPath("~/Web.config");
                xmldoc.Load(configpath);
                //XmlNode xn = xmldoc.SelectSingleNode(string.Format("/configuration/connectionStrings/add[@name='{0}']", content));

                //_connectionString = xn.Attributes["connectionString"].Value;
                XmlNode xn = null;
                if (xmldoc.SelectSingleNode(string.Format("/configuration/connectionStrings/add[@name='{0}']", content)) != null)
                {
                    xn = xmldoc.SelectSingleNode(string.Format("/configuration/connectionStrings/add[@name='{0}']", content));
                    _connectionString = xn.Attributes["connectionString"].Value;
                }
                else
                {
                    xn = xmldoc.SelectSingleNode(string.Format("/configuration/appSettings/add[@key='{0}']", content));
                    _connectionString = xn.Attributes["value"].Value;
                }
            }
            else if (type == ConnectionStringType.EncryptFile)
            {
                if (!System.IO.File.Exists(content))
                {
                    throw new Exception("数据连接文件没有找到 " + content);
                }
                if (!connectionFiles.ContainsKey(content))
                {
                    string encryptContent = System.IO.File.ReadAllText(content);
                    string key = "S8S7FLDL";
                    _connectionString = StringHelper.Decrypt(encryptContent, key);
                    connectionFiles[content] = _connectionString;


                }
                else
                {
                    _connectionString = connectionFiles[content];
                }
            }
            else
            {
                throw new Exception("type参数ConnectionStringType枚举值");
            }

            connString = _connectionString;
        }



        static Dictionary<string, string> connectionFiles = new Dictionary<string, string>();
        public DataBaseHelper(string connName)
            : this(connName, ConnectionStringType.ConnName)
        { }

        public DataBaseHelper(string content, string type)
        {
            _params = new Dictionary<string, object>();
            _outParams = new List<DbParameter>();
            
            if (type == "connstring")
            {
                _connectionString = content;
            }
            else if (type == "connname")
            {
                XmlDocument xmldoc = new XmlDocument();
                string configpath = System.Web.Hosting.HostingEnvironment.MapPath("~/Web.config");
                xmldoc.Load(configpath);
                XmlNode xn = xmldoc.SelectSingleNode(string.Format("/configuration/connectionStrings/add[@name='{0}']", content));
                _connectionString = xn.Attributes["connectionString"].Value;


            }
            else
            {
                throw new Exception("type参数应该是connname或connstring");
            }

            connString = _connectionString;
        }

        public DataBaseHelper(GetConnStringHandler handler)
            : this(handler(), ConnectionStringType.ConnString)
        { }



        #endregion

        #region 子类要实现的抽象方法
        protected abstract void fillCmdParams_(DbCommand cmd);
        protected abstract DbCommand createCmd_(string cmdText, DbConnection conn);
        protected abstract DbCommand createCmd_();
        protected abstract DbDataAdapter createDa_(string cmdText, DbConnection conn);
        protected abstract DbConnection createConn_();

 
        
        #endregion

        #region 私有方法
        private int do_(string text, CommandType type)
        {
            using (DbConnection conn = createConn_())
            {
                conn.Open();
                DbCommand cmd = createCmd_(text, conn);
                cmd.CommandType = type;
                fillCmdParams_(cmd);
                return cmd.ExecuteNonQuery();
            }
        }
        private int doTrans_(string text, CommandType type)
        {
            DbCommand cmd = createCmd_(text, _conn);
            cmd.CommandType = type;
            if (_trans == null)
            {
                throw new Exception("事务没有开启");
            }
            cmd.Transaction = _trans;
            fillCmdParams_(cmd);
            return cmd.ExecuteNonQuery();
        }
        private DataSet doDateSet_(string text, CommandType type)
        {
            using (DbConnection conn = createConn_())
            {
                conn.Open();
                DbDataAdapter da = createDa_(text, conn);
                da.SelectCommand.CommandType = type;
                da.SelectCommand.CommandTimeout = 600;//设置超时时间,by:xugj
                fillCmdParams_(da.SelectCommand);
                DataSet ds = new DataSet();
                da.Fill(ds);
                return ds;
            }
        }
        private DataSet doDataSetTrans_(string text, CommandType type)
        {
            DbDataAdapter da = createDa_(text, _conn);
            da.SelectCommand.CommandType = type;
            if (_trans == null)
            {
                throw new Exception("事务没有开启");
            }
            da.SelectCommand.Transaction = _trans;
            fillCmdParams_(da.SelectCommand);
            DataSet ds = new DataSet();
            da.Fill(ds);
            return ds;
        }
        private object doScalar_(string text, CommandType type)
        {
            using (DbConnection conn = createConn_())
            {
                conn.Open();
                DbCommand cmd = createCmd_(text, conn);
                fillCmdParams_(cmd);
                cmd.CommandType = type;
                return cmd.ExecuteScalar();
            }
        }
        private object doScalarTrans_(string text, CommandType type)
        {
            DbCommand cmd = createCmd_(text, _conn);
            cmd.CommandType = type;
            if (_trans == null)
            {
                throw new Exception("事务没有开启");
            }
            cmd.Transaction = _trans;
            fillCmdParams_(cmd);
            return cmd.ExecuteScalar();
        }
        private DbDataReader doDataReader_(string text, CommandType type)
        {
            DbConnection conn = createConn_();
            try
            {
                conn.Open();
                DbCommand cmd = createCmd_(text, conn);
                cmd.CommandType = type;
                fillCmdParams_(cmd);
                return cmd.ExecuteReader(CommandBehavior.CloseConnection);
            }
            catch (DbException e)
            {
                conn.Close();
                throw e;
            }

        }
        private DbDataReader doDataReaderTrans_(string text, CommandType type)
        {
            DbCommand cmd = createCmd_(text, _conn);
            cmd.CommandType = type;
            if (_trans == null)
            {
                throw new Exception("事务没有开启");
            }
            cmd.Transaction = _trans;
            fillCmdParams_(cmd);
            return cmd.ExecuteReader(CommandBehavior.CloseConnection);

        }
        #endregion

        #region 公共方法
        /// <summary>
        /// 执行一条sql语句，返回影响行数
        /// </summary>
        /// <param name="sql">sql语句</param>
        /// <returns></returns>
        public int Execute(string sql)
        {
            return do_(sql, CommandType.Text);
        }
        public int ExecuteTrans(string sql)
        {
            return doTrans_(sql, CommandType.Text);
        }
        /// <summary>
        /// 执行一个存储过程，返回影响行数
        /// </summary>
        /// <param name="sp">存储过程</param>
        /// <returns></returns>
        public int Run(string sp)
        {
            return do_(sp, CommandType.StoredProcedure);
        }
        public int RunTrans(string sp)
        {
            return doTrans_(sp, CommandType.StoredProcedure);
        }
        /// <summary>
        /// 执行一条sql语句，返回DataTable
        /// </summary>
        /// <param name="sql">sql语句</param>
        /// <returns></returns>
        public DataTable ExecDataTable(string sql)
        {
            return ExecDataSet(sql).Tables[0];
        }
        public DataTable ExecDataTableTrans(string sql)
        {
            return ExecDataSetTrans(sql).Tables[0];
        }

        /// <summary>
        /// 执行一个存储过程，返回DataTable
        /// </summary>
        /// <param name="sp">存储过程</param>
        /// <returns></returns>
        public DataTable RunDataTable(string sp)
        {
            return RunDataSet(sp).Tables[0];
        }
        public DataTable RunDataTableTrans(string sp)
        {
            return RunDataSetTrans(sp).Tables[0];
        }
        /// <summary>
        /// 执行一条sql语句，返回DataSet
        /// </summary>
        /// <param name="sql"></param>
        /// <returns></returns>
        public DataSet ExecDataSet(string sql)
        {
            return doDateSet_(sql, CommandType.Text);
        }
        public DataSet ExecDataSetTrans(string sql)
        {
            return doDataSetTrans_(sql, CommandType.Text);
        }

        /// <summary>
        /// 执行一个存储过程，返回DataSet
        /// </summary>
        /// <param name="sp">存储过程</param>
        /// <returns></returns>
        public DataSet RunDataSet(string sp)
        {
            return doDateSet_(sp, CommandType.StoredProcedure);
        }
        public DataSet RunDataSetTrans(string sp)
        {
            return doDataSetTrans_(sp, CommandType.StoredProcedure);
        }
        /// <summary>
        /// 执行一条sql语句，返回首行首列
        /// </summary>
        /// <param name="sql">sql语句</param>
        /// <returns></returns>
        public object ExecScalar(string sql)
        {
            return doScalar_(sql, CommandType.Text);
        }
        public object ExecScalarTrans(string sql)
        {
            return doScalarTrans_(sql, CommandType.Text);
        }
        /// <summary>
        /// 执行一个存储过程，返回首行首列
        /// </summary>
        /// <param name="sp">存储过程</param>
        /// <returns></returns>
        public object RunScalar(string sp)
        {
            return doScalar_(sp, CommandType.StoredProcedure);
        }
        public object RunScalarTrans(string sp)
        {
            return doScalarTrans_(sp, CommandType.StoredProcedure);
        }
        /// <summary>
        /// 执行一条sql语句，返回DbDataReader
        /// </summary>
        /// <param name="sql">sql语句</param>
        /// <returns></returns>
        public DbDataReader ExecDataReader(string sql)
        {
            return doDataReader_(sql, CommandType.Text);
        }
        public DbDataReader ExecDataReaderTrans(string sql)
        {
            return doDataReaderTrans_(sql, CommandType.Text);
        }
        /// <summary>
        /// 执行一个存储过程，返回DbDataReader
        /// </summary>
        /// <param name="sp">存储过程</param>
        /// <returns></returns>
        public DbDataReader RunDataReader(string sp)
        {
            return doDataReader_(sp, CommandType.StoredProcedure);
        }
        public DbDataReader RunDataReaderTrans(string sp)
        {
            return doDataReaderTrans_(sp, CommandType.StoredProcedure);
        }
        /// <summary>
        /// 得到记录
        /// </summary>
        /// <param name="tableName">表名</param>
        /// <param name="condition">条件，不加where</param>
        /// <returns></returns>
        public int Delete(string tableName, string condition)
        {
            return Execute("delete from [" + tableName + "] where " + condition);
        }
        public int DeleteTrans(string tableName, string condition)
        {
            return ExecuteTrans("delete from [" + tableName + "] where " + condition);
        }
        /// <summary>
        /// 插入一条数据,参数写在Params里，名字与字段名一样
        /// </summary>
        /// <param name="tableName">表名</param>
        /// <returns></returns>
        public int Insert(string tableName)
        {
            StringBuilder sbp = new StringBuilder();
            StringBuilder sbn = new StringBuilder(")values(");
            sbp.AppendFormat("insert into [{0}] (", tableName);
            foreach (string name in Params.Keys)
            {
                sbp.AppendFormat("[{0}],", name);
                sbn.AppendFormat("@{0},", name);
            }
            sbp.Remove(sbp.Length - 1, 1);
            sbn.Remove(sbn.Length - 1, 1);
            string sqlAll = sbp.ToString() + sbn.ToString() + ")";
            return Execute(sqlAll);
        }
        public int InsertTrans(string tableName)
        {
            StringBuilder sbp = new StringBuilder();
            StringBuilder sbn = new StringBuilder(")values(");
            sbp.AppendFormat("insert into [{0}] (", tableName);
            foreach (string name in Params.Keys)
            {
                sbp.AppendFormat("[{0}],", name);
                sbn.AppendFormat("@{0},", name);
            }
            sbp.Remove(sbp.Length - 1, 1);
            sbn.Remove(sbn.Length - 1, 1);
            string sqlAll = sbp.ToString() + sbn.ToString() + ")";
            return ExecuteTrans(sqlAll);
        }
        /// <summary>
        /// 修改一条数据,参数写在Params里，名字与字段名一样
        /// </summary>
        /// <param name="tableName">表名</param>
        /// <param name="PKField">主键</param>
        /// <returns></returns>
        public int Update(string tableName, string PKField)
        {
            StringBuilder sbp = new StringBuilder();
            sbp.AppendFormat("update [{0}] set ", tableName);
            foreach (string name in _params.Keys)
            {
                if (name.ToUpper() != PKField.ToUpper())
                {
                    sbp.AppendFormat("[{0}]=@{0},", name);
                }
            }
            string strn = string.Format(" where [{0}]= @{0}", PKField);
            sbp.Remove(sbp.Length - 1, 1);
            string sqlAll = sbp.ToString() + strn;
            return Execute(sqlAll);
        }
        public int UpdateTrans(string tableName, string PKField)
        {
            StringBuilder sbp = new StringBuilder();
            sbp.AppendFormat("update [{0}] set ", tableName);
            foreach (string name in _params.Keys)
            {
                if (name.ToUpper() != PKField.ToUpper())
                {
                    sbp.AppendFormat("[{0}]=@{0},", name);
                }
            }
            string strn = string.Format(" where [{0}]= @{0}", PKField);
            sbp.Remove(sbp.Length - 1, 1);
            string sqlAll = sbp.ToString() + strn;
            return ExecuteTrans(sqlAll);
        }
        /// <summary>
        /// 修改一条数据,参数写在Params里，名字与字段名一样
        /// </summary>
        /// <param name="tableName">表名</param>
        /// <returns></returns>
        public int Update(string tableName)
        {
            return Update(tableName, "ID");
        }
        public int UpdateTrans(string tableName)
        {
            return UpdateTrans(tableName, "ID");
        }
        /// <summary>
        /// 执行一个事务
        /// </summary>
        /// <param name="transInfos">操作事务</param>
        /// <returns></returns>
        public bool ExecTrans(params TransInfo[] transInfos)
        {
            using (DbConnection conn = createConn_())
            {
                conn.Open();
                DbTransaction trans = conn.BeginTransaction();
                DbCommand cmd = createCmd_();
                cmd.Connection = conn;
                cmd.Transaction = trans;
                try
                {
                    foreach (TransInfo info in transInfos)
                    {
                        cmd.Parameters.Clear();
                        if (info.Params != null && info.Params.Count > 0)
                        {
                            foreach (DbParameter p in info.Params)
                            {
                                cmd.Parameters.Add(p);
                            }
                        }
                        cmd.CommandText = info.Text;
                        cmd.CommandType = info.Type;
                        info.AffectRows = cmd.ExecuteNonQuery();
                    }
                    trans.Commit();
                    return true;
                }
                catch
                {
                    trans.Rollback();
                    throw;
                }
            }
        }
        #endregion

        #region 其他辅助方法
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
                return Convert.ToInt32(ExecScalar(strsql));
            }
            catch 
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
            return Convert.ToInt32(ExecScalar(strsql));
        }
        /// <summary>
        /// 根据表名得到数据，不支持参数化
        /// </summary>
        /// <param name="tableName">表名</param>
        /// <returns></returns>
        public DataTable GetTable(string tableName)
        {
            return ExecDataTable("select * from [" + tableName + "]");
        }

        #endregion

        #region 填充显示方法
        /// <summary>
        /// 从控件填充参数
        /// </summary>
        /// <param name="control"></param>
        public void FillParamsFromControl(Control control)
        {
            foreach (Control c in control.Controls)
            {
                string id = c.ID;
                if (id != null && id.IndexOf("S_") > -1)
                {
                    string key = id.Substring(2);
                    ITextControl tc = c as ITextControl;
                    if (tc != null)
                    {
                        Params.Add(key, tc.Text);
                        continue;
                    }
                    DropDownList ddl = c as DropDownList;
                    if (ddl != null)
                    {
                        Params.Add(key, ddl.SelectedValue);
                        continue;
                    }
                    HiddenField hf = c as HiddenField;
                    if (hf != null)
                    {
                        Params.Add(key, hf.Value);
                        continue;
                    }
                    CheckBox cb = c as CheckBox;
                    if (cb != null)
                    {
                        Params.Add(id.Substring(2), cb.Checked);
                        continue;
                    }
                }
            }
        }
        /// <summary>
        /// 从页面Form里填充参数
        /// </summary>
        public void FillParamsFromPage()
        {
            Page page = HttpContext.Current.Handler as Page;
            FillParamsFromControl(page.Form);
        }
        [Obsolete("这个方法在此类不合适,请用PageHelper里的同名方法", false)]
        public static void ShowFromDataTable(DataTable dataTable)
        {
            Page page = HttpContext.Current.Handler as Page;
            ShowFromDataTable(dataTable, page);
        }
        [Obsolete("这个方法在此类不合适,请用PageHelper里的同名方法", false)]
        public static void ShowFromDataTable(DataTable dataTable, Control contain)
        {
            if (dataTable.Rows.Count > 0)
            {
                DataRow dr = dataTable.Rows[0];
                foreach (DataColumn dc in dataTable.Columns)
                {
                    object val = dr[dc];
                    Control c = contain.FindControl("S_" + dc.ColumnName);
                    if (c == null)
                    {
                        c = contain.FindControl("V_" + dc.ColumnName);
                    }
                    if (c == null)
                    {
                        continue;
                    }
                    ITextControl tc = c as ITextControl;//TextBox/Lable/Literal
                    if (tc != null)
                    {
                        tc.Text = val.ToString();
                        continue;
                    }
                    DropDownList ddl = c as DropDownList;
                    if (ddl != null)
                    {
                        foreach (ListItem li in ddl.Items)
                        {
                            if (li.Value == val.ToString())
                            {
                                li.Selected = true;
                                continue;
                            }
                        }
                    }
                    HiddenField hf = c as HiddenField;
                    if (hf != null)
                    {
                        hf.Value = val.ToString();
                        continue;
                    }
                    CheckBox ckb = c as CheckBox;
                    if (ckb != null)
                    {
                        ckb.Checked = val.Equals(true);
                        continue;
                    }
                    HtmlGenericControl html = c as HtmlGenericControl;
                    if (html != null)
                    {
                        html.InnerHtml = val.ToString();
                        continue;
                    }
                }
            }
        }
        [Obsolete("这个方法在此类不合适,请用PageHelper里的同名方法", false)]
        public static void ShowFromEntity(object entity, Control contain)
        {
            if (entity == null)
                return;
            Type type = entity.GetType();
            foreach (PropertyInfo pinfo in type.GetProperties())
            {
                object objval = pinfo.GetValue(entity, null);
                if (objval == null)
                    continue;
                string val = objval.ToString();
                Control c = contain.FindControl("S_" + pinfo.Name);
                if (c == null)
                {
                    c = contain.FindControl("V_" + pinfo.Name);
                }
                if (c == null)
                {
                    continue;
                }
                ITextControl tc = c as ITextControl;
                if (tc != null)
                {
                    tc.Text = val;
                }
                DropDownList ddl = c as DropDownList;
                if (ddl != null)
                {
                    foreach (ListItem li in ddl.Items)
                    {
                        if (li.Value == val)
                        {
                            li.Selected = true;
                            continue;
                        }
                    }
                }
                HiddenField hf = c as HiddenField;
                if (hf != null)
                {
                    hf.Value = val;
                    continue;
                }
                CheckBox ckb = c as CheckBox;
                if (ckb != null)
                {
                    ckb.Checked = objval.Equals(true) || objval.Equals("1") || objval.Equals(1);
                    continue;
                }
            }
        }
        [Obsolete("这个方法在此类不合适,请用PageHelper里的同名方法", false)]
        public static void ShowFromEntity(object entity)
        {
            Page page = HttpContext.Current.Handler as Page;
            ShowFromEntity(entity, page);
        }
        [Obsolete("这个方法在此类不合适,请用PageHelper里的同名方法", false)]
        public static void FillEntityFromDataRow(object entity, DataRow dr)
        {
            Type type = entity.GetType();
            foreach (DataColumn c in dr.Table.Columns)
            {
                string cname = c.ColumnName;
                if (dr[cname] != null)
                {
                    PropertyInfo property = type.GetProperty(cname);
                    if (property != null)
                    {
                        Type ptype = type.GetProperty(cname).PropertyType;
                        if (dr[cname] is DBNull)
                        {
                            property.SetValue(entity, null, null);
                        }
                        else
                        {
                            property.SetValue(entity, Convert.ChangeType(dr[cname], ptype), null);
                        }
                        continue;
                    }
                }
            }
        }
        [Obsolete("这个方法在此类不合适,请用PageHelper里的同名方法", false)]
        public static void FillEntitiesFromDataTable<T>(IList<T> entities, DataTable dt) where T : class
        {
            if (entities == null)
            {
                return;
            }
            foreach (DataRow dr in dt.Rows)
            {
                ConstructorInfo cinfo = typeof(T).GetConstructor(Type.EmptyTypes);
                T obj = (T)cinfo.Invoke(null);
                FillEntityFromDataRow(obj, dr);
                entities.Add(obj);
            }
        }
        [Obsolete("这个方法在此类不合适,请用PageHelper里的同名方法", false)]
        public static void FillEntityFromControl(object entity, Control contain)
        {
            ControlCollection controls = contain.Controls;
            Type type = entity.GetType();
            foreach (Control c in controls)
            {
                string id = c.ID;
                if (id != null && id.StartsWith("S_"))
                {
                    string propertyName = id.Substring(2);
                    PropertyInfo property = type.GetProperty(propertyName);
                    Type ptype = type.GetProperty(propertyName).PropertyType;
                    ITextControl tc = c as ITextControl;
                    if (tc != null)
                    {
                        property.SetValue(entity, Convert.ChangeType(tc.Text, ptype), null);
                        continue;
                    }
                    DropDownList ddl = c as DropDownList;
                    if (ddl != null)
                    {
                        property.SetValue(entity, Convert.ChangeType(ddl.SelectedValue, ptype), null);
                        continue;
                    }
                    CheckBox cb = c as CheckBox;
                    if (cb != null)
                    {
                        property.SetValue(entity, Convert.ChangeType(cb.Checked, ptype), null);
                        continue;
                    }
                    HiddenField hf = c as HiddenField;
                    if (hf != null)
                    {
                        property.SetValue(entity, Convert.ChangeType(hf.Value, ptype), null);
                        continue;
                    }
                }
            }
        }
        [Obsolete("这个方法在此类不合适,请用PageHelper里的同名方法", false)]
        public static void FillEntityFromPage(object entity)
        {
            Page page = HttpContext.Current.Handler as Page;
            FillEntityFromControl(entity, page);
        }
        #endregion

        #region 事务处理
        /// <summary>
        /// 开始事务,调用事务必须调用CommitTran()提交事务或者调用RollbackTran()回滚事务
        /// </summary>
        public void BeginTran()
        {
            _conn = createConn_();
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
        #endregion
    }
    /// <summary>
    /// 事务中命令信息
    /// </summary>
    public class TransInfo
    {
        private string _text;
        private CommandType _type = CommandType.Text;
        private List<DbParameter> _params;
        private int _affectRows = 0;

        /// <summary>
        /// sql语句或存储过程
        /// </summary>
        public string Text
        {
            get { return _text; }
            set { _text = value; }
        }
        /// <summary>
        /// 命令的类型
        /// </summary>
        public CommandType Type
        {
            get { return _type; }
            set { _type = value; }
        }
        /// <summary>
        /// 命令参数
        /// </summary>
        public List<DbParameter> Params
        {
            get { return _params; }
            set { _params = value; }
        }
        /// <summary>
        /// 命令执行影响的行数
        /// </summary>
        public int AffectRows
        {
            get { return _affectRows; }
            set { _affectRows = value; }
        }

        public TransInfo()
            : this(string.Empty)
        { }
        public TransInfo(string text)
            : this(text, CommandType.Text)
        { }
        public TransInfo(string text, CommandType type)
        {
            _text = text;
            _type = type;
            _params = new List<DbParameter>();
        }
    }
}
