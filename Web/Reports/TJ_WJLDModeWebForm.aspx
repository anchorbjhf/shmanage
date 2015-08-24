<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="TJ_WJLDModeWebForm.aspx.cs" Inherits="Anke.SHManage.Web.Reports.TJ_WJLDModeWebForm" %>

<%@ Register Assembly="Microsoft.ReportViewer.WebForms, Version=11.0.0.0, Culture=neutral, PublicKeyToken=89845dcd8080cc91" Namespace="Microsoft.Reporting.WebForms" TagPrefix="rsweb" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
<meta http-equiv="Content-Type" content="text/html; charset=utf-8"/>
    <title>未接来电统计表</title>
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
            //$.InitDateTime();           

        });
        function fillCombobox() {

            //取DispatcherName 调度员姓名的下拉框的值
            var urlstr = "../BB/AllDictionary/GetPersonName";
            $.EUIcombobox("#Name", {
                url: urlstr,
                valueField: 'Name',
                textField: 'Name',
                editable: false,
                OneOption: [{
                    ID: "",
                    Name: "--请选择--"
                }]
            });
            
            $('#Result').combobox({
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
                    Name: '早释',
                }
                ],
                onSelect: function () {
                    $('#HiddenForResult').val($('#Result').combobox('getValues'));
                },
                onUnselect: function () {
                    $('#HiddenForResult').val($('#Result').combobox('getValues'));
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
                <td>查询时间：</td>
                <td>
                    <asp:TextBox ID="StartDate" class="easyui-datetimebox" Style="width: 150px" runat="server"></asp:TextBox>
                    
                </td>
                <td style="text-align:center">&nbsp;&nbsp; 至</td>
                <td>
                    <asp:TextBox ID="EndDate" class="easyui-datetimebox" Style="width: 150px" runat="server"></asp:TextBox>
                    
                </td>
                <td>&nbsp;&nbsp;&nbsp;&nbsp;&nbsp; 等待时间大于等于（秒）：</td>
                <td>
                   
                    <asp:TextBox ID="Time" class="easyui-textbox" Style="width: 150px" runat="server"></asp:TextBox>
                </td>
                <td>&nbsp;&nbsp;&nbsp;&nbsp; 处理结果：</td>
                <td>
                    <asp:HiddenField ID="HiddenForResult" runat="server" />
                    <select id="Result" class="easyui-combobox" style="width: 150px" data-options="editable:false,multiple:true">                                           
                    </select>
                    <%-- <asp:DropDownList ID="DropDownList_Result" runat="server"  Style="width: 150px">   
                            <asp:ListItem>--请选择--</asp:ListItem>                                                                     
                            <asp:ListItem>未接听</asp:ListItem>
                            <asp:ListItem>早释</asp:ListItem>                                                                             
                    </asp:DropDownList>  --%>      
                </td>
            </tr>
            <tr>
                <td style="text-align:right">&nbsp;&nbsp; 姓名：</td>
                <td>
                    <asp:TextBox ID="Name" class="easyui-combobox" Style="width: 150px" runat="server"></asp:TextBox>
                </td>
                <td style="text-align:right">&nbsp;&nbsp;&nbsp;&nbsp;&nbsp; 工号：</td>
                <td>
                    <asp:TextBox ID="WorkCode" class="easyui-textbox" Style="width: 150px" runat="server"></asp:TextBox>
                </td>
                
                <td>&nbsp;&nbsp;&nbsp;
                   <asp:Button ID="btnSearch" runat="server" Text="查询" OnClick="btnSearch_Click" />
                    
                    &nbsp;
                </td>
            </tr>
        </table>
        <rsweb:ReportViewer ID="ReportViewer1" runat="server" Height="100%" Width="100%"></rsweb:ReportViewer>
    </form>
</body>
</html>
