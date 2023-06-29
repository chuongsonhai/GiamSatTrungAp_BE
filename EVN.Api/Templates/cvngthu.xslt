<?xml version="1.0" encoding="utf-8"?>
<xsl:stylesheet version="1.0" xmlns:xsl="http://www.w3.org/1999/XSL/Transform"
  xmlns:msxsl="urn:schemas-microsoft-com:xslt" exclude-result-prefixes="msxsl js" xmlns="http://www.w3.org/1999/xhtml"
  xmlns:js="urn:custom-javascript" xmlns:ds="http://www.w3.org/2000/09/xmldsig#"
  >

	<xsl:template match="/">
		<html xmlns="http://www.w3.org/1999/xhtml">
			<head>
				<meta http-equiv="Content-Type" content="text/html; charset=utf-8" />
				<style type="text/css">
					@charset "utf-8";.VATTEMP *{box-sizing:border-box;font-weight:100}@page{size:A4;font-size:14px;margin-bottom:10px;font-weight:100}* html,body,bodyhtml{margin:0;padding:0;height:100%;font-size:14px;font-weight:100}@media print{BODY{font-size:14px;background:#fff;-webkit-print-color-adjust:exact;font-weight:100}}@media screen{BODY{font-size:14px;line-height:1.4em;background:#fff;font-weight:100}}#main{margin:0 auto;font-weight:100}.VATTEMP{font-family:'Time New Roman';width:810px;font-size:14px!important;font-weight:100}.VATTEMP p{margin-top:10px !important;} .VATTEMP #header,.VATTEMP #main-content{width:100%;clear:left;overflow:hidden;font-weight:100}.VATTEMP .content{padding:10px;font-weight:100}.VATTEMP .colortext{color:#000}.VATTEMP hr{margin:0 0 .1em!important;padding:0!important}.VATTEMP .dotted{border:none;background:0 0;border-bottom:2px dotted #000;height:14px}.VATTEMP .content{width:800px;clear:left;margin:0 auto;font-weight:100}.VATTEMP .header-title{float:left;width:300px;overflow:hidden;text-align:center}.VATTEMP .header-title p{margin:0}.VATTEMP .header-title h3{text-transform:uppercase;color:#06066f}.VATTEMP .header-right{float:right;overflow:hidden}.VATTEMP .header-right p{margin:0 10px 10px;width:100%}.VATTEMP #header .date{clear:left;margin:15px auto 0;color:#06066f}.VATTEMP.text-upper{text-transform:uppercase}.VATTEMP .text-strong{font-weight:700}.VATTEMP .fl-l{float:left;width:164px;text-align:center}.VATTEMP .fl-r{float:right;width:300px;text-align:center}.VATTEMP .bgimg{border:1px solid red;cursor:pointer}.VATTEMP .bgimg p{color:#000;padding-left:13px;text-align:left}#footer{height:90px}
				</style>
			</head>
			<body>
				<div id="printView">
					<xsl:for-each select="CongVan">
						<div class="VATTEMP" style="background-color:#fff">
							<div class="content">
								<div id="header">
									<div style="width:48%;float:left; text-align: center;">
										<p style="text-transform: uppercase;font-weight: bold;">
											<xsl:value-of select="ChuDauTu"/>
										</p>
										<p>
											Số:&#160;<xsl:value-of select="SoCongVan"/>
										</p>
										<p>
											V/v:&#160;Kiểm tra đóng điện vào lưới điện trung áp
										</p>
									</div>
									<div style="width:52%;float:left;font-weight:700; text-align: center;">
										<p>
											CỘNG HÒA XÃ HỘI CHỦ NGHĨA VIỆT NAM
										</p>
										<p>
											Độc lập - Tự do - Hạnh phúc
										</p>
										<p>
											Hà Nội, ngày&#160;<xsl:value-of select="substring(NgayLap,1,2)"/>&#160;
											tháng&#160;<xsl:value-of select="substring(NgayLap,4,2)"/>&#160;
											năm&#160;<xsl:value-of select="concat('',substring(NgayLap,7,4))"/>
										</p>
									</div>
								</div>
								<div id="main-content" style="margin-top: 20px;">
									<div style="text-align: center;">
										Kính gửi:&#160;
										<label style="text-transform: uppercase;">
											<xsl:value-of select="BenNhan"/>
										</label>
									</div>
									<div style="margin-top: 20px;">
										<div style="margin-top: 20px;">
											<div class="dieu">
												<div  style="margin-top: 20px;">
													<div style="width:96%;float:left;line-height:36px;">
														&#160;&#160;Căn cứ thỏa thuận đấu nối số&#160;<xsl:value-of select="SoThoaThuan"/>&#160;ngày&#160;<xsl:value-of select="NgayThoaThuan"/>&#160;của&#160;<xsl:value-of select="BenNhan"/>&#160;
														và&#160;<xsl:value-of select="ChuDauTu"/>&#160;về việc&#160;<xsl:value-of select="NhuCau"/>
													</div>
													<div style="width:96%;float:left;line-height:36px;">
														&#160;&#160;Căn cứ tiến độ thi công công trình.
													</div>
													<div style="width:96%;float:left;line-height:36px;">
														&#160;&#160;<xsl:value-of select="NhuCau"/>&#160;là chủ đầu tư công trình&#160;<xsl:value-of select="DuAnDien"/>&#160;- dự án:&#160;<xsl:value-of select="DiaChiDungDien"/>.
													</div>
													<!--Quy mô công trình:-->
													<div style="width:96%;float:left;line-height:36px;">
														&#160;&#160;Đến nay công trình đã được thi công hoàn thành, để đảm bảo yêu cầu kỹ thuật và an toàn đóng điện.&#160;<xsl:value-of select="ChuDauTu"/>&#160;kính đề nghị Quý cơ quan tổ chức kiểm tra điều kiện đóng điện điểm đấu nối hạng mục:
														&#160;<xsl:value-of select="DuAnDien"/>&#160;- dự án:&#160;<xsl:value-of select="DiaChiDungDien"/>&#160;để chúng tôi triển khai các công việc tiếp theo sớm đưa công trình đi vào hoạt động phục vụ cho sản xuất của Nhà máy.
													</div>
												</div>
											</div>
										</div>
									</div>
								</div>
								<div id="footer" style="clear:both;margin-top: 50px !important;height:90px;">
									<div class="fl-l" style="width: 50%;float: left;">
										<i style="font-weight:700;">Nơi nhận:</i>
										<div>
											-&#160;Như kính gửi
										</div>
										<div>
											-&#160;Lưu VT.
										</div>
									</div>
									<div class="fl-r" style="width: 50%;float: left;text-align: center;">
										<b style="font-size:16px;font-weight: 700;text-align:center;" class="label-sign">
											CHỦ ĐẦU TƯ
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
