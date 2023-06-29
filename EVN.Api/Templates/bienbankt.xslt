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
				<title>VAT</title>
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
					<xsl:for-each select="BienBan">
						<div class="VATTEMP" style="background-color:#fff">
							<div class="content">
								<div id="header">
									<div style="width:48%;float:left; text-align: center;">
										<p style="text-transform: uppercase;font-weight: bold;">
											<xsl:value-of select="DonVi"/>
										</p>
										<p style="font-weight: 100;">
											<i>
												Số:&#160;<xsl:value-of select="SoBienBan"/>
											</i>
										</p>
									</div>
									<div style="width:52%;float:left;text-align: center;">
										<p style="font-weight: bold;">
											CỘNG HÒA XÃ HỘI CHỦ NGHĨA VIỆT NAM
										</p>
										<p style="font-weight: bold;">
											Độc lập - Tự do - Hạnh phúc
										</p>
										<p style="font-weight: 100;">
											<i>
												Hà Nội, ngày&#160;<xsl:value-of select="substring(NgayLap,1,2)"/>&#160;
												tháng&#160;<xsl:value-of select="substring(NgayLap,4,2)"/>&#160;
												năm&#160;<xsl:value-of select="concat('',substring(NgayLap,7,4))"/>
											</i>
										</p>
									</div>
								</div>
								<div id="main-content" style="margin-top:20px">
									<div style="text-align: center;">
										<b style="font-weight: bold;">BIÊN BẢN</b>
										<br/>
										<b style="font-weight: bold;">
											Kiểm tra điều kiện đóng điện điểm đấu nối
										</b>
									</div>
									<div style="margin-top: 20px;">
										<div style="margin-top: 20px;">
											<div class="dieu">
												<div  style="margin-top: 20px;">
													<div class="dieu-item">
														<b style="line-height:28px;font-weight: bold;">1. THÀNH PHẦN:</b>
														<br/>
														<div class="dieu-title" style="line-height:28px;">
															<b style="font-weight:700">1.1. Đại diện Đơn vị phân phối điện:</b>&#160;<xsl:value-of select="DonVi"/>
														</div>
														<div style="margin-left: 20px;line-height:28px;">
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
														<div class="dieu-title">
															<b style="font-weight:700">1.2. Đại diện Chủ đầu tư công trình:</b>&#160;<xsl:value-of select="KHTen"/>
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
													</div>

													<!-- Thành phần nhóm khảo sát::-->
													<div class="dieu-item">
														<b style="line-height:28px;font-weight: bold;">
															2. NỘI DUNG:
														</b>
														<br/>
														<div style="line-height:28px;">
															<i>
																Căn cứ Hồ sơ đề nghị đấu nối của&#160;<xsl:value-of select="KHTen"/>;
															</i>
														</div>
														<div style="line-height:28px;">
															<i>
																Căn cứ Biên bản thỏa thuận đấu nối công trình:&#160;<xsl:value-of select="ThoaThuanDauNoi"/>;
															</i>
														</div>
														<div style="line-height:28px;">
															<i>
																Sau khi kiểm tra hồ sơ công trình và thực tế tại hiện trường,&#160;<xsl:value-of select="DonVi"/>&#160;và&#160;<xsl:value-of select="KHTen"/>&#160;lập biên bản với các nội dung sau:
															</i>
														</div>
														<div style="line-height:28px;">
															<b style="font-weight:700">2.1. Các nội dung kiểm tra:</b>
														</div>
														<div style="margin-left: 15px;line-height:28px;font-weight:100;">
															- Tên công trình:&#160;<xsl:value-of select="TenCongTrinh"/>
														</div>
														<div style="margin-left: 15px;line-height:28px;font-weight:100;">
															- Địa điểm xây dựng:&#160;<xsl:value-of select="DiaDiemXayDung"/>
														</div>
														<div style="margin-left: 15px;line-height:28px;font-weight:100 !important;">
															- Quy mô xây dựng và các thông số kỹ thuật chủ yếu:<xsl:value-of select="QuyMo" disable-output-escaping="yes"/>
														</div>
														<xsl:choose>
															<xsl:when test="ShowCS > 0">
																<div style="margin-left: 15px;line-height:28px;font-weight:100;">
																	- Công suất:&#160;<xsl:for-each select="DSCongSuat/CongSuat">
																			<xsl:value-of select="SoCongSuat"/>;&#160;
																	</xsl:for-each>
																</div>
															</xsl:when>
															<xsl:otherwise>

															</xsl:otherwise>
														</xsl:choose>

														<div style="margin-left: 15px;line-height:28px;font-weight:100;">
															- Kết quả kiểm tra:&#160;<xsl:value-of select="KetQuaKiemTra"/>
														</div>
														<div style="margin-left: 15px;line-height:28px;font-weight:100;">
															- Các tồn tại:&#160;<xsl:value-of select="TonTai"/>
														</div>
														<div style="margin-left: 15px;line-height:28px;font-weight:100;">
															- Kiến nghị:&#160;<xsl:value-of select="KienNghi"/>
														</div>
														<div style="margin-left: 15px;line-height:28px;font-weight:100;">
															- Các ý kiến khác:&#160;<xsl:value-of select="YKienKhac"/>
														</div>
														<div style="line-height:28px;font-weight:100 !important;">
															<b style="font-weight:700">2.2. Kết luận:</b>
															<xsl:value-of select="KetLuan" disable-output-escaping="yes"/>
														</div>
														<div style="line-height:28px;font-weight:100;">
															<b style="font-weight:700">2.3. Thời hạn dự kiến đóng điện đấu nối:</b>&#160;<xsl:value-of select="ThoiHanDongDien"/>
														</div>
													</div>
												</div>
											</div>
										</div>
									</div>
								</div>
								<div id="footer" style="clear:both;margin-top: 30px !important;height:90px;">
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
													CHỦ ĐẦU TƯ
												</div>
												<div style="float:left;width:50%;font-size:16px;font-weight: 700;text-align:center" class="label-sign">
													NGƯỜI KIỂM TRA
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