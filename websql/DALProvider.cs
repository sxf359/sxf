using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Rztong.IDAL;
using SXF.Kernel;
using SXF.Utils;
using System.Data;
using System.Data.Common;
using System.Data.SqlClient;
using Rztong.Model;

namespace Rztong.DAL
{
    public class DALProvider:BaseDataProvider,IDALProvider
    {
        #region Fields
        private ITableProvider aideInvestorProvider;
        private ITableProvider aideDeliveryprojectProvider;
        private ITableProvider aideGuestProvider;
        private ITableProvider aideGuestForInvestorProvider;
        private ITableProvider aideMemberCAInfoProvider;
        private ITableProvider aideMemberCertificateSortProvider;
        private ITableProvider aideUserProvider;
        private ITableProvider aideCrowdFundingProvider;
        private ITableProvider aideTeamProvider;
        private ITableProvider aideEducationHistoryProvider;
        private ITableProvider aideJobHistoryProvider;
        private ITableProvider aideCrowdFundingInvestProvider;
        private ITableProvider aideGuestForCrowdFundingProvider;
        private ITableProvider aideCostrecordProvider;
        private ITableProvider aideChargeProvider;
        private ITableProvider aideOrderProvider;
        #endregion

        #region 构造函数
        /// <summary>
        /// 构造函数
        /// </summary>
        public DALProvider(string connString)
            : base(connString)
        {
            aideInvestorProvider = GetTableProvider(Investor.Tablename);
            aideDeliveryprojectProvider = GetTableProvider(Deliveryproject.Tablename);
            aideGuestProvider = GetTableProvider(Guest.Tablename);
            aideGuestForInvestorProvider = GetTableProvider(GuestForInvestor.Tablename);
            aideMemberCAInfoProvider = GetTableProvider(MemberCAInfo.Tablename);
            aideMemberCertificateSortProvider = GetTableProvider(MemberCertificateSort.Tablename);
            aideUserProvider = GetTableProvider(User.Tablename);
            aideCrowdFundingProvider = GetTableProvider(CrowdFunding.Tablename);
            aideTeamProvider = GetTableProvider(Team.Tablename);
            aideEducationHistoryProvider = GetTableProvider(EducationHistory.Tablename);
            aideJobHistoryProvider = GetTableProvider(JobHistory.Tablename);
            aideCrowdFundingInvestProvider = GetTableProvider(CrowdFundingInvest.Tablename);
            aideGuestForCrowdFundingProvider = GetTableProvider(GuestForCrowdFunding.Tablename);
            aideCostrecordProvider = GetTableProvider(Costrecord.Tablename);
            aideChargeProvider = GetTableProvider(Charge.Tablename);
            aideOrderProvider = GetTableProvider(Order.Tablename);

        }
        #endregion

        #region Investor
        /// <summary>
        /// 获取Investor列表
        /// </summary>
        /// <param name="pageindex"></param>
        /// <param name="pagesize"></param>
        /// <param name="where"></param>
        /// <param name="orderby"></param>
        /// <returns></returns>
        public PagerSet GetInvestorList(int pageindex, int pagesize, string where, string orderby)
        {
            PagerParameters pp = new PagerParameters(Investor.Tablename, orderby, where, pageindex, pagesize);
            return GetPagerSet2(pp);
        }

        /// <summary>
        /// 根据ID获取Investor对象
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public Investor GetInvestor(int id)
        {
            string sql = string.Format("SELECT * FROM dbo.Investor with(nolock) WHERE ID={0}", id);
            return Database.ExecuteObject<Investor>(sql);
        }
        /// <summary>
        /// 根据where条件获取Investor对象
        /// </summary>
        /// <param name="where"></param>
        /// <returns></returns>
        public Investor GetInvestor(string where)
        {
            return aideInvestorProvider.GetObject<Investor>(where);
        }
        /// <summary>
        /// Investor插入
        /// </summary>
        /// <param name="about"></param>
        /// <returns></returns>
        //public int InsertInvestor(Investor about)
        //{

        //    string sql = "INSERT INTO dbo.Investor  ( Title, AboutContent ) select @Title,@AboutContent";
        //    var parameter = new List<DbParameter>();
        //    parameter.Add(Database.MakeInParam("Title", about.Title));
        //    parameter.Add(Database.MakeInParam("AboutContent", about.AboutContent));
        //    return Database.ExecuteNonQuery(CommandType.Text, sql, parameter.ToArray());
        //}
        /// <summary>
        /// Investor更新
        /// </summary>
        /// <param name="about"></param>
        /// <returns></returns>
        //public int UpdateInvestor(Investor about)
        //{
        //    string sql = "update Investor set Title=@Title,AboutContent=@AboutContent where ID=@ID";
        //    var parameter = new List<DbParameter>();
        //    parameter.Add(Database.MakeInParam("Title", about.Title));
        //    parameter.Add(Database.MakeInParam("AboutContent", about.AboutContent));
        //    parameter.Add(Database.MakeInParam("ID", about.ID));

        //    return Database.ExecuteNonQuery(CommandType.Text, sql, parameter.ToArray());
        //}
        /// <summary>
        /// 根据ID删除Investor数据
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public int DeleteInvestor(int id)
        {
            string sql = string.Format("delete from Investor where id={0}", id);
            return Database.ExecuteNonQuery(sql);
        }
        /// <summary>
        /// Investor表中是否存在该记录
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public bool ExistsInvestor(int id)
        {
            string sql = string.Format("select id from Investor where id={0}", id);
            object o = Database.ExecuteScalar(CommandType.Text, sql);
            return o == null ? false : true;
        }
        #endregion

        #region Deliveryproject
        /// <summary>
        /// 获取Deliveryproject列表
        /// </summary>
        /// <param name="pageindex"></param>
        /// <param name="pagesize"></param>
        /// <param name="where"></param>
        /// <param name="orderby"></param>
        /// <returns></returns>
        public PagerSet GetDeliveryprojectList(int pageindex, int pagesize, string where, string orderby)
        {
            PagerParameters pp = new PagerParameters(Deliveryproject.Tablename, orderby, where, pageindex, pagesize);
            return GetPagerSet2(pp);
        }

        /// <summary>
        /// 根据ID获取Deliveryproject对象
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public Deliveryproject GetDeliveryproject(int id)
        {
            string sql = string.Format("SELECT * FROM dbo.Deliveryproject with(nolock) WHERE ID={0}", id);
            return Database.ExecuteObject<Deliveryproject>(sql);
        }
        /// <summary>
        /// 根据where条件获取Deliveryproject对象
        /// </summary>
        /// <param name="where"></param>
        /// <returns></returns>
        public Deliveryproject GetDeliveryproject(string where)
        {
            return aideDeliveryprojectProvider.GetObject<Deliveryproject>(where);
        }
        /// <summary>
        /// Deliveryproject插入
        /// </summary>
        /// <param name="about"></param>
        /// <returns></returns>
        public int InsertDeliveryproject(Deliveryproject deliveryproject)
        {
            return Database.Save(deliveryproject);
        }
        /// <summary>
        /// Deliveryproject更新
        /// </summary>
        /// <param name="about"></param>
        /// <returns></returns>
        public int UpdateDeliveryproject(Deliveryproject deliveryproject)
        {
            return Database.Update(deliveryproject);
        }
        /// <summary>
        /// 根据ID删除Deliveryproject数据
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public int DeleteDeliveryproject(int id)
        {
            string sql = string.Format("delete from Deliveryproject where id={0}", id);
            return Database.ExecuteNonQuery(sql);
        }
        /// <summary>
        /// Deliveryproject表中是否存在该记录
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public bool ExistsDeliveryproject(int id)
        {
            string sql = string.Format("select id from Deliveryproject where id={0}", id);
            object o = Database.ExecuteScalar(CommandType.Text, sql);
            return o == null ? false : true;
        }
        #endregion

        #region Guest
        /// <summary>
        /// 获取Guest列表
        /// </summary>
        /// <param name="pageindex"></param>
        /// <param name="pagesize"></param>
        /// <param name="where"></param>
        /// <param name="orderby"></param>
        /// <returns></returns>
        public PagerSet GetGuestList(int pageindex, int pagesize, string where, string orderby)
        {
            PagerParameters pp = new PagerParameters(Guest.Tablename, orderby, where, pageindex, pagesize);
            return GetPagerSet2(pp);
        }

        /// <summary>
        /// 根据ID获取Guest对象
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public Guest GetGuest(int id)
        {
            string sql = string.Format("SELECT * FROM dbo.Guest with(nolock) WHERE ID={0}", id);
            return Database.ExecuteObject<Guest>(sql);
        }
        /// <summary>
        /// 根据where条件获取Guest对象
        /// </summary>
        /// <param name="where"></param>
        /// <returns></returns>
        public Guest GetGuest(string where)
        {
            return aideGuestProvider.GetObject<Guest>(where);
        }
        /// <summary>
        /// Guest插入
        /// </summary>
        /// <param name="about"></param>
        /// <returns></returns>
        public int InsertGuest(Guest guest)
        {
            return Database.Save(guest);
        }
        /// <summary>
        /// Guest更新
        /// </summary>
        /// <param name="about"></param>
        /// <returns></returns>
        public int UpdateGuest(Guest guest)
        {
            return Database.Update(guest);
        }
        /// <summary>
        /// 根据ID删除Guest数据
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public int DeleteGuest(int id)
        {
            string sql = string.Format("delete from Guest where id={0}", id);
            return Database.ExecuteNonQuery(sql);
        }
        /// <summary>
        /// Guest表中是否存在该记录
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public bool ExistsGuest(int id)
        {
            string sql = string.Format("select id from Guest where id={0}", id);
            object o = Database.ExecuteScalar(CommandType.Text, sql);
            return o == null ? false : true;
        }
        #endregion

        #region GuestForInvestor
        /// <summary>
        /// 获取GuestForInvestor列表
        /// </summary>
        /// <param name="pageindex"></param>
        /// <param name="pagesize"></param>
        /// <param name="where"></param>
        /// <param name="orderby"></param>
        /// <returns></returns>
        public PagerSet GetGuestForInvestorList(int pageindex, int pagesize, string where, string orderby)
        {
            PagerParameters pp = new PagerParameters(GuestForInvestor.Tablename, orderby, where, pageindex, pagesize);
            return GetPagerSet2(pp);
        }

        /// <summary>
        /// 根据ID获取GuestForInvestor对象
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public GuestForInvestor GetGuestForInvestor(int id)
        {
            string sql = string.Format("SELECT * FROM dbo.GuestForInvestor with(nolock) WHERE ID={0}", id);
            return Database.ExecuteObject<GuestForInvestor>(sql);
        }
        /// <summary>
        /// 根据where条件获取GuestForInvestor对象
        /// </summary>
        /// <param name="where"></param>
        /// <returns></returns>
        public GuestForInvestor GetGuestForInvestor(string where)
        {
            return aideGuestForInvestorProvider.GetObject<GuestForInvestor>(where);
        }
        /// <summary>
        /// GuestForInvestor插入
        /// </summary>
        /// <param name="about"></param>
        /// <returns></returns>
        public int InsertGuestForInvestor(GuestForInvestor GuestForInvestor)
        {
            return Database.Save(GuestForInvestor);
        }
        /// <summary>
        /// GuestForInvestor更新
        /// </summary>
        /// <param name="about"></param>
        /// <returns></returns>
        public int UpdateGuestForInvestor(GuestForInvestor GuestForInvestor)
        {
            return Database.Update(GuestForInvestor);
        }
        /// <summary>
        /// 根据ID删除GuestForInvestor数据
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public int DeleteGuestForInvestor(int id)
        {
            string sql = string.Format("delete from GuestForInvestor where id={0}", id);
            return Database.ExecuteNonQuery(sql);
        }
        /// <summary>
        /// GuestForInvestor表中是否存在该记录
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public bool ExistsGuestForInvestor(int id)
        {
            string sql = string.Format("select id from GuestForInvestor where id={0}", id);
            object o = Database.ExecuteScalar(CommandType.Text, sql);
            return o == null ? false : true;
        }
        #endregion

        #region MemberCAInfo
        /// <summary>
        /// 获取MemberCAInfo列表
        /// </summary>
        /// <param name="pageindex"></param>
        /// <param name="pagesize"></param>
        /// <param name="where"></param>
        /// <param name="orderby"></param>
        /// <returns></returns>
        public PagerSet GetMemberCAInfoList(int pageindex, int pagesize, string where, string orderby)
        {
            PagerParameters pp = new PagerParameters(MemberCAInfo.Tablename, orderby, where, pageindex, pagesize);
            return GetPagerSet2(pp);
        }

        /// <summary>
        /// 根据ID获取MemberCAInfo对象
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public MemberCAInfo GetMemberCAInfo(int id)
        {
            string sql = string.Format("SELECT * FROM dbo.MemberCAInfo with(nolock) WHERE ID={0}", id);
            return Database.ExecuteObject<MemberCAInfo>(sql);
        }
        /// <summary>
        /// 根据where条件获取MemberCAInfo对象
        /// </summary>
        /// <param name="where"></param>
        /// <returns></returns>
        public MemberCAInfo GetMemberCAInfo(string where)
        {
            return aideMemberCAInfoProvider.GetObject<MemberCAInfo>(where);
        }
        /// <summary>
        /// MemberCAInfo插入
        /// </summary>
        /// <param name="about"></param>
        /// <returns></returns>
        public int InsertMemberCAInfo(MemberCAInfo MemberCAInfo)
        {
            return Database.Save(MemberCAInfo);
        }
        /// <summary>
        /// MemberCAInfo更新
        /// </summary>
        /// <param name="about"></param>
        /// <returns></returns>
        public int UpdateMemberCAInfo(MemberCAInfo MemberCAInfo)
        {
            return Database.Update(MemberCAInfo);
        }
        /// <summary>
        /// 根据ID删除MemberCAInfo数据
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public int DeleteMemberCAInfo(int id)
        {
            string sql = string.Format("delete from MemberCAInfo where id={0}", id);
            return Database.ExecuteNonQuery(sql);
        }
        /// <summary>
        /// MemberCAInfo表中是否存在该记录
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public bool ExistsMemberCAInfo(int id)
        {
            string sql = string.Format("select id from MemberCAInfo where id={0}", id);
            object o = Database.ExecuteScalar(CommandType.Text, sql);
            return o == null ? false : true;
        }
        #endregion

        #region MemberCertificateSort
        /// <summary>
        /// 获取MemberCertificateSort列表
        /// </summary>
        /// <param name="pageindex"></param>
        /// <param name="pagesize"></param>
        /// <param name="where"></param>
        /// <param name="orderby"></param>
        /// <returns></returns>
        public PagerSet GetMemberCertificateSortList(int pageindex, int pagesize, string where, string orderby)
        {
            PagerParameters pp = new PagerParameters(MemberCertificateSort.Tablename, orderby, where, pageindex, pagesize);
            return GetPagerSet2(pp);
        }

        /// <summary>
        /// 根据ID获取emberCertificateSort对象
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public MemberCertificateSort GetMemberCertificateSort(int id)
        {
            string sql = string.Format("SELECT * FROM dbo.MemberCertificateSort with(nolock) WHERE ID={0}", id);
            return Database.ExecuteObject<MemberCertificateSort>(sql);
        }
        /// <summary>
        /// 根据where条件获取MemberCertificateSort对象
        /// </summary>
        /// <param name="where"></param>
        /// <returns></returns>
        public MemberCertificateSort GetMemberCertificateSort(string where)
        {
            return aideMemberCertificateSortProvider.GetObject<MemberCertificateSort>(where);
        }
        /// <summary>
        /// MemberCertificateSort插入
        /// </summary>
        /// <param name="about"></param>
        /// <returns></returns>
        public int InsertMemberCertificateSort(MemberCertificateSort MemberCertificateSort)
        {
            return Database.Save(MemberCertificateSort);
        }
        /// <summary>
        /// MemberCertificateSort更新
        /// </summary>
        /// <param name="about"></param>
        /// <returns></returns>
        public int UpdateMemberCertificateSort(MemberCertificateSort MemberCertificateSort)
        {
            return Database.Update(MemberCertificateSort);
        }
        /// <summary>
        /// 根据ID删除MemberCertificateSort数据
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public int DeleteMemberCertificateSort(int id)
        {
            string sql = string.Format("delete from MemberCertificateSort where id={0}", id);
            return Database.ExecuteNonQuery(sql);
        }
        /// <summary>
        /// MemberCertificateSort表中是否存在该记录
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public bool ExistsMemberCertificateSort(int id)
        {
            string sql = string.Format("select id from MemberCertificateSort where id={0}", id);
            object o = Database.ExecuteScalar(CommandType.Text, sql);
            return o == null ? false : true;
        }
        #endregion

        #region User
        /// <summary>
        /// 获取User列表
        /// </summary>
        /// <param name="pageindex"></param>
        /// <param name="pagesize"></param>
        /// <param name="where"></param>
        /// <param name="orderby"></param>
        /// <returns></returns>
        public PagerSet GetUserList(int pageindex, int pagesize, string where, string orderby)
        {
            PagerParameters pp = new PagerParameters(User.Tablename, orderby, where, pageindex, pagesize);
            return GetPagerSet2(pp);
        }

        /// <summary>
        /// 根据ID获取User对象
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public User GetUser(int id)
        {
            string sql = string.Format("SELECT * FROM dbo.[User] with(nolock) WHERE user_ID={0}", id);
            return Database.ExecuteObject<User>(sql);
        }
        /// <summary>
        /// 根据where条件获取User对象
        /// </summary>
        /// <param name="where"></param>
        /// <returns></returns>
        public User GetUser(string where)
        {
            return aideUserProvider.GetObject<User>(where);
        }
        /// <summary>
        /// User插入
        /// </summary>
        /// <param name="about"></param>
        /// <returns></returns>
        public int InsertUser(User user)
        {

            return Database.Save(user);
        }
        /// <summary>
        /// User更新
        /// </summary>
        /// <param name="about"></param>
        /// <returns></returns>
        public int UpdateUser(User user)
        {
            return Database.Update(user); 
        }
        /// <summary>
        /// 根据ID删除User数据
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public int DeleteUser(int id)
        {
            string sql = string.Format("delete from [User] where user_id={0}", id);
            return Database.ExecuteNonQuery(sql); 
        }
        /// <summary>
        /// User表中是否存在该记录
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public bool ExistsUser(int id)
        {
            string sql = string.Format("select user_id from [User] where user_id={0}", id);
            object o = Database.ExecuteScalar(CommandType.Text, sql);
            return o == null ? false : true;
        }
        /// <summary>
        /// User表中是否存在该记录
        /// </summary>
        /// <param name="where">条件语句，不包括where</param>
        /// <returns></returns>
        public bool ExistsUser(string where)
        {
            return Database.Exists(User.Tablename, where);
        }

        /// <summary>
        /// 币增减变动
        /// </summary>
        /// <param name="user">会员名</param>
        /// <param name="const">金额</param>
        /// <param name="Note">备注</param>
        /// <param name="BillType">收支 0：支出 1:收入</param>
        /// <param name="PayType">支付类型</param>
        /// <param name="TranType">交易类型</param>
        /// <returns></returns>
        public string ChangeMoneyTrans(string user,  decimal cost, string note, BillType billType, PayType payType, TranType tranType)
        {
            var parameter = new List<DbParameter>();
            parameter.Add(Database.MakeInParam("User", user)); ;
            parameter.Add(Database.MakeInParam("Cost", cost));
            parameter.Add(Database.MakeInParam("Note", note));
            parameter.Add(Database.MakeInParam("BillType", (int) billType));
            parameter.Add(Database.MakeInParam("PayType", (int) payType));
            parameter.Add(Database.MakeInParam("TranType", (int) tranType));
            parameter.Add(Database.MakeOutParam("msg", typeof(string), 100));
            Database.RunTrans("ChangeMoney", parameter.ToArray());
            //EventLog.WriteLog(parameter[parameter.Count - 2].Value.ToString());
            return parameter[parameter.Count - 1].Value.ToString();
        }
        #endregion

        #region CrowdFunding
        /// <summary>
        /// 获取CrowdFunding列表
        /// </summary>
        /// <param name="pageindex"></param>
        /// <param name="pagesize"></param>
        /// <param name="where"></param>
        /// <param name="orderby"></param>
        /// <returns></returns>
        public PagerSet GetCrowdFundingList(int pageindex, int pagesize, string where, string orderby)
        {
            PagerParameters pp = new PagerParameters(CrowdFunding.Tablename, orderby, where, pageindex, pagesize);
            return GetPagerSet2(pp);
        }

        /// <summary>
        /// 根据ID获取CrowdFunding对象
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public CrowdFunding GetCrowdFunding(int id)
        {
            string sql = string.Format("SELECT * FROM dbo.[CrowdFunding] with(nolock) WHERE ID={0}", id);
            return Database.ExecuteObject<CrowdFunding>(sql);
        }
        /// <summary>
        /// 根据where条件获取CrowdFunding对象
        /// </summary>
        /// <param name="where"></param>
        /// <returns></returns>
        public CrowdFunding GetCrowdFunding(string where)
        {
            return aideCrowdFundingProvider.GetObject<CrowdFunding>(where);
        }
        /// <summary>
        /// CrowdFunding插入
        /// </summary>
        /// <param name="about"></param>
        /// <returns></returns>
        public int InsertCrowdFunding(CrowdFunding CrowdFunding)
        {

            return Database.Save(CrowdFunding);
        }
        /// <summary>
        /// CrowdFunding更新
        /// </summary>
        /// <param name="about"></param>
        /// <returns></returns>
        public int UpdateCrowdFunding(CrowdFunding CrowdFunding)
        {
            return Database.Update(CrowdFunding);
        }
        /// <summary>
        /// 根据ID删除CrowdFunding数据
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public int DeleteCrowdFunding(int id)
        {
            string sql = string.Format("delete from [CrowdFunding] where id={0}", id);
            return Database.ExecuteNonQuery(sql);
        }
        /// <summary>
        /// CrowdFunding表中是否存在该记录
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public bool ExistsCrowdFunding(int id)
        {
            string sql = string.Format("select id from [CrowdFunding] where id={0}", id);
            object o = Database.ExecuteScalar(CommandType.Text, sql);
            return o == null ? false : true;
        }
        /// <summary>
        /// CrowdFunding表中是否存在该记录
        /// </summary>
        /// <param name="where">条件语句，不包括where</param>
        /// <returns></returns>
        public bool ExistsCrowdFunding(string where)
        {
            return Database.Exists(CrowdFunding.Tablename, where);
        }
        #endregion

        #region CrowdFundingInvest
        /// <summary>
        /// 获取CrowdFundingInvest列表
        /// </summary>
        /// <param name="pageindex"></param>
        /// <param name="pagesize"></param>
        /// <param name="where"></param>
        /// <param name="orderby"></param>
        /// <returns></returns>
        public PagerSet GetCrowdFundingInvestList(int pageindex, int pagesize, string where, string orderby)
        {
            PagerParameters pp = new PagerParameters(CrowdFundingInvest.Tablename, orderby, where, pageindex, pagesize);
            return GetPagerSet2(pp);
        }

        /// <summary>
        /// 根据ID获取CrowdFundingInvest对象
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public CrowdFundingInvest GetCrowdFundingInvest(int id)
        {
            string sql = string.Format("SELECT * FROM dbo.[CrowdFundingInvest] with(nolock) WHERE ID={0}", id);
            return Database.ExecuteObject<CrowdFundingInvest>(sql);
        }
        /// <summary>
        /// 根据where条件获取CrowdFundingInvest对象
        /// </summary>
        /// <param name="where"></param>
        /// <returns></returns>
        public CrowdFundingInvest GetCrowdFundingInvest(string where)
        {
            return aideCrowdFundingInvestProvider.GetObject<CrowdFundingInvest>(where);
        }
        /// <summary>
        /// CrowdFundingInvest插入
        /// </summary>
        /// <param name="about"></param>
        /// <returns></returns>
        public int InsertCrowdFundingInvest(CrowdFundingInvest CrowdFundingInvest)
        {

            return Database.Save(CrowdFundingInvest);
        }
        /// <summary>
        /// CrowdFundingInvest更新
        /// </summary>
        /// <param name="about"></param>
        /// <returns></returns>
        public int UpdateCrowdFundingInvest(CrowdFundingInvest CrowdFundingInvest)
        {
            return Database.Update(CrowdFundingInvest);
        }
        /// <summary>
        /// 根据ID删除CrowdFundingInvest数据
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public int DeleteCrowdFundingInvest(int id)
        {
            string sql = string.Format("delete from [CrowdFundingInvest] where id={0}", id);
            return Database.ExecuteNonQuery(sql);
        }
        /// <summary>
        /// CrowdFundingInvest表中是否存在该记录
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public bool ExistsCrowdFundingInvest(int id)
        {
            string sql = string.Format("select id from [CrowdFundingInvest] where id={0}", id);
            object o = Database.ExecuteScalar(CommandType.Text, sql);
            return o == null ? false : true;
        }
        /// <summary>
        /// CrowdFundingInvest表中是否存在该记录
        /// </summary>
        /// <param name="where">条件语句，不包括where</param>
        /// <returns></returns>
        public bool ExistsCrowdFundingInvest(string where)
        {
            return Database.Exists(CrowdFundingInvest.Tablename, where);
        }
        #endregion

        #region Team
        /// <summary>
        /// 获取Team列表
        /// </summary>
        /// <param name="pageindex"></param>
        /// <param name="pagesize"></param>
        /// <param name="where"></param>
        /// <param name="orderby"></param>
        /// <returns></returns>
        public PagerSet GetTeamList(int pageindex, int pagesize, string where, string orderby)
        {
            PagerParameters pp = new PagerParameters(Team.Tablename, orderby, where, pageindex, pagesize);
            return GetPagerSet2(pp);
        }

        /// <summary>
        /// 根据ID获取Team对象
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public Team GetTeam(int id)
        {
            string sql = string.Format("SELECT * FROM dbo.[Team] with(nolock) WHERE ID={0}", id);
            return Database.ExecuteObject<Team>(sql);
        }
        /// <summary>
        /// 根据where条件获取Team对象
        /// </summary>
        /// <param name="where"></param>
        /// <returns></returns>
        public Team GetTeam(string where)
        {
            return aideTeamProvider.GetObject<Team>(where);
        }
        /// <summary>
        /// Team插入
        /// </summary>
        /// <param name="about"></param>
        /// <returns></returns>
        public int InsertTeam(Team Team)
        {

            return Database.Save(Team);
        }
        /// <summary>
        /// Team更新
        /// </summary>
        /// <param name="about"></param>
        /// <returns></returns>
        public int UpdateTeam(Team Team)
        {
            return Database.Update(Team);
        }
        /// <summary>
        /// 根据ID删除Team数据
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public int DeleteTeam(int id)
        {
            string sql = string.Format("delete from [Team] where id={0}", id);
            return Database.ExecuteNonQuery(sql);
        }
        /// <summary>
        /// Team表中是否存在该记录
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public bool ExistsTeam(int id)
        {
            string sql = string.Format("select id from [Team] where id={0}", id);
            object o = Database.ExecuteScalar(CommandType.Text, sql);
            return o == null ? false : true;
        }
        /// <summary>
        /// Team表中是否存在该记录
        /// </summary>
        /// <param name="where">条件语句，不包括where</param>
        /// <returns></returns>
        public bool ExistsTeam(string where)
        {
            return Database.Exists(Team.Tablename, where);
        }
        #endregion

        #region EducationHistory
        /// <summary>
        /// 获取EducationHistory列表
        /// </summary>
        /// <param name="pageindex"></param>
        /// <param name="pagesize"></param>
        /// <param name="where"></param>
        /// <param name="orderby"></param>
        /// <returns></returns>
        public PagerSet GetEducationHistoryList(int pageindex, int pagesize, string where, string orderby)
        {
            PagerParameters pp = new PagerParameters(EducationHistory.Tablename, orderby, where, pageindex, pagesize);
            return GetPagerSet2(pp);
        }

        /// <summary>
        /// 根据ID获取EducationHistory对象
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public EducationHistory GetEducationHistory(int id)
        {
            string sql = string.Format("SELECT * FROM dbo.[EducationHistory] with(nolock) WHERE ID={0}", id);
            return Database.ExecuteObject<EducationHistory>(sql);
        }
        /// <summary>
        /// 根据where条件获取EducationHistory对象
        /// </summary>
        /// <param name="where"></param>
        /// <returns></returns>
        public EducationHistory GetEducationHistory(string where)
        {
            return aideEducationHistoryProvider.GetObject<EducationHistory>(where);
        }
        /// <summary>
        /// EducationHistory插入
        /// </summary>
        /// <param name="about"></param>
        /// <returns></returns>
        public int InsertEducationHistory(EducationHistory EducationHistory)
        {

            return Database.Save(EducationHistory);
        }
        /// <summary>
        /// EducationHistory更新
        /// </summary>
        /// <param name="about"></param>
        /// <returns></returns>
        public int UpdateEducationHistory(EducationHistory EducationHistory)
        {
            return Database.Update(EducationHistory);
        }
        /// <summary>
        /// 根据ID删除EducationHistory数据
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public int DeleteEducationHistory(int id)
        {
            string sql = string.Format("delete from [EducationHistory] where id={0}", id);
            return Database.ExecuteNonQuery(sql);
        }
        /// <summary>
        /// EducationHistory表中是否存在该记录
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public bool ExistsEducationHistory(int id)
        {
            string sql = string.Format("select id from [EducationHistory] where id={0}", id);
            object o = Database.ExecuteScalar(CommandType.Text, sql);
            return o == null ? false : true;
        }
        /// <summary>
        /// EducationHistory表中是否存在该记录
        /// </summary>
        /// <param name="where">条件语句，不包括where</param>
        /// <returns></returns>
        public bool ExistsEducationHistory(string where)
        {
            return Database.Exists(EducationHistory.Tablename, where);
        }
        #endregion

        #region JobHistory
        /// <summary>
        /// 获取JobHistory列表
        /// </summary>
        /// <param name="pageindex"></param>
        /// <param name="pagesize"></param>
        /// <param name="where"></param>
        /// <param name="orderby"></param>
        /// <returns></returns>
        public PagerSet GetJobHistoryList(int pageindex, int pagesize, string where, string orderby)
        {
            PagerParameters pp = new PagerParameters(JobHistory.Tablename, orderby, where, pageindex, pagesize);
            return GetPagerSet2(pp);
        }

        /// <summary>
        /// 根据ID获取JobHistory对象
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public JobHistory GetJobHistory(int id)
        {
            string sql = string.Format("SELECT * FROM dbo.[JobHistory] with(nolock) WHERE ID={0}", id);
            return Database.ExecuteObject<JobHistory>(sql);
        }
        /// <summary>
        /// 根据where条件获取JobHistory对象
        /// </summary>
        /// <param name="where"></param>
        /// <returns></returns>
        public JobHistory GetJobHistory(string where)
        {
            return aideJobHistoryProvider.GetObject<JobHistory>(where);
        }
        /// <summary>
        /// JobHistory插入
        /// </summary>
        /// <param name="about"></param>
        /// <returns></returns>
        public int InsertJobHistory(JobHistory JobHistory)
        {

            return Database.Save(JobHistory);
        }
        /// <summary>
        /// JobHistory更新
        /// </summary>
        /// <param name="about"></param>
        /// <returns></returns>
        public int UpdateJobHistory(JobHistory JobHistory)
        {
            return Database.Update(JobHistory);
        }
        /// <summary>
        /// 根据ID删除JobHistory数据
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public int DeleteJobHistory(int id)
        {
            string sql = string.Format("delete from [JobHistory] where id={0}", id);
            return Database.ExecuteNonQuery(sql);
        }
        /// <summary>
        /// JobHistory表中是否存在该记录
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public bool ExistsJobHistory(int id)
        {
            string sql = string.Format("select id from [JobHistory] where id={0}", id);
            object o = Database.ExecuteScalar(CommandType.Text, sql);
            return o == null ? false : true;
        }
        /// <summary>
        /// JobHistory表中是否存在该记录
        /// </summary>
        /// <param name="where">条件语句，不包括where</param>
        /// <returns></returns>
        public bool ExistsJobHistory(string where)
        {
            return Database.Exists(JobHistory.Tablename, where);
        }
        #endregion

        #region GuestForCrowdFunding
        /// <summary>
        /// 获取GuestForCrowdFunding列表
        /// </summary>
        /// <param name="pageindex"></param>
        /// <param name="pagesize"></param>
        /// <param name="where"></param>
        /// <param name="orderby"></param>
        /// <returns></returns>
        public PagerSet GetGuestForCrowdFundingList(int pageindex, int pagesize, string where, string orderby)
        {
            PagerParameters pp = new PagerParameters(GuestForCrowdFunding.Tablename, orderby, where, pageindex, pagesize);
            return GetPagerSet2(pp);
        }

        /// <summary>
        /// 根据ID获取GuestForCrowdFunding对象
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public GuestForCrowdFunding GetGuestForCrowdFunding(int id)
        {
            string sql = string.Format("SELECT * FROM dbo.GuestForCrowdFunding with(nolock) WHERE ID={0}", id);
            return Database.ExecuteObject<GuestForCrowdFunding>(sql);
        }
        /// <summary>
        /// 根据where条件获取GuestForCrowdFunding对象
        /// </summary>
        /// <param name="where"></param>
        /// <returns></returns>
        public GuestForCrowdFunding GetGuestForCrowdFunding(string where)
        {
            return aideGuestForCrowdFundingProvider.GetObject<GuestForCrowdFunding>(where);
        }
        /// <summary>
        /// GuestForCrowdFunding插入
        /// </summary>
        /// <param name="about"></param>
        /// <returns></returns>
        public int InsertGuestForCrowdFunding(GuestForCrowdFunding GuestForCrowdFunding)
        {
            return Database.Save(GuestForCrowdFunding);
        }
        /// <summary>
        /// GuestForCrowdFunding更新
        /// </summary>
        /// <param name="about"></param>
        /// <returns></returns>
        public int UpdateGuestForCrowdFunding(GuestForCrowdFunding GuestForCrowdFunding)
        {
            return Database.Update(GuestForCrowdFunding);
        }
        /// <summary>
        /// 根据ID删除GuestForCrowdFunding数据
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public int DeleteGuestForCrowdFunding(int id)
        {
            string sql = string.Format("delete from GuestForCrowdFunding where id={0}", id);
            return Database.ExecuteNonQuery(sql);
        }
        /// <summary>
        /// GuestForCrowdFunding表中是否存在该记录
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public bool ExistsGuestForCrowdFunding(int id)
        {
            string sql = string.Format("select id from GuestForCrowdFunding where id={0}", id);
            object o = Database.ExecuteScalar(CommandType.Text, sql);
            return o == null ? false : true;
        }
        #endregion

        #region Costrecord
        /// <summary>
        /// 获取Costrecord列表
        /// </summary>
        /// <param name="pageindex"></param>
        /// <param name="pagesize"></param>
        /// <param name="where"></param>
        /// <param name="orderby"></param>
        /// <returns></returns>
        public PagerSet GetCostrecordList(int pageindex, int pagesize, string where, string orderby)
        {
            PagerParameters pp = new PagerParameters(Costrecord.Tablename, orderby, where, pageindex, pagesize);
            return GetPagerSet2(pp);
        }

        /// <summary>
        /// 根据ID获取Costrecord对象
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public Costrecord GetCostrecord(int id)
        {
            string sql = string.Format("SELECT * FROM dbo.[Costrecord] with(nolock) WHERE ID={0}", id);
            return Database.ExecuteObject<Costrecord>(sql);
        }
        /// <summary>
        /// 根据where条件获取Costrecord对象
        /// </summary>
        /// <param name="where"></param>
        /// <returns></returns>
        public Costrecord GetCostrecord(string where)
        {
            return aideCostrecordProvider.GetObject<Costrecord>(where);
        }
        /// <summary>
        /// Costrecord插入
        /// </summary>
        /// <param name="about"></param>
        /// <returns></returns>
        public int InsertCostrecord(Costrecord Costrecord)
        {

            return Database.Save(Costrecord);
        }
        /// <summary>
        /// Costrecord更新
        /// </summary>
        /// <param name="about"></param>
        /// <returns></returns>
        public int UpdateCostrecord(Costrecord Costrecord)
        {
            return Database.Update(Costrecord);
        }
        /// <summary>
        /// 根据ID删除Costrecord数据
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public int DeleteCostrecord(int id)
        {
            string sql = string.Format("delete from [Costrecord] where id={0}", id);
            return Database.ExecuteNonQuery(sql);
        }
        /// <summary>
        /// Costrecord表中是否存在该记录
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public bool ExistsCostrecord(int id)
        {
            string sql = string.Format("select id from [Costrecord] where id={0}", id);
            object o = Database.ExecuteScalar(CommandType.Text, sql);
            return o == null ? false : true;
        }
        /// <summary>
        /// Costrecord表中是否存在该记录
        /// </summary>
        /// <param name="where">条件语句，不包括where</param>
        /// <returns></returns>
        public bool ExistsCostrecord(string where)
        {
            return Database.Exists(Costrecord.Tablename, where);
        }
        #endregion

        #region Charge
        /// <summary>
        /// 获取Charge列表
        /// </summary>
        /// <param name="pageindex"></param>
        /// <param name="pagesize"></param>
        /// <param name="where"></param>
        /// <param name="orderby"></param>
        /// <returns></returns>
        public PagerSet GetChargeList(int pageindex, int pagesize, string where, string orderby)
        {
            PagerParameters pp = new PagerParameters(Charge.Tablename, orderby, where, pageindex, pagesize);
            return GetPagerSet2(pp);
        }

        /// <summary>
        /// 根据ID获取Charge对象
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public Charge GetCharge(int id)
        {
            string sql = string.Format("SELECT * FROM dbo.[Charge] with(nolock) WHERE ID={0}", id);
            return Database.ExecuteObject<Charge>(sql);
        }
        /// <summary>
        /// 根据where条件获取Charge对象
        /// </summary>
        /// <param name="where"></param>
        /// <returns></returns>
        public Charge GetCharge(string where)
        {
            return aideChargeProvider.GetObject<Charge>(where);
        }
        /// <summary>
        /// Charge插入
        /// </summary>
        /// <param name="about"></param>
        /// <returns></returns>
        public int InsertCharge(Charge Charge)
        {

            return Database.Save(Charge);
        }
        /// <summary>
        /// Charge更新
        /// </summary>
        /// <param name="about"></param>
        /// <returns></returns>
        public int UpdateCharge(Charge Charge)
        {
            return Database.Update(Charge);
        }
        /// <summary>
        /// 根据ID删除Charge数据
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public int DeleteCharge(int id)
        {
            string sql = string.Format("delete from [Charge] where id={0}", id);
            return Database.ExecuteNonQuery(sql);
        }
        /// <summary>
        /// Charge表中是否存在该记录
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public bool ExistsCharge(int id)
        {
            string sql = string.Format("select id from [Charge] where id={0}", id);
            object o = Database.ExecuteScalar(CommandType.Text, sql);
            return o == null ? false : true;
        }
        /// <summary>
        /// Charge表中是否存在该记录
        /// </summary>
        /// <param name="where">条件语句，不包括where</param>
        /// <returns></returns>
        public bool ExistsCharge(string where)
        {
            return Database.Exists(Charge.Tablename, where);
        }
        #endregion

        #region Order
        /// <summary>
        /// 获取Order列表
        /// </summary>
        /// <param name="pageindex"></param>
        /// <param name="pagesize"></param>
        /// <param name="where"></param>
        /// <param name="orderby"></param>
        /// <returns></returns>
        public PagerSet GetOrderList(int pageindex, int pagesize, string where, string orderby)
        {
            PagerParameters pp = new PagerParameters(Order.Tablename, orderby, where, pageindex, pagesize);
            return GetPagerSet2(pp);
        }

        /// <summary>
        /// 根据ID获取Order对象
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public Order GetOrder(int id)
        {
            string sql = string.Format("SELECT * FROM dbo.[Order] with(nolock) WHERE ID={0}", id);
            return Database.ExecuteObject<Order>(sql);
        }
        /// <summary>
        /// 根据where条件获取Order对象
        /// </summary>
        /// <param name="where"></param>
        /// <returns></returns>
        public Order GetOrder(string where)
        {
            return aideOrderProvider.GetObject<Order>(where);
        }
        /// <summary>
        /// Order插入
        /// </summary>
        /// <param name="about"></param>
        /// <returns></returns>
        public int InsertOrder(Order Order)
        {

            return Database.Save(Order);
        }
        /// <summary>
        /// Order更新
        /// </summary>
        /// <param name="about"></param>
        /// <returns></returns>
        public int UpdateOrder(Order Order)
        {
            return Database.Update(Order);
        }
        /// <summary>
        /// 根据ID删除Order数据
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public int DeleteOrder(int id)
        {
            string sql = string.Format("delete from [Order] where id={0}", id);
            return Database.ExecuteNonQuery(sql);
        }
        /// <summary>
        /// Order表中是否存在该记录
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public bool ExistsOrder(int id)
        {
            string sql = string.Format("select id from [Order] where id={0}", id);
            object o = Database.ExecuteScalar(CommandType.Text, sql);
            return o == null ? false : true;
        }
        /// <summary>
        /// Order表中是否存在该记录
        /// </summary>
        /// <param name="where">条件语句，不包括where</param>
        /// <returns></returns>
        public bool ExistsOrder(string where)
        {
            return Database.Exists(Order.Tablename, where);
        }
        #endregion

        #region 公共

        /// <summary>
        /// 分页获取数据列表
        /// </summary>
        /// <param name="tableName"></param>
        /// <param name="pageIndex"></param>
        /// <param name="pageSize"></param>
        /// <param name="condition"></param>
        /// <param name="orderby"></param>
        /// <returns></returns>
        public PagerSet GetList(string tableName, int pageIndex, int pageSize, string condition, string orderby)
        {
            PagerParameters pagerPrams = new PagerParameters(tableName, orderby, condition, pageIndex, pageSize);
            return GetPagerSet2(pagerPrams);
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
            DataSet ds;
            var parameter = new List<DbParameter>();
            parameter.Add(Database.MakeInParam("tblName", tableName));
            parameter.Add(Database.MakeInParam("fields", fields));
            parameter.Add(Database.MakeInParam("sortfields", sortfield));
            parameter.Add(Database.MakeInParam("singleSortType", singleSortType ? "1" : "0"));
            parameter.Add(Database.MakeInParam("pageSize", pageSize));
            parameter.Add(Database.MakeInParam("pageIndex", pageIndex));
            parameter.Add(Database.MakeInParam("strCondition", condition));
            parameter.Add(Database.MakeOutParam("Counts", typeof(int)));
            Database.RunProc("sp_TablesPageNew", parameter, out ds);
            //SqlParameter p = new SqlParameter("@Counts", SqlDbType.Int);
            //this.OutParams.Add(p);
            //DataTable dt = this.RunDataTable("sp_TablesPageNew");
            //count = (int)p.Value;
            //EventLog.WriteLog(parameter[parameter.Count - 2].ToString());
            //EventLog.WriteLog(parameter[parameter.Count - 2].Value.ToString());
            count = (int)parameter[parameter.Count - 2].Value;
            return ds.Tables[0];
        }

        /// <summary>
        /// 执行SQL语句返回受影响的行数
        /// </summary>
        /// <param name="sql"></param>
        public int ExecuteSql(string sql)
        {
            return Database.ExecuteNonQuery(sql);
        }

        /// <summary>
        ///  执行SQL语句返回DataSet
        /// </summary>
        /// <param name="where"></param>
        /// <returns></returns>
        public DataSet GetDataSetBySql(string sql)
        {
            DataSet ds = Database.ExecuteDataset(sql);
            return ds;
        }
        /// <summary>
        /// 执行sql语句返回datatable
        /// </summary>
        /// <param name="sql"></param>
        /// <returns></returns>
        public DataTable ExecDataTable(string sql)
        {
            DataTable dt = Database.ExecuteDataset(sql).Tables[0];
            return dt;
        }

        /// <summary>
        /// 执行SQL语句返回一个值
        /// </summary>
        /// <param name="sql"></param>
        /// <returns></returns>
        public string GetScalarBySql(string sql)
        {

            return Database.ExecuteScalarToStr(CommandType.Text, sql);
        }

        /// <summary>
        /// 获取sql查询详情
        /// </summary>
        /// <returns></returns>
        public string GetQueryDetail()
        {
            return Database.QueryDetail;
        }


        /// <summary>
        /// 返回一个空表结构
        /// </summary>
        /// <param name="tableName">表名</param>
        /// <returns></returns>
        public DataTable GetEmptyTable(string tableName)
        {
            return Database.GetEmptyTable(tableName);
            //string sql=string.Format("select * from {0} where 1=0",tableName);
            //DbDataReader dr=Database.ExecuteReader(CommandType.Text,sql);
            //return dr.GetSchemaTable();
        }

        #region 事务处理
        /// <summary>
        /// 开始事务
        /// </summary>
        public void Begintran()
        {
            Database.BeginTran();
        }

        /// <summary>
        /// 提交事务
        /// </summary>
        public void CommitTran()
        {
            Database.CommitTran();

        }
        /// <summary>
        /// 回滚事务
        /// </summary>
        public void RollbackTran()
        {
            Database.RollbackTran();
        }

        /// <summary>
        /// 事务执行一个sql语句
        /// </summary>
        /// <param name="sql"></param>
        /// <returns></returns>
        public int ExecuteTrans(string sql, params DbParameter[] param)
        {
            return Database.ExecuteTrans(sql, param);
        }
        /// <summary>
        /// 事务执行一个存储过程
        /// </summary>
        /// <param name="sp"></param>
        /// <returns></returns>
        public int RunTrans(string sp, params DbParameter[] param)
        {
            return Database.RunTrans(sp, param);
        }
        /// <summary>
        /// 事务执行一个sql语句，返回datatable表
        /// </summary>
        /// <param name="sql"></param>
        /// <returns></returns>
        public DataTable ExecDataTableTrans(string sql, params DbParameter[] param)
        {
            return Database.ExecDataTableTrans(sql, param);
        }

        /// <summary>
        /// 事务执行一个存储过程，返回DataTable
        /// </summary>
        /// <param name="sp">存储过程</param>
        /// <returns></returns>
        public DataTable RunDataTableTrans(string sp, params DbParameter[] param)
        {
            return Database.RunDataTableTrans(sp, param);
        }
        /// <summary>
        /// 事务执行一条sql语句，返回DataSet
        /// </summary>
        /// <param name="sql"></param>
        /// <returns></returns>
        public DataSet ExecDataSetTrans(string sql, params DbParameter[] param)
        {
            return Database.ExecDataSetTrans(sql, param);
        }

        /// <summary>
        /// 事务执行一个存储过程，返回DataSet
        /// </summary>
        /// <param name="sp">存储过程</param>
        /// <returns></returns>
        public DataSet RunDataSetTrans(string sp, params DbParameter[] param)
        {
            return Database.RunDataSetTrans(sp, param);
        }
        /// <summary>
        /// 事务执行一条sql语句，返回首行首列
        /// </summary>
        /// <param name="sql">sql语句</param>
        /// <returns></returns>
        public object ExecScalarTrans(string sql, params DbParameter[] param)
        {
            return Database.ExecScalarTrans(sql, param);
        }
        /// <summary>
        /// 事务执行一个存储过程，返回首行首列
        /// </summary>
        /// <param name="sp">存储过程</param>
        /// <returns></returns>
        public object RunScalarTrans(string sp, params DbParameter[] param)
        {
            return Database.RunScalarTrans(sp, param);
        }
        /// <summary>
        /// 事务执行一条sql语句，返回DbDataReader
        /// </summary>
        /// <param name="sql">sql语句</param>
        /// <returns></returns>
        public DbDataReader ExecDataReaderTrans(string sql, params DbParameter[] param)
        {
            return Database.ExecDataReaderTrans(sql, param);
        }
        /// <summary>
        /// 事务执行一个存储过程，返回DbDataReader
        /// </summary>
        /// <param name="sp">存储过程</param>
        /// <returns></returns>
        public DbDataReader RunDataReaderTrans(string sp, params DbParameter[] param)
        {
            return Database.RunDataReaderTrans(sp, param);
        }
        #endregion

        /// <summary>
        /// 运行含有GO命令的多条SQL命令
        /// </summary>
        /// <param name="sql"></param>
        public void ExecuteCommandWithSplitter(string sql)
        {
            Database.ExecuteCommandWithSplitter(sql);
        }

        /// <summary>
        /// 数据库备份命令
        /// </summary>
        /// <param name="databaseName"></param>
        /// <param name="saveName"></param>
        /// <returns></returns>
        public void BackupDatabase(string databaseName,string saveName)
        {
            string sql = string.Format("backup database {0} to disk='{1}' WITH CHECKSUM", databaseName, FileManager.GetRealPath(saveName));
            Database.ExecuteNonQueryByTimeout(sql, 180);
        }
        #endregion
    }
}
