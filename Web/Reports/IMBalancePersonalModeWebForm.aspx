<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="IMBalancePersonalModeWebForm.aspx.cs" Inherits="Anke.SHManage.Web.Reports.IMBalancePersonalModeWebForm" %>

<%@ Register Assembly="Microsoft.ReportViewer.WebForms, Version=11.0.0.0, Culture=neutral, PublicKeyToken=89845dcd8080cc91" Namespace="Microsoft.Reporting.WebForms" TagPrefix="rsweb" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
<meta http-equiv="Content-Type" content="text/html; charset=utf-8"/>
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
           //取分类
            var urlstr2 = "../IM/AllDictionary/GetMaterialTypeByUserInfo";
            $.EUIcomboboxTree("#MaterialType", {
                url: urlstr2,//ajax后台取数据路径，返回的是json格式的数据
                OneOption: [{
                    id: "",
                    text: "--请选择--",
                }],
                datainfo: {}
            });

            $("#DeliveryStorage").combotree('clear');
            //取领取人，去向 （其实要的是StorageCode）
            var urlinfo = "../IM/AllDictionary/GetUsers";
            $('#DeliveryPerson').combobox({
                prompt: '--请选择--',
                url: urlinfo,
                valueField: 'UserID',
                textField: 'Name',
                method: 'POST',
                filter: function (q, row) {
                    var opts = $('#DeliveryPerson').combobox('options');
                    return row[opts.textField].indexOf(q) > -1;
                },
                onSelect: function (rec) {
                    var urlstr1 = "../IM/AllDictionary/GetStogageByUser?userId=" + rec.UserID;
                    $.BindCombox("#DeliveryStorage", urlstr1);
                }
            });
        }
    </script>
</head>
<body>
    <form id="form1" runat="server">
        <asp:ScriptManager ID="ScriptManager1" runat="server"></asp:ScriptManager>
        <table>
            <tr>
                 <td  style="vertical-align: text-top; text-align: right">月份：</td>
                <td>
                    <asp:TextBox ID="textMonth" class="easyui-numberbox" data-options="required:true" runat="server"></asp:TextBox>
                </td>
                 <td style="vertical-align: text-top; text-align: right">分类：</td>
                <td>
                    <asp:TextBox ID="MaterialType" class="easyui-combobox" style="width:180px" runat="server"></asp:TextBox>
                </td>
                  <td  style="vertical-align: text-top; text-align: right">领取人：</td>
                <td>
                    <asp:TextBox ID="DeliveryPerson" class="easyui-combobox" data-options="required:true"  runat="server"></asp:TextBox>
                </td>
                <td style="vertical-align: text-top; text-align: right">物资去向：</td>
                <td>
                    <asp:TextBox ID="DeliveryStorage" class="easyui-combotree" runat="server"></asp:TextBox>
                </td>
                <td>
                    <asp:Button ID="btnSearch" runat="server" Text="查询" OnClick="btnSearch_Click" /> 

                </td>
            </tr>
        </table>
    
        <rsweb:ReportViewer ID="ReportViewer1" runat="server" Width="100%" Height="100%"></rsweb:ReportViewer>
    </form>
</body>
</html>
