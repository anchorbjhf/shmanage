using Anke.SHManage.Web.Reports;
using Anke.SHManage.Web.Reports.DataSetForPrintTableAdapters;
using Microsoft.Reporting.WebForms;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Anke.SHManage.Web.Areas.PR.Controllers
{
    public class PrintController : Controller
    {
        //
        // GET: /PR/Print/
        
        public void PatientRecordPrint(string AlarmEventType,string TaskCode,int PatientOrder)
        {
            string aspxUrl = "~/Reports/PrintModeWebForm.aspx?AlarmEventType=" + Server.UrlEncode(AlarmEventType);
            aspxUrl += "&TaskCode=" + TaskCode;
            aspxUrl += "&PatientOrder=" + PatientOrder;
            aspxUrl += "&IfCharge=0";
            Response.Redirect(aspxUrl);
        }
        //收费打印调用,IfCharge = 1
        public void PatientChargePrint(string AlarmeventType, string TaskCode, int PatientOrder)
        {
            string aspxUrl = "~/Reports/PrintModeWebForm.aspx?AlarmEventType=" + Server.UrlEncode(AlarmeventType);
            aspxUrl += "&TaskCode=" + TaskCode;
            aspxUrl += "&PatientOrder=" + PatientOrder;
            aspxUrl += "&IfCharge=1";
            Response.Redirect(aspxUrl);
        }
    }
}

