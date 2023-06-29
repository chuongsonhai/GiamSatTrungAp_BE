using EVN.Core.CMIS;
using FX.Core;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EVN.Core.CMIS
{
    public class TreoThaoService
    {
        public TTKhangTreoThaoResult LayTTinKHangTreoThao(string maDViQLy, string maYCau, string loaiTKiem = "TREOMOI")
        {
            KHangTreoThaoRequest request = new KHangTreoThaoRequest();
            request.MA_DVIQLY = maDViQLy;
            request.MA_YCAU_KNAI = maYCau;
            request.LOAI_TIMKIEM = loaiTKiem;

            string data = JsonConvert.SerializeObject(request);
            CMISAction action = new CMISAction();

            ApiService service = IoC.Resolve<ApiService>();
            var result = service.PostData(action.getTTinKHangTreoThao, data);
            if (result == null) return null;
            var response = JsonConvert.DeserializeObject<IList<TTKhangTreoThaoResult>>(result);
            if (response == null || response.Count() == 0) return null;
            return response.FirstOrDefault();
        }

        public CDDiemDoResult LayCDDiemDo(string maDViQLy, string maDDo)
        {
            CDDiemDoRequest request = new CDDiemDoRequest();
            request.MA_DVIQLY = maDViQLy;
            request.MA_DDO = maDDo;

            string data = JsonConvert.SerializeObject(request);
            CMISAction action = new CMISAction();

            ApiService service = IoC.Resolve<ApiService>();
            var result = service.PostData(action.layCDDiemDoByMaDDo, data);
            if (result == null) return null;
            var response = JsonConvert.DeserializeObject<IList<CDDiemDoResult>>(result);
            if (response == null || response.Count() == 0) return null;
            return response.FirstOrDefault();
        }
    }

    public class KHangTreoThaoRequest
    {
        public string MA_DVIQLY { get; set; }
        public string MA_YCAU_KNAI { get; set; }
        public string LOAI_TIMKIEM { get; set; } = "TREOMOI";
    }

    public class CDDiemDoRequest
    {
        public string MA_DVIQLY { get; set; }
        public string MA_DDO { get; set; }
    }
}
