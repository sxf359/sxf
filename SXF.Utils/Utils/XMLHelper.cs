using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;
using System.Data;
using System.IO;
using System.Web;

namespace SXF.Utils
{
    public class XMLHelper
    {
        #region 私有变量
        /// <summary>
        /// 文件路径
        /// </summary>
        private string xmlFilePath;
        /// <summary>
        /// 文件路径类型
        /// </summary>
        private enumXmlPathType xmlFilePathType;
        /// <summary>
        /// 实例化一个XmlDocument对象
        /// </summary>
        private XmlDocument xmlDoc = new XmlDocument();
        #endregion

        #region 私有函数
        /// <summary>
        /// 创建xml文件
        /// </summary>
        /// <param name="xmlName">文件路径</param>
        /// <returns></returns>
        private bool CreateXML(string xmlName)
        {
            try
            {
                XmlTextWriter xmltx = null;
                if (string.IsNullOrEmpty(xmlName))
                {
                    xmltx = new XmlTextWriter(HttpContext.Current.Server.MapPath("Nimeux.xml"), Encoding.UTF8);
                }
                else
                {
                    xmltx = new XmlTextWriter(xmlName, Encoding.UTF8);
                }
                xmltx.WriteStartDocument();
                xmltx.WriteStartElement("Nimeux");
                xmltx.WriteEndElement();
                xmltx.WriteEndDocument();
                xmltx.Close();
                return true;
            }
            catch { return false; }
        }
        /// <summary>
        /// 获取XmlDocument实例化对象
        /// </summary>
        /// <param name="strEntityTypeName">实体类的名称</param>
        /// <returns>指定的XML描述文件的路径</returns>
        private XmlDocument GetXmlDocument()
        {
            XmlDocument doc = null;

            if (this.xmlFilePathType == enumXmlPathType.AbsolutePath)
            {
                doc = GetXmlDocumentFromFile(xmlFilePath);
            }
            else if (this.xmlFilePathType == enumXmlPathType.VirtualPath)
            {
                doc = GetXmlDocumentFromFile(HttpContext.Current.Server.MapPath(xmlFilePath));
            }
            return doc;
        }
        /// <summary>
        /// 获取XML文件xmldocument对象
        /// </summary>
        /// <param name="tempXmlFilePath">xml文件绝对路径</param>
        /// <returns></returns>
        private XmlDocument GetXmlDocumentFromFile(string tempXmlFilePath)
        {
            try
            {
                string xmlFileFullPath = tempXmlFilePath;
                xmlDoc.Load(xmlFileFullPath);
                return xmlDoc;
            }
            catch
            {
                if (CreateXML(tempXmlFilePath))
                {
                    return GetXmlDocumentFromFile(tempXmlFilePath);
                }
                else
                {
                    return null;
                }
            }
        }
        /// <summary>
        /// 获取根节点路径
        /// </summary>
        /// <returns></returns>
        private string GetPath()
        {
            string root = xmlDoc.DocumentElement.Name;
            string result = root + "[last()]";
            return result;
        }
        #endregion

        #region 共有属性及枚举、方法
        /// <summary>
        /// 获取根节点路径
        /// </summary>
        public string Path
        {
            get { return GetPath(); }
        }
        /// <summary>
        /// 获取根节点名称
        /// </summary>
        public string RootName
        {
            get { return xmlDoc.DocumentElement.Name; }
        }
        /// <summary>
        /// 获取根节点
        /// </summary>
        public XmlNode Root
        {
            get { return xmlDoc.DocumentElement; }
        }
        /// <summary>
        /// 枚举，XML文件的路径
        /// </summary>
        public enum enumXmlPathType
        {
            AbsolutePath,
            VirtualPath
        }
        /// <summary>
        /// 设置XML文件路径属性
        /// </summary>
        public string XmlFilePath
        {
            set
            {
                xmlFilePath = value;
            }
        }
        /// <summary>
        /// 设置文件路径类型
        /// </summary>
        public enumXmlPathType XmlFilePathTyp
        {
            set
            {
                xmlFilePathType = value;
            }
        }
        #endregion

        #region 构造函数
        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="tempXmlFilePath">XML文件相对路径</param>
        public XMLHelper(string tempXmlFilePath)
        {
            this.xmlFilePathType = enumXmlPathType.VirtualPath;
            this.xmlFilePath = tempXmlFilePath;
            GetXmlDocument();
        }
        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="tempXmlFilePath">XML文件路径</param>
        /// <param name="tempXmlFilePathType">文件路径类型</param>
        public XMLHelper(string tempXmlFilePath, enumXmlPathType tempXmlFilePathType)
        {
            this.xmlFilePathType = tempXmlFilePathType;
            this.xmlFilePath = tempXmlFilePath;
            GetXmlDocument();
        }
        #endregion

        #region 读取指定节点的指定属性值
        /// <summary>
        /// 获取指定节点的属性
        /// </summary>
        /// <param name="NodeIndex">节点索引</param>
        /// <param name="strNodePath">节点路径</param>
        /// <param name="strAttribute">属性名称</param>
        /// <returns></returns>
        public string GetXmlNodeAttribute(int NodeIndex, string strNodePath, string strAttribute)
        {
            string strReturn = "";
            try
            {
                if (NodeIndex != -1)
                {
                    //根据指定路径获取节点列表
                    XmlNodeList ndlist = xmlDoc.SelectNodes(strNodePath);
                    //获取指定节点属性
                    XmlAttributeCollection xmlAttr = ndlist[NodeIndex].Attributes;
                    if (xmlAttr.Count != 0)
                    {
                        //获取节点属性列表中的指定属性名称的属性值
                        if (!string.IsNullOrEmpty(strAttribute))
                        {
                            for (int i = 0; i < xmlAttr.Count; i++)
                            {
                                if (xmlAttr.Item(i).Name == strAttribute)
                                    strReturn = xmlAttr.Item(i).Value;
                            }
                        }
                        else
                        {
                            strReturn = xmlAttr.Item(0).Value;
                        }
                    }
                    else
                    {
                        return "";
                    }
                }
                else
                {
                    //根据指定路径获取节点
                    XmlNode xmlNode = xmlDoc.SelectSingleNode(strNodePath);
                    //获取节点的属性
                    XmlAttributeCollection xmlAttr = xmlNode.Attributes;
                    if (xmlAttr.Count != 0)
                    {
                        //获取节点属性列表中的指定属性名称的属性值
                        if (!string.IsNullOrEmpty(strAttribute))
                        {
                            for (int i = 0; i < xmlAttr.Count; i++)
                            {
                                if (xmlAttr.Item(i).Name == strAttribute)
                                    strReturn = xmlAttr.Item(i).Value;
                            }
                        }
                        else
                        {
                            strReturn = xmlAttr.Item(0).Value;
                        }
                    }
                    else
                    {
                        return "";
                    }
                }
            }
            catch (XmlException xmle)
            {
                throw xmle;
            }
            return strReturn;
        }
        /// <summary>
        /// 获取指定节点的属性
        /// </summary>
        /// <param name="NodeIndex">节点索引</param>
        /// <param name="strNodePath">节点路径</param>
        /// <param name="AttributeIndex">属性索引</param>
        /// <returns>属性值</returns>
        public string GetXmlNodeAttribute(int NodeIndex, string strNodePath, int AttributeIndex)
        {
            string strReturn = "";
            try
            {
                if (NodeIndex != -1)
                {
                    //根据指定路径获取节点列表
                    XmlNodeList ndlist = xmlDoc.SelectNodes(strNodePath);
                    //获取指定节点属性
                    XmlAttributeCollection xmlAttr = ndlist[NodeIndex].Attributes;
                    //获取节点属性列表中的指定的属性值
                    strReturn = xmlAttr.Item(AttributeIndex).Value;
                }
                else
                {
                    //根据指定路径获取节点
                    XmlNode xmlNode = xmlDoc.SelectSingleNode(strNodePath);
                    //获取节点的属性
                    XmlAttributeCollection xmlAttr = xmlNode.Attributes;
                    //获取节点属性列表中的指定属性名称的属性值
                    strReturn = xmlAttr.Item(AttributeIndex).Value;
                }
            }
            catch (XmlException xmle)
            {
                throw xmle;
            }
            return strReturn;
        }
        /// <summary>
        /// 获取指定节点的属性
        /// </summary>
        /// <param name="NodeIndex">节点索引</param>
        /// <param name="strNodePath">节点路径</param>
        /// <returns>属性值</returns>
        public string GetXmlNodeAttribute(int NodeIndex, string strNodePath)
        {
            return GetXmlNodeAttribute(NodeIndex, strNodePath, "");
        }
        /// <summary>
        /// 获取节点的属性
        /// </summary>
        /// <param name="strNodePath">节点路径</param>
        /// <returns>属性值</returns>
        public string GetXmlNodeAttribute(string strNodePath)
        {
            return GetXmlNodeAttribute(-1, strNodePath);
        }
        /// <summary>
        /// 获取节点的指定的属性值
        /// </summary>
        /// <param name="strNodePath">节点路径</param>
        /// <param name="strAttributeName">属性名称</param>
        /// <returns>属性值</returns>
        public string GetXmlNodeAttribute(string strNodePath, string strAttributeName)
        {
            return GetXmlNodeAttribute(-1, strNodePath, strAttributeName);
        }
        /// <summary>
        /// 获取节点的指定的属性值
        /// </summary>
        /// <param name="strNodePath">节点路径</param>
        /// <param name="AttributeIndex">节点索引</param>
        /// <returns>属性值</returns>
        public string GetXmlNodeAttribute(string strNodePath, int AttributeIndex)
        {
            return GetXmlNodeAttribute(-1, strNodePath, AttributeIndex);
        }
        #endregion

        #region 设置节点的属性值
        /// <summary>
        /// 设置一个指定节点的指定属性的值
        /// </summary>
        /// <param name="NodeIndex">节点索引</param>
        /// <param name="xmlNodePath">节点路径</param>
        /// <param name="AttributeName">属性名称</param>
        /// <param name="AttributeValue">属性值</param>
        /// <returns></returns>
        public bool SetXmlNodeAttribute(int NodeIndex, string xmlNodePath, string AttributeName, string AttributeValue)
        {
            string temp = string.Format(AttributeName);
            try
            {
                //是否有多个相同的节点
                if (NodeIndex != -1)
                {
                    //获取节点列表
                    XmlNodeList ndlist = xmlDoc.SelectNodes(xmlNodePath);
                    //获取属性列表
                    XmlAttributeCollection xmlAttr = ndlist[NodeIndex].Attributes;
                    //属性是否为空
                    if (xmlAttr != null)
                    {
                        //是否指定属性名称(也就是是否节点是唯一属性)
                        if (!string.IsNullOrEmpty(AttributeName))
                        {
                            //获取属性相对应的属性名次，并设置属性名
                            for (int i = 0; i < xmlAttr.Count; i++)
                            {
                                if (xmlAttr.Item(i).Name == AttributeName)
                                {
                                    xmlAttr.Item(i).Value = AttributeValue;
                                    break;
                                }
                            }
                        }
                        else
                        {
                            //没有制定属性名的情况下返回第一个属性
                            xmlAttr.Item(0).Value = AttributeValue;
                        }
                        return true;
                    }
                    else
                    {
                        return false;
                    }
                }
                else
                {
                    //根据指定路径获取节点
                    XmlNode xmlNode = xmlDoc.SelectSingleNode(xmlNodePath);
                    //获取节点的属性，并循环取出需要的属性值
                    XmlAttributeCollection xmlAttr = xmlNode.Attributes;
                    //属性是否为空
                    if (xmlAttr != null)
                    {
                        //是否指定属性名称(也就是是否节点是唯一属性)
                        if (!string.IsNullOrEmpty(AttributeName))
                        {
                            //获取属性相对应的属性名次，并设置属性名
                            for (int i = 0; i < xmlAttr.Count; i++)
                            {
                                if (xmlAttr.Item(i).Name == AttributeName)
                                {
                                    xmlAttr.Item(i).Value = AttributeValue;
                                    break;
                                }
                            }
                        }
                        else
                        {
                            //没有制定属性名的情况下返回第一个属性
                            xmlAttr.Item(0).Value = AttributeValue;
                        }
                        return true;
                    }
                    else
                    {
                        return true;
                    }
                }
            }
            catch (XmlException xmle)
            {
                throw xmle;
            }
        }
        /// <summary>
        /// 设置一个节点指定属性的值
        /// </summary>
        /// <param name="xmlNodePath">节点路径</param>
        /// <param name="AttributeName">属性名称</param>
        /// <param name="AttributeValue">属性值</param>
        /// <returns>bool值</returns>
        public bool SetXmlNodeAttribute(string xmlNodePath, string AttributeName, string AttributeValue)
        {
            return SetXmlNodeAttribute(-1, xmlNodePath, AttributeName, AttributeValue);
        }
        /// <summary>
        /// 设置节点的唯一属性的值
        /// </summary>
        /// <param name="xmlNodePath">节点路径</param>
        /// <param name="AttributeValue">属性值</param>
        /// <returns></returns>
        public bool SetXmlNodeAttribute(string xmlNodePath, string AttributeValue)
        {
            return SetXmlNodeAttribute(-1, xmlNodePath, "", AttributeValue);
        }
        #endregion

        #region 添加属性
        /// <summary>
        /// 向指定的一个节点添加指定值得属性
        /// </summary>
        /// <param name="NodeIndex">节点索引</param>
        /// <param name="NodePath">节点路径</param>
        /// <param name="AttributeName">添加的属性</param>
        /// <param name="AttributeValue">要添加的属性值</param>
        /// <returns>bool值</returns>
        public bool AddAttribute(int NodeIndex, string NodePath, string AttributeName, string AttributeValue)
        {
            try
            {
                //是否存在多个相同的节点名称
                if (NodeIndex != -1)
                {
                    //获取节点列表
                    XmlNodeList ndlist = xmlDoc.SelectNodes(NodePath);
                    //创建一个属性
                    XmlAttribute nodeAttri = xmlDoc.CreateAttribute(AttributeName);
                    //如果属性值不为空，则添加属性值
                    if (!string.IsNullOrEmpty(AttributeValue))
                    {
                        nodeAttri.Value = AttributeValue;
                    }
                    //将属性添加到节点
                    ndlist[NodeIndex].Attributes.Append(nodeAttri);
                    return true;
                }
                else
                {
                    //获取节点
                    XmlAttribute nodeAttri = xmlDoc.CreateAttribute(AttributeName);
                    //如果属性值不为空，则添加属性值
                    if (string.IsNullOrEmpty(AttributeValue))
                    {
                        nodeAttri.Value = AttributeValue;
                    }
                    XmlNode nodePath = xmlDoc.SelectSingleNode(NodePath);
                    nodePath.Attributes.Append(nodeAttri);
                    return true;
                }
            }
            catch { return false; }
        }
        /// <summary>
        /// 给一个节点添加属性值
        /// </summary>
        /// <param name="NodePath">节点路径</param>
        /// <param name="AttributeName">属性名称</param>
        /// <returns>bool值</returns>
        public bool AddAttribute(string NodePath, string AttributeName)
        {
            return AddAttribute(-1, NodePath, AttributeName);
        }
        /// <summary>
        /// 给一个指定节点添加属性值
        /// </summary>
        /// <param name="NodeIndex">节点索引</param>
        /// <param name="NodePath">节点路径</param>
        /// <param name="AttributeName">属性名称</param>
        /// <returns>bool值</returns>
        public bool AddAttribute(int NodeIndex, string NodePath, string AttributeName)
        {
            return AddAttribute(NodeIndex, NodePath, AttributeName, "");
        }
        #endregion

        #region 删除属性
        /// <summary>
        /// 删除一个指定节点的指定值的属性
        /// </summary>
        /// <param name="NodeIndex">节点索引</param>
        /// <param name="NodePath">节点路径</param>
        /// <param name="AttributeName">属性名称</param>
        /// <param name="AttributeValue">属性值</param>
        /// <returns>bool值</returns>
        public bool DeleteAttribute(int NodeIndex, string NodePath, string AttributeName, string AttributeValue)
        {
            try
            {
                //是否有相同的节点名称
                if (NodeIndex != -1)
                {
                    //获取节点列表
                    XmlNodeList ndlist = xmlDoc.SelectNodes(NodePath);
                    //获取指定列中的节点元素
                    XmlElement xe = (XmlElement)ndlist[NodeIndex];
                    //属性值是否为空
                    if (!string.IsNullOrEmpty(AttributeValue))
                    {
                        //删除指定值得属性
                        if (xe.GetAttribute(AttributeName) == AttributeValue)
                        {
                            xe.RemoveAttribute(AttributeName);
                        }
                    }
                    else
                    {
                        xe.RemoveAttribute(AttributeName);
                    }
                    return true;
                }
                else
                {
                    //获取节点
                    XmlNode xn = xmlDoc.SelectSingleNode(NodePath);
                    //获取列中的元素
                    XmlElement xe = (XmlElement)xn;
                    //属性值是否为空
                    if (!string.IsNullOrEmpty(AttributeValue))
                    {
                        //删除指定值得属性
                        if (xe.GetAttribute(AttributeName) == AttributeValue)
                        {
                            xe.RemoveAttribute(AttributeName);
                        }
                    }
                    else
                    {
                        xe.RemoveAttribute(AttributeName);
                    }
                    return true;
                }
            }
            catch { return false; }
        }
        /// <summary>
        /// 删除一个节点的指定值得属性
        /// </summary>
        /// <param name="NodePath">节点路径</param>
        /// <param name="AttributeName">属性名称</param>
        /// <param name="AttributeValue">属性值</param>
        /// <returns>bool值</returns>
        public bool DeleteAttribute(string NodePath, string AttributeName, string AttributeValue)
        {
            return DeleteAttribute(-1, NodePath, AttributeName, AttributeValue);
        }
        /// <summary>
        /// 删除一个节点的一个属性
        /// </summary>
        /// <param name="NodePath">节点路径</param>
        /// <param name="AttributeName">属性名称</param>
        /// <returns>bool值</returns>
        public bool DeleteAttribute(string NodePath, string AttributeName)
        {
            return DeleteAttribute(NodePath, AttributeName, "");
        }
        /// <summary>
        /// 删除一个指定的节点的属性
        /// </summary>
        /// <param name="NodeIndex">节点索引</param>
        /// <param name="NodePath">节点路径</param>
        /// <param name="AttributeName">属性名</param>
        /// <returns>bool值</returns>
        public bool DeleteAttribute(int NodeIndex, string NodePath, string AttributeName)
        {
            return DeleteAttribute(NodeIndex, NodePath, AttributeName, "");
        }
        #endregion

        #region 获取节点值
        /// <summary>
        /// 获取指定节点的值
        /// </summary>
        /// <param name="Index">指定节点标记</param>
        /// <param name="strNodePath">节点路径</param>
        /// <param name="childNodeName">指定节点下子节点的名称</param>
        /// <returns>bool值</returns>
        public string GetXmlNodeValue(int Index, string strNodePath, string childNodeName)
        {
            string strReturn = String.Empty;
            try
            {
                if (!string.IsNullOrEmpty(childNodeName))
                {
                    if (Index != -1)
                    {
                        //获取所有父节点下相同的节点并生成列表
                        XmlNodeList ndlist = xmlDoc.SelectNodes(strNodePath + "/" + childNodeName);
                        //获取列表中指定Index的节点值
                        strReturn = ndlist[Index].InnerText;
                    }
                    else
                    {
                        //根据路径获取节点
                        XmlNode xmlNode = xmlDoc.SelectSingleNode(strNodePath + "/" + childNodeName);
                        strReturn = xmlNode.InnerText;
                    }
                }
                else
                {
                    if (Index != -1)
                    {
                        //获取所有父节点下相同的节点并生成列表
                        XmlNodeList ndlist = xmlDoc.SelectNodes(strNodePath);
                        //获取列表中指定Index的节点值
                        strReturn = ndlist[Index].InnerText;
                    }
                    else
                    {
                        //根据路径获取节点
                        XmlNode xmlNode = xmlDoc.SelectSingleNode(strNodePath);
                        strReturn = xmlNode.InnerText;
                    }
                }
                return strReturn;
            }
            catch
            {
                return null;
            }
        }
        /// <summary>
        /// 获取节点的值
        /// </summary>
        /// <param name="strNodePath">节点路径</param>
        /// <returns>bool值</returns>
        public string GetXmlNodeValue(string strNodePath)
        {
            return GetXmlNodeValue(-1, strNodePath);
        }
        /// <summary>
        /// 获取指定节点的值
        /// </summary>
        /// <param name="Index">指定节点标记</param>
        /// <param name="strNodePath">节点路径</param>
        /// <returns>bool值</returns>
        public string GetXmlNodeValue(int Index, string strNodePath)
        {
            return GetXmlNodeValue(Index, strNodePath, "");
        }
        #endregion

        #region 设置节点值
        /// <summary>
        /// 设置节点值
        /// </summary>
        /// <param name="index">节点序号</param>
        /// <param name="xmlNodePath">节点路径</param>
        /// <param name="xmlNodeValue">节点值</param> 
        /// <returns>bool值</returns>        
        public bool SetXmlNodeValue(int index, string xmlNodePath, string xmlNodeValue)
        {
            try
            {
                //如果没有指定Index的值，则设置一个节点的值
                if (index != -1)
                {
                    //获取父节点下所有的相同节点
                    XmlNodeList ndlist = xmlDoc.SelectNodes(xmlNodePath);
                    //获取节点列表中的一个节点
                    XmlNode xn = ndlist[index];
                    //设置一个节点值
                    xn.InnerText = xmlNodeValue;
                }
                else
                {
                    //获取节点并设置一个值
                    XmlNode xn = xmlDoc.SelectSingleNode(xmlNodePath);
                    xn.InnerText = xmlNodeValue;
                }
                return true;
            }
            catch
            {
                return false;
            }
        }
        /// <summary>
        /// 设置一个节点的值
        /// </summary>
        /// <param name="xmlNodePath">节点路径</param>
        /// <param name="xmlNodeValue">节点值</param>
        /// <returns>bool值</returns>
        public bool SetXmlNodeValue(string xmlNodePath, string xmlNodeValue)
        {
            return SetXmlNodeValue(-1, xmlNodePath, xmlNodeValue);
        }
        #endregion

        #region 添加节点
        /// <summary>
        /// 在根节点下添加父节点
        /// </summary>
        /// <param name="parentNode">父节点名称</param>
        public bool AddParentNode(string parentNode)
        {
            try
            {
                XmlNode root = xmlDoc.DocumentElement;
                XmlNode parentXmlNode = xmlDoc.CreateElement(parentNode);
                root.AppendChild(parentXmlNode);
                //SaveXmlDocument();
                return true;
            }
            catch { return false; }
        }

        /// <summary>
        /// 在指定父节点下插入一个指定数值、指定属性的子节点
        /// </summary>
        /// <param name="parentNodePath">父节点路径</param>
        /// <param name="childNodeName">子节点名称</param>
        /// <param name="nodevalue">子节点值</param>
        /// <param name="nodeAttributeName">子节点属性名称</param>
        /// <param name="nodeAttributeValue">子节点属性值</param>
        /// <returns>bool值</returns>
        public bool AddChildNode(string parentNodePath, string childNodeName, string nodevalue, string nodeAttributeName, string nodeAttributeValue)
        {
            try
            {
                XmlNodeList ndlist = xmlDoc.SelectNodes(parentNodePath);
                int len = ndlist.Count;
                foreach (XmlNode xn in ndlist)
                {
                    XmlNode childXmlNode = xmlDoc.CreateElement(childNodeName);
                    xn.AppendChild(childXmlNode);
                    XmlElement xnElm = (XmlElement)childXmlNode;
                    if ((!string.IsNullOrEmpty(nodeAttributeName)) && (!string.IsNullOrEmpty(nodeAttributeValue)))
                    {
                        xnElm.SetAttribute(nodeAttributeName, nodeAttributeValue);
                    }
                    if (!string.IsNullOrEmpty(nodevalue))
                    {
                        xnElm.InnerText = nodevalue;
                    }
                }
                //SaveXmlDocument();
                return true;
            }
            catch { return false; }
        }
        /// <summary>
        /// 在指定的父节点下插入一个子节点
        /// </summary>
        /// <param name="childNodeName">父节点路径</param>
        /// <param name="childNodeName">子节点名称</param>
        /// <returns>bool值</returns>
        public bool AddChildNode(string childNodePath, string childNodeName)
        {
            return AddChildNode(childNodePath, childNodeName, "", "", "");
        }
        /// <summary>
        /// 在指定的父节点下出入一个指定属性的子节点
        /// </summary>
        /// <param name="parentNodePath">父节点路径</param>
        /// <param name="childNodeName">子节点名称</param>
        /// <param name="nodeAttributeName">子节点属性名称</param>
        /// <param name="nodeAttributeValue">子节点属性值</param>
        /// <returns>bool值</returns>
        public bool AddChildNode(string parentNodePath, string childNodeName, string nodeAttributeName, string nodeAttributeValue)
        {
            return AddChildNode(parentNodePath, childNodeName, "", nodeAttributeName, nodeAttributeValue);
        }
        /// <summary>
        /// 在指定的父节点下插入一个指定值得子节点
        /// </summary>
        /// <param name="parentNodePath">父节点路径</param>
        /// <param name="childNodeName">子节点名称</param>
        /// <param name="nodevalue">之节点值</param>
        /// <returns>bool值</returns>
        public bool AddChildNode(string parentNodePath, string childNodeName, string nodevalue)
        {
            return AddChildNode(parentNodePath, childNodeName, nodevalue, "", "");
        }
        #endregion

        #region 删除节点
        /// <summary>
        /// 删除指定一个的节点及其下面的子节点
        /// </summary>
        /// <param name="NodeIndex">节点索引</param>
        /// <param name="NodePath">节点路径</param>
        /// <returns>bool值</returns>
        public bool DeleteNode(int NodeIndex, string NodePath)
        {
            try
            {
                //是否有多行，并指定删除哪一行
                if (NodeIndex != -1)
                {
                    //获取节点列表
                    XmlNodeList ndlist = xmlDoc.SelectNodes(NodePath);
                    //获取要删除的节点
                    XmlNode xn = (XmlNode)ndlist[NodeIndex];
                    //删除节点
                    xn.ParentNode.RemoveChild(xn);
                }
                else
                {
                    //获取节点列表
                    XmlNodeList ndlist = xmlDoc.SelectNodes(NodePath);
                    //删除列表
                    foreach (XmlNode xn in ndlist)
                    {
                        xn.ParentNode.RemoveChild(xn);
                    }
                }
            }
            catch { return false; }
            return true;
        }
        /// <summary>
        /// 删除节点及节点下的子节点
        /// </summary>
        /// <param name="NodePath">节点路径</param>
        /// <returns>bool值</returns>
        public bool DeleteNode(string NodePath)
        {
            return DeleteNode(-1, NodePath);
        }
        #endregion

        #region 保存XML文件
        /// <summary>
        /// 保存文件
        /// </summary>
        public void SaveXmlDocument()
        {
            try
            {
                //保存设置的结果
                xmlDoc.Save(HttpContext.Current.Server.MapPath(xmlFilePath));
            }
            catch (XmlException xmle)
            {
                throw xmle;
            }
        }
        /// <summary>
        /// 保存文件
        /// </summary>
        /// <param name="tempXMLFilePath">文件路径</param>
        public void SaveXmlDocument(string tempXMLFilePath)
        {
            try
            {
                //保存设置的结果
                xmlDoc.Save(tempXMLFilePath);
            }
            catch (XmlException xmle)
            {
                throw xmle;
            }
        }
        #endregion
    }
}