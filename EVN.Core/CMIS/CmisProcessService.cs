using EVN.Core.Domain;
using FX.Core;
using log4net;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Web.UI.WebControls.WebParts;

namespace EVN.Core.CMIS
{
    public class CmisProcessService : ICmisProcessService
    {
        ILog log = LogManager.GetLogger(typeof(CmisProcessService));
        public bool TiepNhanYeuCau(CongVanYeuCau congvan, DvTienTrinh tienTrinh)
        {
            try
            {
                YeuCauRequest request = new YeuCauRequest();
                request.DV_YEU_CAU = new DvYeuCau(congvan);
                request.DV_TIEN_TRINH = new List<TienTrinh>() { new TienTrinh(tienTrinh) };
                request.DV_TIEN_TNHAN = new DvTienTNhan(congvan);
                request.CD_KHANG_LIENHE = new List<KHangLienHe>() { new KHangLienHe(congvan) };
                request.DV_HSO_GTO = new List<HsoGto>();

                var ddoddien = new DDoDDien(congvan);
                ddoddien.DV_TIEN_TRINH = new List<TienTrinh>() { new TienTrinh(tienTrinh) };
                request.CD_DDO_DDIEN = new List<DDoDDien>() { ddoddien };
                request.DV_TIEN_TNHAN.MA_BPHAN = tienTrinh.MA_BPHAN_NHAN;

                CMISAction action = new CMISAction();
                string data = JsonConvert.SerializeObject(request);
                ApiService service = IoC.Resolve<ApiService>();
           
                log.ErrorFormat("Tiep nhan CMIS data:{0}", data.ToString());
                var result = service.PostData(action.themMoiTiepNhanYeuCau, data);
                log.Error(result);
                if (result == null) return false;
                var response = JsonConvert.DeserializeObject<ApiResponse>(result);
                return response != null && response.MESSAGE == "OK" && response.TYPE == "OK";
            }
            catch (Exception ex)
            {
                log.Error(ex);
                return false;
            }

        }

        public bool ChuyenTiep(CongVanYeuCau congvan, string maDViTNhan)
        {
            try
            {
                TiepNhanRequest request = new TiepNhanRequest();
                request.MA_DVIQLY = congvan.MaDViQLy;
                request.MA_YCAU_KNAI = congvan.MaYeuCau;

                CMISAction action = new CMISAction();
                string data = JsonConvert.SerializeObject(request);
                ApiService service = IoC.Resolve<ApiService>();
                var result = service.PostData(action.layTiepNhanYeuCauDaTN, data);
                if (result == null) return false;
                var tnhanresp = JsonConvert.DeserializeObject<TiepNhanResponse>(result);
                if (tnhanresp == null) return false;

                ChuyenTiepRequest ctrequest = new ChuyenTiepRequest();
                ctrequest.MA_DVIQLY = congvan.MaDViQLy;
                ctrequest.CHUYEN_CAPDUOI = maDViTNhan;
                ctrequest.MA_YCAU_KNAI = congvan.MaYeuCau;
                ctrequest.DV_YEU_CAU = tnhanresp.bangDvYeuCau;
                ctrequest.DV_TIEN_TRINH = tnhanresp.bangDvTienTrinh;
                ctrequest.DV_TIEN_TNHAN = tnhanresp.bangDvTienTnhan;
                ctrequest.CD_DDO_DDIEN = tnhanresp.bangCdDoDdien;
                ctrequest.CD_BKE_CSUAT_TBI = tnhanresp.bangCdBkeCsuatTbi;
                ctrequest.CD_GTO_HDCHUNG = tnhanresp.bangCdGtoHdchung;
                ctrequest.CD_HO_DCHUNG = tnhanresp.bangCdHoDchung;
                ctrequest.CD_KHANG_LIENHE = tnhanresp.bangCdKhangLienhe;
                ctrequest.DV_HSO_GTO = new List<HSoGiayTo>();

                data = JsonConvert.SerializeObject(ctrequest);
                result = service.PostData(action.chuyenCapDuoi, data);
                if (result == null) return false;
                var response = JsonConvert.DeserializeObject<ApiResponse>(result);
                if (response == null) return false;
                return response != null && (response.MESSAGE == "OK" && response.TYPE == "OK");
            }
            catch(Exception ex)
            {
                log.Error(ex);
                return false;
            }
        }

        public bool LapBBanTrTh(BienBanTT bienban, DvTienTrinh tienTrinh)
        {
            BBanTThao bBanTThao = new BBanTThao(bienban);
            bBanTThao.Init(bienban.CongTos, bienban.MayBienDongs, bienban.MayBienDienAps);

            BBTreoThaoRequest request = new BBTreoThaoRequest();
            request.BBanTThao = bBanTThao;
            request.TTinYCau = new TTinYCau(tienTrinh);

            CMISAction action = new CMISAction();
            string data = JsonConvert.SerializeObject(request);
            ApiService service = IoC.Resolve<ApiService>();
            log.ErrorFormat("LapBBTT {0}:{1}",bienban.MA_YCAU_KNAI,data);
            var result = service.PostData(action.createBBanTrTh, data);
            log.Error(result);
            if (result == null) return false;
            var response = JsonConvert.DeserializeObject<ApiResponse>(result);
            return response != null && (response.MESSAGE == "OK" && response.TYPE == "SUCCESS");
        }

        public HSoGToResult GetlistHSoGTo(string maDViQly, string maYCau)
        {
            try
            {
                HSoGToRequest request = new HSoGToRequest();
                request.strMa_Dviqly = maDViQly;
                request.strMa_YCau = maYCau;
                request.TYPE = "-1";
                CMISAction action = new CMISAction();
                string data = JsonConvert.SerializeObject(request);
                ApiService service = IoC.Resolve<ApiService>();
                var result = service.PostData(action.getListHSGT, data);
                if (result == null) return null;
                var response = JsonConvert.DeserializeObject<HSoGToResult>(result);
                return response;
            }
            catch (Exception ex)
            {
                log.Error(ex);
                return null;
            }
        }

        public bool UploadPdf(string maDViQly, string maYCau, byte[] pdfdata, string maLoaiHSo = "53")
        {
            try
            {
                UploadHSGTRequest request = new UploadHSGTRequest();
                request.MA_DVIQLY = maDViQly;
                request.MA_LOAI_HSO = maLoaiHSo;
                request.FILE_VALUE = Convert.ToBase64String(pdfdata);
                request.MA_YCAU_KNAI = maYCau;

                CMISAction action = new CMISAction();
                string data = JsonConvert.SerializeObject(request);
                ApiService service = IoC.Resolve<ApiService>();
                var result = service.UploadFile(action.uploadHSoGTo, data);
                log.Error(result);
                if (result == null) return false;
                return result.MESSAGE == "OK" && result.TYPE == "SUCCESS";
            }
            catch (Exception ex)
            {
                log.Error(ex);
                return false;
            }
        }

        public byte[] GetData(string maDViQly, string maYCau, string maLoaiHSo)
        {
            CMISAction action = new CMISAction();
            ApiService apisrv = IoC.Resolve<ApiService>();
            string actionUrl = $"{action.getHSGT}{maDViQly}/{maYCau}/{maLoaiHSo}";
            using (HttpClient client = new HttpClient())
            {
                string url = $"{apisrv.CMISUrl}{actionUrl}";
                var msg = client.GetAsync(url).Result;

                if (msg.IsSuccessStatusCode)
                {
                    var contentStream = msg.Content;
                    byte[] pdfdata = contentStream.ReadAsByteArrayAsync().Result;
                    return pdfdata;
                }
                return null;
            }
        }

        public IList<CongToResult> GetCongTo(string maDViQLy, string soCTo)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(maDViQLy) || string.IsNullOrWhiteSpace(soCTo))
                    return new List<CongToResult>();
                ThongTinTBiRequest request = new ThongTinTBiRequest();
                request.MA_DVI_SD = maDViQLy;
                request.LOAI_TBI = "CTO";
                request.SO_TBI = soCTo;
                request.MA_BDONG = "A";

                CMISAction action = new CMISAction();
                string data = JsonConvert.SerializeObject(request);

                log.Error($"data: {data}");
                ApiService service = IoC.Resolve<ApiService>();
                var result = service.PostData(action.layThongTinTbiTreo, data);
                if (result == null) return null;
                var response = JsonConvert.DeserializeObject<ThongTinTBiResponse>(result);
                if (response.LST_TBI.Count() == 0) return new List<CongToResult>();

                log.Error(response.LST_TBI.Count());
                IList<CongToResult> listTbi = new List<CongToResult>();
                foreach (var item in response.LST_TBI)
                {
                    try
                    {
                        var tbi = new CongToResult();
                        tbi.SO_PHA = item["SO_PHA"].ToString();
                        tbi.MA_KHO = item["MA_KHO"].ToString();
                        tbi.NGAY_BDONG = item["NGAY_BDONG"].ToString();
                        tbi.TEN_NVKD = item["TEN_NVKD"].ToString();
                        tbi.MA_NVIENKD = item["MA_NVIENKD"].ToString();
                        tbi.DIEN_AP = item["DIEN_AP"].ToString();
                        tbi.MTEM_KD = item["MTEM_KD"].ToString();
                        tbi.NGAY_KDINH = item["NGAY_KDINH"].ToString();
                        tbi.MA_BDONG = item["MA_BDONG"].ToString();
                        tbi.MA_DVIKD = item["MA_DVIKD"].ToString();
                        tbi.NAM_SX = item["NAM_SX"].ToString();
                        tbi.DONG_DIEN = item["DONG_DIEN"].ToString();
                        tbi.NGAY_LTRINH = item["NGAY_LTRINH"].ToString();
                        tbi.MA_CLOAI = item["MA_CLOAI"].ToString();
                        tbi.VH_CONG = item["VH_CONG"].ToString();
                        tbi.SO_HUU = item["SO_HUU"].ToString();
                        tbi.SERY_TEMKD = item["SERY_TEMKD"].ToString();
                        tbi.KIM_CHITAI = item["KIM_CHITAI"].ToString();
                        tbi.LOAI_SOHUU = item["LOAI_SOHUU"].ToString();
                        tbi.LOAI_CTO = item["LOAI_CTO"].ToString();
                        tbi.SLAN_LT = item["SLAN_LT"].ToString();
                        tbi.SO_CHITAI = item["SO_CHITAI"].ToString();
                        tbi.TYSO_TI = item["TYSO_TI"].ToString();

                        tbi.BCS = item["BCS"].ToString();
                        tbi.MA_CTO = item["MA_CTO"].ToString();
                        tbi.NGAY_NHAP = item["NGAY_NHAP"].ToString();
                        tbi.SO_BBAN_KD = item["SO_BBAN_KD"].ToString();
                        tbi.SO_CTO = item["SO_CTO"].ToString();
                        tbi.MA_DVI_SD = item["MA_DVI_SD"].ToString();
                        tbi.SO_BBAN = item["SO_BBAN"].ToString();
                        tbi.TYSO_TU = item["TYSO_TU"].ToString();
                        listTbi.Add(tbi);
                    }
                    catch (Exception ex)
                    {
                        continue;
                    }
                }
                return listTbi;
            }
            catch (Exception ex)
            {
                log.Error(ex);
                return new List<CongToResult>();
            }
        }

        public IList<TIResult> GetTI(string maDViQLy, string soTI)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(maDViQLy) || string.IsNullOrWhiteSpace(soTI))
                    return new List<TIResult>();

                ThongTinTBiRequest request = new ThongTinTBiRequest();
                request.MA_DVI_SD = maDViQLy;
                request.LOAI_TBI = "TI";
                request.SO_TBI = soTI;
                request.MA_BDONG = "A";

                CMISAction action = new CMISAction();
                string data = JsonConvert.SerializeObject(request);
                log.Error($"data: {data}");
                ApiService service = IoC.Resolve<ApiService>();
                var result = service.PostData(action.layThongTinTbiTreo, data);
                if (result == null) return null;
                var response = JsonConvert.DeserializeObject<ThongTinTBiResponse>(result);
                if (response.LST_TBI.Count() == 0) return new List<TIResult>();

                log.Error(response.LST_TBI.Count());
                IList<TIResult> listTbi = new List<TIResult>();
                foreach (var item in response.LST_TBI)
                {
                    try
                    {
                        var tbi = new TIResult();
                        tbi.SO_PHA = item["SO_PHA"].ToString();
                        tbi.MA_KHO = item["MA_KHO"].ToString();
                        tbi.SO_HUU = item["SO_HUU"].ToString();
                        tbi.SERY_TEMKD = item["SERY_TEMKD"].ToString();
                        tbi.NGAY_BDONG = item["NGAY_BDONG"].ToString();
                        tbi.TEN_NVKD = item["TEN_NVKD"].ToString();
                        tbi.LOAI_SOHUU = item["LOAI_SOHUU"].ToString();
                        tbi.MA_NVIENKD = item["MA_NVIENKD"].ToString();
                        tbi.DIEN_AP = item["DIEN_AP"].ToString();
                        tbi.MTEM_KD = tbi.TEM_KD = item["MTEM_KD"].ToString();
                        log.Error($"{soTI}-{tbi.MTEM_KD}");
                        tbi.TYSO_DAU = item["TYSO_DAU"].ToString();
                        tbi.NGAY_KDINH = item["NGAY_KDINH"].ToString();
                        tbi.MA_BDONG = item["MA_BDONG"].ToString();
                        tbi.MA_CTO = item["MA_CTO"].ToString();
                        tbi.NGAY_NHAP = item["NGAY_NHAP"].ToString();
                        tbi.MA_DVIKD = item["MA_DVIKD"].ToString();
                        tbi.NAM_SX = item["NAM_SX"].ToString();
                        tbi.SO_BBAN_KD = item["SO_BBAN_KD"].ToString();
                        tbi.SO_CTO = item["SO_CTO"].ToString();
                        tbi.MA_CLOAI = item["MA_CLOAI"].ToString();
                        tbi.MA_DVI_SD = item["MA_DVI_SD"].ToString();
                        tbi.SO_BBAN = item["SO_BBAN"].ToString();
                        listTbi.Add(tbi);
                    }
                    catch (Exception ex)
                    {
                        continue;
                    }
                }
                return listTbi;
            }
            catch (Exception ex)
            {
                log.Error(ex);
                return new List<TIResult>();
            }
        }

        public IList<TUResult> GetTU(string maDViQLy, string soTU)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(maDViQLy) || string.IsNullOrWhiteSpace(soTU))
                    return new List<TUResult>();

                ThongTinTBiRequest request = new ThongTinTBiRequest();
                request.MA_DVI_SD = maDViQLy;
                request.LOAI_TBI = "TU";
                request.SO_TBI = soTU;
                request.MA_BDONG = "A";

                CMISAction action = new CMISAction();
                string data = JsonConvert.SerializeObject(request);
                log.Error($"data: {data}");
                ApiService service = IoC.Resolve<ApiService>();
                var result = service.PostData(action.layThongTinTbiTreo, data);
                if (result == null) return null;
                var response = JsonConvert.DeserializeObject<ThongTinTBiResponse>(result);
                if (response.LST_TBI.Count() == 0) return new List<TUResult>();

                log.Error(response.LST_TBI.Count());
                IList<TUResult> listTbi = new List<TUResult>();
                foreach (var item in response.LST_TBI)
                {
                    try
                    {
                        var tbi = new TUResult();
                        tbi.SO_PHA = item["SO_PHA"].ToString();
                        tbi.MA_KHO = item["MA_KHO"].ToString();
                        tbi.SO_HUU = item["SO_HUU"].ToString();
                        tbi.NGAY_BDONG = item["NGAY_BDONG"].ToString();
                        tbi.LOAI_SOHUU = item["LOAI_SOHUU"].ToString();
                        tbi.DIEN_AP = item["DIEN_AP"].ToString();
                        tbi.TYSO_DAU = item["TYSO_DAU"].ToString();
                        tbi.NGAY_KDINH = item["NGAY_KDINH"].ToString();
                        tbi.MA_BDONG = item["MA_BDONG"].ToString();
                        tbi.MA_CTO = item["MA_CTO"].ToString();
                        tbi.NGAY_NHAP = item["NGAY_NHAP"].ToString();
                        tbi.NAM_SX = item["NAM_SX"].ToString();
                        tbi.SO_BBAN_KD = item["SO_BBAN_KD"].ToString();
                        tbi.SO_CTO = item["SO_CTO"].ToString();
                        tbi.MA_CLOAI = item["MA_CLOAI"].ToString();
                        tbi.MA_DVI_SD = item["MA_DVI_SD"].ToString();
                        tbi.SO_BBAN = item["SO_BBAN"].ToString();
                        listTbi.Add(tbi);
                    }
                    catch (Exception ex)
                    {
                        continue;
                    }
                }
                return listTbi;
            }
            catch (Exception ex)
            {
                log.Error(ex);
                return new List<TUResult>();
            }
        }

        public IList<TTRamResult> GetTTTram(string maDViQLy, string maTram)
        {
            try
            {
                TTTramRequest request = new TTTramRequest();
                request.MA_DVIQLY = maDViQLy;
                request.MA_TRAM = maTram;

                CMISAction action = new CMISAction();
                string data = JsonConvert.SerializeObject(request);
                ApiService service = IoC.Resolve<ApiService>();
                var result = service.PostData(action.searchDmTram, data);
                if (result == null) return null;
                var response = JsonConvert.DeserializeObject<IList<TTRamResult>>(result);
                return response;
            }
            catch (Exception ex)
            {
                return new List<TTRamResult>();
            }
        }

        public IList<TienTrinh> GetTienDo(string maDViQLy, string maYCau, string maDDoDDien)
        {
            try
            {
                TienDoRequest request = new TienDoRequest();
                request.MA_DVIQLY = maDViQLy;
                request.MA_YCAU_KNAI = maYCau;
                request.MA_DDO_DDIEN = maDDoDDien;

                CMISAction action = new CMISAction();
                string data = JsonConvert.SerializeObject(request);
                ApiService service = IoC.Resolve<ApiService>();
                var result = service.PostData(action.chiTietTienDo, data);
                if (result == null) return null;
                var response = JsonConvert.DeserializeObject<IList<TienTrinh>>(result);
                return response;
            }
            catch (Exception ex)
            {
                return new List<TienTrinh>();
            }
        }

        public IList<ThongTinNhomGia> GetGiaNhomNNHieuluc(string maCapDA, string ngayHieuLuc)
        {
            try
            {
                JObject request = new JObject();
                request["MA_CAPDA"] = maCapDA;
                request["NGAY_HLUC"] = ngayHieuLuc ??  DateTime.Today.ToString("dd/MM/yyyy");

                CMISAction action = new CMISAction();
                string data = JsonConvert.SerializeObject(request);
                ApiService service = IoC.Resolve<ApiService>();
                var result = service.PostData(action.getGiaNhomNNHieuluc, data);
                if (result == null) return null;
                var response = JsonConvert.DeserializeObject<IList<ThongTinNhomGia>>(result);
                return response;
            }
            catch (Exception ex)
            {
                return new List<ThongTinNhomGia>();
            }
        }

        public IList<ThongTinUQ> GetDanhMucUQ(string maDViQLy, string tenDanhMuc = "D_PCAP_UQUYEN_DVI_MAHSO", string PARAM = "HDNSH")
        {
            try
            {
                JObject request = new JObject();
                request["MA_DVIQLY"] = maDViQLy;
                request["TEN_DANH_MUC"] = tenDanhMuc;
                request["PARAM"] = PARAM;

                CMISAction action = new CMISAction();
                string data = JsonConvert.SerializeObject(request);
                ApiService service = IoC.Resolve<ApiService>();
                var result = service.PostData(action.getDanhMucUQ, data);
                if (result == null) return null;
                var response = JsonConvert.DeserializeObject<ThongTinUQResponse>(result);
                if (response.LST_OBJ != null && response.LST_OBJ.Count() > 0)
                    return response.LST_OBJ;
                return new List<ThongTinUQ>();
            }
            catch (Exception ex)
            {
                return new List<ThongTinUQ>();
            }
        }

        public IList<BoPhan> GetBoPhans(string maDViQLy)
        {
            try
            {
                JObject request = new JObject();
                request["MA_DVIQLY"] = maDViQLy;
                request["MA_BPHAN"] = "";                

                CMISAction action = new CMISAction();
                string data = JsonConvert.SerializeObject(request);
                ApiService service = IoC.Resolve<ApiService>();
                var result = service.PostData(action.getDsBophan, data);
                if (result == null) return null;
                var response = JsonConvert.DeserializeObject<IList<BoPhan>>(result);
                if (response != null && response.Count() > 0)
                    return response;
                return new List<BoPhan>();
            }
            catch (Exception ex)
            {
                log.Error(ex);
                return new List<BoPhan>();
            }
        }

        public IList<BoPhan> GetDSTo(string maDViQLy)
        {
            var list = new List<BoPhan>();
            try
            {
                JObject request = new JObject();
                request["MA_DVIQLY"] = maDViQLy;
                request["MA_TO"] = "";

                CMISAction action = new CMISAction();
                string data = JsonConvert.SerializeObject(request);
                ApiService service = IoC.Resolve<ApiService>();
                var result = service.PostData(action.getDsTo, data);
                if (result == null) return null;
                var response = JsonConvert.DeserializeObject<JArray>(result);
                if (response != null && response.Count() > 0)
                {
                    foreach(var item in response)
                    {
                        list.Add(new BoPhan() { MA_DVIQLY = maDViQLy, MA_BPHAN = item["MA_TO"].ToString(), TEN_BPHAN = item["TEN_TO"].ToString() });
                    }
                }    
                    
                return list;
            }
            catch (Exception ex)
            {
                log.Error(ex);
                return list;
            }
        }

        public IList<NhanVien> GetNhanViens(string maDViQLy)
        {
            try
            {
                JObject request = new JObject();
                request["MA_DVIQLY"] = maDViQLy;
                request["MA_NVIEN"] = "";
                request["TRANG_THAI"] = "-1";

                CMISAction action = new CMISAction();
                string data = JsonConvert.SerializeObject(request);
                ApiService service = IoC.Resolve<ApiService>();
                var result = service.PostData(action.getDsNvien, data);
                if (result == null) return null;
                var response = JsonConvert.DeserializeObject<IList<NhanVien>>(result);
                if (response != null && response.Count() > 0)
                    return response;
                return new List<NhanVien>();
            }
            catch (Exception ex)
            {
                log.Error(ex);
                return new List<NhanVien>();
            }
        }

        public bool SignNhanVien(string maDViQLy, string maYeuCau, string maNVien, string maLHSo, string maDTuongKy, string idKey = "-1")
        {
            try
            {
                JObject request = new JObject();
                request["MA_DVIQLY"] = maDViQLy;
                request["AccessKey"] = "123";
                request["SecretKey"] = "123";

                var yeucaus = new JArray();
                var yeucau = new JObject();
                yeucau["MA_YCAU_KNAI"] = maYeuCau;
                yeucau["MA_NVIEN"] = maNVien;
                yeucau["MA_LOAI_HSO"] = maLHSo;
                yeucau["MA_DTUONG_KY"] = maDTuongKy;
                yeucau["ID_KEY"] = idKey;
                yeucau["NGAY_XNHAN"] = DateTime.Today.ToString("dd/MM/yyyy");
                yeucaus.Add(yeucau);
                request["YEU_CAU"] = yeucaus;

                CMISAction action = new CMISAction();
                string data = JsonConvert.SerializeObject(request);
                ApiService service = IoC.Resolve<ApiService>();
                var result = service.PostData(action.implKySo_NV, data);
                log.Error(result);
                if (result == null) return false;
                var response = JsonConvert.DeserializeObject<ApiResponse>(result);
                return response != null && (response.MESSAGE == "OK" && response.TYPE == "SUCCESS");
            }
            catch (Exception ex)
            {
                log.Error(ex);
                return false;
            }
        }
    }

    public class UploadHSGTRequest
    {
        public string MA_DVIQLY { get; set; }
        public string MA_LOAI_HSO { get; set; } = "53";
        public string DINH_DANG { get; set; } = "pdf";
        public string FILE_VALUE { get; set; }
        public string MA_YCAU_KNAI { get; set; }
    }
}
