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
				<title>Thỏa thuận đảm bảo</title>
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
					<xsl:for-each select="ThoaThuanDamBao">
						<div class="VATTEMP" style="background-color:#fff">
							<div class="content">
								<div id="header">
									<div style=" text-align: center;">
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
										<div style="width:100%;font-size:18px;font-weight:700;">THỎA THUẬN</div>
										<div style="width:100%;font-size:18px;font-weight:700;">
											BẢO ĐẢM THỰC HIỆN HỢP ĐỒNG MUA BÁN ĐIỆN
										</div>

									</div>
									<div style="margin-top: 20px;">
										<div style="margin-top: 20px;">
											<div style="width:100%;clear:both;">
												<div style="width:10%;float:left;">&#160; </div>
												<div style="width:80%;float:left;">

													<div class="dieu">
														<div  style="margin-top: 20px;">
															<div class="dieu-item">
																<div>
																	Hôm nay, vào lúc:&#160;<xsl:value-of select="Gio"/> giờ&#160;<xsl:value-of select="Phut"/> phút,ngày&#160;<xsl:value-of select="substring(NgayLap,1,2)"/>&#160;tháng&#160;<xsl:value-of select="substring(NgayLap,4,2)"/>&#160;năm&#160;<xsl:value-of select="concat('',substring(NgayLap,7,4))"/>, tại &#160;<xsl:value-of select="DiaDiem"/>
																</div>
																<div style="width:100%">
																	Chúng tôi gồm:
																</div>
																<!--Bên A-->
																<div>
																	<div class="dieu-title" style="line-height:36px;">
																		<b style="font-weight:700;">
																			BÊN BÁN ĐIỆN (BÊN A): &#160;<xsl:value-of select="DonVi"/>
																		</b>
																	</div>
																	<div style="width:100%">
																		Mã số thuế:&#160;<xsl:value-of select="MaSoThue"/>
																	</div>
																	<div style="width:100%">
																		Địa chỉ trụ sở chính:&#160;<xsl:value-of select="DiaDiem"/>
																	</div>
																	<div style="width:100%">
																		<div style="width:50%;float:left;">
																			Tài khoản số:&#160;<xsl:value-of select="SoTaiKhoan"/>
																		</div>
																		<div style="width:50%;float:left;">
																			Tại ngân hàng:&#160;<xsl:value-of select="NganHang"/>
																		</div>
																	</div>
																	<div style="width:100%">
																		<div style="width:50%;float:left;">
																			Email:&#160;<xsl:value-of select="Email"/>
																		</div>
																		<div style="width:50%;float:left;">
																			Số điện thoại TTCSKH:&#160;<xsl:value-of select="DienThoaiCSKH"/>
																		</div>
																	</div>
																	<div style="width:100%">
																		Website:&#160;
																	</div>
																	<div style="width:100%">
																		<div style="width:50%;float:left;">
																			Đại diện là ông (bà):&#160;<xsl:value-of select="DaiDien"/>
																		</div>
																		<div style="width:50%;float:left;">
																			Chức vụ:&#160;<xsl:value-of select="ChucVu"/>
																		</div>
																	</div>
																	<div style="width:100%">
																		Theo văn bản ủy quyền số:&#160;<xsl:value-of select="VanBanUQ"/>&#160;ngày&#160;<xsl:value-of select="substring(NgayKyUQ,1,2)"/>/&#160;<xsl:value-of select="substring(NgayKyUQ,4,2)"/>/&#160;<xsl:value-of select="concat('',substring(NgayKyUQ,7,4))"/>&#160;của ông(bà)&#160;<xsl:value-of select="NguoiKyUQ"/>
																	</div>
																</div>

																<!--Bên B-->

																<div>
																	<div class="dieu-title" style="line-height:36px;">
																		<b style="font-weight:700;">
																			BÊN MUA ĐIỆN (BÊN B):&#160;<xsl:value-of select="KHTen"/>
																		</b>
																	</div>
																	<div style="width:100%">
																		Mã số thuế:&#160;<xsl:value-of select="KHMaSoThue"/>
																	</div>
																	<div style="width:100%">
																		Đăng ký kinh doanh/doanh nghiệp:&#160;<xsl:value-of select="KHDangKyKD"/>
																	</div>
																	<div style="width:100%">
																		Địa chỉ trụ sở chính:&#160;<xsl:value-of select="KHDiaChi"/>
																	</div>
																	<div style="width:50%;float:left;">
																		Tài khoản số:&#160;<xsl:value-of select="KHSoTK"/>
																	</div>
																	<div style="width:50%;float:left;">
																		Tại ngân hàng:&#160;<xsl:value-of select="KHNganHang"/>
																	</div>

																	<div style="width:100%">
																		<div style="width:50%;float:left;">
																			Email:&#160;<xsl:value-of select="KHEmail"/>
																		</div>
																		<div style="width:50%;float:left;">
																			Số điện thoại:&#160;<xsl:value-of select="KHDienThoai"/>
																		</div>
																	</div>
																	<div style="width:100%">
																		<div style="width:50%;float:left;">
																			Đại diện là ông (bà):&#160;<xsl:value-of select="KHDaiDien"/>
																		</div>
																		<div style="width:50%;float:left;">
																			Chức vụ:&#160;<xsl:value-of select="KHChucVu"/>
																		</div>
																	</div>
																	<div style="width:100%">
																		Số chứng thực cá nhân (CMND/CCCD/HC):&#160;<xsl:value-of select="KHSoGiayTo"/>&#160;ngày cấp <xsl:value-of select="substring(NgayCap,1,2)"/>/&#160;<xsl:value-of select="substring(NgayCap,4,2)"/>/&#160;<xsl:value-of select="concat('',substring(NgayCap,7,4))"/>&#160;nơi cấp &#160;<xsl:value-of select="NoiCap"/>
																	</div>
																	<div style="width:100%">
																		Theo văn bản ủy quyền số:&#160;<xsl:value-of select="KHVanBanUQ"/>&#160;ngày&#160;<xsl:value-of select="substring(KHNgayUQ,1,2)"/>/&#160;<xsl:value-of select="substring(KHNgayUQ,4,2)"/>/&#160;<xsl:value-of select="concat('',substring(KHNgayUQ,7,4))"/>&#160;của ông(bà)&#160;<xsl:value-of select="KHNguoiUQ"/>
																	</div>
																</div>

																<div style="width:100%">Hai bên thống nhất xác định việc bảo đảm thực hiện hợp đồng mua bán điện như sau</div>

																<div class="item_dieu">
																	<div style="width:100%;">1. Giá trị bảo đảm thực hiện HĐMBĐ </div>
																	<div style="clear: both;" class="nenhd_bg">
																		<table  border="1">
																			<thead>
																				<tr class="data">
																					<td style="width:120px;text-align:center;">
																						<b>Mục đích sử dụng điện</b>
																					</td>
																					<td  style="width:120px;text-align:center;">
																						<b>Sản lượng điện trung bình ngày</b>
																					</td>
																					<td  style="width:120px;text-align:center;">
																						<b>Số ngày bảo đảm thực hiện hợp đồng</b>
																					</td>
																					<td  style="width:120px;text-align:center;">
																						<b>Giá bán điện giờ bình thường</b>
																					</td>
																					<td style="text-align:center;">
																						<b>Thành tiền </b>
																					</td>
																				</tr>
																				<tr class="data">
																					<td  style="width:120px;text-align:center;">(1)</td>
																					<td  style="width:120px;text-align:center;">(2)</td>
																					<td  style="width:120px;text-align:center;">(3)</td>
																					<td  style="width:120px;text-align:center;">(4)</td>
																					<td style="text-align:center;">(5) = (2)x(3)x(4)</td>
																				</tr>
																			</thead>
																			<tbody class="prds" id="bd">
																				<xsl:for-each select="GiaTriDamBao/ChiTietDamBao">
																					<tr class="data">
																						<td>
																							&#160;<xsl:value-of select="MucDich"/>
																						</td>
																						<td>
																							&#160;<xsl:value-of select="SLTrungBinh"/>
																						</td>
																						<td>
																							&#160;<xsl:value-of select="SoNgayDamBao"/>
																						</td>
																						<td>
																							&#160;<xsl:value-of select="GiaBanDien"/>
																						</td>
																						<td>
																							&#160;<xsl:value-of select="ThanhTien"/>
																						</td>
																					</tr>
																				</xsl:for-each>
																			</tbody>
																		</table>
																	</div>
																	<div style="width:100%;">
																		Hai bên thỏa thuận giá trị bảo đảm hợp đồng mua bán điện là&#160;<xsl:value-of select="GiaTriTien"/>&#160;đồng
																	</div>
																	<i style="width:100%;">
																		(Bằng chữ:&#160;<xsl:value-of select="TienBangChu"/>)
																	</i>
																</div>

																<div class="item_dieu">
																	2.	Thời hạn bảo đảm thực hiện hợp đồng mua bán điện: từ ngày&#160;<xsl:value-of select="substring(TuNgay,1,2)"/>/&#160;<xsl:value-of select="substring(TuNgay,4,2)"/>/&#160;<xsl:value-of select="concat('',substring(TuNgay,7,4))"/>&#160;đến ngày&#160;<xsl:value-of select="substring(DenNgay,1,2)"/>/&#160;<xsl:value-of select="substring(DenNgay,4,2)"/>/&#160;<xsl:value-of select="concat('',substring(DenNgay,7,4))"/>
																</div>

																<div class="item_dieu">
																	3.	Hình thức bảo đảm (Bảo lãnh ngân hàng/Đặt cọc): &#160;<xsl:value-of select="HinhThuc"/>
																	
																</div>
																<div class="item_dieu">
																	Thỏa thuận này được các bên xác nhận và ký kết đầy đủ thông qua hình thức dưới đây:
																	<xsl:value-of select="GhiChu" disable-output-escaping="yes"/>
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