<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="TJ_JZCSModeWebForm.aspx.cs" Inherits="Anke.SHManage.Web.Reports.TJ_JZCSModeWebForm" %>

<%@ Register Assembly="Microsoft.ReportViewer.WebForms, Version=11.0.0.0, Culture=neutral, PublicKeyToken=89845dcd8080cc91" Namespace="Microsoft.Reporting.WebForms" TagPrefix="rsweb" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <meta http-equiv="Content-Type" content="text/html; charset=utf-8" />
    <title><%=Title%></title>
    <script src="../Content/EasyUI/jquery.js"></script>
    <script src="../Content/EasyUI/jquery.easyui.js"></script>
    <script src="../Content/EasyUI/locale/easyui-lang-zh_CN.js"></script>
    <link href="../Content/EasyUI/themes/icon.css" rel="stylesheet" />
    <link href="../Content/EasyUI/themes/default/easyui.css" rel="stylesheet" />
    <script src="../Scripts/jquery.AnkeExtend.js"></script>
    <script type="text/javascript">
        //页面加载
        $(document).ready(function () {

            reload();
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

            //取事件类型           
            var urlstr = "../BB/AllDictionary/GetPatientVersion";
            $.EUIcomboboxForReport("#txtPatientVersion", {
                url: urlstr,
                valueField: 'Name',
                textField: 'Name',
                editable: false,
                OneOption: [{
                    ID: "",
                    Name: "--请选择--"
                }]

            });

            //病种分类
            var urlstr = "../BB/AllDictionary/GetDiseasesClassifications";
            $.EUIcomboboxForReport("#txtDiseasesClassification", {
                url: urlstr,
                valueField: 'Name',
                textField: 'Name',
                editable: false,
                OneOption: [{
                    ID: "",
                    Name: "--请选择--"
                }]


            });
            //病情类型
            var urlstr = "../BB/AllDictionary/GetIllnessClassification";
            $.EUIcomboboxForReport("#txtIllnessClassification", {
                url: urlstr,
                valueField: 'Name',
                textField: 'Name',
                editable: false,
                OneOption: [{
                    ID: "",
                    Name: "--请选择--"
                }]

            });

            //急救效果
            var urlstr = "../BB/AllDictionary/GetFirstAidEffect";
            $.EUIcomboboxForReport("#txtFirstAidEffect", {
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
            $.EUIcomboboxForReport("#txtDeathCase", {
                url: urlstr,
                valueField: 'Name',
                textField: 'Name',
                editable: false,
                OneOption: [{
                    ID: "",
                    Name: "--请选择--"
                }]


            });
        };
        // 实现UpdatePanel中可以再添加easyUI的样式
        function reload() {
            Sys.WebForms.PageRequestManager.getInstance().add_endRequest(EndRequestHandler);
        }
        function EndRequestHandler() {
            fillCombobox();
            $('#StartDate').datetimebox();
            $('#EndDate').datetimebox();
            $('#txtAgentWorkID').textbox();
            $('#txtDoctorAndNurse').textbox();
            $('#txtMeasures').textbox();
            $('#txtFirstImpression').textbox();
            $('#txtSendAddress').textbox();
        }

       
    </script>
</head>
<body>

    <form id="form1" runat="server">
        <asp:ScriptManager ID="ScriptManager1" runat="server"></asp:ScriptManager>
        <asp:UpdatePanel ID="UpdatePanel1" runat="server">
            <ContentTemplate>
                <table>
                    <tr>
                        <td>查询时间：</td>
                        <td>
                            <asp:TextBox ID="StartDate" class="easyui-datetimebox" Style="width: 150px" runat="server"></asp:TextBox>
                        </td>
                        <td style="text-align: center">至</td>
                        <td>
                            <asp:TextBox ID="EndDate" class="easyui-datetimebox" Style="width: 150px" runat="server"></asp:TextBox>
                        </td>
                        <td>经办人工号：</td>
                        <td>
                            <asp:TextBox ID="txtAgentWorkID" class="easyui-textbox" Style="width: 150px" runat="server"></asp:TextBox>
                        </td>
                        <td>急救医护人：</td>
                        <td>
                            <asp:TextBox ID="txtDoctorAndNurse" class="easyui-textbox" Style="width: 150px" runat="server"></asp:TextBox>
                        </td>

                        <td>事件类型：</td>
                        <td>
                            <asp:TextBox ID="txtPatientVersion" class="easyui-combobox" Style="width: 150px" runat="server"></asp:TextBox>
                        </td>
                    </tr>
                    <tr>
                        <td style="text-align: right">分中心：</td>
                        <td>
                             <asp:TextBox ID="Center" class="easyui-combobox" Style="width: 150px" runat="server"></asp:TextBox>
                            <%--<asp:DropDownList ID="DropDownList_Center" runat="server" Width="150px" AutoPostBack="True" OnSelectedIndexChanged="DropDownList_Center_SelectedIndexChanged"></asp:DropDownList>--%>
                        </td>
                        <td style="text-align: right">分站：</td>
                        <td>
                             <asp:TextBox ID="Station" class="easyui-combobox" Style="width: 150px" runat="server"></asp:TextBox>
                            <%--<asp:DropDownList ID="DropDownList_Station" runat="server" Width="150px" AutoPostBack="True"></asp:DropDownList>--%>
                        </td>
                        <td style="text-align: right">病种分类：</td>
                        <td>
                            <asp:TextBox ID="txtDiseasesClassification" class="easyui-combobox" Style="width: 150px" runat="server"></asp:TextBox>
                        </td>
                        <td style="text-align: right">病情类型：</td>
                        <td>
                            <asp:TextBox ID="txtIllnessClassification" class="easyui-combobox" Style="width: 150px" runat="server"></asp:TextBox>
                        </td>
                        <td style="text-align: right">死亡类型：</td>
                        <td>
                            <asp:TextBox ID="txtDeathCase" class="easyui-combobox" Style="width: 150px" runat="server"></asp:TextBox>
                        </td>
                    </tr>
                    <tr>
                        <td style="text-align: right">救治措施：</td>
                        <td>
                            <asp:TextBox ID="txtMeasures" class="easyui-textbox" Style="width: 150px" runat="server"></asp:TextBox>
                        </td>
                        <td style="text-align: right">急救效果：</td>
                        <td>
                            <asp:TextBox ID="txtFirstAidEffect" class="easyui-combobox" Style="width: 150px" runat="server"></asp:TextBox>
                        </td>
                        <td style="text-align: right">诊断：</td>
                        <td>
                            <asp:TextBox ID="txtFirstImpression" class="easyui-textbox" Style="width: 150px" runat="server"></asp:TextBox>
                        </td>
                        <td style="text-align: right">送往地点：</td>
                        <td>
                            <asp:TextBox ID="txtSendAddress" class="easyui-textbox" Style="width: 150px" runat="server"></asp:TextBox>
                        </td>
                        <td></td>
                        <td>
                            <asp:Button ID="btnSearch" runat="server" Text="查询" OnClick="btnSearch_Click" />
                        </td>
                    </tr>
                </table>
            </ContentTemplate>
        </asp:UpdatePanel>
        <rsweb:ReportViewer ID="ReportViewer1" runat="server" Width="100%" Height="100%"></rsweb:ReportViewer>
    </form>
</body>
</html>
