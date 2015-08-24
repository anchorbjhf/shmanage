﻿<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="TJ_LDJLModeWebForm.aspx.cs" Inherits="Anke.SHManage.Web.Reports.TJ_LDJLModeWebForm" %>

<%@ Register Assembly="Microsoft.ReportViewer.WebForms, Version=11.0.0.0, Culture=neutral, PublicKeyToken=89845dcd8080cc91" Namespace="Microsoft.Reporting.WebForms" TagPrefix="rsweb" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
<meta http-equiv="Content-Type" content="text/html; charset=utf-8"/>
    <title>来电记录统计表</title>
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
            $('#actionResult').combobox({
                valueField: 'ID',
                textField: 'Name',
                method: 'get',
                editable: false,
                panelHeight: 'auto',
                multiple: true,
                prompt: '--请选择--',
                data: [
                {
                    ID: '0',
                    Name: '未接听',
                },
                {
                    ID: '1',
                    Name: '已接听'
                },
                {
                    ID: '2',
                    Name: '早释',
                }
                ],
                onSelect: function () {
                    $('#HiddenForResult').val($('#actionResult').combobox('getValues'));
                },
                onUnselect: function () {
                    $('#HiddenForResult').val($('#actionResult').combobox('getValues'));
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
                <td>&nbsp;&nbsp;&nbsp; 起始时间：</td>
                <td>
                    <asp:TextBox ID="StartDate" class="easyui-datetimebox" Style="width: 150px" runat="server"></asp:TextBox>

                </td>
                <td>&nbsp;&nbsp;&nbsp;&nbsp;&nbsp; 截止时间：</td>
                <td>
                    <asp:TextBox ID="EndDate" class="easyui-datetimebox" Style="width: 150px" runat="server"></asp:TextBox>
                </td>               
            </tr>
            <tr>
                <td>&nbsp;&nbsp;&nbsp; 主叫号码：</td>
                <td>
                    <asp:TextBox ID="TelNumber" class="easyui-textbox" Style="width: 150px" runat="server"></asp:TextBox>
                </td>
                <td>&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;处理结果：</td>
                <td>                   
                    <asp:HiddenField ID="HiddenForResult" runat="server" />
                    <select id="actionResult" class="easyui-combobox" style="width: 150px" data-options="editable:false,multiple:true">                                                             
                    </select>                                 
                </td>             
                <td>&nbsp;&nbsp;
                   <asp:Button ID="btnSearch" runat="server" Text="查询" OnClick="btnSearch_Click"  />

                    &nbsp;
                </td>
            </tr>
        </table>
        <rsweb:ReportViewer ID="ReportViewer1" runat="server" Height="100%" Width="100%"></rsweb:ReportViewer>
    </form>
</body>
</html>
