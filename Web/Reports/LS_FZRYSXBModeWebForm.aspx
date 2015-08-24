<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="LS_FZRYSXBModeWebForm.aspx.cs" Inherits="Anke.SHManage.Web.Reports.TJ_FZRYSXBModWebForm" %>

<%@ Register Assembly="Microsoft.ReportViewer.WebForms, Version=11.0.0.0, Culture=neutral, PublicKeyToken=89845dcd8080cc91" Namespace="Microsoft.Reporting.WebForms" TagPrefix="rsweb" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <meta http-equiv="Content-Type" content="text/html; charset=utf-8" />
    <title>分站人员上下班流水表</title>
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
                $("#BelongCenter").combobox({
                    url: "../BB/AllDictionary/GetCenterName",
                    valueField: 'ID',
                    textField: 'Name',
                    editable: false,
                    multiple: true,
                    prompt: '--请选择--',
                    onSelect: function (rec) {
                        var ids = $('#BelongCenter').combobox('getValues');
                        //根据分中心取分站
                        $("#BelongSation").combobox({
                            url: "../BB/AllDictionary/GetSationName?centerID=" + ids,
                            method: 'POST',
                            valueField: 'ID',
                            textField: 'Name',
                            multiple: true,
                            editable: false,
                            onSelect: function () {
                                $('#BelongSation').combobox('getValues')
                            },
                            onUnselect: function () {
                                $('#BelongSation').combobox('getValues')
                            }
                        });
                        $("#BelongSation").combobox('clear');
                    },
                    onUnselect: function (rec) {
                        var ids = $('#BelongCenter').combobox('getValues');
                        //根据分中心取分站
                        $("#BelongSation").combobox({
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
                        $("#BelongSation").combobox('clear');
                    },
                });

                var centers = $('#BelongCenter').combobox('getValues');
                $("#BelongSation").combobox({
                    url: "../BB/AllDictionary/GetSationName?centerID=" + centers,
                    method: 'POST',
                    valueField: 'ID',
                    textField: 'Name',
                    multiple: true,
                    editable: false,
                    onSelect: function () {
                        $('#BelongSation').combobox('getValues')
                    },
                    onUnselect: function () {
                        $('#BelongSation').combobox('getValues')
                    }
                });
                //取角色下拉框的值
                $('#Role').combobox({
                    url: "../BB/AllDictionary/GetRole",
                    valueField: 'ID',
                    textField: 'Name',
                    editable: false,
                    multiple: true,
                    onSelect: function () {
                        $('#HiddenForRole').val($('#Role').combobox('getValues'));
                    },
                    onUnselect: function () {
                        $('#HiddenForRole').val($('#Role').combobox('getValues'));
                    }
                });
                
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
                <td style="text-align:center">至</td>
                <td>
                    <asp:TextBox ID="EndDate" class="easyui-datetimebox" Style="width: 150px" runat="server"></asp:TextBox>
                    
                </td>
                <td>&nbsp;&nbsp;&nbsp; 所属中心：</td>
                <td>
                    <%--<asp:HiddenField ID="HiddenForCenter" runat="server" />--%>
                    <asp:TextBox ID="BelongCenter" class="easyui-combobox" Style="width: 150px" runat="server"></asp:TextBox>
                </td>
                <td>&nbsp;&nbsp;&nbsp; 所属分站：</td>
                <td>
                    <asp:TextBox ID="BelongSation" class="easyui-combobox" Style="width: 150px" runat="server"></asp:TextBox>
                </td>
            </tr>
            <tr>
                <td style="text-align:right">职务：</td>
                <td>
                   <asp:HiddenField ID="HiddenForRole" runat="server" />
                    <select id="Role" class="easyui-combobox" style="width: 150px" data-options="editable:false,multiple:true">                      
                        <%--<option value="司机">'司机'</option> 
                        <option value="医生">'医生'</option>
                        <option value="护士">'护士'</option>
                        <option value="担架员">'担架员'</option> --%>
                    </select>
                    
                </td>
                <td style="text-align:right">&nbsp; 车号：</td>
                <td>
                    <asp:TextBox ID="CarNumber" class="easyui-textbox" Style="width: 150px" runat="server"></asp:TextBox>
                </td>
                <td style="text-align:right">工号：</td>
                <td>
                    <asp:TextBox ID="WorkNumber" class="easyui-textbox" Style="width: 150px" runat="server"></asp:TextBox>
                </td>
                <td style="text-align:right">姓名：</td>
                <td>
                    <asp:TextBox ID="Name" class="easyui-textbox" Style="width: 150px" runat="server"></asp:TextBox>
                </td>
                <td>&nbsp;
                   <asp:Button ID="btnSearch" runat="server" Text="查询" OnClick="btnSearch_Click" />
                    
                    &nbsp;
                </td>
            </tr>
        </table>
        <rsweb:ReportViewer ID="ReportViewer1" runat="server" Height="100%" Width="100%"></rsweb:ReportViewer>
    </form>
</body>
</html>
