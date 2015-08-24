﻿<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="TJ_JJRYGZXLModeWebForm.aspx.cs" Inherits="Anke.SHManage.Web.Reports.TJ_JJRYGZXLModeWebForm" %>

<%@ Register Assembly="Microsoft.ReportViewer.WebForms, Version=11.0.0.0, Culture=neutral, PublicKeyToken=89845dcd8080cc91" Namespace="Microsoft.Reporting.WebForms" TagPrefix="rsweb" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
<meta http-equiv="Content-Type" content="text/html; charset=utf-8"/>
    <title>急救人员工作效率统计表</title>
    <script src="../Content/EasyUI/jquery.js"></script>
    <script src="../Content/EasyUI/jquery.easyui.js"></script>
    <script src="../Content/EasyUI/locale/easyui-lang-zh_CN.js"></script>
    <link href="../Content/EasyUI/themes/icon.css" rel="stylesheet" />
    <link href="../Content/EasyUI/themes/default/easyui.css" rel="stylesheet" />
    <script src="../Scripts/jquery.AnkeExtend.js"></script>
    <script type="text/javascript">
        //页面加载
        $(document).ready(function () {
            Initial();
        });
        function Initial() {
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
            })
        };
    </script>
</head>
<body>
    <form id="form1" runat="server">
    <asp:ScriptManager ID="ScriptManager1" runat="server"></asp:ScriptManager>
        <table>
            <tr>
                <td style="text-align:right">起始时间：</td>
                <td>
                    <asp:TextBox ID="StartDate" class="easyui-datetimebox" Style="width: 150px" runat="server"></asp:TextBox>

                </td>
                <td >&nbsp;&nbsp; 截止时间：</td>
                <td>
                    <asp:TextBox ID="EndDate" class="easyui-datetimebox" Style="width: 150px" runat="server"></asp:TextBox>

                </td>
                <td >&nbsp;&nbsp;&nbsp;&nbsp;&nbsp; 所属中心：</td>
                <td>
                    <asp:TextBox ID="Center" class="easyui-combobox" Style="width: 150px" runat="server"></asp:TextBox>
                </td>
                <td style="text-align:right">&nbsp; 所属分站：</td>
                <td>                   
                    <asp:TextBox ID="Station" class="easyui-combobox" Style="width: 150px" runat="server"></asp:TextBox>
                </td>
            </tr>
            <tr>

                <td style="text-align:right">&nbsp;姓名：</td>
                <td>
                    <asp:TextBox ID="Name" class="easyui-textbox" Style="width: 150px" runat="server"></asp:TextBox>
                </td>
                <td style="text-align:right">&nbsp;工号：</td>
                <td>
                    <asp:TextBox ID="WorkCode" class="easyui-textbox" runat="server" Width="150px"></asp:TextBox>
                </td>
                
                <td>               
                    &nbsp;&nbsp;&nbsp; <asp:Button ID="btnSearch" runat="server" Text="查询" OnClick="btnSearch_Click" Width="57px"  />

                    &nbsp;&nbsp;&nbsp
                </td>
            </tr>
        </table>
        <rsweb:ReportViewer ID="ReportViewer1" runat="server" Height="100%" Width="100%"></rsweb:ReportViewer>
    </form>
</body>
</html>