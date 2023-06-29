<?xml version="1.0" encoding="utf-8"?>
<xsl:stylesheet version="1.0" xmlns:xsl="http://www.w3.org/1999/XSL/Transform"
  xmlns:msxsl="urn:schemas-microsoft-com:xslt" exclude-result-prefixes="msxsl js" xmlns="http://www.w3.org/1999/xhtml"
  xmlns:js="urn:custom-javascript" xmlns:ds="http://www.w3.org/2000/09/xmldsig#"
  >

	<xsl:template match="/">
		<html xmlns="http://www.w3.org/1999/xhtml">
			<head>
				<meta http-equiv="Content-Type" content="text/html; charset=utf-8" />
				<meta http-equiv="X-UA-Compatible" content="IE=edge,chrome=1" />
				<!--<link href="styles.css" type="text/css" rel="stylesheet" />-->
				<title>Bảng kê thiết bị</title>
				<style type="text/css">
					@charset "utf-8";.VATTEMP
					*{box-sizing:border-box;font-weight:100}@page{size:A4;font-size:18px;
					margin-bottom:10px;font-weight:100}* html,body,bodyhtml{margin:0;padding:0;height:100%;
					font-size:18px;font-weight:100}@media print
					{BODY{font-size:18px;background:#fff;-webkit-print-color-adjust:exact;font-weight:100}}
					@media screen{BODY{font-size:18px;line-height:1.4em;background:#fff;font-weight:100}}#main{margin:0 auto;font-weight:100}
					.VATTEMP{font-family:'Time New Roman';width:810px;font-size:18px!important;font-weight:100}.VATTEMP p{margin-top:10px !important;}
					.VATTEMP #header,.VATTEMP #main-content{width:100%;clear:left;overflow:hidden;font-weight:100}
					.VATTEMP .content{padding:10px;font-weight:100}.VATTEMP .colortext{color:#000}
					.VATTEMP hr{margin:0 0 .1em!important;padding:0!important}
					.VATTEMP .dotted{border:none;background:0 0;border-bottom:2px dotted #000;height:18px}
					.VATTEMP .content{width:800px;clear:left;margin:0 auto;font-weight:100}
					.VATTEMP .header-title{float:left;width:300px;overflow:hidden;text-align:center}
					.VATTEMP .header-title p{margin:0}.VATTEMP .header-title h3{text-transform:uppercase;color:#06066f}
					.VATTEMP .header-right{float:right;overflow:hidden}
					.VATTEMP .header-right p{margin:0 10px 10px;width:100%}
					.VATTEMP #header .date{clear:left;margin:15px auto 0;color:#06066f}
					.VATTEMP.text-upper{text-transform:uppercase}.VATTEMP .text-strong{font-weight:700}
					.VATTEMP .fl-l{float:left;width:164px;text-align:center}.VATTEMP .fl-r{float:right;width:300px;text-align:center}
					.VATTEMP .bgimg{border:1px solid red;cursor:pointer}.VATTEMP .bgimg p{color:#000;padding-left:13px;text-align:left}#footer{height:90px}
				</style>
			</head>

			<body>
				<div id="printView" style="line-height: 22px;">
					<xsl:for-each select="ThietBi">
						<div class="VATTEMP" style="background-color:#fff">
							<div class="content">
								<div id="header">
									<div style="font-weight:700; text-align: center;">
										<div style="font-weight:700;width:100%;font-size:20px;">
											CỘNG HÒA XÃ HỘI CHỦ NGHĨA VIỆT NAM
										</div>
										<div style="font-weight:700;width:100%;font-size:18px;">
											Độc lập - Tự do - Hạnh phúc
										</div>
									</div>
								</div>
								<div id="main-content" style="margin-top: 20px;">
									<div style="text-align: center;">
										<div style="font-weight:700;width:100%;font-size:18px;">BẢNG KÊ THIẾT BỊ SỬ DỤNG ĐIỆN</div>

									</div>
									<div style="margin-top: 20px;">
										<div style="margin-top: 20px;">
											<div style="width:100%;clear:both;">
												<div style="width:10%;float:left;">&#160; </div>
												<div style="width:80%;float:left;">


													<div class="dieu">
														<div  style="margin-top: 20px;">

															<div class="dieu-item">
																<div  style="width:100%">
																	Tên khách hàng:&#160;<xsl:value-of select="KHTen"/>
																</div>
																<div style="width:100%">
																	Địa chỉ dùng điện:&#160;<xsl:value-of select="DiaChiDungDien"/>
																</div>
																<div style="width:100%">
																	Thiết bị sử dụng điện
																</div>
																<div style="width:100%">
																	Thống kê thiết bị sử dụng điện: tổng công suất lắp đặt &#160;<xsl:value-of select="TongCongSuat"/> kW
																</div>
																<div style="clear: both;" class="nenhd_bg">
																	<table border="1" >
																		<thead>
																			<tr>
																				<td rowspan="2" width="40px">
																					<b>STT</b>
																				</td>
																				<td width="200px" rowspan="2">
																					<b>Tên thiết bị</b>
																				</td>
																				<td rowspan="2" width="70px">
																					<b>Công suất (kW)</b>
																				</td>
																				<td rowspan="2" width="70px">
																					<b>Số lượng</b>
																				</td>
																				<td rowspan="2" width="90px">
																					<b>Hệ số đồng thời</b>
																				</td>
																				<td colspan="2" width="180px">
																					<b>Thời gian sử dụng ngày</b>
																				</td>
																				<td rowspan="2" width="90px">
																					<b>Tổng công suất (kW)</b>
																				</td>
																				<td rowspan="2" width="90px">
																					<b>Điện năng sử dụng (kW/tháng)</b>
																				</td>
																				<td rowspan="2" width="90px">
																					<b>ghi chú</b>
																				</td>
																			</tr>
																			<tr>
																				<td width="90px">
																					<b>Số giờ /ngày</b>
																				</td>
																				<td width="90px">
																					<b>Số ngày/tháng</b>
																				</td>
																			</tr>
																		</thead>
																		<tbody class="prds" id="bd">
																			<xsl:for-each select="ThietBiChiTiets/ThietBiChiTiet">
																				<tr>
																					<td  width="40px">
																						&#160;<xsl:value-of select="STT"/>
																					</td>
																					<td >
																						&#160;<xsl:value-of select="Ten"/>
																					</td>
																					<td  width="70px">
																						&#160;<xsl:value-of select="CongSuat"/>
																					</td>
																					<td  width="70px">
																						&#160;<xsl:value-of select="SoLuong"/>
																					</td>
																					<td  width="90px">
																						&#160;<xsl:value-of select="HeSoDongThoi"/>
																					</td>
																					<td  width="90px">
																						&#160;<xsl:value-of select="SoGio"/>
																					</td>
																					<td  width="90px">
																						&#160;<xsl:value-of select="SoNgay"/>
																					</td>
																					<td  width="90px">
																						&#160;<xsl:value-of select="TongCongSuat"/>
																					</td>
																					<td  width="90px">
																						&#160;<xsl:value-of select="DienNangSuDung"/>
																					</td>
																					<td  width="90px">
																						&#160;<xsl:value-of select="GhiChu"/>
																					</td>
																				</tr>
																			</xsl:for-each>
																		</tbody>
																	</table>
																</div>
															</div>
														</div>
													</div>
												</div>
												<div style="width:10%;float:left;">&#160; </div>
											</div>
										</div>
									</div>
								</div>
								<div id="footer" style="clear:both;margin-top: 50px !important;height:90px;">
									<div class="fl-l" style="width: 50%;float: left;text-align: center;">
										<b style="font-size:16px;font-weight: 700;text-align:center" class="label-sign">
											&#160;
										</b>
									</div>
									<div class="fl-r" style="width: 50%;float: left;text-align: center;">
										<p style="font-style:italic">
											Hà Nội, ngày&#160;<xsl:value-of select="substring(NgayLap,1,2)"/>&#160;
											tháng&#160;<xsl:value-of select="substring(NgayLap,4,2)"/>&#160;
											năm&#160;<xsl:value-of select="concat('',substring(NgayLap,7,4))"/>
										</p>
										<b style="font-size:16px;font-weight: 700;text-align:center;" class="label-sign">
											KHÁCH HÀNG
										</b>
									</div>
								</div>
							</div>
						</div>
					</xsl:for-each>
				</div>
			</body>
		</html>
	</xsl:template>
</xsl:stylesheet>