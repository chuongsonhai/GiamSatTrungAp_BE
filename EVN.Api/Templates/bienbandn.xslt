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
				<title>BIÊN BẢN</title>
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
					<xsl:for-each select="BienBanDN">
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
									<div style="width:52%;float:left; text-align: center;">
										<p style="font-weight:700;">
											CỘNG HÒA XÃ HỘI CHỦ NGHĨA VIỆT NAM
										</p>
										<p style="font-weight:700;">
											Độc lập - Tự do - Hạnh phúc
										</p>
										<p style="font-weight:700;">
											Hà Nội, ngày&#160;<xsl:value-of select="substring(NgayLap,1,2)"/>&#160;
											tháng&#160;<xsl:value-of select="substring(NgayLap,4,2)"/>&#160;
											năm&#160;<xsl:value-of select="concat('',substring(NgayLap,7,4))"/>
										</p>
									</div>
								</div>
								<div id="main-content" style="margin:20px">
									<div style="text-align: center;">
										<b style="font-weight:700;">BIÊN BẢN</b>
										<br/>
										<b style="font-weight:700;">
											Thỏa thuận đấu nối giữa&#160;<xsl:value-of select="EVNDonVi"/>&#160;và&#160;<xsl:value-of select="KHTen"/>
										</b>
									</div>
									<div style="margin-top: 20px;font-weight:100">
										<div style="width:100%;clear:both;">
											<div style="width:7%;float:left;">&#160; </div>
											<div style="width:3%;float:left;">-</div>
											<div style="width:80%;float:left;margin-bottom: 6px;font-weight:100">
												Căn cứ Thông tư số&#160;<xsl:value-of select="SoThongTu"/>&#160;ngày&#160;<xsl:value-of select="NgayThongTu"/>&#160;của Bộ
												trưởng Bộ Công Thương ban hành Quy định hệ thống điện phân phối;
											</div>
											<div style="width:10%;float:left;">&#160; </div>
										</div>
										<div style="width:100%;clear:both;">
											<div style="width:7%;float:left;">&#160; </div>
											<div style="width:3%;float:left;">-</div>
											<div style="width:80%;float:left;margin-bottom: 6px;font-weight:100">
												Căn cứ công văn đề nghị đấu nối ngày&#160;<xsl:value-of select="substring(NgayCongVan,1,2)"/>&#160;tháng&#160;<xsl:value-of select="substring(NgayCongVan,4,2)"/>&#160;năm&#160;<xsl:value-of select="concat('',substring(NgayCongVan,7,4))"/>&#160;của
												&#160;<xsl:value-of select="KHTen"/>&#160;gửi&#160;<xsl:value-of select="EVNDonVi"/>;
											</div>
											<div style="width:10%;float:left;">&#160; </div>
										</div>
										<div style="width:100%;clear:both;">
											<div style="width:7%;float:left;">&#160; </div>
											<div style="width:3%;float:left;">-</div>
											<div style="width:80%;float:left;margin-bottom: 6px;font-weight:100">
												Căn cứ hồ sơ đề nghị đấu nối của &#160;<xsl:value-of select="KHTen"/>&#160;gửi&#160;<xsl:value-of select="EVNDonVi"/>&#160;ngày&#160;<xsl:value-of select="substring(NgayCongVan,1,2)"/>&#160;tháng&#160;<xsl:value-of select="substring(NgayCongVan,4,2)"/>&#160;năm&#160;<xsl:value-of select="concat('',substring(NgayCongVan,7,4))"/>&#160;;
											</div>
											<div style="width:10%;float:left;">&#160; </div>
										</div>
										<div style="width:100%;clear:both;">
											<div style="width:7%;float:left;">&#160; </div>
											<div style="width:3%;float:left;">-</div>
											<div style="width:80%;float:left;margin-bottom: 6px;font-weight:100">
												Căn cứ vào Biên bản khảo sát đấu nối số&#160;<xsl:value-of select="SoBienBanKS"/>&#160;ngày&#160;<xsl:value-of select="substring(NgayKhaoSat,1,2)"/>&#160;tháng&#160;<xsl:value-of select="substring(NgayKhaoSat,4,2)"/>&#160;năm&#160;<xsl:value-of select="concat('',substring(NgayKhaoSat,7,4))"/>. ;
											</div>
											<div style="width:10%;float:left;">&#160; </div>
										</div>
										<div style="width:100%;clear:both;">
											<div style="width:7%;float:left;">&#160; </div>
											<div style="width:3%;float:left;">-</div>
											<div style="width:80%;float:left;margin-bottom: 6px;font-weight:100">
												Căn cứ vào yêu cầu và khả năng cung cấp dịch vụ phân phối điện,
											</div>
											<div style="width:10%;float:left;">&#160; </div>
										</div>

									</div>
									<div style="margin-top: 20px;">
										<div style="width:100%;font-weight:100;clear:both;">
											<div style="width:10%;float:left;">&#160; </div>
											<div style="width:80%;float:left;">

												<p>
													Hôm nay, ngày&#160;<xsl:value-of select="substring(NgayLap,1,2)"/>&#160;tháng&#160;<xsl:value-of select="substring(NgayLap,4,2)"/>&#160;năm&#160;<xsl:value-of select="concat('',substring(NgayLap,7,4))"/>&#160;tại Hà Nội, chúng tôi gồm:
												</p>
												<b  style="width:100%;float:left;font-weight:700">
													Bên A:&#160;<xsl:value-of select="EVNDonVi"/>
												</b>
												<div style="width:5%;float:left;">&#160;</div>
												<div style="width:95%;float:left;font-weight:100">
													<p>
														Đại diện là:&#160;<xsl:value-of select="EVNDaiDien"/>
													</p>
													<p>
														Chức vụ:&#160;<xsl:value-of select="EVNChucVu"/>
													</p>
													<p>
														Địa chỉ:&#160;<xsl:value-of select="EVNDiaChi"/>
													</p>
													<p>
														Điện thoại:&#160;<xsl:value-of select="EVNDienThoai"/>
													</p>
													<p>
														Fax:&#160;<xsl:value-of select="EVNFax"/>
													</p>
													<p>
														Tài khoản số:&#160;<xsl:value-of select="EVNTaiKhoan"/>
													</p>
													<p>
														Mã số thuế: &#160;<xsl:value-of select="EVNMaSoThue"/>
													</p>
												</div>
												<b style="width:100%;float:left;font-weight:700">
													Bên B:&#160;<xsl:value-of select="KHTen"/>
												</b>
												<div style="width:5%;float:left;">&#160;</div>
												<div style="width:95%;float:left;font-weight:100">
													<p>
														Đại diện là:&#160;<xsl:value-of select="KHDaiDien"/>
													</p>
													<p>
														Chức vụ:&#160;<xsl:value-of select="KHChucDanh"/>
													</p>
													<p>
														Địa chỉ:&#160;<xsl:value-of select="KHDiaChi"/>
													</p>
													<p>
														Điện thoại:&#160;<xsl:value-of select="KHDienThoai"/>
													</p>
													<p>
														Tài khoản số:&#160;<xsl:value-of select="KHTaiKhoan"/>
													</p>
													<p>
														Mã số thuế:&#160;<xsl:value-of select="KHMaSoThue"/>
													</p>
												</div>
											</div>
											<div style="width:10%;float:left;">&#160; </div>


										</div>
										<div style="margin-top: 20px;font-weight:100;width:100%;clear:both;">
											<div style="width:10%;float:left;">&#160; </div>
											<div style="width:80%;float:left;">
												<p>
													Hai bên đồng ý ký kết Thỏa thuận đấu nối với các nội dung sau:
												</p>
												<div style="font-weight:100">
													<b>Điều 1.</b>&#160;<xsl:value-of select="EVNDonVi"/> thống nhất
													phương án đấu nối công trình điện:&#160;<xsl:value-of select="TenCongTrinh"/>&#160;của&#160;<xsl:value-of select="KHTen"/>&#160;tại
													&#160;<xsl:value-of select="DiaDiemXayDung"/>&#160;vào lưới điện phân phối, cụ thể như sau:
													<div  style="margin-top: 20px;">
														<b style="font-weight:700">
															1. Quy mô công trình:
														</b>
														<xsl:choose>
															<xsl:when test="QuyMo/MoTa!=''">
																<div style="clear:both;font-weight:100">
																	<xsl:value-of select="QuyMo/MoTa" disable-output-escaping="yes"/>
																</div>
															</xsl:when>
														</xsl:choose>
														<div style="margin-left: 20px;font-weight:100">
															<span>
																1.1. Điểm đầu:&#160;
															</span>
															<xsl:value-of select="QuyMo/DiemDau" disable-output-escaping="yes"/>
														</div>
														<div style="margin-left: 20px;font-weight:100">
															<p>
																1.2. Điểm cuối:&#160;<xsl:value-of select="QuyMo/DiemCuoi"/>
															</p>
														</div>
														<div style="margin-left: 20px;font-weight:100">
															<p>1.3. Đường giây:</p>
															<div style="margin-left: 20px;font-weight:100">
																<p>
																	1.3.1 Cấp điện áp dấu nối:&#160;<xsl:value-of select="QuyMo/CapDienDauNoi"/>
																</p>
																<p>
																	1.3.2 Dây dẫn:&#160;<xsl:value-of select="QuyMo/DayDan"/>
																</p>
																<p>
																	1.3.3 Số mạch:&#160;<xsl:value-of select="QuyMo/SoMach"/>
																</p>
																<p>
																	1.3.4 Chiều dài tuyến:&#160;<xsl:value-of select="QuyMo/ChieuDaiTuyen"/>
																</p>
																<p>
																	1.3.5 Kết cấu:&#160;<xsl:value-of select="QuyMo/KetCau"/>
																</p>
																<p>
																	1.3.6 Chế độ vận hành:&#160;<xsl:value-of select="QuyMo/CheDoVanHanh"/>
																</p>
															</div>
														</div>
														<div style="margin-left: 20px;font-weight:100">
															<xsl:for-each select="TramBienAps/TramBienAp">
																<p>
																	1.<xsl:value-of select="position() + 3"/>. Trạm biến áp:&#160;<xsl:value-of select="TenTram"/>
																</p>
																<div style="margin-left: 20px;">
																	<p>
																		a) Kiểu trạm:&#160;<xsl:value-of select="KieuTram"/>
																	</p>
																	<p>
																		b) Vị trí xây lắp trạm:&#160;<xsl:value-of select="ViTriXayLap"/>
																	</p>
																	<p>
																		c) Máy biến áp:&#160;<xsl:value-of select="MayBienAp"/>
																	</p>
																	<p>
																		&#160;&#160;- Công suất:&#160;<xsl:value-of select="CongSuat"/>
																	</p>
																	<p>
																		&#160;&#160;- Cấp điện áp:&#160;<xsl:value-of select="CapDienAp"/>
																	</p>
																	<p>
																		&#160;&#160;- Tổ đấu dây cấp:&#160;<xsl:value-of select="ToDauDayCap"/>
																	</p>
																	<span>
																		d) Tủ trung thế:&#160;
																	</span>
																	<xsl:value-of select="TuTrungThe" disable-output-escaping="yes"/>
																	<span>
																		e) Thiết bị hạ thế:
																	</span>
																	<xsl:value-of select="ThietBiHaThe" disable-output-escaping="yes"/>
																</div>
															</xsl:for-each>
														</div>
													</div>

													<div  style="margin-top: 20px;">
														<b style="font-weight:700">
															2. Hệ thống đo đếm điện năng:
														</b>
														<div style="margin-left: 20px;font-weight:100">
															<xsl:choose>
																<xsl:when test="HTDoDem!=''">
																	<xsl:value-of select="HTDoDem" disable-output-escaping="yes"/>
																</xsl:when>
																<xsl:otherwise>
																	&#160;
																</xsl:otherwise>
															</xsl:choose>
														</div>
													</div>
													<div  style="margin-top: 20px;">
														<b style="font-weight:700">
															3. Ranh giới đầu tư:
														</b>
														<div style="margin-left: 20px;font-weight:100">
															<xsl:choose>
																<xsl:when test="RanhGioiDauTu!=''">
																	<xsl:value-of select="RanhGioiDauTu" disable-output-escaping="yes"/>
																</xsl:when>
																<xsl:otherwise>
																	&#160;
																</xsl:otherwise>
															</xsl:choose>
														</div>
													</div>

													<div  style="margin-top: 20px;">
														<b style="font-weight:700">
															4. Yêu cầu về giải pháp kỹ thuật:
														</b>
														<div style="margin-left: 20px;font-weight:100">
															<xsl:choose>
																<xsl:when test="YeuCauKyThuat!=''">
																	<xsl:value-of select="YeuCauKyThuat" disable-output-escaping="yes"/>
																</xsl:when>
																<xsl:otherwise>
																	&#160;
																</xsl:otherwise>
															</xsl:choose>
														</div>
													</div>
												</div>

												<!--Điều 2-->
												<br/>
												<div class="dieu">
													<b style="font-weight:700">
														Điều 2. Trách nhiệm của các bên
													</b>
													<br/>
													<b style="font-weight:700;width:100%;">1. Trách nhiệm của Bên A</b>
													<p style="clear:both;margin-left:20px;font-weight:100">
														<xsl:value-of select="EVNDonVi"/>&#160;có trách nhiệm đầu
														tư xây dựng lưới điện phân phối để kết nối với lưới điện của&#160;<xsl:value-of select="KHTen"/>&#160;theo đúng ranh giới đầu tư xây dựng quy định tại khoản
														3 Điều 1 của Biên bản Thỏa thuận đấu nối này.
													</p>
													<b style="font-weight:700;width:100%;">
														2. Trách nhiệm của Bên B
													</b>
													<div style="clear:both;padding-left:20px;width:100%;font-weight:100">
														<div style="width:4%;float:left;">
															2.1.
														</div>
														<div style="width:96%;float:left;">
															<xsl:value-of select="KHTen"/>&#160;có trách nhiệm đầu tư xây dựng lưới
															điện phân phối của mình để kết nối với lưới điện của&#160;<xsl:value-of select="EVNDonVi"/>&#160;theo đúng ranh giới đầu tư
															xây dựng quy định tại khoản 3 Điều 1 của Biên bản Thỏa thuận đấu nối
															này.
														</div>
													</div>
													<div style="clear:both;padding-left:20px;width:100%;font-weight:100">
														<div style="width:4%;float:left;">
															2.2.
														</div>
														<div style="width:96%;float:left;">
															<xsl:value-of select="KHTen"/>&#160;cam kết quản lý, vận hành hệ thống
															điện/nhà máy điện của mình tuân thủ Thông tư số&#160;<xsl:value-of select="SoThongTu"/>&#160;ngày&#160;<xsl:value-of select="NgayThongTu"/>&#160;của Bộ trưởng Bộ Công Thương ban hành Quy
															định hệ thống điện phân phối và các quy định khác có liên quan.
														</div>
													</div>
												</div>

												<!--Điều 3-->
												<br/>
												<div class="dieu">
													<b style="font-weight:700;">Điều 3. Ngày đấu nối</b>
													<div style="clear:both;padding-left:20px;">
														Ngày đóng điện dự kiến là&#160;<xsl:value-of select="NgayDauNoi"/>&#160;(ngày, tháng, năm).
													</div>
												</div>

												<!--Điều 4-->
												<br/>
												<div class="dieu">
													<b style="font-weight:700;">Điều 4. Chi phí kiểm tra và thử nghiệm bổ sung</b>
													<div style="clear:both;padding-left:20px;">
														Chi phí kiểm tra và thử nghiệm bổ sung áp dụng Thông tư số&#160;<xsl:value-of select="SoThongTu"/>&#160;ngày&#160;<xsl:value-of select="NgayThongTu"/>&#160;của Bộ Công thương ban hành.
													</div>
												</div>

												<!--Điều 5-->
												<br/>
												<div class="dieu">
													<b style="font-weight:700;">Điều 5. Các thỏa thuận khác</b>
													<div style="padding-left:20px;">
														<xsl:choose>
															<xsl:when test="ThoaThuanKhac!=''">
																<xsl:value-of select="ThoaThuanKhac" disable-output-escaping="yes"/>
															</xsl:when>
															<xsl:otherwise>
																&#160;
															</xsl:otherwise>
														</xsl:choose>
													</div>
												</div>

												<!--Điều 6-->
												<br/>
												<div class="dieu">
													<b style="font-weight:700;">Điều 6. Tách đấu nối</b>
													<br/>
													<div style="width:4% !important;float:left;">
														1.
													</div>
													<div style="width:96%;float:left;">
														Bên B có quyền đề nghị tách đấu nối tự nguyện trong các trường hợp cụ thể
														quy định tại Tài liệu đính kèm số 5 và phải tuân thủ các quy định có liên
														quan tại Quy định hệ thống điện phân phối do Bộ Công Thương ban hành.
													</div>
													<div style="width:4%;float:left;">
														2.
													</div>
													<div style="width:96%;float:left;">
														Bên A có quyền tách đấu nối bắt buộc trong các trường hợp quy định tại
														Thông tư số&#160;<xsl:value-of select="SoThongTu"/>&#160;ngày&#160;<xsl:value-of select="NgayThongTu"/>&#160;của Bộ trưởng
														Bộ Công Thương.
													</div>
												</div>

												<!--Điều 7-->
												<br/>
												<div class="dieu">
													<b style="font-weight:700;">Điều 7. Hiệu lực thi hành</b>
													<br/>
													<div style="width:4%;float:left;">
														1.
													</div>
													<div style="width:96%;float:left;">
														Biên bản Thỏa thuận đấu nối này có hiệu lực kể từ ngày ký
													</div>
													<div style="width:4%;float:left;">
														2.
													</div>
													<div style="width:96%;float:left;">
														Thời hạn có hiệu lực của Biên bản Thỏa thuận đấu nối: 02 năm
													</div>
													<div style="width:4%;float:left;">
														3.
													</div>
													<div style="width:96%;float:left;">
														Biên bản Thỏa thuận đấu nối này được lập thành 05 bản có giá trị như nhau,
														mỗi bên giữ 02 bản và 01 bản gửi tới cấp điều độ có quyền điều khiển./.
													</div>
												</div>
											</div>
											<div style="width:10%;float:left;">&#160; </div>


										</div>
									</div>
								</div>
								<div id="footer" style="clear:both;margin-top: 50px !important;height:90px;">
									<table border="none" style="width:100%">
										<tr>
											<td colspan="2">
												<div style="float:left;width:50%;font-size:16px;font-weight: 700;text-align:center" class="label-sign">
													ĐẠI DIỆN BÊN B
												</div>
												<div style="float:left;width:50%;font-size:16px;font-weight: 700;text-align:center" class="label-sign">
													ĐẠI DIỆN BÊN A
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