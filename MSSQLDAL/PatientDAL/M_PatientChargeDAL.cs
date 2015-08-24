using Anke.SHManage.IDAL;
using Anke.SHManage.Model;
using Anke.SHManage.Model.ViewModel;
using Anke.SHManage.Utility;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Anke.SHManage.MSSQLDAL
{

    /// <summary>
    /// 病历收费 查询
    /// </summary>
    public partial class M_PatientChargeDAL : BaseDAL<M_PatientCharge>, IM_PatientChargeDAL
    {
        IDALContext dalContext = new DALContextFactory().GetDALContext();
        AKSHManageEntities db = DBContextFactory.GetDBContext() as AKSHManageEntities;



        /// <summary>
        /// 获取病历收费相关
        /// </summary>
        /// <param name="taskCode"></param>
        /// <param name="PatientOrder"></param>
        public IEnumerable<PatientChargeInfo> getPatientCharge(string taskCode, string PatientOrder)
        {

            string sqlStr = @"  select 
                           Name=prd.DrugName 
                           ,Price= prd.Price 
	                       ,Counts= sum(prd.Dosage) 
                           ,ChargeCounts =case when sum(prd.Dosage)%isnull(prd.FeeScale,1)>0 then round((sum(prd.Dosage)/isnull(prd.FeeScale,1)),0)+1 else round((sum(prd.Dosage)/isnull(prd.FeeScale,1)),0) end 
	                       ,ChargeWay =prd.ChargeWay 
	                       ,FeeScale = prd.FeeScale 
	                       ,TotalPrice =case when (im.LimitMaxPrice is not null )  then (case when (prd.Price * (case when sum(prd.Dosage)%isnull(prd.FeeScale,1)>0 then round((sum(prd.Dosage)/isnull(prd.FeeScale,1)),0)+1 else round((sum(prd.Dosage)/isnull(prd.FeeScale,1)),0) end) > im.LimitMaxPrice) then im.LimitMaxPrice else 
	                       prd.Price * (case when sum(prd.Dosage)%isnull(prd.FeeScale,1)>0 then round((sum(prd.Dosage)/isnull(prd.FeeScale,1)),0)+1 else round((sum(prd.Dosage)/isnull(prd.FeeScale,1)),0) end) end) else prd.Price * (case when sum(prd.Dosage)%isnull(prd.FeeScale,1)>0 then round((sum(prd.Dosage)/isnull(prd.FeeScale,1)),0)+1 else round((sum(prd.Dosage)/isnull(prd.FeeScale,1)),0) end) end  
	                       ,ChargeType = '药费'
	                        from M_PatientRecordDrug prd  
		                    left join I_Material im on prd.DrugCode=im.ID 
	                        where TaskCode=@TaskCode and  PatientOrder =@PatientOrder  and im.IsActive = 1 
		                    group by prd.DrugName,prd.Price,prd.ChargeWay,prd.FeeScale,im.LimitMaxPrice 
                             
                                union all

                                select 
                                 Name=prs.SanitationName 
                                ,Price= isnull(prs.Price,0)
                                ,Counts= sum(isnull(prs.NumberOfTimes,0))
                                ,ChargeCounts = sum(isnull(prs.NumberOfTimes,0))
                                ,ChargeWay =prs.ChargeWay
                                ,FeeScale = 1
                                ,TotalPrice = case when im.LimitMaxPrice is not null   then (case when ( isnull(prs.Price,0) *  sum(isnull(prs.NumberOfTimes,0))> im.LimitMaxPrice) then  im.LimitMaxPrice  else 
                                isnull(prs.Price,0) *  sum(isnull(prs.NumberOfTimes,0)) end ) else isnull(prs.Price,0) *  sum(isnull(prs.NumberOfTimes,0)) end
                                ,ChargeType = '材料费' 
                                from M_PatientRecordSanitation prs 
                                left join I_Material im  on prs.SanitationCode = im.ID 
                                where TaskCode=@TaskCode and  PatientOrder =@PatientOrder 
                                and im.IsActive=1 
                                 group by prs.SanitationName,prs.Price,prs.ChargeWay,im.LimitMaxPrice 

							   union all

                                select
                                 Name=prm.RescueMeasureName 
                                ,Price= isnull(prm.Price,0)
                                ,Counts= sum(isnull(prm.NumberOfTimes,0))
                                ,ChargeCounts = sum(isnull(prm.NumberOfTimes,0))
                                ,ChargeWay =prm.ChargeWay
								,FeeScale = 1
								 ,TotalPrice = case when im.LimitMaxPrice is not null   then (case when ( isnull(prm.Price,0) *  sum(isnull(prm.NumberOfTimes,0))> im.LimitMaxPrice) then  im.LimitMaxPrice  else 
								isnull(prm.Price,0) *  sum(isnull(prm.NumberOfTimes,0)) end ) else isnull(prm.Price,0) *  sum(isnull(prm.NumberOfTimes,0)) end
                                ,ChargeType = '检查费'
                                from M_PatientRecordMeasure prm
								 left join   I_Material im  on prm.RescueMeasureCode = im.ID
                                where TaskCode=@TaskCode and PatientOrder =@PatientOrder and prm.OtherTypeID = 'PRMeasureType-99' 
								and  im.IsActive=1 
                                 group by prm.RescueMeasureName,prm.Price,prm.ChargeWay,im.LimitMaxPrice 

                                union all

                                select
                                 Name=prm.RescueMeasureName 
                                ,Price= isnull(prm.Price,0)
                                ,Counts= sum(isnull(prm.NumberOfTimes,0))
                                ,ChargeCounts = sum(isnull(prm.NumberOfTimes,0))
                                ,ChargeWay =prm.ChargeWay
								,FeeScale = 1
                                 ,TotalPrice = case when im.LimitMaxPrice is not null   then (case when ( isnull(prm.Price,0) *  sum(isnull(prm.NumberOfTimes,0))> im.LimitMaxPrice) then  im.LimitMaxPrice  else 
								isnull(prm.Price,0) *  sum(isnull(prm.NumberOfTimes,0)) end ) else isnull(prm.Price,0) *  sum(isnull(prm.NumberOfTimes,0)) end
								,ChargeType = '治疗费'
                                from M_PatientRecordMeasure prm 
								left join  I_Material im  on prm.RescueMeasureCode = im.ID
                                where  TaskCode=@TaskCode and  PatientOrder =@PatientOrder and prm.OtherTypeID <> 'PRMeasureType-99' 
                                and im.IsActive=1 
                                group by prm.RescueMeasureName,prm.Price,prm.ChargeWay,im.LimitMaxPrice  
                               
                                union all
                                select
                                Name=pr.RescueType
                                ,Price= case when RescueType='小抢救' then 30 else 40 end
                                ,Counts=1
                                ,ChargeCounts = 1
                                ,ChargeWay = case when RescueType='小抢救' then '30.00元/次' else '40.00元/次' end
                                ,FeeScale = 1
                                ,TotalPrice =case when RescueType='小抢救' then 30 else 40 end
                                ,ChargeType = '治疗费'
                                from  M_PatientRecord pr 
                                where TaskCode=@TaskCode and  PatientOrder =@PatientOrder 
                                and pr.RescueType in ('中抢救','小抢救') ";

            //List<SqlParameter> listPara = new List<SqlParameter>();
            //listPara.Add(new SqlParameter("@TaskCode", taskCode));
            //listPara.Add(new SqlParameter("@PatientOrder", PatientOrder));    //listPara.ToArray()

            SqlParameter[] prams = new SqlParameter[] { 
                        new SqlParameter("@TaskCode",SqlDbType.NVarChar,100),
                        new SqlParameter("@PatientOrder",SqlDbType.Int)
 
                    };
            prams[0].Value = taskCode;
            prams[1].Value = PatientOrder;


            DataSet ds = SqlHelper.ExecuteDataSet(SqlHelper.MainConnectionString, CommandType.Text, sqlStr, prams);

            List<PatientChargeInfo> list = new List<PatientChargeInfo>();
            if (ds.Tables[0].Rows.Count > 0)
            {
                PatientChargeInfo info = new PatientChargeInfo();

                foreach (DataRow dr in ds.Tables[0].Rows)
                {
                    info = new PatientChargeInfo();
                    info.Name = dr["Name"].ToString();
                    info.Price = DBConvert.ConvertDBTypeToNullableDecimal(dr["Price"]);
                    info.Counts = DBConvert.ConvertDBTypeToNullableInt(dr["Counts"]);
                    info.ChargeCounts = DBConvert.ConvertDBTypeToNullableInt(dr["ChargeCounts"]);
                    info.ChargeWay = dr["ChargeWay"].ToString();
                    info.FeeScale = Convert.ToInt32(dr["FeeScale"]);
                    info.TotalPrice = DBConvert.ConvertDBTypeToNullableDecimal(dr["TotalPrice"]);
                    info.ChargeType = dr["ChargeType"].ToString();

                    list.Add(info);
                }
            }

            return list;
        }


        //病历收费打印 （救护车费，等车费，公里收费，院前急救费, 全部收费总计）
        public DataTable getPatientChargePrintCar(string taskCode, int PatientOrder)
        {
            StringBuilder sqlStr = new StringBuilder();

            sqlStr.Append(@"  select PatientName = mpg.PatientName,  
                                CarFee = mpg.CarFee, 
                                ChargeKM=mpg.ChargeKM,
                                WaitingFee=mpg.WaitingFee,
                                EmergencyFee=mpg.EmergencyFee, 
                                ReceivableTotal=mpg.ReceivableTotal, 
                                PaidMoney=mpg.PaidMoney
                                from M_PatientCharge mpg 
                        where TaskCode='" + taskCode + "' and PatientOrder='" + PatientOrder + "' ");
            
            //SqlParameter[] prams = new SqlParameter[] { 
            //            new SqlParameter("@TaskCode",SqlDbType.NVarChar,100),
            //            new SqlParameter("@PatientOrder",SqlDbType.Int)
            //        };
            //prams[0].Value = taskCode;
            //prams[1].Value = PatientOrder;

            DataSet ds = SqlHelper.ExecuteDataSet(SqlHelper.MainConnectionString, CommandType.Text, sqlStr.ToString(), null);
            return ds.Tables[0];
        }
        //病历收费打印 （药费，材料费，检查费，治疗费）
        public DataTable getPatientChargePrintOther(string taskCode, int PatientOrder)
        {
            StringBuilder sqlStr = new StringBuilder();
            sqlStr.Append(@" select 
                            Name=prd.DrugName 
                           ,Price= prd.Price 
	                       ,Counts= sum(prd.Dosage) 
                           ,ChargeCounts =case when sum(prd.Dosage)%isnull(prd.FeeScale,1)>0 then round((sum(prd.Dosage)/isnull(prd.FeeScale,1)),0)+1 else round((sum(prd.Dosage)/isnull(prd.FeeScale,1)),0) end 
	                       ,ChargeWay =prd.ChargeWay 
	                       ,FeeScale = prd.FeeScale 
	                       ,TotalPrice =case when (im.LimitMaxPrice is not null )  then (case when (prd.Price * (case when sum(prd.Dosage)%isnull(prd.FeeScale,1)>0 then round((sum(prd.Dosage)/isnull(prd.FeeScale,1)),0)+1 else round((sum(prd.Dosage)/isnull(prd.FeeScale,1)),0) end) > im.LimitMaxPrice) then im.LimitMaxPrice else 
	                       prd.Price * (case when sum(prd.Dosage)%isnull(prd.FeeScale,1)>0 then round((sum(prd.Dosage)/isnull(prd.FeeScale,1)),0)+1 else round((sum(prd.Dosage)/isnull(prd.FeeScale,1)),0) end) end) else prd.Price * (case when sum(prd.Dosage)%isnull(prd.FeeScale,1)>0 then round((sum(prd.Dosage)/isnull(prd.FeeScale,1)),0)+1 else round((sum(prd.Dosage)/isnull(prd.FeeScale,1)),0) end) end  
	                       ,ChargeType = '药费'
	                        from M_PatientRecordDrug prd  
		                    left join I_Material im on prd.DrugCode=im.ID 
	                        where TaskCode=@TaskCode and  PatientOrder =@PatientOrder  and im.IsActive = 1  and TotalPrice<>0
		                    group by prd.DrugName,prd.Price,prd.ChargeWay,prd.FeeScale,im.LimitMaxPrice 
                       
                                union all

                                 select 
                                 Name=prs.SanitationName 
                                ,Price= isnull(prs.Price,0)
                                ,Counts= sum(isnull(prs.NumberOfTimes,0))
                                ,ChargeCounts = sum(isnull(prs.NumberOfTimes,0))
                                ,ChargeWay =prs.ChargeWay
                                ,FeeScale = 1
                                ,TotalPrice = case when im.LimitMaxPrice is not null   then (case when ( isnull(prs.Price,0) *  sum(isnull(prs.NumberOfTimes,0))> im.LimitMaxPrice) then  im.LimitMaxPrice  else 
                                isnull(prs.Price,0) *  sum(isnull(prs.NumberOfTimes,0)) end ) else isnull(prs.Price,0) *  sum(isnull(prs.NumberOfTimes,0)) end
                                ,ChargeType = '材料费' 
                                from M_PatientRecordSanitation prs 
                                left join I_Material im  on prs.SanitationCode = im.ID 
                                where TaskCode=@TaskCode and  PatientOrder =@PatientOrder 
                                and im.IsActive=1  and TotalPrice<>0 
                                 group by prs.SanitationName,prs.Price,prs.ChargeWay,im.LimitMaxPrice 



                              

							   union all
                                select
                                 Name=prm.RescueMeasureName 
                                ,Price= isnull(prm.Price,0)
                                ,Counts= sum(isnull(prm.NumberOfTimes,0))
                                ,ChargeCounts = sum(isnull(prm.NumberOfTimes,0))
                                ,ChargeWay =prm.ChargeWay
								,FeeScale = 1
								 ,TotalPrice = case when im.LimitMaxPrice is not null   then (case when ( isnull(prm.Price,0) *  sum(isnull(prm.NumberOfTimes,0))> im.LimitMaxPrice) then  im.LimitMaxPrice  else 
								isnull(prm.Price,0) *  sum(isnull(prm.NumberOfTimes,0)) end ) else isnull(prm.Price,0) *  sum(isnull(prm.NumberOfTimes,0)) end
                                ,ChargeType = '检查费'
                                from M_PatientRecordMeasure prm
								 left join   I_Material im  on prm.RescueMeasureCode = im.ID
                                where TaskCode=@TaskCode and PatientOrder =@PatientOrder and prm.OtherTypeID = 'PRMeasureType-99' 
								and  im.IsActive=1    and TotalPrice<>0 
                                 group by prm.RescueMeasureName,prm.Price,prm.ChargeWay,im.LimitMaxPrice 
                      
                                union all

                                select
                                 Name=prm.RescueMeasureName 
                                ,Price= isnull(prm.Price,0)
                                ,Counts= sum(isnull(prm.NumberOfTimes,0))
                                ,ChargeCounts = sum(isnull(prm.NumberOfTimes,0))
                                ,ChargeWay =prm.ChargeWay
								,FeeScale = 1
                                 ,TotalPrice = case when im.LimitMaxPrice is not null   then (case when ( isnull(prm.Price,0) *  sum(isnull(prm.NumberOfTimes,0))> im.LimitMaxPrice) then  im.LimitMaxPrice  else 
								isnull(prm.Price,0) *  sum(isnull(prm.NumberOfTimes,0)) end ) else isnull(prm.Price,0) *  sum(isnull(prm.NumberOfTimes,0)) end
								,ChargeType = '治疗费'
                                from M_PatientRecordMeasure prm 
								left join  I_Material im  on prm.RescueMeasureCode = im.ID
                                where  TaskCode=@TaskCode and  PatientOrder =@PatientOrder and prm.OtherTypeID <> 'PRMeasureType-99' 
                                and im.IsActive=1  and TotalPrice<>0
                                group by prm.RescueMeasureName,prm.Price,prm.ChargeWay,im.LimitMaxPrice  
                               
                                union all 

                                select 
                                Name=pr.RescueType 
                                ,Price= case when RescueType='小抢救' then 30 else 40 end 
                                ,Counts=1 
                                ,ChargeCounts = 1 
                                ,ChargeWay = case when RescueType='小抢救' then '30.00元/次' else '40.00元/次' end 
                                ,FeeScale = 1 
                                ,TotalPrice =case when RescueType='小抢救' then 30 else 40 end 
                                ,ChargeType = '治疗费' 
                                from  M_PatientRecord pr 
                                where TaskCode=@TaskCode and  PatientOrder =@PatientOrder 
                                and pr.RescueType in ('中抢救','小抢救') ");
            SqlParameter[] prams = new SqlParameter[] { 
                        new SqlParameter("@TaskCode",SqlDbType.NVarChar,100),
                        new SqlParameter("@PatientOrder",SqlDbType.Int)
                    };
            prams[0].Value = taskCode;
            prams[1].Value = PatientOrder;

            DataSet ds = SqlHelper.ExecuteDataSet(SqlHelper.MainConnectionString, CommandType.Text, sqlStr.ToString(), prams);
            return ds.Tables[0];
        }

        //转运类型为回家，收费打印只取治疗费
        public DataTable getPatientChargePrintZhiLiao(string taskCode, int PatientOrder)
        {
            StringBuilder sqlStr = new StringBuilder();
            sqlStr.Append(@" select 
                                 Name=prm.RescueMeasureName 
                                ,Price= isnull(prm.Price,0)
                                ,Counts= sum(isnull(prm.NumberOfTimes,0))
                                ,ChargeCounts = sum(isnull(prm.NumberOfTimes,0))
                                ,ChargeWay =prm.ChargeWay
								,FeeScale = 1
                                 ,TotalPrice = case when im.LimitMaxPrice is not null   then (case when ( isnull(prm.Price,0) *  sum(isnull(prm.NumberOfTimes,0))> im.LimitMaxPrice) then  im.LimitMaxPrice  else 
								isnull(prm.Price,0) *  sum(isnull(prm.NumberOfTimes,0)) end ) else isnull(prm.Price,0) *  sum(isnull(prm.NumberOfTimes,0)) end
								,ChargeType = '治疗费'
                                from M_PatientRecordMeasure prm 
								left join  I_Material im  on prm.RescueMeasureCode = im.ID
                                where  TaskCode=@TaskCode and  PatientOrder =@PatientOrder and prm.OtherTypeID <> 'PRMeasureType-99' 
                                and im.IsActive=1  and TotalPrice<>0
                                group by prm.RescueMeasureName,prm.Price,prm.ChargeWay,im.LimitMaxPrice  

                                union all

                                select
                                Name=pr.RescueType
                                ,Price= case when RescueType='小抢救' then 30 else 40 end
                                ,Counts=1
                                ,ChargeCounts = 1
                                ,ChargeWay = case when RescueType='小抢救' then '30.00元/次' else '40.00元/次' end
                                ,FeeScale = 1
                                ,TotalPrice =case when RescueType='小抢救' then 30 else 40 end
                                ,ChargeType = '治疗费'
                                from  M_PatientRecord pr 
                                where TaskCode=@TaskCode and  PatientOrder =@PatientOrder 
                                and pr.RescueType in ('中抢救','小抢救') ");
            SqlParameter[] prams = new SqlParameter[] { 
                        new SqlParameter("@TaskCode",SqlDbType.NVarChar,100),
                        new SqlParameter("@PatientOrder",SqlDbType.Int)
                    };
            prams[0].Value = taskCode;
            prams[1].Value = PatientOrder;

            DataSet ds = SqlHelper.ExecuteDataSet(SqlHelper.MainConnectionString, CommandType.Text, sqlStr.ToString(), prams);
            return ds.Tables[0];
        }
    }

}
