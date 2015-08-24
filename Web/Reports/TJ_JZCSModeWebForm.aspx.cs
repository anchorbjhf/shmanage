using Anke.SHManage.BLL;
using Anke.SHManage.MSSQLDAL.TJDAL;
using Anke.SHManage.Utility;
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
    public partial class TJ_JZCSModeWebForm : System.Web.UI.Page
    {
        private string reportName;
        protected void Page_Load(object sender, EventArgs e)
        {

            if (!IsPostBack)
            {
                InitPage();
            }
            else
            {
                reportName = Convert.ToString(ViewState["reportName"]);
            }
        }
        protected void btnSearch_Click(object sender, EventArgs e)
        {
            BindReportDataSource();
            //this.Station.Text = "";
            
       
            //this.PatientVersion.Text = "";

        }
        private void InitPage()
        {
            this.StartDate.Text = DateTime.Now.ToString("yyyy-MM-dd 00:00:00");
            this.EndDate.Text = DateTime.Now.ToString("yyyy-MM-dd 23:59:59");
            reportName = Request.QueryString["ReportName"];
            ViewState["reportName"] = reportName;
            //引用DropDownListBLL的 GetCenterOnaspx获取分中心，GetStationOnaspx获取分站。使用ControlHelper.SetDropDownList赋值给DropDownList
            //DropDownListBLL dbll = new DropDownListBLL();
            //ControlHelper.SetDropDownList(DropDownList_Center, dbll.GetCenterOnaspx(), "-1");//分中心
            //ControlHelper.SetDropDownList(DropDownList_Station, dbll.GetStationOnaspx("-1"), "-1");//分站

            BindReportDataSource();
        }
        protected void BindReportDataSource()
        {
            DateTime beginTime = Convert.ToDateTime(this.StartDate.Text);
            DateTime endTime = Convert.ToDateTime(this.EndDate.Text);
            string eventType = Convert.ToString(this.txtPatientVersion.Text);
            string txtCenter = Convert.ToString(this.Center.Text);
            string stationID = Convert.ToString(this.Station.Text);
           // string txtCenter = Convert.ToString(this.DropDownList_Center.SelectedValue);
           //// string txtCenter = Convert.ToString(DropDownList_Center.Text);
           // string stationID = Convert.ToString(this.DropDownList_Station.SelectedValue);
            string agentID = Convert.ToString(this.txtAgentWorkID.Text);
            string doctorAndNurse = Convert.ToString(this.txtDoctorAndNurse.Text);
            string txtDiseasesClassification = Convert.ToString(this.txtDiseasesClassification.Text);
            string txtIllnessClassification = Convert.ToString(this.txtIllnessClassification.Text);
            string txtDeathCase = Convert.ToString(this.txtDeathCase.Text);
            string txtMeasures = Convert.ToString(this.txtMeasures.Text);
            string txtFirstAidEffect = Convert.ToString(this.txtFirstAidEffect.Text);
            string txtFirstImpression = Convert.ToString(this.txtFirstImpression.Text);
            string txtSendAddress = Convert.ToString(this.txtSendAddress.Text);
            string start1 = beginTime.ToString();
            string end1 = endTime.ToString();

            this.ReportViewer1.LocalReport.DataSources.Clear();
            this.ReportViewer1.LocalReport.ReportPath = Server.MapPath(@"~/Reports/" + reportName);
            Microsoft.Reporting.WebForms.ReportParameter st = new Microsoft.Reporting.WebForms.ReportParameter("StartTime", start1);
            Microsoft.Reporting.WebForms.ReportParameter ed = new Microsoft.Reporting.WebForms.ReportParameter("EndTime", end1);
            this.ReportViewer1.LocalReport.SetParameters(new Microsoft.Reporting.WebForms.ReportParameter[] { st, ed });
            TJDAL dal = new TJDAL();
            switch (reportName)
            {
                case "TJ_JZCS.rdlc":
                    this.Page.Title = "救治措施统计表";
                    DataTable dt = dal.Get_TJ_JZCS(beginTime, endTime, eventType, stationID, agentID, doctorAndNurse,
                  txtCenter, txtDiseasesClassification, txtIllnessClassification, txtDeathCase, txtMeasures, txtFirstAidEffect,
                  txtFirstImpression, txtSendAddress);
                    this.ReportViewer1.LocalReport.DataSources.Add(new ReportDataSource("DataSetForTJ_JZCS", dt));
                    break;
                case "TJ_JJPF.rdlc":
                    this.Page.Title = "急救评分统计表";
                    DataTable dt1 = dal.Get_TJ_JJPF(beginTime, endTime, eventType, stationID, agentID, doctorAndNurse,
                  txtCenter, txtDiseasesClassification, txtIllnessClassification, txtDeathCase, txtMeasures, txtFirstAidEffect,
                  txtFirstImpression, txtSendAddress);
                    this.ReportViewer1.LocalReport.DataSources.Add(new ReportDataSource("DataSetForTJ_JJPF", dt1));
                    break;                
                case "TJ_BLZDSJ.rdlc":
                    this.Page.Title = "出车数,病历数,重大事件数统计表";
                    DataTable dt2 = dal.Get_TJ_BLZDSJ(beginTime, endTime, eventType, stationID, agentID, doctorAndNurse,
                  txtCenter, txtDiseasesClassification, txtIllnessClassification, txtDeathCase, txtMeasures, txtFirstAidEffect,
                  txtFirstImpression, txtSendAddress);
                    this.ReportViewer1.LocalReport.DataSources.Add(new ReportDataSource("DataSetForTJ_BLZDSJ", dt2));
                    break;
                case "TJ_BQFL.rdlc":
                    this.Page.Title = "病情分类统计表";
                    DataTable dt3 = dal.Get_TJ_BQFL(beginTime, endTime, eventType, stationID, agentID, doctorAndNurse,
                  txtCenter, txtDiseasesClassification, txtIllnessClassification, txtDeathCase, txtMeasures, txtFirstAidEffect,
                  txtFirstImpression, txtSendAddress);
                    this.ReportViewer1.LocalReport.DataSources.Add(new ReportDataSource("DataSetForTJ_BQFL", dt3));
                    break;
                case "TJ_JJXG.rdlc":
                    this.Page.Title = "急救效果统计表";
                    DataTable dt4 = dal.Get_TJ_JJXG(beginTime, endTime, eventType, stationID, agentID, doctorAndNurse,
                  txtCenter, txtDiseasesClassification, txtIllnessClassification, txtDeathCase, txtMeasures, txtFirstAidEffect,
                  txtFirstImpression, txtSendAddress);
                    this.ReportViewer1.LocalReport.DataSources.Add(new ReportDataSource("DataSetForTJ_JJXG", dt4));
                    break;
                case "TJ_BQYB.rdlc":
                    this.Page.Title = "病情预报统计表";
                    DataTable dt5 = dal.Get_TJ_BQYB(beginTime, endTime, eventType, stationID, agentID, doctorAndNurse,
                  txtCenter, txtDiseasesClassification, txtIllnessClassification, txtDeathCase, txtMeasures, txtFirstAidEffect,
                  txtFirstImpression, txtSendAddress);
                    this.ReportViewer1.LocalReport.DataSources.Add(new ReportDataSource("DataSetForTJ_BQYB", dt5));
                    break;
                case "TJ_HZSW.rdlc":
                    this.Page.Title = "患者死亡统计表";
                    DataTable dt6 = dal.Get_TJ_HZSW(beginTime, endTime, eventType, stationID, agentID, doctorAndNurse,
                  txtCenter, txtDiseasesClassification, txtIllnessClassification, txtDeathCase, txtMeasures, txtFirstAidEffect,
                  txtFirstImpression, txtSendAddress);
                    this.ReportViewer1.LocalReport.DataSources.Add(new ReportDataSource("DataSetForTJ_HZSW", dt6));
                    break;
                case "TJ_HC.rdlc":
                    this.Page.Title = "急救耗材统计表";
                    DataTable dt7 = dal.Get_TJ_HC(beginTime, endTime, eventType, stationID, agentID, doctorAndNurse,
                  txtCenter, txtDiseasesClassification, txtIllnessClassification, txtDeathCase, txtMeasures, txtFirstAidEffect,
                  txtFirstImpression, txtSendAddress);
                    this.ReportViewer1.LocalReport.DataSources.Add(new ReportDataSource("DataSetForTJ_HC", dt7));
                    break;
                case "TJ_XB.rdlc":
                    this.Page.Title = "性别统计表";
                    DataTable dt8 = dal.Get_TJ_XB(beginTime, endTime, eventType, stationID, agentID, doctorAndNurse,
                  txtCenter, txtDiseasesClassification, txtIllnessClassification, txtDeathCase, txtMeasures, txtFirstAidEffect,
                  txtFirstImpression, txtSendAddress);
                    this.ReportViewer1.LocalReport.DataSources.Add(new ReportDataSource("DataSetForTJ_XB", dt8));
                    break;
                case "TJ_NLD.rdlc":
                    this.Page.Title = "年龄段统计表";
                    DataTable dt9 = dal.Get_TJ_NLFB(beginTime, endTime, eventType, stationID, agentID, doctorAndNurse,
                  txtCenter, txtDiseasesClassification, txtIllnessClassification, txtDeathCase, txtMeasures, txtFirstAidEffect,
                  txtFirstImpression, txtSendAddress);
                    this.ReportViewer1.LocalReport.DataSources.Add(new ReportDataSource("DataSetForTJ_NLDFB", dt9));
                    break;
                case "TJ_SJLX.rdlc":
                    this.Page.Title = "事件类型统计表";
                    DataTable dt10 = dal.Get_TJ_SJLX(beginTime, endTime, eventType, stationID, agentID, doctorAndNurse,
                  txtCenter, txtDiseasesClassification, txtIllnessClassification, txtDeathCase, txtMeasures, txtFirstAidEffect,
                  txtFirstImpression, txtSendAddress);
                    this.ReportViewer1.LocalReport.DataSources.Add(new ReportDataSource("DataSetForTJ_SJLX", dt10));
                    break;
                case "TJ_BZFL.rdlc":
                    this.Page.Title = "病种分类统计表";
                    DataTable dt11 = dal.Get_TJ_BZFL(beginTime, endTime, eventType, stationID, agentID, doctorAndNurse,
                  txtCenter, txtDiseasesClassification, txtIllnessClassification, txtDeathCase, txtMeasures, txtFirstAidEffect,
                  txtFirstImpression, txtSendAddress);
                    this.ReportViewer1.LocalReport.DataSources.Add(new ReportDataSource("DataSetForTJ_BZFL", dt11));
                    break;
                case "TJ_QXJC.rdlc":
                    this.Page.Title = "器械检查统计表";
                    DataTable dt12 = dal.Get_TJ_QXJC(beginTime, endTime, eventType, stationID, agentID, doctorAndNurse,
                  txtCenter, txtDiseasesClassification, txtIllnessClassification, txtDeathCase, txtMeasures, txtFirstAidEffect,
                  txtFirstImpression, txtSendAddress);
                    this.ReportViewer1.LocalReport.DataSources.Add(new ReportDataSource("DataSetForTJ_QXJC", dt12));
                    break;
                case "TJ_YPYYL.rdlc":
                    this.Page.Title = "药品用药量统计表";
                    DataTable dt13 = dal.Get_TJ_YPYYL(beginTime, endTime, eventType, stationID, agentID, doctorAndNurse,
                  txtCenter, txtDiseasesClassification, txtIllnessClassification, txtDeathCase, txtMeasures, txtFirstAidEffect,
                  txtFirstImpression, txtSendAddress);
                    this.ReportViewer1.LocalReport.DataSources.Add(new ReportDataSource("DataSetForTJ_YPYYL", dt13));
                    break;
                case "TJ_JJZLJC.rdlc":
                    this.Page.Title = "拒绝治疗、体检、器械检查、提供病史统计表";
                    DataTable dt14 = dal.Get_TJ_JJZLJC(beginTime, endTime, eventType, stationID, agentID, doctorAndNurse,
                  txtCenter, txtDiseasesClassification, txtIllnessClassification, txtDeathCase, txtMeasures, txtFirstAidEffect,
                  txtFirstImpression, txtSendAddress);
                    this.ReportViewer1.LocalReport.DataSources.Add(new ReportDataSource("DataSetForTJ_JJZLJC", dt14));
                    break;
                case "TJ_HJYY.rdlc":
                    this.Page.Title = "呼救医院统计表";
                    DataTable dt15 = dal.Get_TJ_HJYY(beginTime, endTime, eventType, stationID, agentID, doctorAndNurse,
                  txtCenter, txtDiseasesClassification, txtIllnessClassification, txtDeathCase, txtMeasures, txtFirstAidEffect,
                  txtFirstImpression, txtSendAddress);
                    this.ReportViewer1.LocalReport.DataSources.Add(new ReportDataSource("DataSetForTJ_HJYY", dt15));
                    break;
                case "TJ_SDYY.rdlc":
                    this.Page.Title = "送达医院统计表";
                    DataTable dt16 = dal.Get_TJ_SDYY(beginTime, endTime, eventType, stationID, agentID, doctorAndNurse,
                  txtCenter, txtDiseasesClassification, txtIllnessClassification, txtDeathCase, txtMeasures, txtFirstAidEffect,
                  txtFirstImpression, txtSendAddress);
                    this.ReportViewer1.LocalReport.DataSources.Add(new ReportDataSource("DataSetForTJ_SDYY", dt16));
                    break;
                case "TJ_RWLX.rdlc":
                    this.Page.Title = "任务类型统计表";
                    DataTable dt17 = dal.Get_TJ_RWLX(beginTime, endTime, eventType, stationID, agentID, doctorAndNurse,
                  txtCenter, txtDiseasesClassification, txtIllnessClassification, txtDeathCase, txtMeasures, txtFirstAidEffect,
                  txtFirstImpression, txtSendAddress);
                    this.ReportViewer1.LocalReport.DataSources.Add(new ReportDataSource("DataSetForTJ_RWLX", dt17));
                    break;
                case "TJ_HF.rdlc":
                    this.Page.Title = "回访统计表";
                    DataTable dt18 = dal.Get_TJ_HF(beginTime, endTime, eventType, stationID, agentID, doctorAndNurse,
                  txtCenter, txtDiseasesClassification, txtIllnessClassification, txtDeathCase, txtMeasures, txtFirstAidEffect,
                  txtFirstImpression, txtSendAddress);
                    this.ReportViewer1.LocalReport.DataSources.Add(new ReportDataSource("DataSetForTJ_HF", dt18));
                    break;
                default:
                    break;
            }
            this.ReportViewer1.LocalReport.Refresh();
        }
        //分中心的下拉框选择后，触发以下事件，根据分中心取出所有分站的内容。
        //protected void DropDownList_Center_SelectedIndexChanged(object sender, EventArgs e)
        //{
        //    string centerCode = DropDownList_Center.SelectedValue;
        //    DropDownList_Station.Items.Clear();
        //    DropDownListBLL sbll = new DropDownListBLL();
        //    ControlHelper.SetDropDownList(DropDownList_Station, sbll.GetStationOnaspx(DropDownList_Center.SelectedValue), "-1");//分站
        //}
    }
}