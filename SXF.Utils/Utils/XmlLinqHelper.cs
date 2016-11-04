using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Linq;


namespace SXF.Utils
{
    /// <summary>
    /// 利用linq方法处理xml对象类
    /// </summary>
    public class XmlLinqHelper
    {


        /// <summary>
        /// 把对象转化为一个xml元素
        /// </summary>
        /// <returns>一个构建好的包含航段信息的xml元素</returns>
        //public XElement ToXml()
        //{
        //    return new XElement("Voyage",
        //        new XAttribute("Airway", Airway),
        //        new XAttribute("FlightNumber", FlightNumber),
        //        new XAttribute("Cabin", Cabin),
        //        new XAttribute("DepartureAirport", DepartureAirport),
        //        new XAttribute("ArrivalAirport", ArrivalAirport),
        //        new XAttribute("DepartureDatetime", DepartureDatetime),
        //        new XAttribute("ArrivalDatetime", ArrivalDatetime)
        //        );
        //}

        /// <summary>
        ///从xml元素获取航段对象
        /// </summary>
        /// <param name="ele">包含航段信息的xml元素</param>
        /// <returns>新的航段对象</returns>
        //public static Voyage FromXml(XElement ele)
        //{
        //    return new Voyage()
        //    {
        //        Airway = ele.Attribute("Airway").Value,
        //        FlightNumber = ele.Attribute("FlightNumber").Value,
        //        Cabin = ele.Attribute("Cabin").Value,
        //        DepartureAirport = ele.Attribute("DepartureAirport").Value,
        //        ArrivalAirport = ele.Attribute("ArrivalAirport").Value,
        //        DepartureDatetime = ele.Attribute("DepartureDatetime").Value,
        //        ArrivalDatetime = ele.Attribute("ArrivalDatetime").Value
        //    };

        //}

        /// <summary>
        /// 把xml对象存到相对应的xml文件中
        /// </summary>
        /// <param name="xElement">XElement对象</param>
        /// <param name="xmlAbsolutePath">xml文件的绝对路径</param>
        public void Save(XElement xElement, string xmlAbsolutePath)
        {
            xElement.Save(xmlAbsolutePath);
            //xElement.Save(
             
        }
        /// <summary>
        /// 把xml对象存到System.IO.TextWriter中
        /// </summary>
        /// <param name="xElement"></param>
        /// <param name="textWriter"></param>
        public void Save(XElement xElement, System.IO.TextWriter textWriter)
        {
            xElement.Save(textWriter);
            //xElement.Save(System.Xml.XmlWriter
        }
        /// <summary>
        /// 把xml对象存到System.Xml.XmlWriter中
        /// </summary>
        /// <param name="xElement"></param>
        /// <param name="xmlWriter"></param>
        public void Save(XElement xElement, System.Xml.XmlWriter xmlWriter)
        {
            xElement.Save(xmlWriter); 
            
        }

       
    }
}
