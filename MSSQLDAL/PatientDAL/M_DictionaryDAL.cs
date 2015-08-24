using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data;
using System.Data.SqlClient;
using System.Data.Entity;
using Anke.SHManage.Utility;
using Anke.SHManage.Model;
using Anke.SHManage.IDAL;

namespace Anke.SHManage.MSSQLDAL
{
    /// <summary>
    /// 病历字典表信息
    /// </summary>
    public partial class M_DictionaryDAL : IM_DictionaryDAL
    {
        private string m_TableName = "";//字典表名
        AKSHManageEntities db = DBContextFactory.GetDBContext() as AKSHManageEntities;

        #region 根据类型编码获取字典表信息
        public List<M_Dictionary> GetMSDictionaryInfos(string typeCode, string isPatient)
        {
            M_Dictionary info;
            List<M_Dictionary> list = new List<M_Dictionary>();
            StringBuilder sbSQL = new StringBuilder();
            sbSQL.Append(" select td.ID,td.Name,td.TypeID,Type=tdt.Description,td.SN,td.IsActive ");
            //SqlConnectionStringBuilder builder = new SqlConnectionStringBuilder(SqlHelper.ConnectionPatientRecordTransaction);//取管理库的链接字符串
            //是病历字典表
            if (isPatient == "1")
            {
                sbSQL.Append(" from dbo.M_Dictionary td ");
                sbSQL.Append(" left join dbo.M_DictionaryType tdt on tdt.TypeID=td.TypeID ");
                sbSQL.Append(" where td.IsActive=1 ");
                WhereClauseUtility.AddStringInSelectQuery("td.TypeID", typeCode, sbSQL);//20150723修改为允许一次查询多个类
            }
            else
            {
                sbSQL.Append(" from dbo.TDictionary td ");
                sbSQL.Append(" left join dbo.TDictionaryType tdt on tdt.TypeID=td.TypeID ");
                sbSQL.Append(" where td.IsActive=1 ");//具体再修改
                WhereClauseUtility.AddStringEqual("td.TypeID", typeCode, sbSQL);
            }

            sbSQL.Append("order by td.SN ");
            using (DataSet ds = SqlHelper.ExecuteDataSet(SqlHelper.MainConnectionString, CommandType.Text, sbSQL.ToString(), null))
            {
                foreach (DataRow dr in ds.Tables[0].Rows)
                {
                    info = new M_Dictionary();
                    info.ID = DBConvert.ConvertStringToString(dr["ID"]);
                    info.Name = DBConvert.ConvertStringToString(dr["Name"]);
                    info.TypeID = DBConvert.ConvertStringToString(dr["TypeID"]);
                    info.SN = Convert.ToInt32(dr["SN"]);
                    info.IsActive = Convert.ToBoolean(dr["IsActive"]);
                    list.Add(info);
                }
                return list;
            }
        }
        #endregion

        #region 根据表名得到主库字典数据(有效数据)
        /// <summary>
        /// 得到主库字典数据(有效数据)
        /// </summary>
        /// <param name="eDictionary"></param>
        public List<M_Dictionary> GetMainDictionary(string tableName, string code)
        {
            m_TableName = tableName;
            string Conn = SqlHelper.AttemperConnectionString;
            return GetDictionary(Conn,code);
        }
        private List<M_Dictionary> GetDictionary(string Conn,string code)
        {
            M_Dictionary info;
            List<M_Dictionary> list = new List<M_Dictionary>();
            StringBuilder strSQL = new StringBuilder();
            if (m_TableName == "TStation")
            {
                strSQL.Append(" select 编码,名称,顺序号,是否有效 from " + m_TableName + " ");
                strSQL.Append(" where 是否有效=1 ");
                WhereClauseUtility.AddIntEqual("中心编码", Convert.ToInt32(code), strSQL);
                strSQL.Append(" order by 顺序号");
            }
            else if (m_TableName == "TAmbulance")
            {
                strSQL.Append(" select 编码=车辆编码,名称=实际标识,顺序号=1,是否有效 from " + m_TableName + " ");
                strSQL.Append(" where 是否有效=1 ");
                WhereClauseUtility.AddStringEqual("分站编码", code, strSQL);
                strSQL.Append(" order by 实际标识");
            }
            else if (m_TableName == "TPerson")
            {
                strSQL.Append(" select 编码,名称=姓名,顺序号,是否有效 from " + m_TableName + " ");
                strSQL.Append(" where 是否有效=1 and 类型编码=2 order by 姓名");
            }
            else
            {
                strSQL.Append(" select 编码,名称,顺序号,是否有效 from " + m_TableName + " ");
                strSQL.Append(" where 是否有效=1 order by 顺序号");
            }
            try
            {
                using (DataSet ds = SqlHelper.ExecuteDataSet(Conn, CommandType.Text, strSQL.ToString(), null))
                {
                    foreach (DataRow dr in ds.Tables[0].Rows)
                    {
                        info = new M_Dictionary();
                        info.ID = DBConvert.ConvertStringToString(dr["编码"]);
                        info.Name = DBConvert.ConvertStringToString(dr["名称"]);
                        info.SN = Convert.ToInt32(dr["顺序号"]);
                        info.IsActive = Convert.ToBoolean(dr["是否有效"]);
                        list.Add(info);
                    }
                    return list;
                }
            }
            catch (Exception )
            {
                //Log4Net.LogError("DictionaryDAL/GetDictionary", ex.Message);
                return null;
            }
        }
        #endregion

        #region 根据表名来得到管理库字典数据(有效数据)
        /// <summary>
        /// 根据表名来得到管理库字典数据(有效数据)
        /// </summary>
        /// <param name="tableName"></param>
        public List<M_Dictionary> GetManagerDictionary(string tableName)
        {
            m_TableName = tableName;
            string Conn = SqlHelper.MainConnectionString;
            return GetManagerDictioinary(Conn);
        }
        /// <summary>
        /// 根据表名来得到管理库字典数据(有效数据)
        /// </summary>
        /// <param name="tableName"></param>
        /// <returns></returns>
        private List<M_Dictionary> GetManagerDictioinary(string Conn)
        {
            M_Dictionary info;
            List<M_Dictionary> list = new List<M_Dictionary>();
            StringBuilder strSQL = new StringBuilder();
            strSQL.Append(" select ID,Name,SN,IsActive from " + m_TableName + " ");
            strSQL.Append(" where IsActive=1 order by SN");
            try
            {
                using (DataSet ds = SqlHelper.ExecuteDataSet(Conn, CommandType.Text, strSQL.ToString(), null))
                {
                    foreach (DataRow dr in ds.Tables[0].Rows)
                    {
                        info = new M_Dictionary();
                        info.ID = DBConvert.ConvertStringToString(dr["ID"]);
                        info.Name = DBConvert.ConvertStringToString(dr["Name"]);
                        info.SN = Convert.ToInt32(dr["SN"]);
                        info.IsActive = Convert.ToBoolean(dr["IsActive"]);
                        list.Add(info);
                    }
                    return list;
                }
            }
            catch (Exception )
            {
                //Log4Net.LogError("DictionaryDAL/GetDictionary", ex.Message);
                return null;
            }
        }
        #endregion


        #region 根据表名来获取字典信息--病历CheckBox用
        /// <summary>
        /// 根据表名来获取字典信息
        /// </summary>
        /// <param name="tableName"></param>
        /// <returns></returns>
        public IList<M_CheckModel> GetCheckBoxModelByTableName(string tableName)
        {
            string list = @"select Id =t.Name,Name =t.Name,Tags =t.Name  from  " + tableName + " t where t.IsActive =1";

            IList<M_CheckModel> Templist = db.Database.SqlQuery<M_CheckModel>(list).ToList();

            return Templist;

        }
        #endregion

        #region 根据类型编码来获取病历字典表信息--病历CheckBox用
        /// <summary>
        /// 根据类型编码来获取病历字典表信息--病历CheckBox用
        /// </summary>
        /// <param name="typeCode"></param>
        /// <returns></returns>
        public IList<M_CheckModel> GetCheckBoxModel(string typeCode)
        {
            string list = @"select Id =t.Name,Name =t.Name,Tags =t.Name  from M_Dictionary t where t.IsActive =1 and t.TypeID='" + typeCode + "' order by t.SN";

            IList<M_CheckModel> Templist = db.Database.SqlQuery<M_CheckModel>(list).ToList();

            return Templist;

        }
        #endregion

        #region 生成CheckBoxList选项或者RadioButtonList
        /// <summary>
        /// 生成CheckBoxList选项或者RadioButtonList
        /// </summary>
        /// <param name="Prefix"></param>
        /// <param name="TypeID"></param>
        /// <param name="Type"></param>
        /// <returns></returns>
        public object GetCheckBoxOrRadioButtonList(string Prefix, string TypeID, string Type)
        {
            var result = "";
            DbSet<M_Dictionary> set = db.Set<M_Dictionary>();
            //获取CheckBoxList或者RadioButtonList选项集合
            List<M_Dictionary> List = set.SqlQuery(@"Select * from M_Dictionary where TypeID='" + TypeID + "' and IsActive='1' Order By SN ").ToList();
            foreach (M_Dictionary Group in List)
            {
                //获取选项
                result += "<input id=\"" + Prefix + "-" + Group.ID + "\" type=\"" + Type + "\" name=\"" + Group.TypeID + "\" value=\"" + Group.Name + "\" /><label >" + Group.Name + "</label>";
            }
            return result;
        }
        /// <summary>
        /// 根据表名来获取CheckBoxList或者RadioButtonList选项集合
        /// </summary>
        /// <param name="tableName"></param>
        /// <returns></returns>
        public object GetCheckBoxListByTableName(string tableName, string Prefix, string Type)
        {
            var result = "";
            //DbSet<M_CheckModel> set = dbContext.Set<M_CheckModel>();
            //获取CheckBoxList或者RadioButtonList选项集合
            string list = @"select Id=CONVERT(varchar(20),t.ID),Name =t.Name,Tags =t.Name  from  " + tableName + " t where t.IsActive =1 Order By SN";
            List<M_CheckModel> List = db.Database.SqlQuery<M_CheckModel>(list).ToList();

            foreach (M_CheckModel Group in List)
            {
                //获取选项
                result += "<input id=\"" + Prefix + "-" + Group.Id + "\" type=\"" + Type + "\" name=\"" + Prefix + "\" value=\"" + Group.Name + "\" /><label >" + Group.Name + "</label>";
            }
            return result;

        }
        #endregion
    }
}
