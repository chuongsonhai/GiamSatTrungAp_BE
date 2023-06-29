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
        <title>Hợp đồng mua bán điện</title>
        <style type="text/css">

			@charset "utf-8";.VATTEMP *{box-sizing:border-box;font-weight:100}@page{size:A4;font-size:14px;margin-bottom:10px;font-weight:100}* html,body,bodyhtml{margin:0;padding:0;height:100%;font-size:14px;font-weight:100}@media print{BODY{font-size:14px;background:#fff;-webkit-print-color-adjust:exact;font-weight:100}}@media screen{BODY{font-size:14px;line-height:1.4em;background:#fff;font-weight:100}}#main{margin:0 auto;font-weight:100}.VATTEMP{font-family:'Time New Roman';width:810px;font-size:14px!important;font-weight:100}.VATTEMP p{margin-top:10px !important;} .VATTEMP #header,.VATTEMP #main-content{width:100%;clear:left;overflow:hidden;font-weight:100}.VATTEMP .content{padding:10px;font-weight:100}.VATTEMP .colortext{color:#000}.VATTEMP hr{margin:0 0 .1em!important;padding:0!important}.VATTEMP .dotted{border:none;background:0 0;border-bottom:2px dotted #000;height:14px}.VATTEMP .content{width:800px;clear:left;margin:0 auto;font-weight:100}.VATTEMP .header-title{float:left;width:300px;overflow:hidden;text-align:center}.VATTEMP .header-title p{margin:0}.VATTEMP .header-title h3{text-transform:uppercase;color:#06066f}.VATTEMP .header-right{float:right;overflow:hidden}.VATTEMP .header-right p{margin:0 10px 10px;width:100%}.VATTEMP #header .date{clear:left;margin:15px auto 0;color:#06066f}.VATTEMP.text-upper{text-transform:uppercase}.VATTEMP .text-strong{font-weight:700}.VATTEMP .fl-l{float:left;width:164px;text-align:center}.VATTEMP .fl-r{float:right;width:300px;text-align:center}.VATTEMP .bgimg{border:1px solid red;cursor:pointer}.VATTEMP .bgimg p{color:#000;padding-left:13px;text-align:left}#footer{height:90px}
		</style>
      </head>
      <body>
        <div id="printView" style="line-height: 22px;">
          <xsl:for-each select="HopDong">
            <div class="VATTEMP" style="background-color:#fff">
              <div class="content">
                <div style="font-weight:700;text-align:center">
                  <p>
                    CỘNG HÒA XÃ HỘI CHỦ NGHĨA VIỆT NAM
                  </p>
                  <p>
                    Độc lập - Tự do - Hạnh phúc
                  </p>
                  <p>
                    Hà Nội, ngày&#160;<xsl:value-of select="substring(NgayHopDong,1,2)"/>&#160;
                    tháng&#160;<xsl:value-of select="substring(NgayHopDong,4,2)"/>&#160;
                    năm&#160;<xsl:value-of select="concat('',substring(NgayHopDong,7,4))"/>
                  </p>
                </div>
                <div id="main-content" style="margin-top: 20px;">
                  <div style="font-weight:700; text-align: center;">
                    <div style="width:100%;font-size:20px;">
                      HỢP ĐỒNG MUA BÁN ĐIỆN NGOÀI MỤC ĐÍCH SINH HOẠT
                    </div>
                  </div>
                  <div style="text-align: center;">
                    <div style="width:100%;font-size:14px;">
                      Mã tỉnh (TP) &#160;<xsl:value-of select="MaTinh"/> Mã huyện (quận) &#160;<xsl:value-of select="MaQuanHuyen"/>  Mã loại HĐ &#160;<xsl:value-of select="MaLoaiHD"/> Số Hợp Đồng &#160;<xsl:value-of select="SoHopDong"/>
                    </div>
                  </div>
                  <div style="margin-top: 20px;">
                    <div style="margin-top: 20px;">
                      <div class="dieu">
                        <div  style="margin-top: 20px;">

                          <div class="dieu-item">
                            <div  style="width:100%">
                              Hôm nay, ngày&#160;<xsl:value-of select="substring(NgayHopDong,1,2)"/>&#160;tháng&#160;<xsl:value-of select="substring(NgayHopDong,4,2)"/>&#160;năm&#160;<xsl:value-of select="concat('',substring(NgayHopDong,7,4))"/>, chúng tôi gồm có:
                            </div>
                            <div style="width:100%">
                              <b>
                                A. BÊN BÁN ĐIỆN: &#160;<xsl:value-of select="DonVi"/>
                              </b>
                            </div>
                            <div style="width:100%">
                              Mã số thuế:&#160;<xsl:value-of select="MaSoThue"/>
                            </div>
                            <div style="width:100%">
                              Địa chỉ trụ sở chính:&#160;<xsl:value-of select="DiaChi"/>
                            </div>
                            <div style="width:100%">
                              <div style="width:50%;float:left;">
                                Tài khoản ngân hàng số:&#160;<xsl:value-of select="SoTaiKhoan"/>
                              </div>
                              <div style="width:50%;float:left;">
                                tại ngân hàng:&#160;<xsl:value-of select="NganHang"/>
                              </div>
                            </div>
                            <div style="width:100%">
                              Email:&#160;<xsl:value-of select="Email"/>
                            </div>
                            <div style="width:100%">
                              Điện thoại CSKH:&#160;<xsl:value-of select="DienThoaiCSKH"/>
                            </div>
                            <div style="width:100%">
                              Đại diện là ông (bà):&#160;<xsl:value-of select="DaiDien"/>
                              <div style="">
                                -	Chức vụ:&#160;<xsl:value-of select="ChucVu"/>
                              </div>
                              <div style="">
                                -	Theo văn bản ủy quyền số:&#160;<xsl:value-of select="VanBanUQ"/>&#160;của Ông/Bà &#160;<xsl:value-of select="NguoiKyUQ"/>&#160;vào ngày&#160;<xsl:value-of select="substring(NgayUQ,1,2)"/>&#160;tháng&#160;<xsl:value-of select="substring(NgayUQ,4,2)"/>&#160;năm&#160;<xsl:value-of select="concat('',substring(NgayUQ,7,4))"/>
                              </div>
                            </div>
                            <div style="width:100%">
                              <i>
                                Dưới đây gọi tắt là “<b>Bên A</b>”
                              </i>
                            </div>
                            <div style="width:100%">
                              Và
                            </div>
                            <div style="width:100%">
                              <b>
                                B. BÊN MUA ĐIỆN: &#160;<xsl:value-of select="KHTen"/>
                              </b>
                            </div>
                            <div style="width:100%">
                              Mã số thuế:&#160;<xsl:value-of select="KHMaSoThue"/>
                            </div>
                            <div style="width:100%">
                              Đăng ký kinh doanh/ doanh nghiệp:&#160;<xsl:value-of select="KHDangKyKD"/>
                            </div>
                            <div style="width:100%">
                              Địa chỉ trụ sở chính/ thường trú:&#160;<xsl:value-of select="KHDiaChi"/>
                            </div>
                            <div style="width:100%">
                              <div style="width:50%;float:left;">
                                Tài khoản ngân hàng số:&#160;<xsl:value-of select="KHSoTK"/>
                              </div>
                              <div style="width:50%;float:left;">
                                tại ngân hàng:&#160;<xsl:value-of select="KHNganHang"/>
                              </div>
                            </div>
                            <div style="width:100%">
                              <div style="width:50%;float:left;">
                                Email:&#160;<xsl:value-of select="KHEmail"/>
                              </div>
                              <div style="width:50%;float:left;">
                                Điện thoại:&#160;<xsl:value-of select="KHSoTK"/>
                              </div>
                            </div>
                            <div style="width:100%">
                              Đại diện là ông (bà):
                              <div style="">
                                -	Chức vụ:&#160;<xsl:value-of select="KHDaiDien"/>
                              </div>
                              <div style="">
                                -	Số chứng thực cá nhân (CMND/CCCD/ HC): 	&#160;<xsl:value-of select="KHSoGiayTo"/>  Ngày cấp:&#160;<xsl:value-of select="substring(KHNgayUQ,1,2)"/>&#160;/&#160;<xsl:value-of select="substring(KHNgayUQ,4,2)"/>&#160;/&#160;<xsl:value-of select="concat('',substring(KHNgayUQ,7,4))"/>   Nơi cấp:	&#160;<xsl:value-of select="NoiCap"/>
                              </div>
                              <div style="">
                                -	Theo văn bản ủy quyền số:		&#160;<xsl:value-of select="KHVanBanUQ"/>  của Ông/Bà &#160;<xsl:value-of select="KHNguoiUQ"/> vào ngày&#160;<xsl:value-of select="substring(KHNgayUQ,1,2)"/>&#160;tháng&#160;<xsl:value-of select="substring(KHNgayUQ,4,2)"/>&#160;năm&#160;<xsl:value-of select="concat('',substring(KHNgayUQ,7,4))"/>
                              </div>
                            </div>
                            <div style="width:100%">
                              <i>
                                Dưới đây gọi tắt là “<b>Bên B</b>”
                              </i>
                            </div>
                            <div style="width:100%">
                              Bên A và Bên B sau đây được gọi riêng là “Bên” và gọi chung là “Các Bên”
                            </div>
                            <div style="width:100%">
                              Các Bên nhất trí ký kết Hợp Đồng với những điều khoản và điều kiện như sau:
                            </div>
                            <div  style="margin-top: 20px;">
                              <b style="line-height:36px;">ĐIỀU 1: ĐỊNH NGHĨA</b>
                              <div class="dieu-title" style="line-height:36px;">
                                Trong phạm vi Hợp Đồng này (trừ khi được Các Bên thống nhất mô tả hoặc quy định khác đi), các thuật ngữ dưới đây được hiểu như sau:
                              </div>
                              <div style="line-height:36px;">
                                <div style="float:left;width:2%">
                                  <b>1.1</b>
                                </div>
                                <div style="float:left;width:97%">
                                  <i>Biên Bản Treo, Tháo Các Thiết Bị Đo Đếm Điện</i> là Biên bản theo mẫu do Bên A quy định, có nội dung ghi nhận các thông tin về kết quả treo, tháo Thiết Bị Đo Đếm Điện, thông số kỹ thuật của Thiết Bị Đo Đếm Điện và chỉ số Công Tơ vào thời điểm treo, tháo Thiết Bị Đo Đếm Điện;
                                </div>
                              </div>
                              <div style="line-height:36px;">
                                <div style="float:left;width:2%">
                                  <b>1.2</b>
                                </div>
                                <div style="float:left;width:97%">
                                  <i>Biện Pháp Bảo Đảm</i> là biện pháp bảo đảm thực hiện Hợp Đồng nêu tại Điều 3.1;
                                </div>
                              </div>
                              <div style="line-height:36px;">
                                <div style="float:left;width:2%">
                                  <b>1.3</b>
                                </div>
                                <div style="float:left;width:97%">
                                  <i>Cơ Quan Nhà Nước</i> là các cơ quan, chính quyền các cấp của Việt Nam hay người có thẩm quyền của các cơ quan đó, có quyền, trách nhiệm cấp các văn bản hành chính liên quan đến Hợp Đồng hoặc yêu cầu Các Bên cung cấp, thực hiện các nghĩa vụ theo quy định của Pháp Luật;
                                </div>
                              </div>
                              <div style="line-height:36px;">
                                <div style="float:left;width:2%">
                                  <b>1.4</b>
                                </div>
                                <div style="float:left;width:97%">
                                  <i>Công Tơ</i> là công tơ đo đếm, là thiết bị đo đếm điện năng thực hiện tích phân công suất theo thời gian, lưu và hiển thị giá trị điện năng đo đếm được;
                                </div>
                              </div>
                              <div style="line-height:36px;">
                                <div style="float:left;width:2%">
                                  <b>1.5</b>
                                </div>
                                <div style="float:left;width:97%">
                                  <i>Địa Điểm Sử Dụng Điện</i> là địa điểm được Bên B đăng ký với Bên A để Bên A cấp điện và Bên B sử dụng điện theo quy định của Hợp Đồng;
                                </div>
                              </div>
                              <div style="line-height:36px;">
                                <div style="float:left;width:2%">
                                  <b>1.6</b>
                                </div>
                                <div style="float:left;width:97%">
                                  <i>Giá Trị Tài Sản Bảo Đảm</i> là giá trị của tài sản bảo đảm được Các Bên ghi nhận tại Điều 3.1.a;
                                </div>
                              </div>
                              <div style="line-height:36px;">
                                <div style="float:left;width:2%">
                                  <b>1.7</b>
                                </div>
                                <div style="float:left;width:97%">
                                  <i>Hợp Đồng</i> là Hợp đồng mua bán điện ngoài mục đích sinh hoạt này được ký kết giữa Các Bên ngày&#160;<xsl:value-of select="substring(NgayLap,1,2)"/>&#160;tháng&#160;<xsl:value-of select="substring(NgayLap,4,2)"/>&#160;năm&#160;<xsl:value-of select="concat('',substring(NgayLap,7,4))"/>và các bản phụ lục sửa đổi, bổ sung theo từng thời điểm;
                                </div>
                              </div>
                              <div style="line-height:36px;">
                                <div style="float:left;width:2%">
                                  <b>1.8</b>
                                </div>
                                <div style="float:left;width:97%">
                                  <i>Lãi Suất Chậm Trả</i> là mức lãi suất cho vay cao nhất của ngân hàng mà Bên A có tài khoản ghi trong Hợp Đồng tại thời điểm phát sinh Nghĩa Vụ Thanh Toán, được Bên A thông báo cho Bên B khi phát sinh việc áp dụng Lãi Suất Chậm Trả theo quy định của Hợp Đồng;
                                </div>
                              </div>
                              <div style="line-height:36px;">
                                <div style="float:left;width:2%">
                                  <b>1.9</b>
                                </div>
                                <div style="float:left;width:97%">
                                  <i>Nghĩa Vụ Thanh Toán</i> là một phần hoặc toàn bộ nghĩa vụ của Bên B đối với việc thanh toán khoản tiền phát sinh từ Hợp Đồng cho Bên A, không chỉ bao gồm tiền điện, tiền mua công suất phản kháng, tiền lãi chậm trả, tiền bồi thường thiệt hại, tiền phạt vi phạm, chi phí tạm ngừng, ngừng, cấp điện trở lại, bổ sung Khoản Khấu Trừ vào tài sản bảo đảm theo đúng quy định tại Hợp Đồng;
                                </div>
                              </div>
                              <div style="line-height:36px;">
                                <div style="float:left;width:2%">
                                  <b>1.10</b>
                                </div>
                                <div style="float:left;width:97%">
                                  <i>Pháp Luật</i> là toàn bộ các quy định pháp luật hiện hành của Việt Nam có liên quan đến hoặc điều chỉnh mối quan hệ giữa Bên A và Bên B theo Hợp Đồng, bao gồm những quy định được sửa đổi, bổ sung tại từng thời điểm;
                                </div>
                              </div>
                              <div style="line-height:36px;">
                                <div style="float:left;width:2%">
                                  <b>1.11</b>
                                </div>
                                <div style="float:left;width:97%">
                                  <i>Quy Trình Chấm Dứt Hợp Đồng</i>  là quy trình được áp dụng để xử lý việc chấm dứt Hợp Đồng, được quy định tại Điều 17.2;
                                </div>
                              </div>
                              <div style="line-height:36px;">
                                <div style="float:left;width:2%">
                                  <b>1.12</b>
                                </div>
                                <div style="float:left;width:97%">
                                  <i>Thiết Bị Đo Đếm Điện</i> là thiết bị đo công suất, điện năng, dòng điện, điện áp, tần số, hệ số công suất, bao gồm các loại công tơ, các loại đồng hồ đo điện và các thiết bị, phụ kiện kèm theo;
                                </div>
                              </div>
                              <div style="line-height:36px;">
                                <div style="float:left;width:2%">
                                  <b>1.13</b>
                                </div>
                                <div style="float:left;width:97%">
                                  <i>Thời Hạn</i> là thời hạn của Hợp Đồng được xác định theo Điều 3.4.b;
                                </div>
                              </div>
                              <div style="line-height:36px;">
                                <div style="float:left;width:2%">
                                  <b>1.14</b>
                                </div>
                                <div style="float:left;width:97%">
                                  <i>Vi Phạm Nghĩa Vụ Thanh Toán</i> là hành vi của Bên B không thực hiện đầy đủ và/ hoặc không thực hiện đúng Nghĩa Vụ Thanh Toán theo quy định tại Hợp Đồng.
                                </div>
                              </div>
                            </div>

                            <div  style="margin-top: 20px;">
                              <b style="line-height:36px;">ĐIỀU 2: MUA BÁN ĐIỆN NĂNG</b>
                              <div class="dieu-title" style="line-height:36px;">
                                Trong phạm vi Hợp Đồng này (trừ khi được Các Bên thống nhất mô tả hoặc quy định khác đi), các thuật ngữ dưới đây được hiểu như sau:
                              </div>
                              <div style="line-height:36px;">
                                <div style="float:left;width:2%">
                                  <b>2.1</b>
                                </div>
                                <div style="float:left;width:97%">
                                  Địa điểm sử dụng điện:
                                </div>
                              </div>
                              <div style="line-height:36px;">
                                <div style="float:left;width:2%">
                                  <b>2.2</b>
                                </div>
                                <div style="float:left;width:97%">
                                  Mục đích sử dụng điện:
                                </div>
                              </div>
                              <div style="line-height:36px;">
                                <div style="float:left;width:2%">
                                  <b>2.3</b>
                                </div>
                                <div style="float:left;width:97%">
                                  Cấp điện áp:
                                </div>
                              </div>
                              <div style="line-height:36px;">
                                <div style="float:left;width:2%">
                                  <b>2.4</b>
                                </div>
                                <div style="float:left;width:97%">
                                  Công suất, điện năng sử dụng:
                                </div>
                              </div>
                              <div style="line-height:36px;">
                                <div style="float:left;width:2%">
                                  <b>2.5</b>
                                </div>
                                <div style="float:left;width:97%">
                                  Điểm đấu nối cấp điện:
                                </div>
                              </div>
                            </div>
                          </div>
                        </div>
                      </div>
                    </div>
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