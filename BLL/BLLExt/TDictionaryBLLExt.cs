using Anke.SHManage.Model;
using Anke.SHManage.MSSQLDAL;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Anke.SHManage.BLL
{


    public partial class TDictionaryBLL : BaseBLL<TDictionary>
    {
        /// <summary>
        /// 递归查询字典表
        /// </summary>
        /// <param name="dicID"></param>
        /// <returns></returns>
        public List<TDictionary> getTDicRecursion(string dicID)
        {
            string sql = @"with dic as( 
                            select * from TDictionary where ID in (" + dicID + @")
                             union all 
                             select TDictionary.* from dic, TDictionary 
                             where dic.id = TDictionary.ParentID
                            )
                            select * from dic";

            return base.DALContext.ITDictionaryDAL.ExcuteSqlToList(sql);
        }

        public List<TDictionary> getOtherTypeID()
        {

            string sql = @"select *
                            from TDictionary 
                            where TypeID='NULLTYPE'
                            and IsActive=1
                            union  
                            select *
                            from TDictionary 
                            where TypeID='PRConsumableType'
                            and IsActive=1
                            union  
                            select *
                            from TDictionary 
                            where TypeID='PRDrugType'
                            and IsActive=1";

            return base.DALContext.ITDictionaryDAL.ExcuteSqlToList(sql);
        }

        public List<TDictionary> getMeasureTypeID()
        {

            string sql = @" select *
                            from TDictionary 
                            where TypeID='PRMeasureType'
                            and IsActive=1 ";

            return base.DALContext.ITDictionaryDAL.ExcuteSqlToList(sql);
        }

        public int GetMaxDictionaryID(string TypeID , string tableName)
        {
            StringBuilder sb = new StringBuilder();
            sb.Append(@"
                select count(TypeID) +1  from  "+ tableName +@"  
                where  TypeID = '" + TypeID + @"' ");
            SqlDataReader dr = SqlHelper.ExecuteReader(SqlHelper.MainConnectionString, CommandType.Text, sb.ToString(), null);
            int maxSN = 0;
            while (dr.Read())
            {
                maxSN = dr.GetInt32(0);
            }
            return maxSN;
        }

          public int GetMaxDictionaryTypeID()
        {
            StringBuilder sb = new StringBuilder();
            sb.Append(@"
         (select cast (Max(ID) as int)
                from TDictionaryType) 
                    ");
            SqlDataReader dr = SqlHelper.ExecuteReader(SqlHelper.MainConnectionString, CommandType.Text, sb.ToString(), null);
            int maxSN = 0;
            while (dr.Read())
            {
                maxSN = dr.GetInt32(0);
            }
            return maxSN;
        }

          public int GetDictionarySNByID( string  ID)
          {
              StringBuilder sb = new StringBuilder();
              sb.Append(@"
       select SN from TDictionary where ID = '"+ID +@"'
                    ");
              SqlDataReader dr = SqlHelper.ExecuteReader(SqlHelper.MainConnectionString, CommandType.Text, sb.ToString(), null);
              int maxSN = 0;
              while (dr.Read())
              {
                  maxSN = dr.GetInt32(0);
              }
              return maxSN;
          }
    }
}
