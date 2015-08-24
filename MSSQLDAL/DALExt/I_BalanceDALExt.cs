using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Anke.SHManage.Model;
using Anke.SHManage.IDAL;
using System.Data;


namespace Anke.SHManage.MSSQLDAL
{
    public partial class I_BalanceDALExt
    {
        IDALContext dalContext = new DALContextFactory().GetDALContext();
        AKSHManageEntities db = DBContextFactory.GetDBContext() as AKSHManageEntities;

        public object GetBalanceList(int page, int rows, string monthtime, string Type, ref int rowCounts)
        {
            var q = (from b in db.I_BalanceList
                     select new
                     {
                         MaterialTypeID = b.MaterialTypeID,
                         ReportTime = b.ReportTime,
                         Name = b.TDictionary.Name

                     });
            if (!string.IsNullOrEmpty(monthtime))
            {
                q = q.Where(a => a.ReportTime == monthtime);

            }
            if (!string.IsNullOrEmpty(Type))
            {
                q = q.Where(a => a.MaterialTypeID == Type);
            }
            rowCounts = q.Count();
            var r = q.OrderBy(a => a.ReportTime).Skip((page - 1) * rows).Take(rows).ToList();
            return r;
        }
        /// <summary>
        /// 根据物资类型查询产生的最大统计报表月份
        /// </summary>
        /// <param name="MTypeID"></param>
        /// <returns></returns>
        public string GetBalanceMax(string MTypeID)
        {
            try
            {
                string max = (from b in db.I_BalanceList
                              where b.MaterialTypeID == MTypeID
                              orderby b.ReportTime descending
                              select b.ReportTime).First();
                return max;
            }
            catch
            {
                return "";
            }
            
        }
        /// <summary>
        /// 插入I_Balance数据
        /// </summary>
        /// <param name="list"></param>
        /// <param name="errorMsg"></param>
        /// <returns></returns>
        public bool AddBalance(List<I_Balance> list,I_BalanceList blinfo,ref string errorMsg)
        {
            using (var tran = db.Database.BeginTransaction())
            {

                try
                {
                    db.I_BalanceList.Add(blinfo);
                    db.I_Balance.AddRange(list);
                    tran.Commit(); //提交事务
                    if (db.SaveChanges() > 0)
                        return true;
                    else
                        return false;
                }
                catch (Exception ex)
                {
                    if (tran != null)
                        tran.Rollback();  //回滚事务
                    errorMsg = "存储数据错误：" + ex.Message;
                    return false;
                }
            }
        }
        /// <summary>
        /// 根据月份查询财务统计报表
        /// </summary>
        /// <param name="reportTime"></param>
        /// <param name="MTypeID"></param>
        /// <param name="errorMsg"></param>
        /// <returns></returns>
        public List<I_Balance> GetI_BalanceNewList(string tempReportTime,string reportTime, string MTypeID,DateTime beginTime,DateTime endTime, ref string errorMsg)
        {
            try
            {
                StringBuilder sbSQL = new StringBuilder();//sql语句
                sbSQL.Append("declare @reportTime varchar(50) ");
                sbSQL.Append("set @reportTime='" + tempReportTime + "' ");

                sbSQL.Append("declare @MTypeID varchar(50) ");
                sbSQL.Append("set @MTypeID='" + MTypeID + "' ");

                sbSQL.Append("declare @beginTime varchar(50) ");
                sbSQL.Append("set @beginTime='" + beginTime + "' ");

                sbSQL.Append("declare @endTime varchar(50) ");
                sbSQL.Append("set @endTime='" + endTime + "' ");

                string sqlStr = @"   SELECT iin.MaterialID,im.Name
                                ,(ISNULL((SELECT SurplusCounts FROM I_Balance where MaterialID=iin.MaterialID and ReportTime=@reportTime),0)) as 上月数量
                                ,(ISNULL((SELECT SurplusPrice FROM I_Balance where MaterialID=iin.MaterialID and ReportTime=@reportTime),0)) as 上月价格
                                ,sum(case when iin.Is_Entry=1 then iin.ChangeSurplus else 0 end) as 进货数量
                                ,sum(case when iin.Is_Entry=1 then iin.SurplusPrice else 0 end) as 进货价格
                                ,sum(case when iin.Is_Entry=0 then iin.ChangeSurplus else 0 end) as 出货数量
                                ,cast(round(
								(((sum(case when iin.Is_Entry=1 then iin.SurplusPrice else 0 end)
								+(ISNULL((SELECT SurplusPrice FROM I_Balance where MaterialID=iin.MaterialID and ReportTime=@reportTime),0)))
								/((sum(case when iin.Is_Entry=1 then iin.ChangeSurplus else 0 end))
								+(ISNULL((SELECT SurplusCounts FROM I_Balance where MaterialID=iin.MaterialID and ReportTime=@reportTime),0))))
                        ),2)   as   numeric(5,2))
								*sum(case when iin.Is_Entry=0 then iin.ChangeSurplus else 0 end)
								 as 出货价格
                            ,sum(iin.ChangeSurplus)+
                            (ISNULL((SELECT SurplusCounts FROM I_Balance where MaterialID=iin.MaterialID and ReportTime=@reportTime),0)) as 结余数量
                            ,(sum(case when iin.Is_Entry=1 then iin.SurplusPrice else 0 end)+
                            cast(round(
								(((sum(case when iin.Is_Entry=1 then iin.SurplusPrice else 0 end)
								+(ISNULL((SELECT SurplusPrice FROM I_Balance where MaterialID=iin.MaterialID and ReportTime=@reportTime),0)))
								/((sum(case when iin.Is_Entry=1 then iin.ChangeSurplus else 0 end))
								+(ISNULL((SELECT SurplusCounts FROM I_Balance where MaterialID=iin.MaterialID and ReportTime=@reportTime),0))))
                            ),2)   as   numeric(5,2))
								*sum(case when iin.Is_Entry=0 then iin.ChangeSurplus else 0 end)
								
                            +(ISNULL((SELECT SurplusPrice FROM I_Balance where MaterialID=iin.MaterialID and ReportTime=@reportTime),0)))
                                as 结余价格

                            FROM I_InventoryRecord iin
                            left join I_Material im on iin.MaterialID = im.ID
                            where 
                            iin.OperatorDateTime >= @beginTime
                            and 
                            iin.OperatorDateTime < @endTime
                            and 
                            im.MTypeID=@MTypeID
                            and iin.StorageCode =2 
                            group by iin.MaterialID,im.Name";
                sbSQL.Append(sqlStr);
                DataSet ds = SqlHelper.ExecuteDataSet(SqlHelper.MainConnectionString, CommandType.Text, sbSQL.ToString(), null);
                List<I_Balance> list = new List<I_Balance>();
                if (ds.Tables[0].Rows.Count > 0)
                {
                    foreach (DataRow dr in ds.Tables[0].Rows)
                    {
                        I_Balance info = new I_Balance().ToPOCO();
                        info.MaterialID = Convert.ToInt32(dr["MaterialID"]);
                        info.MName = dr["Name"].ToString();
                        info.ReportTime = reportTime;
                        info.BeginningCounts = Convert.ToDouble(dr["上月数量"]);
                        info.BeginningPrice = Convert.ToDecimal(dr["上月价格"]);
                        info.IncomeCounts = Convert.ToDouble(dr["进货数量"]);
                        info.IncomePrice = Convert.ToDecimal(dr["进货价格"]);
                        info.PayCounts = Convert.ToDouble(dr["出货数量"]);
                        info.PayPrice = Convert.ToDecimal(dr["出货价格"]);
                        info.UpdataPrice = 0;
                        info.SurplusCounts = Convert.ToDouble(dr["结余数量"]);
                        info.SurplusPrice = Convert.ToDecimal(dr["结余价格"]);
                        info.SurplusTime = DateTime.Now;
                        list.Add(info);
                    }
                }
                return list;
            }
            catch (Exception ex)
            {
                errorMsg = "系统错误：" + ex.Message;
                return null;
            }
        }
    }
}
