using Anke.SHManage.MSSQLDAL;
using Anke.SHManage.Web.Reports.DataSetForPrintTableAdapters;
using Microsoft.Reporting.WebForms;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace Anke.SHManage.Web.Reports
{
    public partial class PrintModeWebForm : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                Data_Binding();
            }
        }

        private void Data_Binding()
        {
            string strReportName = Server.UrlDecode(Request.QueryString["AlarmEventType"]); //System.Web.HttpContext.Current.Session["ReportName"].ToString();
            string taskCode = Request.QueryString["TaskCode"];
            int patientOrder = Convert.ToInt32(Request.QueryString["PatientOrder"]);
            //ifCharge为 0 ，一般病历单打印，ifCharge为1 ，病历收费打印
            int ifCharge = Convert.ToInt32(Request.QueryString["IfCharge"]);
            if (ifCharge == 0)
            {
                switch (strReportName)
                {
                    case "救治":
                        Print_PatientRecord_JijiuTableAdapter ada = new Print_PatientRecord_JijiuTableAdapter();
                        DataSetForPrint.Print_PatientRecord_JijiuDataTable dt = ada.GetData(taskCode, patientOrder);
                        ReportDataSource rdsjz = new ReportDataSource("DataSet1", dt.AsEnumerable());
                        this.ReportViewer1.LocalReport.ReportPath = Server.MapPath("~/Reports/PrintPatient.rdlc");
                        this.ReportViewer1.LocalReport.DataSources.Add(rdsjz);

                        break;
                    case "一般转院":
                    case "急救转院":
                        Print_PatientRecord_ZhuanYuanTableAdapter adazhuanyuan = new Print_PatientRecord_ZhuanYuanTableAdapter();
                        DataSetForPrint.Print_PatientRecord_ZhuanYuanDataTable dtzhuanyuan = adazhuanyuan.GetData(taskCode, patientOrder);
                        ReportDataSource rdszy = new ReportDataSource("DataSetForZhuanYuan", dtzhuanyuan.AsEnumerable());
                        this.ReportViewer1.LocalReport.ReportPath = Server.MapPath("~/Reports/PrintPatientZhuanYun.rdlc");
                        this.ReportViewer1.LocalReport.DataSources.Add(rdszy);
                        break;

                    case "回家":
                        Print_PatientRecord_ChuyuanTableAdapter adachuyuan = new Print_PatientRecord_ChuyuanTableAdapter();
                        DataSetForPrint.Print_PatientRecord_ChuyuanDataTable dtchuyuan = adachuyuan.GetData(taskCode, patientOrder);
                        ReportDataSource rdshj = new ReportDataSource("DataSetChuyuan", dtchuyuan.AsEnumerable());
                        this.ReportViewer1.LocalReport.ReportPath = Server.MapPath("~/Reports/PrintPatientChuYuan.rdlc");
                        this.ReportViewer1.LocalReport.DataSources.Add(rdshj);
                        break;

                    case "空车":
                    case "取暖箱":
                        Print_PatientRecord_KongCheTableAdapter adakongche = new Print_PatientRecord_KongCheTableAdapter();
                        DataSetForPrint.Print_PatientRecord_KongCheDataTable dtkongche = adakongche.GetData(taskCode, patientOrder);
                        ReportDataSource rdskc = new ReportDataSource("DataSetKongChe", dtkongche.AsEnumerable());
                        this.ReportViewer1.LocalReport.ReportPath = Server.MapPath("~/Reports/PrintPatientKongChe.rdlc");
                        this.ReportViewer1.LocalReport.DataSources.Add(rdskc);
                        break;
                    default:
                        break;
                }

                this.ReportViewer1.LocalReport.Refresh();
            }
            else
            {
                switch (strReportName)
                {
                    case "救治":
                        this.ReportViewer1.LocalReport.DataSources.Clear();
                        this.ReportViewer1.LocalReport.ReportPath = Server.MapPath(@"~/Reports/PatientChargePrintForJiJiuOrZhuanyuan.rdlc");
                        M_PatientChargeDAL dal = new M_PatientChargeDAL();
                        DataTable dt = dal.getPatientChargePrintCar(taskCode, patientOrder);
                        this.ReportViewer1.LocalReport.DataSources.Add(new ReportDataSource("DataSetForCPCar", dt));
                        DataTable dt1 = dal.getPatientChargePrintOther(taskCode, patientOrder);
                        this.ReportViewer1.LocalReport.DataSources.Add(new ReportDataSource("DataSetForCPOther", dt1));
                        break;
                    case "一般转院":
                    case "急救转院":
                        this.ReportViewer1.LocalReport.DataSources.Clear();
                        this.ReportViewer1.LocalReport.ReportPath = Server.MapPath(@"~/Reports/PatientChargePrintForJiJiuOrZhuanyuan.rdlc");
                        M_PatientChargeDAL dal1 = new M_PatientChargeDAL();
                        DataTable dt2 = dal1.getPatientChargePrintCar(taskCode, patientOrder);
                        this.ReportViewer1.LocalReport.DataSources.Add(new ReportDataSource("DataSetForCPCar", dt2));
                        DataTable dt3 = dal1.getPatientChargePrintOther(taskCode, patientOrder);
                        this.ReportViewer1.LocalReport.DataSources.Add(new ReportDataSource("DataSetForCPOther", dt3));
                        break;

                    case "回家":
                        this.ReportViewer1.LocalReport.DataSources.Clear();
                        this.ReportViewer1.LocalReport.ReportPath = Server.MapPath(@"~/Reports/PatientChargePrintForHuiJia.rdlc");
                        M_PatientChargeDAL dal2 = new M_PatientChargeDAL();
                        DataTable dt4 = dal2.getPatientChargePrintCar(taskCode, patientOrder);
                        this.ReportViewer1.LocalReport.DataSources.Add(new ReportDataSource("DataSetForHJCar", dt4));
                        DataTable dt5 = dal2.getPatientChargePrintZhiLiao(taskCode, patientOrder);
                        this.ReportViewer1.LocalReport.DataSources.Add(new ReportDataSource("DataSetForHJZhiLiao", dt5));
                        break;

                    case "空车":
                    case "取暖箱":
                        this.ReportViewer1.LocalReport.DataSources.Clear();
                        this.ReportViewer1.LocalReport.ReportPath = Server.MapPath(@"~/Reports/PatientChargePrintForKongChe.rdlc");
                        M_PatientChargeDAL dal3 = new M_PatientChargeDAL();
                        DataTable dt6 = dal3.getPatientChargePrintCar(taskCode, patientOrder);
                        this.ReportViewer1.LocalReport.DataSources.Add(new ReportDataSource("DataSetForChargePrintKongChe", dt6));
                        break;
                    default:
                        break;
                }
                this.ReportViewer1.LocalReport.Refresh();
            }
        }
    }
}