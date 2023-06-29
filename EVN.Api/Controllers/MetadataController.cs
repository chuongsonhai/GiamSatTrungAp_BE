using EVN.Api.Jwt;
using EVN.Api.Model;
using EVN.Core.CMIS;
using EVN.Core.Domain;
using EVN.Core.IServices;
using FX.Core;
using log4net;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http;

namespace EVN.Api.Controllers
{
    [RoutePrefix("api/metadata")]
    public class MetadataController : ApiController
    {
        private ILog log = LogManager.GetLogger(typeof(MetadataController));
        [HttpGet]
        [JwtAuthentication]
        [Route("trongai")]
        public IHttpActionResult TroNgai()
        {
            ResponseResult result = new ResponseResult();
            try
            {
                ITroNgaiService service = IoC.Resolve<ITroNgaiService>();
                var list = service.Query.OrderBy(p => p.MA_TNGAI).ToList();
                result.data = list;
                result.success = true;
                return Ok(result);
            }
            catch (Exception ex)
            {
                result.success = false;
                result.message = ex.Message;
                return Ok(result);
            }
        }

        [HttpPost]
        [JwtAuthentication]
        [Route("tientrinhs")]
        public IHttpActionResult TienTrinhs(TTrinhRequest request)
        {
            ResponseResult result = new ResponseResult();
            try
            {
                ICongVanYeuCauService ycaudnservice = IoC.Resolve<ICongVanYeuCauService>();
                IYCauNghiemThuService ycauntservice = IoC.Resolve<IYCauNghiemThuService>();
                ICauHinhDKService service = IoC.Resolve<ICauHinhDKService>();
                ICongViecService congviecsrv = IoC.Resolve<ICongViecService>();
                IBoPhanService bphansrv = IoC.Resolve<IBoPhanService>();
                INhanVienService nviensrv = IoC.Resolve<INhanVienService>();
                IBPhanCongViecService bphancvsrv = IoC.Resolve<IBPhanCongViecService>();
                ITThaiYeuCauService ttycausrv = IoC.Resolve<ITThaiYeuCauService>();

                var ycaudn = ycaudnservice.GetbyMaYCau(request.maYCau);
                string maCViec = ycaudn.MaCViec;
                string maLoaiYCau = ycaudn.MaLoaiYeuCau;
                string maDViQLy = ycaudn.MaDViQLy;

                var tthaiycau = ttycausrv.GetbyStatus((int)ycaudn.TrangThai, 0);
                if (request.nghiemThu)
                {
                    var ycaunt = ycauntservice.GetbyMaYCau(request.maYCau);
                    tthaiycau = ttycausrv.GetbyStatus((int)ycaunt.TrangThai, 1);
                }

                var maCViecs = new List<string>();
                if (tthaiycau != null)
                {
                    maCViecs = tthaiycau.CVIEC.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries).ToList();
                    maCViec = tthaiycau.CVIEC_TRUOC;
                }

                var list = service.GetbyMaCViec(maLoaiYCau, maCViec);
                if (maCViecs.Count() > 0)
                    list = list.Where(p => maCViecs.Contains(p.MA_CVIEC_TIEP)).ToList();
                var bphancvs = bphancvsrv.GetbyTienTrinh(maDViQLy, maLoaiYCau, list.Select(p => p.MA_CVIEC_TIEP).ToArray());

                var congviec = congviecsrv.Getbykey(maCViec);
                IList<CauHinhCViec> cviecs = new List<CauHinhCViec>();
                foreach (var item in list)
                {
                    var cviectiep = congviecsrv.Getbykey(item.MA_CVIEC_TIEP);
                    CauHinhCViec cviec = new CauHinhCViec();
                    cviec.MaLoaiYCau = maLoaiYCau;
                    cviec.MaCViec = congviec.MA_CVIEC;
                    cviec.TenCongViec = congviec.TEN_CVIEC;
                    cviec.MaCViecTiep = cviectiep.MA_CVIEC;
                    cviec.TenCongViecTiep = cviectiep.TEN_CVIEC;
                    cviec.CoReNhanh = item.TRO_NGAI;
                    cviecs.Add(cviec);
                }
                var mabphans = bphancvs.Select(p => p.MA_BPHAN).ToArray();

                TienTrinhDataModels model = new TienTrinhDataModels();
                model.boPhans = bphansrv.GetbyMaDVi(maDViQLy, mabphans);
                model.deptId = model.boPhans.FirstOrDefault().MA_BPHAN;
                var nhanviens = nviensrv.GetbyMaBPhan(maDViQLy, model.deptId);
                if (request.truongBPhan && nhanviens.Any(p => p.TRUONG_BPHAN))
                    nhanviens = nhanviens.Where(p => p.TRUONG_BPHAN).ToList();
                model.nhanViens = nhanviens;
                model.staffCode = nhanviens.Count() > 0 ? nhanviens.FirstOrDefault().MA_NVIEN : "";
                model.congViecs = cviecs;
                model.maCViec = cviecs.FirstOrDefault().MaCViecTiep;

                result.data = model;
                result.success = true;
                return Ok(result);
            }
            catch (Exception ex)
            {
                log.Error(ex);
                result.success = false;
                result.message = ex.Message;
                return Ok(result);
            }
        }

        [HttpGet]
        [JwtAuthentication]
        [Route("congviecs/{maYCau}/{maCViec}")]
        public IHttpActionResult CongViecs(string maYCau, string maCViec)
        {
            ResponseResult result = new ResponseResult();
            try
            {
                ICongVanYeuCauService congvansrv = IoC.Resolve<ICongVanYeuCauService>();
                ICauHinhDKService service = IoC.Resolve<ICauHinhDKService>();
                ICongViecService congviecsrv = IoC.Resolve<ICongViecService>();
                IBoPhanService bphansrv = IoC.Resolve<IBoPhanService>();
                INhanVienService nviensrv = IoC.Resolve<INhanVienService>();
                IBPhanCongViecService bphancvsrv = IoC.Resolve<IBPhanCongViecService>();
                IDvTienTrinhService ttrinhsrv = IoC.Resolve<IDvTienTrinhService>();
                ITNgaiCongViecService tngaicvsrv = IoC.Resolve<ITNgaiCongViecService>();

                var congvan = congvansrv.GetbyMaYCau(maYCau);
                var list = service.GetbyMaCViec(congvan.MaLoaiYeuCau, maCViec);                
                var bphancvs = bphancvsrv.GetbyTienTrinh(congvan.MaDViQLy, congvan.MaLoaiYeuCau, list.Select(p => p.MA_CVIEC_TIEP).ToArray());
                var congviec = congviecsrv.Getbykey(congvan.MaCViec);
                IList<CauHinhCViec> cviecs = new List<CauHinhCViec>();
                foreach (var item in list)
                {
                    var cviectiep = congviecsrv.Getbykey(item.MA_CVIEC_TIEP);
                    CauHinhCViec cviec = new CauHinhCViec();
                    cviec.MaLoaiYCau = congvan.MaLoaiYeuCau;
                    cviec.MaCViec = congviec.MA_CVIEC;
                    cviec.TenCongViec = congviec.TEN_CVIEC;
                    cviec.MaCViecTiep = cviectiep.MA_CVIEC;
                    cviec.TenCongViecTiep = cviectiep.TEN_CVIEC;
                    cviec.CoReNhanh = item.TRO_NGAI;
                    if (cviec.CoReNhanh)
                    {
                        var listcvtn = tngaicvsrv.GetAll().ToList();
                        if (listcvtn.Count > 0)
                        {
                            cviec.DSMaTroNgai = listcvtn.Where(x => x.MA_CVIEC == item.MA_CVIEC_TIEP).Select(p => p.MA_TNGAI).ToArray();
                        }
                    }
                    cviecs.Add(cviec);
                }
                var mabphans = bphancvs.Select(p => p.MA_BPHAN).ToArray();
                TienTrinhDataModels model = new TienTrinhDataModels();
                model.boPhans = bphansrv.GetbyMaDVi(congvan.MaDViQLy, mabphans);
                model.deptId = model.boPhans.FirstOrDefault().MA_BPHAN;
                var tientrinh = ttrinhsrv.GetbyYCau(maYCau, maCViec, -1);
                if (tientrinh != null && !string.IsNullOrWhiteSpace(tientrinh.MA_BPHAN_NHAN))
                    model.deptId = tientrinh.MA_BPHAN_NHAN;
                model.nhanViens = new List<NhanVien>();
                //model.nhanViens = nviensrv.GetbyMaBPhan(congvan.MaDViQLy, model.deptId);
                foreach (var bPhan in mabphans) { model.nhanViens.AddRange(nviensrv.GetbyMaBPhan(congvan.MaDViQLy, bPhan)); }
                model.staffCode = model.nhanViens.Count() > 0 ? model.nhanViens.FirstOrDefault().MA_NVIEN : "";
                model.congViecs = cviecs;
                model.maCViec = cviecs.FirstOrDefault().MaCViecTiep;
                log.Error($"list: {string.Join(",", model.nhanViens.Select(p => p.TEN_NVIEN).ToArray())}");
                result.data = model;
                result.success = true;
                return Ok(result);
            }
            catch (Exception ex)
            {
                result.success = false;
                result.message = ex.Message;
                return Ok(result);
            }
        }

        [HttpPost]
        [JwtAuthentication]
        [Route("gettngaicviecs")]
        public IHttpActionResult GetTNgaiCViecs(TNgaiCViecRequest request)
        {
            ResponseResult result = new ResponseResult();
            try
            {
                string maLoaiYCau = "TBAC_D";
                ICauHinhDKService service = IoC.Resolve<ICauHinhDKService>();
                ICongViecService congviecsrv = IoC.Resolve<ICongViecService>();
                ITNgaiCongViecService tngaicvsrv = IoC.Resolve<ITNgaiCongViecService>();
                var list = service.GetbyMaCViec(maLoaiYCau, request.maCViec);
                var tngaicvs = tngaicvsrv.GetbyTroNgai(request.maTNgai);
                if (tngaicvs.Count() > 0)
                {
                    string[] maCViecs = tngaicvs.Select(p => p.MA_CVIEC).ToArray();
                    list = list.Where(p => maCViecs.Contains(p.MA_CVIEC_TIEP)).OrderBy(p => p.ORDERNUMBER).ToList();
                }

                var congviec = congviecsrv.Getbykey(request.maCViec);
                IList<CauHinhCViec> cviecs = new List<CauHinhCViec>();
                foreach (var item in list)
                {
                    var cviectiep = congviecsrv.Getbykey(item.MA_CVIEC_TIEP);
                    CauHinhCViec cviec = new CauHinhCViec();
                    cviec.MaLoaiYCau = maLoaiYCau;
                    cviec.MaCViec = congviec.MA_CVIEC;
                    cviec.TenCongViec = congviec.TEN_CVIEC;
                    cviec.MaCViecTiep = cviectiep.MA_CVIEC;
                    cviec.TenCongViecTiep = cviectiep.TEN_CVIEC;
                    cviec.CoReNhanh = item.TRO_NGAI;
                    cviecs.Add(cviec);
                }

                result.data = cviecs;
                result.success = true;
                return Ok(result);
            }
            catch (Exception ex)
            {
                result.success = false;
                result.message = ex.Message;
                return Ok(result);
            }
        }

        [HttpGet]
        [JwtAuthentication]
        [Route("listuq")]
        public IHttpActionResult ListUQ()
        {
            ResponseResult result = new ResponseResult();
            try
            {
                IUserdataService userdatasrv = IoC.Resolve<IUserdataService>();
                ICmisProcessService cmisProcess = new CmisProcessService();

                var userdata = userdatasrv.GetbyName(HttpContext.Current.User.Identity.Name);

                var data = cmisProcess.GetDanhMucUQ(userdata.maDViQLy);
                result.data = data;
                result.success = true;
                return Ok(result);
            }
            catch (Exception ex)
            {
                result.success = false;
                result.message = "Không tìm thấy thông tin";
                return Ok(result);
            }
        }

        [HttpGet]
        [JwtAuthentication]
        [Route("listgianhom/{maCapDAp}")]
        public IHttpActionResult ListGiaNhom(string maCapDAp)
        {
            ResponseResult result = new ResponseResult();
            try
            {
                ICmisProcessService cmisProcess = new CmisProcessService();

                string ngayHieuLuc = DateTime.Now.ToString("dd/MM/yyyy");
                var items = cmisProcess.GetGiaNhomNNHieuluc(maCapDAp, ngayHieuLuc);
                var data = new GiaNhomData();
                data.ListKT = items.Where(p => p.THOIGIAN_BDIEN == "KT").Select(p => new Select2DataResult { id = p.DON_GIA, text = p.DON_GIA }).ToList(); 
                data.ListBT = items.Where(p => p.THOIGIAN_BDIEN == "BT").Select(p => new Select2DataResult { id = p.DON_GIA, text = p.DON_GIA }).ToList();
                data.ListCD = items.Where(p => p.THOIGIAN_BDIEN == "CD").Select(p => new Select2DataResult { id = p.DON_GIA, text = p.DON_GIA }).ToList(); 
                data.ListTD = items.Where(p => p.THOIGIAN_BDIEN == "TD").Select(p => new Select2DataResult { id = p.DON_GIA, text = p.DON_GIA }).ToList(); 
                result.data = data;
                result.success = true;
                return Ok(result);
            }
            catch (Exception ex)
            {
                result.success = false;
                result.message = "Không tìm thấy thông tin";
                return Ok(result);
            }
        }
    }
}
