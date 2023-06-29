using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.IO;

namespace EVN.Core.CMIS
{
    public class CMISAction
    {
        public CMISAction()
        {
            string path = AppDomain.CurrentDomain.BaseDirectory + "App_Data\\CMISAction.json";
            using (StreamReader file = new StreamReader(path))
            {
                string content = file.ReadToEnd();
                var data = JsonConvert.DeserializeObject<JObject>(content);
                uploadHSoGTo = data["uploadHSoGTo"].ToString();
                getListHSGT = data["getListHSGT"].ToString();
                getHSGT = data["getHSGT"].ToString();
                updateKQuaKSat = data["updateKQuaKSat"].ToString();
                themDvTienTrinh = data["themDvTienTrinh"].ToString();
                themMoiTiepNhanYeuCau = data["themMoiTiepNhanYeuCau"].ToString();
                createBBanTrTh = data["createBBanTrTh"].ToString();
                layThongTinTbiTreo = data["layThongTinTbiTreo"].ToString();
                searchDmTram = data["searchDmTram"].ToString();
                layTiepNhanYeuCauDaTN = data["layTiepNhanYeuCauDaTN"].ToString();
                chuyenCapDuoi = data["chuyenCapDuoi"].ToString();
                chiTietTienDo = data["chiTietTienDo"].ToString();

                getTTinKHangTreoThao = data["getTTinKHangTreoThao"].ToString();
                layCDDiemDoByMaDDo = data["layCDDiemDoByMaDDo"].ToString();

                getGiaNhomNNHieuluc = data["getGiaNhomNNHieuluc"].ToString();
                getDanhMucUQ = data["getDanhMucUQ"].ToString();
                getDsBophan = data["getDsBophan"].ToString();
                getDsTo = data["getDsTo"].ToString();

                getDsNvien = data["getDsNvien"].ToString();

                implKySo_NV = data["implKySo_NV"].ToString();
            }
        }
        public string uploadHSoGTo { get; set; }
        public string getListHSGT { get; set; }
        public string getHSGT { get; set; }
        public string updateKQuaKSat { get; set; }
        public string themDvTienTrinh { get; set; }
        public string themMoiTiepNhanYeuCau { get; set; }
        public string createBBanTrTh { get; set; }
        public string layThongTinTbiTreo { get; set; }
        public string searchDmTram { get; set; }
        public string layTiepNhanYeuCauDaTN { get; set; }
        public string chuyenCapDuoi { get; set; }
        public string chiTietTienDo { get; set; }

        public string getTTinKHangTreoThao { get; set; }
        public string layCDDiemDoByMaDDo { get; set; }

        public string getGiaNhomNNHieuluc { get; set; }
        public string getDanhMucUQ { get; set; }

        public string getDsBophan { get; set; }
        public string getDsTo { get; set; }
        public string getDsNvien { get; set; }

        public string implKySo_NV { get; set; }
    }
}
