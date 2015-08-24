<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="LS_BLModeWebForm.aspx.cs" Inherits="Anke.SHManage.Web.Reports.LS_BLModeWebForm" %>

<%@ Register Assembly="Microsoft.ReportViewer.WebForms, Version=11.0.0.0, Culture=neutral, PublicKeyToken=89845dcd8080cc91" Namespace="Microsoft.Reporting.WebForms" TagPrefix="rsweb" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <meta http-equiv="Content-Type" content="text/html; charset=utf-8" />
    <title></title>
    <script src="../Content/EasyUI/jquery.js"></script>
    <script src="../Content/EasyUI/jquery.easyui.js"></script>
    <script src="../Content/EasyUI/locale/easyui-lang-zh_CN.js"></script>
    <link href="../Content/EasyUI/themes/icon.css" rel="stylesheet" />
    <link href="../Content/EasyUI/themes/default/easyui.css" rel="stylesheet" />
    <script src="../Scripts/jquery.AnkeExtend.js"></script>
    <script type="text/javascript">
        //页面加载
        $(document).ready(function () {

            fillCombobox();

        });

        function fillCombobox() {
            //取所属分中心的下拉框的值
            $("#Center").combobox({
                url: "../BB/AllDictionary/GetCenterName",
                valueField: 'ID',
                textField: 'Name',
                editable: false,
                multiple: true,
                prompt: '--请选择--',
                onSelect: function (rec) {
                    var ids = $('#Center').combobox('getValues');
                    //根据分中心取分站
                    $("#Station").combobox({
                        url: "../BB/AllDictionary/GetSationName?centerID=" + ids,
                        method: 'POST',
                        valueField: 'ID',
                        textField: 'Name',
                        multiple: true,
                        editable: false,
                        onSelect: function () {
                            $('#Station').combobox('getValues')
                        },
                        onUnselect: function () {
                            $('#Station').combobox('getValues')
                        }
                    });
                    $("#Station").combobox('clear');
                },
                onUnselect: function (rec) {
                    var ids = $('#Center').combobox('getValues');                
                    //根据分中心取分站
                    $("#Station").combobox({
                        url: "../BB/AllDictionary/GetSationName?centerID=" + ids,
                        method: 'POST',
                        valueField: 'ID',
                        textField: 'Name',
                        multiple: true,
                        editable: false
                        //onSelect: function () {
                        //    //$('#Station').combobox('getValues')
                        //},
                        //onUnselect: function () {
                        //    $('#Station').combobox('getValues')
                        //}
                    });
                    // $("#Station").combobox('setValues', '');
                    $("#Station").combobox('clear');
                },              
            });

            var centers = $('#Center').combobox('getValues');
            $("#Station").combobox({
                url: "../BB/AllDictionary/GetSationName?centerID=" + centers,
                method: 'POST',
                valueField: 'ID',
                textField: 'Name',
                multiple: true,
                editable: false,
                onSelect: function () {
                    $('#Station').combobox('getValues')
                },
                onUnselect: function () {
                    $('#Station').combobox('getValues')
                }
            });
       
            
            //取病种分类           
            var urlstr = "../BB/AllDictionary/GetDiseasesClassifications";
            $.EUIcombobox("#DiseasesClassification", {
                url: urlstr,
                valueField: 'Name',
                textField: 'Name',
                editable: false,
                OneOption: [{
                    ID: "",
                    Name: "--请选择--"
                }]
            });
            //取事件类型
            var urlstr = "../BB/AllDictionary/GetEventType";
            $.EUIcombobox("#EventType", {
                url: urlstr,
                valueField: 'Name',
                textField: 'Name',
                editable: false,
                OneOption: [{
                    ID: "",
                    Name: "--请选择--"
                }]
            });
            //$('#EventType').combobox({
            //    editable: false,
            //    multiple: true,
            //    onSelect: function () {
            //        $('#HiddenForEventType').val($('#EventType').combobox('getText'));
            //    }
            //})
            //$('#EventType').combobox('clear');

            //取病情分类           
            var urlstr = "../BB/AllDictionary/GetIllnessClassification";
            $.EUIcombobox("#IllnessClassification", {
                url: urlstr,
                valueField: 'Name',
                textField: 'Name',
                editable: false,
                OneOption: [{
                    ID: "",
                    Name: "--请选择--"
                }]
            });
            //取病情预报           
            var urlstr = "../BB/AllDictionary/GetIllnessForecast";
            $.EUIcombobox("#IllnessForecast", {
                url: urlstr,
                valueField: 'Name',
                textField: 'Name',
                editable: false,
                OneOption: [{
                    ID: "",
                    Name: "--请选择--"
                }]
            });
            //取急救效果         
            var urlstr = "../BB/AllDictionary/GetFirstAidEffect";
            $.EUIcombobox("#FirstAidEffect", {
                url: urlstr,
                valueField: 'Name',
                textField: 'Name',
                editable: false,
                OneOption: [{
                    ID: "",
                    Name: "--请选择--"
                }]
            });
            //取病家合作度         
            var urlstr = "../BB/AllDictionary/GetDiseaseCooperation";
            $.EUIcombobox("#DiseaseCooperation", {
                url: urlstr,
                valueField: 'Name',
                textField: 'Name',
                editable: false,
                OneOption: [{
                    ID: "",
                    Name: "--请选择--"
                }]
            });
            //死亡类型
            var urlstr = "../BB/AllDictionary/GetDeathCase";
            $.EUIcombobox("#DeathCase", {
                url: urlstr,
                valueField: 'Name',
                textField: 'Name',
                editable: false,
                OneOption: [{
                    ID: "",
                    Name: "--请选择--"
                }]
            });
            //死亡证明类型
            var urlstr = "../BB/AllDictionary/GetDeathCertificate";
            $.EUIcombobox("#DeathCertificate", {
                url: urlstr,
                valueField: 'Name',
                textField: 'Name',
                editable: false,
                OneOption: [{
                    ID: "",
                    Name: "--请选择--"
                }]
            });
        }
    </script>
</head>
<body>
    <form id="form1" runat="server">
        <asp:ScriptManager ID="ScriptManager1" runat="server"></asp:ScriptManager>
        <table>
            <tr>
                <td>查询时间：</td>
                <td>
                    <asp:TextBox ID="StartDate" class="easyui-datetimebox" Style="width: 150px" runat="server"></asp:TextBox>

                </td>
                <td style="text-align: center">&nbsp; 至</td>
                <td>
                    <asp:TextBox ID="EndDate" class="easyui-datetimebox" Style="width: 150px" runat="server"></asp:TextBox>

                </td>
                <td>&nbsp; 事件类型：</td>
                <td>
                    <asp:TextBox ID="EventType" class="easyui-combobox" Style="width: 150px" runat="server"></asp:TextBox>
                   <%-- <asp:HiddenField ID="HiddenForEventType" runat="server" />
                    <select id="EventType" class="easyui-combobox" style="width: 150px" data-options="editable:false,multiple:true">
                        <option value="救治">'救治'</option>
                        <option value="一般转院">'一般转院'</option>
                        <option value="急救转院">'急救转院'</option>
                        <option value="空车">'空车'</option>
                        <option value="回家">'回家'</option>
                    </select>--%>
                </td>
                <td>&nbsp;&nbsp; 病种分类： </td>
                <td>
                    <asp:TextBox ID="DiseasesClassification" class="easyui-combobox" Style="width: 150px" runat="server"></asp:TextBox>
                </td>
                <td style="text-align: right">&nbsp; 急救医护：</td>
                <td>
                    <asp:TextBox ID="DoctorAndNurse" class="easyui-textbox" Style="width: 150px" runat="server"></asp:TextBox>
                </td>
                <td>&nbsp;&nbsp;&nbsp;&nbsp;
                   <asp:Button ID="btnSearch" runat="server" Text="查询" OnClick="btnSearch_Click" />

                    &nbsp;
                </td>
            </tr>
            <tr>
                <td style="text-align: right">&nbsp; 分中心：</td>
                <td>
                    <asp:TextBox ID="Center" class="easyui-combobox" Style="width: 150px" runat="server"></asp:TextBox>
                </td>
                <td style="text-align: right">&nbsp; 分站：</td>
                <td>
                    <asp:TextBox ID="Station" class="easyui-combobox" Style="width: 150px" runat="server"></asp:TextBox>
                </td>
                <td>&nbsp; 患者姓名：</td>
                <td>
                    <asp:TextBox ID="Name" class="easyui-textbox" Style="width: 150px" runat="server"></asp:TextBox>
                </td>
                <td style="text-align: right">&nbsp; 司机：</td>
                <td>
                    <asp:TextBox ID="Driver" class="easyui-textbox" Style="width: 150px" runat="server"></asp:TextBox>
                </td>
                <td style="text-align: right">&nbsp; 担架员：</td>
                <td>
                    <asp:TextBox ID="Stretcher" class="easyui-textbox" Style="width: 150px" runat="server"></asp:TextBox>
                </td>
            </tr>
            <tr>
                <td style="text-align: right">&nbsp; 主诉：</td>
                <td>
                    <asp:TextBox ID="AlarmReason" class="easyui-textbox" Style="width: 150px" runat="server"></asp:TextBox>
                </td>
                <td>&nbsp; 病情分类：</td>
                <td>
                    <asp:TextBox ID="IllnessClassification" class="easyui-combobox" Style="width: 150px" runat="server"></asp:TextBox>
                </td>
                <td>&nbsp; 病情预报：</td>
                <td>
                    <asp:TextBox ID="IllnessForecast" class="easyui-combobox" Style="width: 150px" runat="server"></asp:TextBox>
                </td>
                <td style="text-align: right">&nbsp; 急救效果：</td>
                <td>
                    <asp:TextBox ID="FirstAidEffect" class="easyui-combobox" Style="width: 150px" runat="server"></asp:TextBox>
                </td>
                <td>&nbsp; 病家合作度：</td>
                <td>
                    <asp:TextBox ID="DiseaseCooperation" class="easyui-combobox" Style="width: 150px" runat="server"></asp:TextBox>
                </td>
            </tr>
            <tr>
                <td style="text-align: right">&nbsp; 初步印象：</td>
                <td>
                    <asp:TextBox ID="FirstImpression" class="easyui-textbox" Style="width: 150px" runat="server"></asp:TextBox>
                </td>
                <td>&nbsp; 死亡类型：</td>
                <td>
                    <asp:TextBox ID="DeathCase" class="easyui-combobox" Style="width: 150px" runat="server"></asp:TextBox>
                </td>
                <td>&nbsp; 死亡证明：</td>
                <td>
                    <asp:TextBox ID="DeathCertificate" class="easyui-combobox" Style="width: 150px" runat="server"></asp:TextBox>
                </td>
                <td style="text-align: right">&nbsp; 救治措施：</td>
                <td>
                    <asp:TextBox ID="TreatmentMeasure" class="easyui-textbox" Style="width: 150px" runat="server"></asp:TextBox>
                </td>               
            </tr>
        </table>
        <rsweb:ReportViewer ID="ReportViewer1" runat="server" Height="100%" Width="100%"></rsweb:ReportViewer>
    </form>
</body>
</html>
