namespace SXF.Utils
{
    using Microsoft.VisualBasic;
    using System;
    using System.Collections.Specialized;
    using System.IO;
    using System.Reflection;
    using System.Security.Cryptography;
    using System.Text;
    using System.Text.RegularExpressions;
    using System.Web;
    public enum RemoveTextMode
    {
        ByteMode ,
        Normal
    }
    /// <summary>
    /// 提供用于处理字符串的方法
    /// </summary>
    public class TextUtility
    {
        private static readonly string PROLONG_SYMBOL = "...";
        private static RNGCryptoServiceProvider rng = new RNGCryptoServiceProvider();

        private TextUtility( )
        {
        }
      
        #region 生成随机数
        /// <summary>
        /// 创建验证码随机字符串(数字和字母)
        /// </summary>
        /// <param name="len">最大长度</param>
        /// <returns>返回指定最大长度的随机字符串</returns>
        public static string CreateAuthStr( int len )
        {
            StringBuilder builder = new StringBuilder();
            Random random = new Random();
            for ( int i = 0; i < len; i++ )
            {
                int num2 = random.Next();
                if ( ( num2 % 2 ) == 0 )
                {
                    builder.Append( ( char )( 0x30 + ( ( ushort )( num2 % 10 ) ) ) );
                }
                else
                {
                    builder.Append( ( char )( 0x41 + ( ( ushort )( num2 % 0x1a ) ) ) );
                }
            }
            return builder.ToString();
        }
        /// <summary>
        /// 创建验证码随机字符串
        /// </summary>
        /// <param name="len">最大长度</param>
        /// <param name="onlyNum">是否纯数字</param>
        /// <returns>返回指定最大长度的随机字符串</returns>
        public static string CreateAuthStr( int len , bool onlyNum )
        {
            if ( !onlyNum )
            {
                return CreateAuthStr( len );
            }
            StringBuilder builder = new StringBuilder();
            Random random = new Random();
            for ( int i = 0; i < len; i++ )
            {
                int num2 = random.Next();
                builder.Append( ( char )( 0x30 + ( ( ushort )( num2 % 10 ) ) ) );
            }
            return builder.ToString();
        }
        /// <summary>
        /// 生成随机字符串
        /// </summary>
        /// <param name="length">目标字符串的长度</param>
        /// <param name="useNum">是否包含数字，1=包含，默认为包含</param>
        /// <param name="useLow">是否包含小写字母，1=包含，默认为包含</param>
        /// <param name="useUpp">是否包含大写字母，1=包含，默认为包含</param>
        /// <param name="useSpe">是否包含特殊字符，1=包含，默认为不包含</param>
        /// <param name="custom">要包含的自定义字符，直接输入要包含的字符列表</param>
        /// <returns>指定长度的随机字符串</returns>
        public static string CreateRandom( int length , int useNum , int useLow , int useUpp , int useSpe , string custom )
        {
            byte[ ] data = new byte[ 4 ];
            new RNGCryptoServiceProvider().GetBytes( data );
            Random random = new Random( BitConverter.ToInt32( data , 0 ) );
            string str = null;
            string str2 = custom;
            if ( useNum == 1 )
            {
                str2 = str2 + "0123456789";
            }
            if ( useLow == 1 )
            {
                str2 = str2 + "abcdefghijklmnopqrstuvwxyz";
            }
            if ( useUpp == 1 )
            {
                str2 = str2 + "ABCDEFGHIJKLMNOPQRSTUVWXYZ";
            }
            if ( useSpe == 1 )
            {
                str2 = str2 + "!\"#$%&'()*+,-./:;<=>?@[\\]^_`{|}~";
            }
            for ( int i = 0; i < length; i++ )
            {
                str = str + str2.Substring( random.Next( 0 , str2.Length - 1 ) , 1 );
            }
            return str;
        }
        /// <summary>
        /// 获取一个由26个小写字母组成的指定长度的随即字符串
        /// </summary>
        /// <param name="len">最大长度</param>
        /// <returns></returns>
        public static string CreateRandomLowercase( int len )
        {
            StringBuilder builder = new StringBuilder();
            Random random = new Random();
            for ( int i = 0; i < len; i++ )
            {
                int num2 = random.Next();
                builder.Append( ( char )( 0x61 + ( ( ushort )( num2 % 0x1a ) ) ) );
            }
            return builder.ToString();
        }
        /// <summary>
        /// 获取指定长度的纯数字随机数字串(以时间做随机种子)
        /// </summary>
        /// <param name="len"></param>
        /// <returns></returns>
        public static string CreateRandomNum( int len )
        {
            StringBuilder builder = new StringBuilder();
            Random random = new Random( ( int )DateTime.Now.Ticks );
            for ( int i = 0; i < len; i++ )
            {
                int num = random.Next();
                builder.Append( ( char )( 0x30 + ( ( ushort )( num % 10 ) ) ) );
            }
            return builder.ToString();
        }

        public static string CreateRandomNum2( int len )
        {
            StringBuilder builder = new StringBuilder();
            Random random = new Random( GetNewSeed() );
            for ( int i = 0; i < len; i++ )
            {
                int num = random.Next();
                builder.Append( ( char )( 0x30 + ( ( ushort )( num % 10 ) ) ) );
            }
            return builder.ToString();
        }
        /// <summary>
        /// 产生随机数的种子
        /// </summary>
        /// <returns></returns>
        public static int GetNewSeed( )
        {
            byte[ ] data = new byte[ 4 ];
            rng.GetBytes( data );
            return BitConverter.ToInt32( data , 0 );
        }
        #endregion

        /// <summary>
        /// 创建指定长度的临时密码
        /// </summary>
        /// <param name="length">临时密码最大长度</param>
        /// <returns></returns>
        public static string CreateTemporaryPassword( int length )
        {
            string str = Guid.NewGuid().ToString( "N" );
            for ( int i = 0; i < ( length / 0x20 ); i++ )
            {
                str = str + Guid.NewGuid().ToString( "N" );
            }
            return str.Substring( 0 , length );
        }

        #region 截取字符串
        /// <summary>
        /// 从给定字符串(originalVal)左侧开始截取指定长度(cutLength)个字符,[使用字节宽度]
        /// </summary>
        /// <param name="originalVal"></param>
        /// <param name="cutLength"></param>
        /// <returns></returns>
        public static string CutLeft( string originalVal , int cutLength )
        {
            if ( string.IsNullOrEmpty( originalVal ) )
            {
                return string.Empty;
            }
            if ( cutLength < 1 )
            {
                return originalVal;
            }
            byte[ ] bytes = Encoding.Default.GetBytes( originalVal );
            if ( bytes.Length <= cutLength )
            {
                return originalVal;
            }
            int length = cutLength;
            int[ ] numArray = new int[ cutLength ];
            byte[ ] destinationArray = null;
            int num2 = 0;
            for ( int i = 0; i < cutLength; i++ )
            {
                if ( bytes[ i ] > 0x7f )
                {
                    num2++;
                    if ( num2 == 3 )
                    {
                        num2 = 1;
                    }
                }
                else
                {
                    num2 = 0;
                }
                numArray[ i ] = num2;
            }
            if ( ( bytes[ cutLength - 1 ] > 0x7f ) && ( numArray[ cutLength - 1 ] == 1 ) )
            {
                length = cutLength + 1;
            }
            destinationArray = new byte[ length ];
            Array.Copy( bytes , destinationArray , length );
            return Encoding.Default.GetString( destinationArray );
        }
        /// <summary>
        /// 从给定字符串(originalVal)右侧开始截取指定长度(cutLength)个字符,[使用方法Substring()]
        /// </summary>
        /// <param name="originalVal"></param>
        /// <param name="cutLength"></param>
        /// <returns></returns>
        public static string CutRight( string originalVal , int cutLength )
        {
            if ( cutLength < 0 )
            {
                cutLength = Math.Abs( cutLength );
            }
            if ( originalVal.Length <= cutLength )
            {
                return originalVal;
            }
            return originalVal.Substring( originalVal.Length - cutLength );
        }

        public static string CutString( string originalVal , int startIndex )
        {
            return CutString( originalVal , startIndex , originalVal.Length );
        }
        /// <summary>
        ///  从给定字符串(originalVal)的(startIndex)索引位置开始截取指定长度(cutLength)的字符串
        /// </summary>
        /// <param name="originalVal"></param>
        /// <param name="startIndex"></param>
        /// <param name="cutLength"></param>
        /// <returns></returns>
        public static string CutString( string originalVal , int startIndex , int cutLength )
        {
            if ( startIndex >= 0 )
            {
                if ( cutLength < 0 )
                {
                    cutLength *= -1;
                    if ( ( startIndex - cutLength ) < 0 )
                    {
                        cutLength = startIndex;
                        startIndex = 0;
                    }
                    else
                    {
                        startIndex -= cutLength;
                    }
                }
                if ( startIndex > originalVal.Length )
                {
                    return "";
                }
            }
            else if ( ( cutLength >= 0 ) && ( ( cutLength + startIndex ) > 0 ) )
            {
                cutLength += startIndex;
                startIndex = 0;
            }
            else
            {
                return "";
            }
            if ( ( originalVal.Length - startIndex ) < cutLength )
            {
                cutLength = originalVal.Length - startIndex;
            }
            try
            {
                return originalVal.Substring( startIndex , cutLength );
            }
            catch
            {
                return originalVal;
            }
        }

        public static string CutStringProlongSymbol( string originalVal , int cutLength )
        {
            if ( originalVal.Length <= cutLength )
            {
                return originalVal;
            }
            else
            {
                return ( CutLeft( originalVal , cutLength ) + PROLONG_SYMBOL );
            }
        }

        public static string CutStringProlongSymbol( string originalVal , int cutLength , string prolongSymbol )
        {
            if ( string.IsNullOrEmpty( prolongSymbol ) )
            {
                prolongSymbol = PROLONG_SYMBOL;
            }
            return ( CutLeft( originalVal , cutLength ) + prolongSymbol );
        }

        public static string CutStringTitle( object content , int cutLength )
        {
            string str = Regex.Replace( content.ToString() , "<[^>]+>" , "" );
            if ( ( str.Length > cutLength ) && ( str.Length > 2 ) )
            {
                str = str.Substring( 0 , cutLength - 2 ) + "...";
            }
            if ( str.IndexOf( "<" ) > -1 )
            {
                str = str.Remove( str.LastIndexOf( "<" ) , str.Length - str.LastIndexOf( "<" ) );
            }
            return str;
        }
        #endregion

       
        
        public static bool EmptyTrimOrNull( string text )
        {
            return ( ( text == null ) || ( text.Trim().Length == 0 ) );
        }
      
        #region IP地址
        /// <summary>
        /// 格式化 IP 地址, fields 3,2,1 保留左起三位，二位，一位
        ///    隐藏IP地址最后一位用*号代替
        /// </summary>
        /// <param name="ip"></param>
        /// <param name="fields"></param>
        /// <returns></returns>
        public static string FormatIP( string ip , int fields )
        {
            if ( string.IsNullOrEmpty( ip ) )
            {
                return "(未记录)";
            }
            if ( fields > 3 )
            {
                return ip;
            }
            if ( ip.Contains( ":" ) )
            {
                return "(不支持ipv6)";
            }
            string[ ] strArray = ip.Split( new char[ ] { '.' } );
            if ( strArray.Length != 4 )
            {
                return "(未记录)";
            }
            if ( fields == 3 )
            {
                return ( strArray[ 0 ] + "." + strArray[ 1 ] + "." + strArray[ 2 ] + ".*" );
            }
            if ( fields == 2 )
            {
                return ( strArray[ 0 ] + "." + strArray[ 1 ] + ".*.*" );
            }
            if ( fields == 1 )
            {
                return ( strArray[ 0 ] + ".*.*.*" );
            }
            return "*.*.*.*";
        }
        #endregion

        #region 邮箱地址
        /// <summary>
        ///  Email 编码
        /// </summary>
        /// <param name="originalStr"></param>
        /// <returns></returns>
        public static string EmailEncode( string originalStr )
        {
            string str = TextEncode( originalStr ).Replace( "@" , "&#64;" ).Replace( "." , "&#46;" );
            return JoinString( "<a href='mailto:" , new string[ ] { str , "'>" , str , "</a>" } );
        }
        /// <summary>
        ///  获取 Email 的主机名称
        /// </summary>
        /// <param name="strEmail">Email 地址</param>
        /// <returns></returns>
        public static string GetEmailHostName( string strEmail )
        {
            if ( string.IsNullOrEmpty( strEmail ) || ( strEmail.IndexOf( "@" ) < 0 ) )
            {
                return string.Empty;
            }
            return strEmail.Substring( strEmail.LastIndexOf( "@" ) + 1 ).ToLower();
        }
        #endregion
        /// <summary>
        /// 格式化货币
        /// </summary>
        /// <param name="money"></param>
        /// <returns></returns>
        public static string FormatMoney( decimal money )
        {
            return money.ToString( "0.00" );
        }

        #region 时间日期
        /// <summary>
        /// 二个时间差了多少天,多少小时的计算 
        /// </summary>
        /// <param name="todate"></param>
        /// <param name="fodate"></param>
        /// <returns></returns>
        public static string[ ] DiffDateAndTime( object todate , object fodate )
        {
            string[ ] strArray = new string[ 2 ];
            TimeSpan span = ( TimeSpan )( DateTime.Parse( todate.ToString() ) - DateTime.Parse( fodate.ToString() ) );
            double num = span.TotalSeconds / 86400.0;
            string str = num.ToString();
            int length = num.ToString().Length;
            int startIndex = num.ToString().LastIndexOf( "." );
            int num4 = ( int )Math.Round( num , 10 );
            int num5 = ( int )( double.Parse( "0" + num.ToString().Substring( startIndex , length - startIndex ) ) * 24.0 );
            strArray[ 0 ] = num4.ToString();
            strArray[ 1 ] = num5.ToString();
            return strArray;
        }
        /// <summary>
        /// 二个时间差了多少天,多少小时的计算 
        /// </summary>
        /// <param name="todate"></param>
        /// <param name="fodate"></param>
        /// <param name="v1"></param>
        /// <param name="v2"></param>
        /// <param name="v3"></param>
        /// <param name="v4"></param>
        /// <param name="v5"></param>
        /// <param name="v6"></param>
        /// <returns></returns>
        public static string DiffDateAndTime( object todate , object fodate , string v1 , string v2 , string v3 , string v4 , string v5 , string v6 )
        {
            TimeSpan span = ( TimeSpan )( DateTime.Parse( todate.ToString() ) - DateTime.Parse( fodate.ToString() ) );
            int num = ( ( int )span.TotalDays ) / 0x16d;
            int num2 = ( int )( ( ( span.TotalDays / 365.0 ) - ( ( int )( span.TotalDays / 365.0 ) ) ) * 12.0 );
            int num3 = ( span.Days - ( num * 0x16d ) ) - ( num2 * 30 );
            int hours = span.Hours;
            int minutes = span.Minutes;
            string str = "";
            if ( 0 != num )
            {
                str = str + num.ToString() + v1;
            }
            if ( 0 != num2 )
            {
                str = str + num2.ToString() + v2;
            }
            if ( 0 != num3 )
            {
                str = str + num3.ToString() + v3;
            }
            if ( 0 != hours )
            {
                str = str + hours.ToString() + v4;
            }
            if ( 0 != minutes )
            {
                str = str + minutes.ToString() + v5;
            }
            if ( ( ( ( num == 0 ) && ( num2 == 0 ) ) && ( ( num3 == 0 ) && ( hours == 0 ) ) ) && ( 0 == minutes ) )
            {
                return v6;
            }
            return str;
        }
        /// <summary>
        /// 计算给定的日期时间距离现在的天数
        /// </summary>
        /// <param name="oneDateTime">要计算的日期对象</param>
        /// <returns></returns>
        public static int DiffDateDays( DateTime oneDateTime )
        {
            TimeSpan span = ( TimeSpan )( DateTime.Now - oneDateTime );
            if ( span.TotalDays > 2147483647.0 )
            {
                return 0x7fffffff;
            }
            if ( span.TotalSeconds < -2147483648.0 )
            {
                return -2147483648;
            }
            return ( int )span.TotalDays;
        }
        /// <summary>
        /// 计算给定的日期时间距离现在的天数
        /// </summary>
        /// <param name="oneDateTime">要计算的日期字符串</param>
        /// <returns></returns>
        public static int DiffDateDays( string oneDateTime )
        {
            if ( string.IsNullOrEmpty( oneDateTime ) )
            {
                return 0;
            }
            return DiffDateDays( DateTime.Parse( oneDateTime ) );
        }

        /// <summary>
        /// 把给定的日期格式化为距现在的模糊时间段，比如 1 分钟前
        /// </summary>
        /// <param name="dateSpan">日期</param>
        /// <returns></returns>
        public static string FormatDateSpan( object dateSpan )
        {
            DateTime time = ( DateTime )dateSpan;
            TimeSpan span = ( TimeSpan )( DateTime.Now - time );
            if ( span.TotalDays >= 365.0 )
            {
                return string.Format( "{0} 年前" , ( int )( span.TotalDays / 365.0 ) );
            }
            if ( span.TotalDays >= 30.0 )
            {
                return string.Format( "{0} 月前" , ( int )( span.TotalDays / 30.0 ) );
            }
            if ( span.TotalDays >= 7.0 )
            {
                return string.Format( "{0} 周前" , ( int )( span.TotalDays / 7.0 ) );
            }
            if ( span.TotalDays >= 1.0 )
            {
                return string.Format( "{0} 天前" , ( int )span.TotalDays );
            }
            if ( span.TotalHours >= 1.0 )
            {
                return string.Format( "{0} 小时前" , ( int )span.TotalHours );
            }
            if ( span.TotalMinutes >= 1.0 )
            {
                return string.Format( "{0} 分钟前" , ( int )span.TotalMinutes );
            }
            return "1 分钟前";
        }
        /// <summary>
        /// 格式化日期输出
        /// 1 ToString
        /// 2 ToShortDateString
        /// 3 yyyy年MM月dd日HH点mm分ss秒
        /// 4 yyyy年MM月dd日
        /// 5 yyyy年MM月dd日HH点mm分
        /// 6 yyyy-MM-dd HH:mm
        /// 7 yy年MM月dd日 HH点mm分
        /// </summary>
        /// <param name="oneDateVal">日期对象</param>
        /// <param name="formatType">输出类型</param>
        /// <returns></returns>
        public static string FormatDateTime( DateTime oneDateVal , int formatType )
        {
            double num = 0.0;
            DateTime time = oneDateVal.AddHours( num );
            switch ( formatType )
            {
                case 2:
                    return time.ToShortDateString();

                case 3:
                    return time.ToString( "yyyy年MM月dd日 HH点mm分ss秒" );

                case 4:
                    return time.ToString( "yyyy年MM月dd日" );

                case 5:
                    return time.ToString( "yyyy年MM月dd日 HH点mm分" );

                case 6:
                    return time.ToString( "yyyy-MM-dd HH:mm" );

                case 7:
                    return time.ToString( "yy年MM月dd日 HH点mm分" );
            }
            return time.ToString();
        }
        /// <summary>
        /// 格式化日期输出
        /// 1 ToString
        /// 2 ToShortDateString
        /// 3 yyyy年MM月dd日HH点mm分ss秒
        /// 4 yyyy年MM月dd日
        /// 5 yyyy年MM月dd日HH点mm分
        /// 6 yyyy-MM-dd HH:mm
        /// 7 yy年MM月dd日 HH点mm分
        /// </summary>
        /// <param name="oneDateVal"></param>
        /// <param name="formatType"></param>
        /// <returns></returns>
        public static string FormatDateTime( string oneDateVal , int formatType )
        {
            return FormatDateTime( DateTime.Parse( oneDateVal ) , formatType );
        }
        /// <summary>
        /// 计算模糊时间段，秒换算为X天X时X分X秒
        /// </summary>
        /// <param name="second">秒数</param>
        /// <returns></returns>
        public static string FormatSecondSpan( long second )
        {
            string str;
            TimeSpan span = TimeSpan.FromSeconds( ( double )second );
            if ( span.Days > 0 )
            {
                str = span.Days.ToString() + "天";
            }
            else
            {
                str = string.Empty;
            }
            if ( span.Hours > 0 )
            {
                str = str + span.Hours.ToString() + "时";
            }
            if ( span.Minutes > 0 )
            {
                str = str + span.Minutes.ToString() + "分";
            }
            if ( span.Seconds > 0 )
            {
                str = str + span.Seconds.ToString() + "秒";
            }
            return str;
        }
        /// <summary>
        /// 获取长日期字符串表示 yyyyMMddHHmmss000
        /// </summary>
        /// <returns></returns>
        public static string GetDateTimeLongString( )
        {
            DateTime now = DateTime.Now;
            return ( now.ToString( "yyyyMMddHHmmss" ) + now.Millisecond.ToString( "000" ) );
        }
        /// <summary>
        /// 获取长日期字符串表示 yyyyMMddHHmmss000,可以添加前缀
        /// </summary>
        /// <param name="prefix">前缀字符</param>
        /// <returns></returns>
        public static string GetDateTimeLongString( string prefix )
        {
            if ( string.IsNullOrEmpty( prefix ) )
            {
                prefix = string.Empty;
            }
            return ( prefix + GetDateTimeLongString() );
        }
        #endregion
        

       
        /// <summary>
        /// 在字符串末尾添加一个字串
        /// </summary>
        /// <param name="originalVal"></param>
        /// <param name="lastStr"></param>
        /// <returns></returns>
        public static string AddLast( string originalVal , string lastStr )
        {
            if ( originalVal.EndsWith( lastStr ) )
            {
                return originalVal;
            }
            return ( originalVal + lastStr );
        }
        public static string GetFullPath( string strPath )
        {
            string str = AddLast( AppDomain.CurrentDomain.BaseDirectory , @"\" );
            if ( strPath.IndexOf( ":" ) < 0 )
            {
                string str2 = strPath.Replace( @"..\" , "" );
                if ( str2 != strPath )
                {
                    int num = ( ( strPath.Length - str2.Length ) / @"..\".Length ) + 1;
                    for ( int i = 0; i < num; i++ )
                    {
                        str = str.Substring( 0 , str.LastIndexOf( @"\" ) );
                    }
                    str2 = @"\" + str2;
                }
                strPath = str + str2;
            }
            return strPath;
        }
        /// <summary>
        /// 获取一个目录的绝对路径（适用于WEB应用程序）
        /// </summary>
        /// <param name="folderPath"></param>
        /// <returns></returns>
        public static string GetMapPath( string folderPath )
        {
            if ( folderPath.IndexOf( @":\" ) > 0 )
            {
                return AddLast( folderPath , @"\" );
            }
            if ( folderPath.StartsWith( "~/" ) )
            {
                return AddLast( HttpContext.Current.Server.MapPath( folderPath ) , @"\" );
            }
            string str2 = HttpContext.Current.Request.ApplicationPath + "/";
            return AddLast( HttpContext.Current.Server.MapPath( str2 + folderPath ) , @"\" );
        }
       

        public static string GetRealPath( string strPath )
        {
            if ( string.IsNullOrEmpty( strPath ) )
            {
                throw new Exception( "strPath 不能为空！" );
            }
            HttpContext current = null;
            try
            {
                current = HttpContext.Current;
            }
            catch
            {
                current = null;
            }
            if ( current != null )
            {
                return current.Server.MapPath( strPath );
            }
            string str2 = Path.Combine( strPath , "" );
            str2 = str2.StartsWith( @"\\" ) ? str2.Remove( 0 , 2 ) : str2;
            return ( AppDomain.CurrentDomain.BaseDirectory + Path.Combine( strPath , "" ) );
        }
    
        /// <summary>
        /// 判断字符串1，是否存在于字符串数组中
        /// </summary>
        /// <param name="matchStr">要匹配的字符串</param>
        /// <param name="strArray">字符串数组</param>
        /// <returns>存在返回是 true,否 false</returns>
        public static bool InArray( string matchStr , string[ ] strArray )
        {
            if ( !string.IsNullOrEmpty( matchStr ) )
            {
                for ( int i = 0; i < strArray.Length; i++ )
                {
                    if ( matchStr == strArray[ i ] )
                    {
                        return true;
                    }
                }
            }
            return false;
        }

        public static bool InArray( string matchStr , string originalStr , string separator )
        {
            string[ ] strArray = SplitStrArray( originalStr , separator );
            for ( int i = 0; i < strArray.Length; i++ )
            {
                if ( matchStr == strArray[ i ] )
                {
                    return true;
                }
            }
            return false;
        }

        public static bool InArray( string matchStr , string[ ] strArray , bool ignoreCase )
        {
            return ( InArrayIndexOf( matchStr , strArray , ignoreCase ) >= 0 );
        }

        public static bool InArray( string matchStr , string strArray , string separator , bool ignoreCase )
        {
            return InArray( matchStr , SplitStrArray( strArray , separator ) , ignoreCase );
        }

        public static int InArrayIndexOf( string originalStr , string[ ] strArray )
        {
            return InArrayIndexOf( originalStr , strArray , true );
        }

        public static int InArrayIndexOf( string originalStr , string[ ] strArray , bool ignoreCase )
        {
            for ( int i = 0; i < strArray.Length; i++ )
            {
                if ( ignoreCase )
                {
                    if ( originalStr.ToLower() == strArray[ i ].ToLower() )
                    {
                        return i;
                    }
                }
                else if ( originalStr == strArray[ i ] )
                {
                    return i;
                }
            }
            return -1;
        }

        public static bool InIPArray( string ip , string[ ] ipArray )
        {
            if ( !string.IsNullOrEmpty( ip ) && Validate.IsIP( ip ) )
            {
                string[ ] strArray = SplitStrArray( ip , "." );
                for ( int i = 0; i < ipArray.Length; i++ )
                {
                    string[ ] strArray2 = SplitStrArray( ipArray[ i ] , "." );
                    int num2 = 0;
                    for ( int j = 0; j < strArray2.Length; j++ )
                    {
                        if ( strArray2[ j ] == "*" )
                        {
                            return true;
                        }
                        if ( ( strArray.Length <= j ) || ( strArray2[ j ] != strArray[ j ] ) )
                        {
                            break;
                        }
                        num2++;
                    }
                    if ( num2 == 4 )
                    {
                        return true;
                    }
                }
            }
            return false;
        }

        public static string JavaScriptEncode( object obj )
        {
            if ( null == obj )
            {
                return string.Empty;
            }
            return JavaScriptEncode( obj.ToString() );
        }

        public static string JavaScriptEncode( string originalStr )
        {
            if ( string.IsNullOrEmpty( originalStr ) )
            {
                return string.Empty;
            }
            StringBuilder builder = new StringBuilder( originalStr );
            return builder.Replace( @"\" , @"\\" ).Replace( "/" , @"\/" ).Replace( "'" , @"\'" ).Replace( "\"" , "\\\"" ).Replace( "\r\n" , "\r" ).Replace( "\r" , @"\r" ).ToString();
        }

        public static string Join( string separator , params string[ ] value )
        {
            return JoinString( separator , value );
        }

        public static string JoinString( params string[ ] value )
        {
            return JoinString( string.Empty , value );
        }

        private static string JoinString( string separator , params string[ ] value )
        {
            if ( null == value )
            {
                throw new ArgumentNullException( "value" );
            }
            if ( 0 == value.Length )
            {
                return string.Empty;
            }
            return string.Join( separator , value );
        }

        public static int Length( string originalVal )
        {
            return Encoding.Default.GetBytes( originalVal ).Length;
        }
      

        public static string RegexReplaceTags( string originalStr , string specialChares , params object[ ] entityClasses )
        {
            string name = "";
            string pattern = "";
            string replacement = "";
            foreach ( object obj2 in entityClasses )
            {
                foreach ( PropertyInfo info in obj2.GetType().GetProperties() )
                {
                    name = info.Name;
                    pattern = specialChares + name + specialChares;
                    replacement = info.GetValue( obj2 , null ).ToString();
                    originalStr = Regex.Replace( originalStr , pattern , replacement , RegexOptions.IgnoreCase );
                }
            }
            return originalStr;
        }
      
        public static string RepeatStr( string repeatStr , int repeatCount )
        {
            StringBuilder builder = new StringBuilder( repeatCount );
            for ( int i = 0; i < repeatCount; i++ )
            {
                builder.Append( repeatStr );
            }
            return builder.ToString();
        }
        /// <summary>
        /// 替换非中文字符
        /// </summary>
        /// <param name="originalVal"></param>
        /// <returns></returns>
        public static string ReplaceCnChar( string originalVal )
        {
            if ( string.IsNullOrEmpty( originalVal ) )
            {
                return string.Empty;
            }
            return Regex.Replace( originalVal , @"[^\u4E00-\u9FA5]" , "" );
        }
        /// <summary>
        /// 替换搜索引擎Lucene指认的特殊字符,<![CDATA["+-,&&,||!(){}[]^"~*?:\"]]>
        /// </summary>
        /// <param name="originalVal"></param>
        /// <returns></returns>
        public static string ReplaceLuceneSpecialChar( string originalVal )
        {
            if ( string.IsNullOrEmpty( originalVal ) )
            {
                return string.Empty;
            }
            StringBuilder builder = new StringBuilder( originalVal );
            builder.Replace( "+" , string.Empty );
            builder.Replace( "-" , string.Empty );
            builder.Replace( "&&" , string.Empty );
            builder.Replace( "||" , string.Empty );
            builder.Replace( "!" , string.Empty );
            builder.Replace( "(" , string.Empty );
            builder.Replace( ")" , string.Empty );
            builder.Replace( "{" , string.Empty );
            builder.Replace( "}" , string.Empty );
            builder.Replace( "[" , string.Empty );
            builder.Replace( "]" , string.Empty );
            builder.Replace( "^" , string.Empty );
            builder.Replace( "\"" , string.Empty );
            builder.Replace( "~" , string.Empty );
            builder.Replace( "*" , string.Empty );
            builder.Replace( "?" , string.Empty );
            builder.Replace( ":" , string.Empty );
            builder.Replace( @"\" , string.Empty );
            return builder.ToString();
        }

        public static string ReplaceStrUseSC( string originalStr , StringCollection sc )
        {
            if ( string.IsNullOrEmpty( originalStr ) )
            {
                return string.Empty;
            }
            foreach ( string str in sc )
            {
                originalStr = Regex.Replace( originalStr , str , "*".PadLeft( str.Length , '*' ) , RegexOptions.IgnoreCase );
            }
            return originalStr;
        }

        public static string ReplaceStrUseSC( string originalStr , string[ ] sc )
        {
            if ( string.IsNullOrEmpty( originalStr ) )
            {
                return string.Empty;
            }
            foreach ( string str in sc )
            {
                originalStr = Regex.Replace( originalStr , str , "*".PadLeft( str.Length , '*' ) , RegexOptions.IgnoreCase );
            }
            return originalStr;
        }

        public static string ReplaceStrUseStr( string originalStr , string replacedStr , string replaceStr )
        {
            if ( string.IsNullOrEmpty( originalStr ) )
            {
                return string.Empty;
            }
            return Regex.Replace( originalStr , replacedStr , replaceStr , RegexOptions.IgnoreCase );
        }
        /// <summary>
        ///  用字符串(separator)把给定的字符串(originalStr)分割成字符数组
        /// </summary>
        /// <param name="originalStr"></param>
        /// <param name="separator"></param>
        /// <returns></returns>
        public static string[ ] SplitStrArray( string originalStr , string separator )
        {
            if ( originalStr.IndexOf( separator ) < 0 )
            {
                return new string[ ] { originalStr };
            }
            return Regex.Split( originalStr , separator.Replace( "." , @"\." ) , RegexOptions.IgnoreCase );
        }
        /// <summary>
        /// 分割文本内容 - 按行分割 <![CDATA[<br />,<p>]]>
        /// </summary>
        /// <param name="originalContent"></param>
        /// <param name="splitLines">分割行数</param>
        /// <returns></returns>
        public static string SplitStrUseLines( string originalContent , int splitLines )
        {
            if ( string.IsNullOrEmpty( originalContent ) )
            {
                return string.Empty;
            }
            string str = string.Empty;
            int startIndex = 1;
            int num2 = 0;
            int num3 = originalContent.Length - 5;
            startIndex = 1;
            while ( startIndex < num3 )
            {
                if ( originalContent.Substring( startIndex , 6 ).ToLower().Equals( "<br />" ) )
                {
                    num2++;
                }
                if ( originalContent.Substring( startIndex , 5 ).ToLower().Equals( "<br/>" ) )
                {
                    num2++;
                }
                if ( originalContent.Substring( startIndex , 4 ).ToLower().Equals( "<br>" ) )
                {
                    num2++;
                }
                if ( originalContent.Substring( startIndex , 3 ).ToLower().Equals( "<p>" ) )
                {
                    num2++;
                }
                if ( num2 >= splitLines )
                {
                    break;
                }
                startIndex++;
            }
            if ( num2 >= splitLines )
            {
                if ( startIndex == num3 )
                {
                    str = originalContent.Substring( 0 , startIndex - 1 );
                }
                else
                {
                    str = originalContent.Substring( 0 , startIndex );
                }
                return str;
            }
            return originalContent;
        }
        /// <summary>
        /// 用字符串(separator)分割给定的字符串(originalStr)
        /// </summary>
        /// <param name="originalStr">原始字符串</param>
        /// <param name="separator">分割字符串</param>
        /// <returns></returns>
        public static string SplitStrUseStr( string originalStr , string separator )
        {
            StringBuilder builder = new StringBuilder();
            builder.Append( separator );
            for ( int i = 0; i < originalStr.Length; i++ )
            {
                builder.Append( originalStr[ i ] );
                builder.Append( separator );
            }
            return builder.ToString();
        }

        public static string SqlEncode( string strSQL )
        {
            if ( string.IsNullOrEmpty( strSQL ) )
            {
                return string.Empty;
            }
            return strSQL.Trim().Replace( "'" , "''" );
        }
        /// <summary>
        ///  文本解码
        /// </summary>
        /// <param name="originalStr"></param>
        /// <returns></returns>
        public static string TextDecode( string originalStr )
        {
            StringBuilder builder = new StringBuilder( originalStr );
            builder.Replace( "<br/><br/>" , "\r\n" );
            builder.Replace( "<br/>" , "\r" );
            builder.Replace( "<p></p>" , "\r\n\r\n" );
            return builder.ToString();
        }
        /// <summary>
        /// 文本编码,回车/换行符 到 HTML 转换
        /// </summary>
        /// <param name="originalStr"></param>
        /// <returns></returns>
        public static string TextEncode( string originalStr )
        {
            if ( string.IsNullOrEmpty( originalStr ) )
            {
                return string.Empty;
            }
            StringBuilder builder = new StringBuilder( originalStr );
            builder.Replace( "\r\n" , "<br />" );
            builder.Replace( "\n" , "<br />" );
            return builder.ToString();
        }

        /// <summary>
        ///  转换字符串首字符为小写字符(对英文字符有效)
        /// </summary>
        /// <param name="originalVal"></param>
        /// <returns></returns>
        public static string TransformFirstToLower( string originalVal )
        {
            if ( string.IsNullOrEmpty( originalVal ) )
            {
                return originalVal;
            }
            if ( originalVal.Length >= 2 )
            {
                return ( originalVal.Substring( 0 , 1 ).ToLower() + originalVal.Substring( 1 ) );
            }
            return originalVal.ToUpper();
        }
        /// <summary>
        /// 转换字符串首字符为大写字符(对英文字符有效)
        /// </summary>
        /// <param name="originalVal">原始字符串</param>
        /// <returns></returns>
        public static string TransformFirstToUpper( string originalVal )
        {
            if ( string.IsNullOrEmpty( originalVal ) )
            {
                return originalVal;
            }
            if ( originalVal.Length >= 2 )
            {
                return ( originalVal.Substring( 0 , 1 ).ToUpper() + originalVal.Substring( 1 ) );
            }
            return originalVal.ToUpper();
        }

        /// <summary>
        /// unescape
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        public static string UnEscape(string str)
        {
            if (str == null)
                return String.Empty;

            StringBuilder sb = new StringBuilder();
            int len = str.Length;
            int i = 0;
            while (i != len)
            {
                if (Uri.IsHexEncoding(str, i))
                    sb.Append(Uri.HexUnescape(str, ref i));
                else
                    sb.Append(str[i++]);
            }

            return sb.ToString();
        }  
    }
}

