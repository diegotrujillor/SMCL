<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="ReportMonitoringComments.aspx.cs" Inherits="SMCL.Extensions.Containers.ReportMonitoringComments" %>

<%@ Register Assembly="Microsoft.ReportViewer.WebForms, Version=10.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a"
    Namespace="Microsoft.Reporting.WebForms" TagPrefix="rsweb" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>SMCL @ Reporte Datos Señales</title>
    <link rel="Shortcut Icon" href="../../Content/Images/johnson_and_johnson_no_bg.ico" />
    
    <link href="../../Content/Reports.css" rel="stylesheet" type="text/css" />
    
    <link href="../../Content/themes/ui-lightness/jquery-ui-1.8.19.custom.css" rel="stylesheet" type="text/css" media="all" />
    <script type="text/javascript" src="../../Scripts/jquery-1.7.1.min.js"></script>
    <script type="text/javascript" src="../../Scripts/jquery-ui-1.8.19.custom.min.js"></script>
    <script type="text/javascript" src="../../Scripts/jquery-ui-datepicker-es.js"></script>

    <script type="text/javascript">
        $(function () {
            $('#startDate').datepicker({
                dateFormat: 'yy/mm/dd',
                changeMonth: true,
                changeYear: true,
                maxDate: '+0d',
                onSelect:
                        function (dateText, inst) {
                            $('#finalDate').datepicker('option', 'minDate', new Date(dateText));
                        }
            });
            $('#startDate').datepicker($.datepicker.regional['es']);
            $('#startDate').datepicker('option', 'showAnim', 'slide');

            $('#finalDate').datepicker({
                dateFormat: 'yy/mm/dd',
                changeMonth: true,
                changeYear: true,
                maxDate: '+0d'
            });
            $('#finalDate').datepicker($.datepicker.regional['es']);
            $('#finalDate').datepicker('option', 'showAnim', 'slide');
        });
    </script>
    <style type="text/css">
        .style1
        {
            height: 16px;
        }
    </style>
</head>
<body>
    <form id="formReportingData" runat="server" class="report_form1">
        <input id="alarmTypeExclusionId" name="alarmTypeExclusionId" runat="server" type="hidden" />
        <table id="no_css" style="width: 100%; table-layout:fixed;">
            <tr>
                <td style="width: 25%">
                    <a href="http://www.jnjcolombia.com.co" target="_blank">
                        <img src="../../Content/Images/jnj-blanco.png" alt="J&J" 
                        title="Johnson & Johnson de Colombia S.A" width="99%" height="60px" border="0" />
                    </a>
                </td>
                <td  style="width: 65%">
                    <h2 style="color: #FFF; font-size: x-large; padding-left: 3%; text-align: center;">
                        <%= System.Configuration.ConfigurationManager.AppSettings["HeaderMessage"].ToString() %>
                    </h2>
                </td>
                <td  style="width: 10%">
                    <img src="../../Content/Images/qa_jyj.png" alt="QA" title="Quality Assurance Latin America" width="90%" style="padding-top: 5%;" />
                </td>
            </tr>
        </table>
        <div class="report_outer_div">
            <asp:ScriptManager ID="ScriptManager1" runat="server">
            </asp:ScriptManager>
            <div class="report_filters_div">
                <table id="report_table_filters">
                    <tr>
                        <td colspan="4">
                            <br />
                            <label style="font-weight: bold;">
                                Parámetros para generación de Reporte:
                            </label>
                            <br />
                        </td>
                        <td style="text-align: right;">
                            <br />
                            <a href="../../Home/Reporting">
                                <img src="../../Content/Images/jnj_reports_go_back.png" alt="regresar" title="Regresar" border="0" />
                            </a>
                            <span style="color: #AAA;">&nbsp;Regresar...</span>
                        </td>
                    </tr>
                    <tr>
                        <td style="color: #FF0000; font-weight: bold">
                            <label>
                                Área (*):
                            </label>
                        </td>
                        <td style="color: #FF0000; font-weight: bold">
                            <label>
                                Equipo (*):
                            </label>
                        </td>
                        <td>
                            <label style="color: #FF0000; font-weight: bold">
                                Tipo Alarma (*):
                            </label>
                        </td>
                        <td>
                            <label style="color: #FF0000; font-weight: bold">
                                Señal (*):
                            </label>
                        </td>
                    </tr>
                    <tr>
                        <td class="style1">
                            <asp:DropDownList ID="ddlAreas" runat="server" AppendDataBoundItems="True" 
                                DataTextField="ARE_NAME" DataValueField="ARE_ID" 
                                OnSelectedIndexChanged="ddlAreas_SelectedIndexChanged" 
                                CssClass="report_dropdownlist_filters" AutoPostBack="True" 
                                oninit="ddlAreas_Init">
                                <asp:ListItem Value="0">-- Seleccione --</asp:ListItem>
                                <asp:ListItem Value="-1">-- Todos --</asp:ListItem>
                            </asp:DropDownList>
                        </td>
                        <td class="style1">
                            <asp:DropDownList ID="ddlAppliances" runat="server" 
                                DataTextField="APP_NAME" 
                                DataValueField="APP_ID" 
                                OnSelectedIndexChanged="ddlAppliances_SelectedIndexChanged" 
                                CssClass="report_dropdownlist_filters" AutoPostBack="True">
                                <asp:ListItem Value="0">-- Seleccione --</asp:ListItem>
                                <asp:ListItem Value="-1">-- Todos --</asp:ListItem>
                            </asp:DropDownList>
                        </td>
                        <td class="style1">
                            <asp:DropDownList ID="ddlAlarms" runat="server" AppendDataBoundItems="True" 
                                DataTextField="ALA_TYP_NAME" 
                                DataValueField="ALA_TYP_ID" 
                                onselectedindexchanged="ddlAlarms_SelectedIndexChanged" 
                                CssClass="report_dropdownlist_filters" 
                                oninit="ddlAlarms_Init">
                                <asp:ListItem Value="0">-- Seleccione --</asp:ListItem>
                                <asp:ListItem Value="-1">Altas y bajas</asp:ListItem>
                            </asp:DropDownList>
                        </td>
                        <td class="style1">
                            <asp:DropDownList ID="ddlSignals" runat="server" AppendDataBoundItems="True" 
                                DataTextField="SIG_NAME" 
                                DataValueField="SIG_ID" 
                                onselectedindexchanged="ddlSignals_SelectedIndexChanged" 
                                CssClass="report_dropdownlist_filters" 
                                oninit="ddlSignals_Init">
                                <asp:ListItem Value="0">-- Seleccione --</asp:ListItem>
                                <asp:ListItem Value="-1">-- Todos --</asp:ListItem>
                            </asp:DropDownList>
                        </td>
                    </tr>
                    <tr>
                        <td>
                            <label style="font-weight: bold; color: #FF0000">
                                Fecha Desde (*):
                            </label>
                        </td>
                        <td>
                            <label style="font-weight: bold; color: #FF0000">
                                Fecha Hasta (*):
                            </label>
                        </td>
                        <td>
                            &nbsp;</td>
                        <td>
                            &nbsp;</td>
                    </tr>
                    <tr>
                        <td>
                            <input type="text" name="startDate" id="startDate" runat="server" maxlength="10" size="20" />
                        </td>
                        <td>
                            <input type="text" name="finalDate" id="finalDate" runat="server" maxlength="10" size="20" />
                        </td>
                        <td>
                            &nbsp;</td>
                        <td>
                            &nbsp;</td>
                    </tr>
                </table>
                <br />
                <div style="border-top: solid 1px #CCC;" />
                <rsweb:ReportViewer ID="ReportViewer1" runat="server" Font-Names="Verdana" 
                Font-Size="8pt" InteractiveDeviceInfos="(Colección)" 
                WaitMessageFont-Names="Verdana" WaitMessageFont-Size="14pt" Width="100%" 
                    BorderWidth="1px" BorderColor="LightGray" ShowFindControls="False">
                <LocalReport ReportPath="Content\Reports\Report_Data_Signals.rdlc">
                    <DataSources>
                        <rsweb:ReportDataSource DataSourceId="odsMonitoring" 
                            Name="Report_Data_Signals_DS" />
                    </DataSources>
                </LocalReport>
                </rsweb:ReportViewer>

                    <asp:ObjectDataSource ID="odsMonitoring" runat="server" 
                        OldValuesParameterFormatString="original_{0}" SelectMethod="GetData" 
                        
                        
                        TypeName="SMCL.Models.DataSetMonitoringTableAdapters.SMCL_GET_SIGNALS_DATATableAdapter" 
                        ondatabinding="odsMonitoring_DataBinding">
                        <SelectParameters>
                            <asp:ControlParameter ControlID="ddlAlarms" DefaultValue="0" Name="ALARM_PARAM" 
                                PropertyName="SelectedValue" Type="Decimal" />
                            <asp:ControlParameter ControlID="ddlSignals" DefaultValue="0" 
                                Name="SIGNAL_PARAM" PropertyName="SelectedValue" Type="Decimal" />
                            <asp:ControlParameter ControlID="ddlAppliances" DefaultValue="0" 
                                Name="APPLIANCE_PARAM" PropertyName="SelectedValue" Type="Decimal" />
                            <asp:ControlParameter ControlID="ddlAreas" DefaultValue="0" Name="AREA_PARAM" 
                                PropertyName="SelectedValue" Type="Decimal" />
                            <asp:ControlParameter ControlID="startDate" DefaultValue="0" 
                                Name="INIT_DATE_PARAM" PropertyName="Value" Type="String" />
                            <asp:ControlParameter ControlID="finalDate" DefaultValue="0" 
                                Name="END_DATE_PARAM" PropertyName="Value" Type="String" />
                            <asp:ControlParameter ControlID="alarmTypeExclusionId" DefaultValue="0" 
                                Name="ALARM_TYPE_EXCLUSION" PropertyName="Value" Type="Decimal" />
                            <asp:Parameter Direction="Output" Name="RESULTADO_C" Type="Object" />
                        </SelectParameters>
                    </asp:ObjectDataSource>

                <a href="../../Home/Reporting" style="text-decoration: none;">
                    <img src="../../Content/Images/jnj_reports_go_back2.png" alt="regresar" title="Regresar" border="0" width="24px" height="24px" />
                </a>
                <span style="color: #AAA;">&nbsp;Regresar...</span>
            </div>
        </div>
    </form>
    <table align="right" style="table-layout: fixed;">
		<tr>
			<td style="width: 5%;">
				<a href="http://validator.w3.org/check?uri=referer"><img
						src="../../Content/Images/jnj_valid_xhtml10.png"
						alt="Valid XHTML 1.0 Transitional" height="22" width="62"
						border="0" /> </a>
			</td>
			<td style="width: 5%;">
				<a href="http://jigsaw.w3.org/css-validator/check/referer">
					<img style="border: 0; width: 62px; height: 22px" border="0"
						src="../../Content/Images/jnj_valid_css.gif"
						alt="Valid CSS!" /> </a>
			</td>
			<td style="width: 5%;">
				<a href="http://validator.w3.org/" target="_blank"> <img
						src="../../Content/Images/jnj_valid_html5_badge_h_css3_semantics.png"
						width="62" height="24" border="0"
						alt="HTML5 Powered with CSS3 / Styling, and Semantics"
						title="HTML5 Powered with CSS3 / Styling, and Semantics" />
				</a>
			</td>
			<td style="width: 85%; text-align: right;">
				<img src="../../Content/Images/1.gif" alt="bullet-1" />
				<label style="font-family: verdana; color: #00ffff; font-size: x-small;">
                <%= System.Configuration.ConfigurationManager.AppSettings["FooterMessage1"].ToString() %>
                    <a href="<%= System.Configuration.ConfigurationManager.AppSettings["FooterMessage1Link"].ToString() %>" target="_blank" style="color: #00ffff; text-decoration: none; font-weight: bold; font-family: verdana; font-size: x-small;">
                <%= System.Configuration.ConfigurationManager.AppSettings["FooterMessage2"].ToString() %>
					</a>
                    <br />
                    <%= System.Configuration.ConfigurationManager.AppSettings["FooterMessage3"].ToString() %><b><%= System.Configuration.ConfigurationManager.AppSettings["FooterMessage4"].ToString() %></b>
                </label>
			</td>
		</tr>
	</table>
</body>
</html>