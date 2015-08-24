using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data;
using Anke.SHManage.Utility;
using Anke.SHManage.IDAL;
using Anke.SHManage.Model;

namespace Anke.SHManage.MSSQLDAL
{
    public class M_DictionaryTreeDAL : IM_DictionaryTreeDAL
    {
        AKSHManageEntities db = DBContextFactory.GetDBContext() as AKSHManageEntities;

        public List<CommonTree> GetMSDictionaryTreeInfoList(string tableName, string ParentID, string TypeID)
        {
            try
            {
                StringBuilder sbSQL = new StringBuilder();
                if (TypeID == "")
                {
                    //if (tableName == "M_ZICDNew" || tableName == "M_ZSymptomPendingInvestigation")
                    //{
                    sbSQL.Append(" select id=CONVERT(varchar(20),td.ID),td.Name,ParentID=CONVERT(varchar(20),td.ParentID),td.SN,td.IsActive,TypeID='',PinYin=isnull(PinYin,8888) ");
                    //}
                    //else
                    //{
                    //    sbSQL.Append(" select id=CONVERT(varchar(20),td.ID),td.Name,ParentID=CONVERT(varchar(20),td.ParentID),td.SN,td.IsActive,TypeID='',PinYin='' ");
                    //}
                }
                else
                {
                    sbSQL.Append(" select id=td.ID,td.Name,td.ParentID,td.SN,td.IsActive,td.TypeID,PinYin='' ");
                }
                sbSQL.Append(" from " + tableName + " td ");
                sbSQL.Append(" where td.IsActive=1 ");
                //WhereClauseUtility.AddStringEqual("td.ParentID", ParentID, sbSQL);
                if (ParentID != "")
                {
                    if (TypeID == "")
                    {
                        sbSQL.Append(" and td.ParentID=" + ParentID + "");
                    }
                    else
                    {
                        sbSQL.Append(" and td.ParentID='" + ParentID + "'");
                    }
                }
                WhereClauseUtility.AddStringEqual("td.TypeID", TypeID, sbSQL);
                sbSQL.Append(" order by SN ");

                List<CommonTree> Templist = db.Database.SqlQuery<CommonTree>(sbSQL.ToString()).ToList();

                return Templist;
            }
            catch (Exception ex)
            {
                LogUtility.Error("M_DictionaryTreeDAL/GetMSDictionaryTreeInfoList()", ex.ToString());
                return null;
            }
        }

        //获取药品、耗材、救治措施树形结构
        public List<M_ChargeItemInfo> GetMSChargeItemTreeInfoList(string ParentID, string TypeID)
        {
            try
            {
                StringBuilder sbSQL = new StringBuilder();
                sbSQL.Append(" select ID,Name,ParentID,TypeID,Specification,Unit,PinYin,Remark,Number,GiveMedicineWay");
                sbSQL.Append(" from (");
                sbSQL.Append(" select ID=convert(varchar(20),im.ID),Name=im.Name,");
                sbSQL.Append(" ParentID=case when td.ParentID = '-1' then '-1' else im.OtherTypeID end,");
                sbSQL.Append(" td.TypeID,Specification,Unit=td1.Name,PinYin,Remark,im.SN,");
                sbSQL.Append(" Number=case when im.ID=392 then '4' else '1' end ");
                sbSQL.Append(" ,im.GiveMedicineWay ");
                sbSQL.Append(" from dbo.I_Material im ");
                sbSQL.Append(" left join dbo.TDictionary td on im.OtherTypeID = td.ID ");
                sbSQL.Append(" left join dbo.TDictionary td1 on im.Unit = td1.ID ");
                sbSQL.Append(" where td.IsActive=1 and im.IsActive=1 and td.TypeID='" + TypeID + "' ");
                sbSQL.Append(" union all select ID=td.ID,Name=td.Name, ");
                sbSQL.Append(" ParentID=case when ParentID='" + TypeID + "-1' then '-1' else ParentID end ");
                sbSQL.Append(" ,td.TypeID,Specification='',Unit='',PinYin='',Remark='',SN,Number='',GiveMedicineWay=''");
                sbSQL.Append(" from TDictionary td ");
                sbSQL.Append(" where td.IsActive=1 and td.TypeID='" + TypeID + "' and td.ParentID !='-1'");
                sbSQL.Append(" )t  ");
                if (ParentID == "-1")
                {
                    sbSQL.Append(" where t.ParentID='" + ParentID + "' ");
                }
                sbSQL.Append(" order by SN ");

                List<M_ChargeItemInfo> Templist = db.Database.SqlQuery<M_ChargeItemInfo>(sbSQL.ToString()).ToList();

                return Templist;
            }
            catch (Exception ex)
            {
                LogUtility.Error("M_DictionaryTreeDAL/GetMSChargeItemTreeInfoList()", ex.ToString());
                return null;
            }
        }
    }
}
