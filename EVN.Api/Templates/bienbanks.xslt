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
				<title>Biên bản khảo sát</title>
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
				<div id="printView">
					<xsl:for-each select="BienBanKS">
						<div class="VATTEMP" style="background-color:#fff">
							<div class="content">
								<div id="header">
									<div style="width:48%;float:left; text-align: center;">
										<p style="text-transform: uppercase;font-weight: bold;">
											<xsl:value-of select="EVNDonVi"/>
										</p>
										<p>
											Số:&#160;<xsl:value-of select="SoBienBan"/>
										</p>
									</div>
									<div style="width:52%;float:left;font-weight:700; text-align: center;">
										<p style="font-weight: bold;">
											CỘNG HÒA XÃ HỘI CHỦ NGHĨA VIỆT NAM
										</p>
										<p style="font-weight: bold;">
											Độc lập - Tự do - Hạnh phúc
										</p>
										<p style="font-weight: bold;">
											Hà Nội, ngày&#160;<xsl:value-of select="substring(NgayLap,1,2)"/>&#160;
											tháng&#160;<xsl:value-of select="substring(NgayLap,4,2)"/>&#160;
											năm&#160;<xsl:value-of select="concat('',substring(NgayLap,7,4))"/>
										</p>
									</div>
								</div>
								<div id="main-content" style="margin-top: 20px;">
									<div style="text-align: center;">
										<b style="font-weight: bold;">BIÊN BẢN</b>
										<br/>
										<b style="font-weight: bold;">
											Khảo sát lưới điện
										</b>
									</div>
									<div style="margin-top: 20px;">
										<div style="margin-top: 20px;">
											<div style="width:100%;clear:both;">
												<div style="width:10%;float:left;">&#160; </div>
												<div style="width:80%;float:left;">
													<div class="dieu">
														<div  style="margin-top: 20px;">
															<div class="dieu-item">
																<b style="line-height:36px;font-weight: bold;">1. Thông tin chung:</b>
																<br/>
																<div class="dieu-title" style="line-height:36px;">
																	1.1. Tên chủ đầu tư:&#160;<xsl:value-of select="ChuDauTu"/>
																</div>

																<div class="dieu-title">
																	1.2. Số và ngày văn bản đề nghị Thỏa thuận đấu nối:&#160;<xsl:value-of select="SoCongVan"/>&#160;ngày&#160;<xsl:value-of select="NgayCongVan"/>
																</div>

																<div class="dieu-title" style="line-height:36px;">
																	1.3. Tên công trình:&#160;<xsl:value-of select="TenCongTrinh"/>
																</div>

																<div class="dieu-title" style="line-height:36px;">
																	1.4. Địa điểm xây dựng công trình điện:&#160;<xsl:value-of select="DiaDiemXayDung"/>
																</div>
															</div>

															<div class="dieu-item">
																<b style="line-height:36px;font-weight: bold;">
																	2. Thành phần nhóm khảo sát:
																</b>
																<br/>
																<div class="dieu-title" style="line-height:36px;">
																	2.1. Tên chủ đầu tư:
																</div>

																<xsl:for-each select="ThanhPhans/ChuDauTu">
																	<div style="margin-left: 20px;">
																		<div style="width:50%;float:left;line-height:28px;">
																			Ông (bà):&#160;<xsl:value-of select="DaiDien"/><br/>
																		</div>
																		<div style="width:50%;float:left;line-height:28px;">
																			Chức danh:&#160;<xsl:value-of select="ChucVu"/><br/>
																		</div>
																	</div>
																</xsl:for-each>

																<div class="dieu-title" style="line-height:36px;">
																	2.2. Đại diện Ban Kỹ thuật/Phòng Kỹ thuật:
																</div>

																<xsl:for-each select="ThanhPhans/DonVi">
																	<div style="margin-left: 20px;">
																		<div style="width:50%;float:left;line-height:28px;">
																			Ông (bà):&#160;<xsl:value-of select="DaiDien"/><br/>
																		</div>
																		<div style="width:50%;float:left;line-height:28px;">
																			Chức danh:&#160;<xsl:value-of select="ChucVu"/><br/>
																		</div>
																	</div>
																</xsl:for-each>

															</div>

															<!-- Tiến trình thỏa thuận đấu nối-->
															<div class="dieu-item">
																<b style="line-height:36px;font-weight: bold;">
																	3. Tiến trình thỏa thuận đấu nối
																</b>
																<br/>
																<div style="width:4%;float:left;line-height:36px;">
																	3.1.
																</div>
																<div style="width:96%;float:left;line-height:36px;">
																	Ngày được giao thực hiện Thỏa thuận đấu nối:&#160;<xsl:value-of select="NgayLap"/>
																</div>
																<div style="width:4%;float:left;line-height:36px;">
																	3.2.
																</div>
																<div style="width:96%;float:left;line-height:36px;">
																	Các trở ngại trong Thỏa thuận đấu nối:&#160;<xsl:value-of select="TroNgai"/>
																</div>
																<div style="width:4%;float:left;line-height:36px;">
																	3.3.
																</div>
																<div style="width:96%;float:left;line-height:36px;">
																	Ngày khảo sát (thực tế):&#160;<xsl:value-of select="NgayKhaoSat"/>
																</div>
															</div>

															<!-- Tiến trình thỏa thuận đấu nối-->
															<div class="dieu-item">
																<b style="line-height:36px;font-weight: bold;">
																	4. Nội dung khảo sát và phương án đấu nối sơ bộ:
																</b>
																<br/>
																<div style="width:4%;float:left;line-height:36px;">
																	4.1.
																</div>
																<div style="width:96%;float:left;line-height:36px;">
																	Cấp điện áp đấu nối (kV):&#160;<xsl:value-of select="CapDienApDauNoi"/>
																</div>
																<div style="width:4%;float:left;line-height:36px;">
																	4.2.
																</div>
																<div style="width:96%;float:left;line-height:36px;">
																	Tên lộ đường dây dự kiến đấu nối:&#160;<xsl:value-of select="TenLoDuongDay"/>
																</div>
																<div style="width:4%;float:left;line-height:36px;">
																	4.3.
																</div>
																<div style="width:96%;float:left;line-height:36px;">
																	Điểm đấu dự kiến:<xsl:value-of select="DiemDauDuKien" disable-output-escaping="yes"/>
																</div>
																<div style="width:4%;float:left;line-height:36px;">
																	4.4.
																</div>
																<div style="width:96%;float:left;line-height:36px;">
																	Dây dẫn:&#160;<xsl:value-of select="DayDan"/>
																</div>
																<div style="width:4%;float:left;line-height:36px;">
																	4.5.
																</div>
																<div style="width:96%;float:left;line-height:36px;">
																	Tổng số trạm biến áp (trạm):&#160;<xsl:value-of select="SoTramBienAp"/>
																</div>
																<div style="width:4%;float:left;line-height:36px;">
																	4.6.
																</div>
																<div style="width:96%;float:left;line-height:36px;">
																	Tổng số máy biến áp (máy):&#160;<xsl:value-of select="SoMayBienAp"/>
																</div>
																<div style="width:4%;float:left;line-height:36px;">
																	4.7.
																</div>
																<div style="width:96%;float:left;line-height:36px;">
																	Tổng công suất các trạm biến áp (kVA):&#160;<xsl:value-of select="TongCongSuat"/>
																</div>
																<div style="width:4%;float:left;line-height:36px;">
																	4.8.
																</div>
																<div style="width:96%;float:left;line-height:36px;">
																	Các thỏa thuận kỹ thuật khác:
																	<xsl:value-of select="ThoaThuanKyThuat" disable-output-escaping="yes"/>
																</div>
															</div>

															<div class="dieu-item" style="line-height:36px;">
																<xsl:value-of select="ChuDauTu"/>&#160;và&#160;<xsl:value-of select="EVNDonVi"/>&#160;thống nhất lập Biên bản Thỏa
																thuận đấu nối dựa trên các nội dung chính được đề cập trong Biên bản khảo sát lưới điện này.
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
									<table border="none" style="width:100%">
										<tr>
											<td>&#160;</td>
											<td>&#160;</td>
										</tr>
										<tr>
											<td>&#160;</td>
											<td>&#160;</td>
										</tr>
										<tr>
											<td>&#160;</td>
											<td>&#160;</td>
										</tr>
										<tr>
											<td colspan="2">
												<div style="float:left;width:50%;font-size:16px;font-weight: 700;text-align:center" class="label-sign">
													ĐẠI DIỆN CHỦ ĐẦU TƯ
												</div>
												<div style="float:left;width:50%;font-size:16px;font-weight: 700;text-align:center" class="label-sign">
													ĐẠI DIỆN<br/>BAN KỸ THUẬT/ PHÒNG KỸ THUẬT
												</div>
											</td>
										</tr>
										<tr>
											<td>&#160;</td>
											<td>&#160;</td>
										</tr>
										<tr>
											<td>&#160;</td>
											<td>&#160;</td>
										</tr>
										<tr>
											<td>&#160;</td>
											<td>&#160;</td>
										</tr>
										<tr>
											<td>&#160;</td>
											<td>&#160;</td>
										</tr>
										<tr>
											<td>&#160;</td>
											<td>&#160;</td>
										</tr>
										<tr>
											<td>&#160;</td>
											<td>&#160;</td>
										</tr>
										<tr>
											<td>&#160;</td>
											<td>&#160;</td>
										</tr>
										<tr>
											<td>&#160;</td>
											<td>&#160;</td>
										</tr>
										<tr>
											<td>&#160;</td>
											<td>&#160;</td>
										</tr>
										<tr>
											<td>&#160;</td>
											<td>&#160;</td>
										</tr>
										<tr>
											<td>&#160;</td>
											<td>&#160;</td>
										</tr>
										<tr>
											<td>&#160;</td>
											<td>&#160;</td>
										</tr>
										<tr>
											<td>&#160;</td>
											<td>&#160;</td>
										</tr>
										<tr>
											<td>&#160;</td>
											<td>&#160;</td>
										</tr>
									</table>
								</div>
							</div>
						</div>
					</xsl:for-each>
				</div>
			</body>
		</html>
	</xsl:template>
</xsl:stylesheet>