<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="ReportGraphicSignalPanel.aspx.cs" Inherits="SMCL.Extensions.ReportGraphicSignalPanel" %>

<%@ Register assembly="Microsoft.ReportViewer.WebForms, Version=10.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a" namespace="Microsoft.Reporting.WebForms" tagprefix="rsweb" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>SMCL @ Reporte Gráficas Señales</title>
    <link rel="Shortcut Icon" href="../../Content/Images/johnson_and_johnson_no_bg.ico" />

    <link href="../../Content/Reports.css" rel="stylesheet" type="text/css" />
    
    <link rel="stylesheet" type="text/css" href="../../Content/jquery.jqplot.min.css" />
    <link type="text/css" rel="stylesheet" href="../../Content/shCoreDefault.min.css" />
    <link type="text/css" rel="stylesheet" href="../../Content/shThemejqPlot.min.css" />
    <!--[if lt IE 9]><script language="javascript" type="text/javascript" src="../../Scripts/excanvas.js"></script><![endif]-->
    <script type="text/javascript" src="../../Scripts/jquery-main-jqplot.min.js"></script>

    
    <link href="../../Content/themes/ui-lightness/jquery-ui-1.8.19.custom.css" rel="stylesheet" type="text/css" media="all" />
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
</head>
<body>
    <form id="formReportingGraph" runat="server" class="report_form1">
        <table id="no_css" style="width: 100%; table-layout: fixed;">
            <tr>
                <td style="width: 25%">
                    <a href="http://www.jnjcolombia.com.co" target="_blank">
                        <img src="../../Content/Images/jnj-blanco.png" alt="J&J" title="Johnson & Johnson de Colombia S.A"
                            width="99%" height="60px" border="0" />
                    </a>
                </td>
                <td style="width: 65%">
                    <h2 style="color: #FFF; font-size: x-large; padding-left: 3%; text-align: center;">
                        <%= System.Configuration.ConfigurationManager.AppSettings["HeaderMessage"].ToString() %>
                    </h2>
                </td>
                <td style="width: 10%">
                    <img src="../../Content/Images/qa_jyj.png" alt="QA" title="Quality Assurance Latin America"
                        width="90%" style="padding-top: 5%;" />
                </td>
            </tr>
        </table>
        <div class="report_outer_div">
            <asp:ScriptManager ID="ScriptManager2" runat="server">
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
                        <td style="color: #FF0000; font-weight: bold">
                            <label>
                                Tipo Alarma (*):
                            </label>
                        </td>
                        <td style="color: #FF0000; font-weight: bold">
                            <label>
                                Señal (*):
                            </label>
                        </td>
                    </tr>
                    <tr>
                        <td>
                            <asp:DropDownList ID="ddlAreasG" runat="server" AppendDataBoundItems="True" 
                                DataTextField="ARE_NAME" DataValueField="ARE_ID" 
                                OnSelectedIndexChanged="ddlAreasG_SelectedIndexChanged" 
                                CssClass="report_dropdownlist_filters" AutoPostBack="True" 
                                oninit="ddlAreasG_Init">
                                <asp:ListItem Value="0">-- Seleccione --</asp:ListItem>
                                <asp:ListItem Value="-1">-- Todos --</asp:ListItem>
                            </asp:DropDownList>
                        </td>
                        <td>
                            <asp:DropDownList ID="ddlAppliancesG" runat="server" 
                                DataTextField="APP_NAME" 
                                DataValueField="APP_ID" 
                                OnSelectedIndexChanged="ddlAppliancesG_SelectedIndexChanged" 
                                CssClass="report_dropdownlist_filters">
                                <asp:ListItem Value="0">-- Seleccione --</asp:ListItem>
                                <asp:ListItem Value="-1">-- Todos --</asp:ListItem>
                            </asp:DropDownList>
                        </td>
                        <td>
                            <asp:DropDownList ID="ddlAlarmsG" runat="server" AppendDataBoundItems="True" DataTextField="ALA_TYP_NAME" 
                                DataValueField="ALA_TYP_ID" 
                                onselectedindexchanged="ddlAlarmsG_SelectedIndexChanged" 
                                CssClass="report_dropdownlist_filters" 
                                oninit="ddlAlarmsG_Init">
                                <asp:ListItem Value="0">-- Seleccione --</asp:ListItem>
                                <asp:ListItem Value="-1">-- Todos --</asp:ListItem>
                            </asp:DropDownList>
                        </td>
                        <td>
                            <asp:DropDownList ID="ddlSignalsG" runat="server" AppendDataBoundItems="True" DataTextField="SIG_NAME" 
                                DataValueField="SIG_ID" 
                                onselectedindexchanged="ddlSignalsG_SelectedIndexChanged" 
                                CssClass="report_dropdownlist_filters" 
                                oninit="ddlSignalsG_Init">
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
                            <div id="btnGetData" class="orange" onclick="loadDataGraph()">
                                Generar Gráfica
                            </div>
                        </td>
                        <td>
                            <img src="../../Content/Images/png2.png" alt="pdf" title="Guardar gráfica como imágen.." onclick="chartToImage()" />
                        </td>
                    </tr>
                </table>
                <br />
                <div id="container-chart1" style="border-top: solid 1px #CCC;" />
                <br />
                
                <%--Gráfica de tendencias--%>
                <div id="loadingChart1" style="text-align: center;"></div>
                <div id="chart1" style="width: 80%; margin: auto;">
                </div>
                <style type="text/css">
                    .button-reset
                    {
                        margin-right: 10%;
                    }
                </style>
                <div style="text-align: right;">
                    <button id="reset-chart1" class="button-reset">Restaurar</button>
                </div>
                <br />

                <rsweb:ReportViewer ID="ReportViewer2" runat="server" Font-Names="Verdana" 
                Font-Size="8pt" InteractiveDeviceInfos="(Collection)" 
                WaitMessageFont-Names="Verdana" WaitMessageFont-Size="14pt" Width="100%" 
                    BorderWidth="1px" BorderColor="LightGray" ShowFindControls="False">
                <LocalReport ReportPath="Content\Reports\Report_Graphic_Signals.rdlc">
                    <DataSources>
                        <rsweb:ReportDataSource DataSourceId="odsReportDataSignalsG" 
                            Name="Report_Data_Signals_DS" />
                    </DataSources>
                </LocalReport>
                </rsweb:ReportViewer>

                <asp:ObjectDataSource ID="odsReportDataSignalsG" runat="server" 
                    OldValuesParameterFormatString="original_{0}" SelectMethod="GetData" 
                    
                        
                        TypeName="SMCL.Models.DataSetMonitoringTableAdapters.SMCL_GET_SIGNALS_DATATableAdapter">
                    <SelectParameters>
                        <asp:ControlParameter ControlID="ddlAlarmsG" DefaultValue="0" Name="ALARM_PARAM" 
                            PropertyName="SelectedValue" Type="Decimal" />
                        <asp:ControlParameter ControlID="ddlSignalsG" DefaultValue="0" 
                            Name="SIGNAL_PARAM" PropertyName="SelectedValue" Type="Decimal" />
                        <asp:ControlParameter ControlID="ddlAppliancesG" DefaultValue="0" 
                            Name="APPLIANCE_PARAM" PropertyName="SelectedValue" Type="Decimal" />
                        <asp:ControlParameter ControlID="ddlAreasG" DefaultValue="0" Name="AREA_PARAM" 
                            PropertyName="SelectedValue" Type="Decimal" />
                        <asp:ControlParameter ControlID="startDate" DefaultValue="0" 
                            Name="INIT_DATE_PARAM" PropertyName="Value" Type="String" />
                        <asp:ControlParameter ControlID="finalDate" DefaultValue="0" 
                            Name="END_DATE_PARAM" PropertyName="Value" Type="String" />
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
    <table align="right">
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
                    Copyright 2011 &#169; All rights reserved. Powered by
                    <a href="http://www.premize.com" target="_blank" style="color: #00ffff; text-decoration: none; font-weight: bold; font-family: verdana; font-size: x-small;">
                        Premize S.A.S
					</a>
                    <br />
                    Sitio publicado por <b>Johnson & Johnson de Colombia S.A.</b>
                </label>
			</td>
		</tr>
	</table>
</body>
    <script type="text/javascript">
        function getDataGraph() {
            if (!this.validateFilterOnBefore()) {
                alert('Seleccione al menos una opción en cada filtro');
                document.getElementById("loadingChart1").innerHTML = "";
                return;
            }

            $.ajax({
                type: "GET",
                url: '../../Home/GraphDataList?Callback=paintGraph' + '&iArea=' + document.getElementById("ddlAreasG").value
                                                                    + '&iAppliance=' + document.getElementById("ddlAppliancesG").value
                                                                    + '&iAlarm=' + document.getElementById("ddlAlarmsG").value
                                                                    + '&iSignal=' + document.getElementById("ddlSignalsG").value
                                                                    + '&iStartDate=' + document.getElementById("startDate").value
                                                                    + '&iFinalDate=' + document.getElementById("finalDate").value,
                contentType: "application/json; charset=utf-8",
                dataType: "json",
                success: function (jsonData) {
                    if (!$.isEmptyObject(jsonData)) {
                        document.getElementById("chart1").innerHTML = "";
                        document.getElementById("chart1").style.height = 300 + "px";

                        for (var i = 0; i < jsonData.length; i++) {
                            jsonData[i][1] = parseFloat(jsonData[i][1].toString());
                        }

                        $(document).ready(function () {
                            document.getElementById("loadingChart1").innerHTML = "";

                            var maxValue = getAlarmValue('<%=ConfigurationManager.AppSettings["HighAlarmId"].ToString() %>');
                            var minValue = getAlarmValue('<%=ConfigurationManager.AppSettings["LowAlarmId"].ToString() %>');

                            var plot1 = $.jqplot('chart1', [jsonData], {
                                title: 'Tendencias gráficas por señal',
                                axesDefaults: {
                                    labelRenderer: $.jqplot.CanvasAxisLabelRenderer
                                },
                                series: [{
                                    label: 'Johnson & Johnson de Colombia S.A'
                                }],
                                axes: {
                                    xaxis: {
                                        label: "Escala de tiempo",
                                        renderer: $.jqplot.DateAxisRenderer,
                                        tickOptions: {
                                            formatString: '%F'
                                        }
                                    },
                                    yaxis: {
                                        label: "Valor/Monitoreo",
                                        tickOptions: {
                                            formatString: '%d'
                                        }
                                    }
                                },
                                canvasOverlay: {
                                    show: true,
                                    objects: [
                                        { dashedHorizontalLine: {
                                            y: maxValue,
                                            lineWidth: 2,
                                            xOffset: 0,
                                            color: 'rgb(133, 120, 24)',
                                            shadow: false
                                        }
                                        },
                                        { dashedHorizontalLine: {
                                            y: minValue,
                                            lineWidth: 2,
                                            xOffset: 0,
                                            color: 'rgb(133, 120, 24)',
                                            shadow: false
                                        }
                                        },
                                    ]
                                },
                                cursor: {
                                    show: true,
                                    zoom: true,
                                    showTooltip: true,
                                    tooltipLocation: 'sw'
                                }
                            });

                            $('.button-reset').click(function () { plot1.resetZoom() });
                        });
                    } else {
                        $('#chart1').hide(2000);
                        $('#reset-chart1').hide();
                        document.getElementById("loadingChart1").innerHTML = "(No se encontraron registros para los parámetros establecidos)";
                    }
                }
            });
        }

        function validateFilterOnBefore() {
            if (document.getElementById("ddlAreasG").value == 0 ||
                document.getElementById("ddlAppliancesG").value == 0 ||
                document.getElementById("ddlAlarmsG").value == 0 ||
                document.getElementById("ddlSignalsG").value == 0 ||
                this.isEmpty(document.getElementById("startDate").value) ||
                this.isEmpty(document.getElementById("finalDate").value)) {

                return false;
            }
            return true;
        }

        function isEmpty(obj) {
            if (typeof obj == 'undefined' || obj === null || obj === '') return true;
            if (typeof obj == 'number' && isNaN(obj)) return true;
            if (obj instanceof Date && isNaN(Number(obj))) return true;
            return false;
        }
        
        function chartToImage() {
            var newCanvas = document.createElement("canvas");
            newCanvas.width = $("#chart1").width();
            newCanvas.height = $("#chart1").height();
            var baseOffset = $("#chart1").offset();

            $("#chart1 canvas").each(function () {
                var offset = $(this).offset();
                newCanvas.getContext("2d").drawImage(this,
                    offset.left - baseOffset.left,
                    offset.top - baseOffset.top);
            });

            document.location.href = newCanvas.toDataURL();
        }

        function getAlarmValue(alarmId) {
            var alarmValue = 0;
            
            $.ajax({
                type: "GET",
                dataType: "json",
                contentType: "application/json; charset=utf-8",
                url: '../../Home/GetAlarmValue?Callback=alarmValues' + '&iAppliance=' + document.getElementById("ddlAppliancesG").value
                                                                     + '&iSignal=' + document.getElementById("ddlSignalsG").value
                                                                     + '&iAlarm=' + alarmId,
                async: false,
                success: function (jsonData) {
                    alarmValue = jsonData;
                }
            });

            return alarmValue;
        }

        function loadDataGraph() {
            $("#loadingChart1").html('<img src="../../Content/datatable/proccessing4.gif" alt="processing" />');
            $("#chart1").load(this.getDataGraph());
        }
    </script>
    <!-- Don't touch this! -->
    <script type="text/javascript" src="../../Scripts/jquery.jqplot.min.js"></script>
    <script type="text/javascript" src="../../Scripts/shCore.min.js"></script>
    <script type="text/javascript" src="../../Scripts/shBrushJScript.min.js"></script>
    <script type="text/javascript" src="../../Scripts/shBrushXml.min.js"></script>
    <!-- End Don't touch this! -->
    <!-- Additional plugins go here -->
    <script language="javascript" type="text/javascript" src="../../Scripts/jqplot.cursor.min.js"></script>
    <script language="javascript" type="text/javascript" src="../../Scripts/jqplot.dateAxisRenderer.min.js"></script>
    <script type="text/javascript" src="../../Scripts/jqplot.canvasOverlay.min.js"></script>
    <script type="text/javascript" src="../../Scripts/jqplot.canvasAxisLabelRenderer.min.js"></script>
    <script type="text/javascript" src="../../Scripts/jqplot.canvasTextRenderer.min.js"></script>
    <!-- End additional plugins -->
</html>
