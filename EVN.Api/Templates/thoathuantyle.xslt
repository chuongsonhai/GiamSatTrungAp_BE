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
				<title>Thỏa thuận tỷ lệ</title>
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
					<xsl:for-each select="ThoaThuanTyLe">
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
										<div style="width:100%;font-size:18px;font-weight:700;">BIÊN BẢN THỎA THUẬN</div>
										<div style="width:100%;font-size:18px;font-weight:700;">
											TỶ LỆ MỤC ĐÍCH SỬ DỤNG ĐIỆN
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
																	Hôm nay, ngày&#160;<xsl:value-of select="substring(NgayLap,1,2)"/>&#160;tháng&#160;<xsl:value-of select="substring(NgayLap,4,2)"/>&#160;năm&#160;<xsl:value-of select="concat('',substring(NgayLap,7,4))"/>, chúng tôi gồm:
																</div>
																
																<!--Bên A-->
																<div>
																	<div class="dieu-title" style="line-height:36px;">
																		<b style="font-weight:700;">
																			BÊN BÁN ĐIỆN (BÊN A): TỔNG CÔNG TY ĐIỆN LỰC TP HÀ NỘI
																		</b>
																	</div>
																	<div style="width:100%">
																		Địa chỉ trụ sở chính:&#160;69 Đinh Tiên Hoàng, phường Lý Thái Tổ, quận Hoàn Kiếm, TP Hà Nội, Việt Nam
																	</div>
																	<div style="width:100%">
																		Đơn vị thực hiện hợp đồng:&#160;<xsl:value-of select="DonVi"/>
																	</div>
																	<div style="width:100%">
																		Địa chỉ giao dịch:&#160;<xsl:value-of select="DiaDiem"/>
																	</div>
																	<div style="width:100%">
																		Mã số thuế:&#160;<xsl:value-of select="MaSoThue"/>
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
																		<div style="width:30%;float:left;">
																			Số điện thoại:&#160;<xsl:value-of select="DienThoai"/>
																		</div>
																		<div style="width:30%;float:left;">
																			Fax:&#160;<xsl:value-of select="Fax"/>
																		</div>
																		<div style="width:40%;float:left;">
																			Email:&#160;<xsl:value-of select="Email"/>
																		</div>
																	</div>
																	<div style="width:100%">
																		<div style="width:50%;float:left;">
																			Số điện thoại TTCSKH:&#160;<xsl:value-of select="DienThoaiCSKH"/>
																		</div>
																		<div style="width:50%;float:left;">
																			Website:&#160;<xsl:value-of select="Website"/>
																		</div>
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
																		Số CMND/CCCD/Hộ chiếu:&#160;<xsl:value-of select="SoGiayTo"/>&#160;do&#160;<xsl:value-of select="NoiCap"/>&#160;cấp ngày&#160;<xsl:value-of select="substring(NgayCap,1,2)"/>/&#160;<xsl:value-of select="substring(NgayCap,4,2)"/>/&#160;<xsl:value-of select="concat('',substring(NgayCap,7,4))"/>
																	</div>
																	<div style="width:100%">
																		Theo văn bản ủy quyền số:&#160;<xsl:value-of select="VanBanUQ"/>&#160;ngày&#160;<xsl:value-of select="substring(NgayUQ,1,2)"/>/&#160;<xsl:value-of select="substring(NgayUQ,4,2)"/>/&#160;<xsl:value-of select="concat('',substring(NgayUQ,7,4))"/>&#160;của ông(bà)&#160;<xsl:value-of select="NguoiKyUQ"/>
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
																		Địa chỉ giao dịch:&#160;<xsl:value-of select="KHDiaChiGiaoDich"/>
																	</div>
																	<div style="width:100%">
																		Địa chỉ dùng điện:&#160;<xsl:value-of select="KHDiaChiDungDien"/>
																	</div>
																	<div style="width:100%">
																		Đăng ký kinh doanh/ doanh nghiệp số :&#160;<xsl:value-of select="KHDangKyKD"/>&#160;do&#160;<xsl:value-of select="KHNoiCapDangKyKD"/>&#160;cấp ngày&#160;<xsl:value-of select="substring(KHNgayCaoDangKyKD,1,2)"/>/&#160;<xsl:value-of select="substring(KHNgayCaoDangKyKD,4,2)"/>/&#160;<xsl:value-of select="concat('',substring(KHNgayCaoDangKyKD,7,4))"/>
																	</div>
																	<div style="width:100%">
																		Mã số thuế:&#160;<xsl:value-of select="KHMaSoThue"/>
																	</div>
																	<div style="width:100%">
																		<div style="width:50%;float:left;">
																			Tài khoản số:&#160;<xsl:value-of select="KHSoTK"/>
																		</div>
																		<div style="width:50%;float:left;">
																			Tại ngân hàng:&#160;<xsl:value-of select="KHNganHang"/>
																		</div>
																	</div>
																	<div style="width:100%" >
																		<div style="width:50%;float:left;">
																			Số điện thoại:&#160;<xsl:value-of select="KHDienThoai"/>
																		</div>
																		<div style="width:20%;float:left;">
																			Fax:&#160;<xsl:value-of select="KHFax"/>
																		</div>
																		<div style="width:30%;float:left;">
																			Email:&#160;<xsl:value-of select="KHEmail"/>
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
																		Số CMND/CCCD/Hộ chiếu:&#160;<xsl:value-of select="KHSoGiayTo"/>&#160;do&#160;<xsl:value-of select="KHNoiCap"/>&#160;cấp ngày&#160;<xsl:value-of select="substring(KHNgayCap,1,2)"/>/&#160;<xsl:value-of select="substring(KHNgayCap,4,2)"/>/&#160;<xsl:value-of select="concat('',substring(KHNgayCap,7,4))"/>
																	</div>
																	<div style="width:100%">
																		Theo văn bản ủy quyền số:&#160;<xsl:value-of select="KHVanBanUQ"/>&#160;ngày&#160;<xsl:value-of select="substring(KHNgayUQ,1,2)"/>/&#160;<xsl:value-of select="substring(KHNgayUQ,4,2)"/>/&#160;<xsl:value-of select="concat('',substring(KHNgayUQ,7,4))"/>&#160;của ông(bà)&#160;<xsl:value-of select="KHNguoiKyUQ"/>
																	</div>
																	
																</div>

																<div style="width:100%">Hai bên cùng nhau xem xét mục đích sử dụng điện của Bên B để xác định tỷ lệ/sản lượng điện năng theo từng mục đích, cụ thể như sau:</div>

																<div class="item_dieu">
																	<div style="width:100%;">1.	Mục đích thực tế sử dụng điện của Bên B </div>
																	<div style="clear: both;" class="nenhd_bg">
																		<table  border="1">
																			<thead>
																				<tr class="data">
																					<td rowspan="2" style="width:120px;text-align:center;">
																						<b>TT</b>
																					</td>
																					<td rowspan="2" style="width:120px;text-align:center;">
																						<b>Tên thiết bị</b>
																					</td>
																					<td rowspan="2" style="width:120px;text-align:center;">
																						<b>
																							Công suất(kW)
																						</b>
																					</td>
																					<td rowspan="2" style="width:120px;text-align:center;">
																						<b>Số lượng</b>
																					</td>
																					<td rowspan="2" style="text-align:center;">
																						<b>
																							Hệ số đồng thời
																						</b>
																					</td>
																					<td colspan="2" style="text-align:center;">
																						<b>
																							Thời gian sử dụng
																						</b>
																					</td>
																					<td rowspan="2" style="text-align:center;">
																						<b>
																							Tổng công suất sử dụng (kW)
																						</b>
																					</td>
																					<td rowspan="2" style="text-align:center;">
																						<b>
																							Điện năng sử dụng (kWh/tháng)
																						</b>
																					</td>
																					<td rowspan="2" style="text-align:center;">
																						<b>
																							Mục
																							Đích SDĐ
																						</b>
																					</td>
																				</tr>
																				<tr class="data">
																					<td style="text-align:center;">
																						<b>
																							Số giờ/ngày
																						</b>
																					</td>
																					<td  style="text-align:center;">
																						<b>
																							Số ngày/tháng
																						</b>
																					</td>
																				</tr>
																				<tr class="data">
																					<td  style="width:120px;text-align:center;">(1)</td>
																					<td  style="width:120px;text-align:center;">(2)</td>
																					<td  style="width:120px;text-align:center;">(3)</td>
																					<td  style="width:120px;text-align:center;">(4)</td>
																					<td  style="width:120px;text-align:center;">(5)</td>
																					<td  style="width:120px;text-align:center;">(6)</td>
																					<td  style="width:120px;text-align:center;">(7)</td>
																					<td style="text-align:center;">(8) = (3)x(4)x(5)</td>
																					<td style="text-align:center;">(9) = (6)x(7)x(8)</td>
																					<td  style="width:120px;text-align:center;">(10)</td>
																				</tr>
																			</thead>
																			<tbody class="prds" id="bd">
																				<xsl:for-each select="MucDichThucTeSDD/ChiTietMucDich">
																					<tr class="data">
																						<td>
																							&#160;<xsl:value-of select="STT"/>
																						</td>
																						<td>
																							&#160;<xsl:value-of select="TenThietBi"/>
																						</td>
																						<td>
																							&#160;<xsl:value-of select="CongSuat"/>
																						</td>
																						<td>
																							&#160;<xsl:value-of select="SoLuong"/>
																						</td>
																						<td>
																							&#160;<xsl:value-of select="HeSoDongThoi"/>
																						</td>
																						<td>
																							&#160;<xsl:value-of select="SoGio"/>
																						</td>
																						<td>
																							&#160;<xsl:value-of select="SoNgay"/>
																						</td>
																						<td>
																							&#160;<xsl:value-of select="TongCongSuatSuDung"/>
																						</td>
																						<td>
																							&#160;<xsl:value-of select="DienNangSuDung"/>
																						</td>
																						<td>
																							&#160;<xsl:value-of select="MucDichSDD"/>
																						</td>
																					</tr>
																				</xsl:for-each>
																				<tr class="data">
																					
																					<td colspan="2">
																						Tổng cộng
																					</td>
																					<td>
																						
																					</td>
																					<td>
																					
																					</td>
																					<td>
																					
																					</td>
																					<td>
																					
																					</td>
																					<td>
																						
																					</td>
																					<td>
																						&#160;<xsl:value-of select="TongCS"/>
																					</td>
																					<td>
																						&#160;<xsl:value-of select="TongDN"/>
																					</td>
																					<td>
																					
																					</td>
																				</tr>
																			</tbody>
																		</table>
																	</div>

																	<div style="width:100%;">2.	Giá điện theo từng mục đích sử dụng điện </div>
																	<div style="clear: both;" class="nenhd_bg">
																		<table  border="1">
																			<thead>
																				<tr class="data">
																					<td rowspan="3" style="width:120px;text-align:center;">
																						<b>Số công tơ</b>
																					</td>
																					<td rowspan="3" style="width:120px;text-align:center;">
																						<b>Mã ghi chỉ số</b>
																					</td>
																					<td rowspan="3" style="width:120px;text-align:center;">
																						<b>
																							Áp dụng từ chỉ số
																						</b>
																					</td>
																					<td rowspan="3" style="width:120px;text-align:center;">
																						<b>Mục đích sử dụng</b>
																					</td>
																					<td rowspan="3" style="text-align:center;">
																						<b>
																							Tỷ lệ %
																							hoặc kWh

																						</b>
																					</td>
																					<td colspan="4" style="text-align:center;">
																						<b>
																							Giá bán điện chưa có thuế GTGT(đ/kWh)
																						</b>
																					</td>
																					
																				</tr>
																				<tr class="data">
																					<td rowspan="2" style="text-align:center;">
																						<b>
																							Không theo thời gian
																						</b>
																					</td>
																					<td colspan="3"  style="text-align:center;">
																						<b>
																							Theo thời gian
																						</b>
																					</td>
																				</tr>
																				<tr class="data">
																					<td style="text-align:center;">
																						<b>
																							Giờ BT
																						</b>
																					</td>
																					<td  style="text-align:center;">
																						<b>
																							Giờ CĐ
																						</b>
																					</td>
																					<td  style="text-align:center;">
																						<b>
																							Giờ TĐ
																						</b>
																					</td>
																				</tr>
																				
																			</thead>
																			<tbody class="prds" id="bd">
																				<xsl:for-each select="GiaDienTheoMucDich/ChiTietGiaDien">
																					<tr class="data">
																						<td>
																							&#160;<xsl:value-of select="SoCongTo"/>
																						</td>
																						<td>
																							&#160;<xsl:value-of select="MaGhiChiSo"/>
																						</td>
																						<td>
																							&#160;<xsl:value-of select="ApDungTuChiSo"/>
																						</td>
																						<td>
																							&#160;<xsl:value-of select="MucDichSuDung"/>
																						</td>
																						<td>
																							&#160;<xsl:value-of select="TyLe"/>
																						</td>
																						<td>
																							&#160;<xsl:value-of select="GDKhongTheoTG"/>
																						</td>
																						<td>
																							&#160;<xsl:value-of select="GDGioBT"/>
																						</td>
																						<td>
																							&#160;<xsl:value-of select="GDGioCD"/>
																						</td>
																						<td>
																							&#160;<xsl:value-of select="GDGioTD"/>
																						</td>
																				
																					</tr>
																				</xsl:for-each>
																			</tbody>
																		</table>
																	</div>
																	<div style="width:100%;">
																		Biên bản này có hiệu lực kể từ ngày ký, được các bên xác nhận và ký kết đầy đủ thông
																	</div>
																	<i style="width:100%;">
																		qua hình thức dưới đây:
																	</i>
																</div>

																<div class="item_dieu" style=" font-style: italic;">
																	Trong trường hợp ký kết biên bản bằng văn bản giấy:
																	
																</div>
																<div class="item_dieu">
																	
																	Biên bản này được lập thành 02 (hai) bản gốc với đầy đủ chữ ký của các Bên. Mỗi Bên giữ 01 (một) bản để làm căn cứ thực hiện.
																</div>

																<div class="item_dieu" style=" font-style: italic;">
																	Trong trường hợp ký kết biên bản bằng điện tử:
																</div>
																<div class="item_dieu">
																	Biên bản được lưu trữ tại hệ thống phương tiện lưu trữ điện tử của Bên A tại website [•]  và/hoặc ứng dụng [•]. Bên B có quyền truy cập để tra cứu nội dung này.
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
													BÊN MUA ĐIỆN
												</div>
												<div style="float:left;width:50%;font-size:16px;font-weight: 700;text-align:center" class="label-sign">
													BÊN BÁN ĐIỆN
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