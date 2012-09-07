<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="ReportGraphicSignalPanel.aspx.cs"
	Inherits="SMCL.Extensions.ReportGraphicSignalPanel" %>

<%@ Register Assembly="Microsoft.ReportViewer.WebForms, Version=10.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a"
	Namespace="Microsoft.Reporting.WebForms" TagPrefix="rsweb" %>
<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
	<title>SMCL @ Reporte Gráficas Señales</title>
	<link rel="Shortcut Icon" href="../../Content/Images/johnson_and_johnson_no_bg.ico" />
	<link href="../../Content/Reports.css" rel="stylesheet" type="text/css" />
	
	<%--highcharts--%>
	<script type="text/javascript" src="../../Scripts/jquery.min.js"></script>
	<script type="text/javascript">
		jQuery.noConflict();
	</script>
	<script type="text/javascript" src="../../Scripts/highcharts.src.js"></script>
	<script type="text/javascript" src="../../Scripts/exporting.js"></script>
	
	<%--datepicker--%>
	<link href="../../Content/themes/ui-lightness/jquery-ui-1.8.19.custom.css" rel="stylesheet"
		type="text/css" media="all" />
	<script type="text/javascript" src="../../Scripts/jquery-ui-1.8.19.custom.min.js"></script>
	<script type="text/javascript" src="../../Scripts/jquery-ui-datepicker-es.js"></script>
	<script type="text/javascript">
		jQuery(function () {
			jQuery('#startDate').datepicker({
				dateFormat: 'yy/mm/dd',
				changeMonth: true,
				changeYear: true,
				maxDate: '+0d',
				onSelect:
						function (dateText, inst) {
							jQuery('#finalDate').datepicker('option', 'minDate', new Date(dateText));
						}
			});
			jQuery('#startDate').datepicker(jQuery.datepicker.regional['es']);
			jQuery('#startDate').datepicker('option', 'showAnim', 'slide');

			jQuery('#finalDate').datepicker({
				dateFormat: 'yy/mm/dd',
				changeMonth: true,
				changeYear: true,
				maxDate: '+0d'
			});
			jQuery('#finalDate').datepicker(jQuery.datepicker.regional['es']);
			jQuery('#finalDate').datepicker('option', 'showAnim', 'slide');
		});
	</script>
</head>
<body>
	<form id="formReportingGraph" runat="server" class="report_form1">
	<asp:HiddenField runat="server" ID="ddlAlarmsG" Value="-1" />
	<input id="alarmTypeExclusionId" name="alarmTypeExclusionId" runat="server" type="hidden" />
	<input id="alarmTypeRecoverId" name="alarmTypeRecoverId" runat="server" type="hidden" />
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
							<img src="../../Content/Images/jnj_reports_go_back.png" alt="regresar" title="Regresar"
								border="0" />
						</a><span style="color: #AAA;">&nbsp;Regresar...</span>
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
							Señal (*):
						</label>
					</td>
				</tr>
				<tr>
					<td>
						<asp:DropDownList ID="ddlAreasG" runat="server" AppendDataBoundItems="True" DataTextField="ARE_NAME"
							DataValueField="ARE_ID" OnSelectedIndexChanged="ddlAreasG_SelectedIndexChanged"
							CssClass="report_dropdownlist_filters" AutoPostBack="True" OnInit="ddlAreasG_Init">
							<asp:ListItem Value="0">-- Seleccione --</asp:ListItem>
						</asp:DropDownList>
					</td>
					<td>
						<asp:DropDownList ID="ddlAppliancesG" runat="server" DataTextField="APP_NAME" DataValueField="APP_ID"
							OnSelectedIndexChanged="ddlAppliancesG_SelectedIndexChanged" CssClass="report_dropdownlist_filters">
							<asp:ListItem Value="0">-- Seleccione --</asp:ListItem>
						</asp:DropDownList>
					</td>
					<td>
						<asp:DropDownList ID="ddlSignalsG" runat="server" AppendDataBoundItems="True" DataTextField="SIG_NAME"
							DataValueField="SIG_ID" OnSelectedIndexChanged="ddlSignalsG_SelectedIndexChanged"
							CssClass="report_dropdownlist_filters" OnInit="ddlSignalsG_Init">
							<asp:ListItem Value="0">-- Seleccione --</asp:ListItem>
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
						&nbsp;
					</td>
					<td>
						&nbsp;
					</td>
				</tr>
				<tr>
					<td>
						<input type="text" name="startDate" id="startDate" runat="server" maxlength="10"
							size="20" />
					</td>
					<td>
						<input type="text" name="finalDate" id="finalDate" runat="server" maxlength="10"
							size="20" />
					</td>
					<td>
						<div id="btnGetData" class="orange" onclick="loadDataGraph()">
							Generar Gráfica
						</div>
					</td>
				</tr>
			</table>
			<br />
			<br />
			<%--Segmento para generación de gráfica--%>
			<div id="container" style="width: 90%; margin: 0 auto"></div>
			<div id="loadingChart1" style="text-align: center;"></div>
			<%--Fin segmento para generación de gráfica--%>
			<br />
			<br />
			<rsweb:reportviewer id="ReportViewer2" runat="server" font-names="Verdana" font-size="8pt"
				interactivedeviceinfos="(Collection)" waitmessagefont-names="Verdana" waitmessagefont-size="14pt"
				width="100%" borderwidth="1px" bordercolor="LightGray" showfindcontrols="False">
				<LocalReport ReportPath="Content\Reports\Report_Graphic_Signals.rdlc">
					<DataSources>
						<rsweb:ReportDataSource DataSourceId="odsReportDataSignalsG" Name="Report_Data_Signals_DS" />
					</DataSources>
				</LocalReport>
			</rsweb:reportviewer>
			<asp:ObjectDataSource ID="odsReportDataSignalsG" runat="server" OldValuesParameterFormatString="original_{0}"
				SelectMethod="GetData" 
				TypeName="SMCL.DataSetReportingTableAdapters.SMCL_GET_SIGNALS_DATATableAdapter">
				<SelectParameters>
					<asp:ControlParameter ControlID="ddlAlarmsG" DefaultValue="0" Name="ALARM_PARAM"
						PropertyName="Value" Type="Decimal" />
					<asp:ControlParameter ControlID="ddlSignalsG" DefaultValue="0" Name="SIGNAL_PARAM"
						PropertyName="SelectedValue" Type="Decimal" />
					<asp:ControlParameter ControlID="ddlAppliancesG" DefaultValue="0" Name="APPLIANCE_PARAM"
						PropertyName="SelectedValue" Type="Decimal" />
					<asp:ControlParameter ControlID="ddlAreasG" DefaultValue="0" Name="AREA_PARAM" PropertyName="SelectedValue"
						Type="Decimal" />
					<asp:ControlParameter ControlID="startDate" DefaultValue="0" Name="INIT_DATE_PARAM"
						PropertyName="Value" Type="String" />
					<asp:ControlParameter ControlID="finalDate" DefaultValue="0" Name="END_DATE_PARAM"
						PropertyName="Value" Type="String" />
					<asp:ControlParameter ControlID="alarmTypeExclusionId" DefaultValue="0" Name="ALARM_TYPE_EXCLUSION"
						PropertyName="Value" Type="Decimal" />
					<asp:ControlParameter ControlID="alarmTypeRecoverId" DefaultValue="0" 
						Name="ALARM_TYPE_RECOVER" PropertyName="Value" Type="Decimal" />
					<asp:Parameter Direction="Output" Name="RESULTADO_C" Type="Object" />
				</SelectParameters>
			</asp:ObjectDataSource>
			<a href="../../Home/Reporting" style="text-decoration: none;">
				<img src="../../Content/Images/jnj_reports_go_back2.png" alt="regresar" title="Regresar"
					border="0" width="24px" height="24px" />
			</a><span style="color: #AAA;">&nbsp;Regresar...</span>
		</div>
	</div>
	</form>
	<table align="right">
		<tr>
			<td style="width: 5%;">
				<a href="http://validator.w3.org/check?uri=referer">
					<img src="../../Content/Images/jnj_valid_xhtml10.png" alt="Valid XHTML 1.0 Transitional"
						height="22" width="62" border="0" />
				</a>
			</td>
			<td style="width: 5%;">
				<a href="http://jigsaw.w3.org/css-validator/check/referer">
					<img style="border: 0; width: 62px; height: 22px" border="0" src="../../Content/Images/jnj_valid_css.gif"
						alt="Valid CSS!" />
				</a>
			</td>
			<td style="width: 5%;">
				<a href="http://validator.w3.org/" target="_blank">
					<img src="../../Content/Images/jnj_valid_html5_badge_h_css3_semantics.png" width="62"
						height="24" border="0" alt="HTML5 Powered with CSS3 / Styling, and Semantics"
						title="HTML5 Powered with CSS3 / Styling, and Semantics" />
				</a>
			</td>
			<td style="width: 85%; text-align: right;">
				<img src="../../Content/Images/1.gif" alt="bullet-1" />
				<label style="font-family: verdana; color: #00ffff; font-size: x-small;">
					Copyright 2011 &#169; All rights reserved. Powered by <a href="http://www.premize.com"
						target="_blank" style="color: #00ffff; text-decoration: none; font-weight: bold;
						font-family: verdana; font-size: x-small;">Premize S.A.S </a>
					<br />
					Sitio publicado por <b>Johnson & Johnson de Colombia S.A.</b>
				</label>
			</td>
		</tr>
	</table>
</body>
<script type="text/javascript">
	function getDataGraph() {
		var chart;

		if (!this.validateFilterOnBefore()) {
			alert('Seleccione al menos una opción en cada filtro');
			document.getElementById("loadingChart1").innerHTML = new String();
			return;
		}

		jQuery(document).ready(function () {
			jQuery.ajax({
				type: "GET",
				url: '../../Home/GraphDataList?Callback=paintGraph' + '&iArea=' + document.getElementById("ddlAreasG").value
																	+ '&iAppliance=' + document.getElementById("ddlAppliancesG").value
																	+ '&iAlarm=' + document.getElementById("ddlAlarmsG").value
																	+ '&iSignal=' + document.getElementById("ddlSignalsG").value
																	+ '&iStartDate=' + document.getElementById("startDate").value
																	+ '&iFinalDate=' + document.getElementById("finalDate").value,
				contentType: "application/json; charset=utf-8",
				dataType: "json",
				cache: false,
				success: function (jsonData) {
					if (!jQuery.isEmptyObject(jsonData)) {
						document.getElementById("loadingChart1").innerHTML = new String();
						var maxValue = getAlarmValue('<%=ConfigurationManager.AppSettings["HighAlarmId"].ToString() %>');
						var minValue = getAlarmValue('<%=ConfigurationManager.AppSettings["LowAlarmId"].ToString() %>');

						Highcharts.setOptions({
							lang: {
								loading: 'Cargando...',
								months: ['Enero', 'Febrero', 'Marzo', 'Abril', 'Mayo', 'Junio', 'Julio',
										 'Agosto', 'Septiembre', 'Octubre', 'Noviembre', 'Diciembre'],
								shortMonths: ['Ene', 'Feb', 'Mar', 'Abr', 'May', 'Jun', 'Jul', 'Ago', 'Sep', 'Oct', 'Nov', 'Dic'],
								weekdays: ['Domingo', 'Lunes', 'Martes', 'Miércoles', 'Jueves', 'Viernes', 'Sábado'],
								decimalPoint: ',',
								thousandsSep: '.',
								downloadPNG: 'Descargar imágen PNG',
								downloadJPEG: 'Descargar imágen JPEG',
								downloadPDF: 'Descargar documento PDF',
								downloadSVG: 'Descargar imágen vector SVG',
								resetZoom: '<%=ConfigurationManager.AppSettings["GraphChartZoomTitle"].ToString() %>',
								resetZoomTitle: '<%=ConfigurationManager.AppSettings["GraphChartZoomTitle"].ToString() %>' + '1:1'
							}
						});

						chart = new Highcharts.Chart({
							chart: {
								renderTo: 'container',
								type: 'line',
								zoomType: 'x',
								spacingRight: 20
							},
							title: {
								text: '<%=ConfigurationManager.AppSettings["GraphChartTitle"].ToString() %>'
							},
							subtitle: {
								text: document.ontouchstart === undefined ?
								'<%=ConfigurationManager.AppSettings["GraphChartSubTitle"].ToString() %>' :
								'Drag your finger over the plot to zoom in'
							},
							xAxis: {
								type: 'datetime',
								maxZoom: 60000,
								title: {
									text: '<%=ConfigurationManager.AppSettings["GraphChartXAxisTitle"].ToString() %>'
								}
							},
							yAxis: {
								title: {
									text: '<%=ConfigurationManager.AppSettings["GraphChartYAxisTitle"].ToString() %>'
								},
								plotLines: [{
									color: 'green',
									width: 2,
									value: minValue,
									dashStyle: 'shortdash',
									zIndex: 5,
									label: {
										text: '<%=ConfigurationManager.AppSettings["GraphChartMinLimitTitle"].ToString() %>'
									}
								},
								{
									color: 'red',
									width: 2,
									value: maxValue,
									dashStyle: 'shortdash',
									zIndex: 6,
									label: {
										text: '<%=ConfigurationManager.AppSettings["GraphChartMaxLimitTitle"].ToString() %>'
									}
								}]
							},
							tooltip: {
								crosshairs: true,
								shared: true
							},
							legend: {
								enabled: false
							},
							plotOptions: {
								area: {
									fillColor: {
										linearGradient: { x1: 0, y1: 0, x2: 0, y2: 1 },
										stops: [
											[0, Highcharts.getOptions().colors[0]],
											[1, 'rgba(2, 0, 0, 0)']
										]
									},
									lineWidth: 1,
									marker: {
										enabled: false,
										states: {
											hover: {
												enabled: true,
												radius: 5
											}
										}
									},
									shadow: false,
									states: {
										hover: {
											lineWidth: 1
										}
									}
								}
							},
							series: [{
								name: '<%=ConfigurationManager.AppSettings["GraphChartSeriesPointTitle"].ToString() %>',
								type: 'line',
								pointInterval: 24 * 3600 * 1000,
								pointStart: Date.UTC(2006, 0, 01),
								data: jsonData
							}]
						});
					} else {
						jQuery('#container').hide(2000);
						document.getElementById("loadingChart1").innerHTML = new String('<%=ConfigurationManager.AppSettings["GraphChartSeriesNoDataTitle"].ToString() %>');
					}
				}
			});
		});
	}

	function validateFilterOnBefore() {
		if (document.getElementById("ddlAreasG").value == 0 ||
			document.getElementById("ddlAppliancesG").value == 0 ||
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

	function getAlarmValue(alarmId) {
		var alarmValue = 0;

		jQuery.ajax({
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
		jQuery("#loadingChart1").html('<img src="../../Content/datatable/proccessing4.gif" alt="Procesando" />');
		jQuery("#container").load(this.getDataGraph());
	}
</script>
</html>
