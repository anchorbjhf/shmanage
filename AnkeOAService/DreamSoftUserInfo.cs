using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Xml.Serialization;

namespace Anke.AnkeOAService
{


    #region XML格式
    // <?xml version="1.0" encoding="gb2312" ?> 
    //<Root>
    // <ID>用户ID</ID> 
    // <Name>用户姓名</Name> 
    // <NameSpellIndex>用户拼音</NameSpellIndex> 
    // <Mail>Email地址</Mail> 
    // <Password>密码（明文）</Password> 
    // <HomeAddress>家庭住址</HomeAddress> 
    // <HomeTelephone>家庭电话</HomeTelephone> 
    // <CompanyAddress>工作地址</CompanyAddress> 
    // <CompanyTelephone>工作电话</CompanyTelephone> 
    // <Mobile>手机</Mobile> 
    // <Remark>备注</Remark> 

    // <OrganizeIDs>
    // <OrganizeID Name="所属部门ID" IsMain="是否主部门" Index="在部门内排序顺序" /> 
    // <OrganizeID>.</OrganizeID> 
    // <OrganizeID>.</OrganizeID> 
    // <OrganizeID>.</OrganizeID> 
    // </OrganizeIDs>


    // <Sex>性别</Sex> 
    // <Fax>传真</Fax> 
    // <Birthday>生日</Birthday> 
    // <Education>学历</Education> 
    // <MSN>MSN</MSN> 
    // <QQ>QQ</QQ> 
    // <State>状态</State> 
    // <Nick>昵称</Nick> 
    // <Rank>职级</Rank> 
    // <Title>职称</Title> 
    // <Position>职位</Position> 
    // <LogName>登录名</LogName> 
    // <CardId>工号</CardId> 
    //</Root> 
    #endregion

    [XmlRoot(ElementName = "Root")]
    public class DreamSoftUserInfo
    {
        [XmlElement(ElementName = "ID")]
        public string ID { get; set; }

        [XmlElement(ElementName = "Name")]
        public string Name { get; set; }

        [XmlElement(ElementName = "NameSpellIndex")]
        public string NameSpellIndex { get; set; }

        [XmlElement(ElementName = "Mail")]
        public string Mail { get; set; }

        [XmlElement(ElementName = "Password")]
        public string Password { get; set; }

        [XmlElement(ElementName = "HomeAddress")]
        public string HomeAddress { get; set; }

        [XmlElement(ElementName = "HomeTelephone")]
        public string HomeTelephone { get; set; }

        [XmlElement(ElementName = "CompanyAddress")]
        public string CompanyAddress { get; set; }

        [XmlElement(ElementName = "CompanyTelephone")]
        public string CompanyTelephone { get; set; }

        [XmlElement(ElementName = "Mobile")]
        public string Mobile { get; set; }

        [XmlElement(ElementName = "Remark")]
        public string Remark { get; set; }

        [XmlElement(ElementName = "Sex")]
        public string Sex { get; set; }

        [XmlElement(ElementName = "Fax")]
        public string Fax { get; set; }

        [XmlElement(ElementName = "Birthday")]
        public string Birthday { get; set; }

        [XmlElement(ElementName = "Education")]
        public string Education { get; set; }

        [XmlElement(ElementName = "MSN")]
        public string MSN { get; set; }

        [XmlElement(ElementName = "QQ")]
        public string QQ { get; set; }

        [XmlElement(ElementName = "State")]
        public string State { get; set; }

        [XmlElement(ElementName = "Nick")]
        public string Nick { get; set; }

        [XmlElement(ElementName = "Rank")]
        public string Rank { get; set; }

        [XmlElement(ElementName = "Title")]
        public string Title { get; set; }

        [XmlElement(ElementName = "Position")]
        public string Position { get; set; }

        [XmlElement(ElementName = "LogName")]
        public string LogName { get; set; }

        [XmlElement(ElementName = "CardId")]
        public string CardId { get; set; }

        [XmlArray("OrganizeIDs"), XmlArrayItem("OrganizeID")]
        public OrganizeID[] OrganizeIDs { get; set; }

    }

    public class OrganizeID
    {
        [XmlAttribute("Name")]
        public string Name { get; set; }

        [XmlAttribute("IsMain")]
        public string IsMain { get; set; }

        [XmlAttribute("Index")]
        public string Index { get; set; }
    }
}