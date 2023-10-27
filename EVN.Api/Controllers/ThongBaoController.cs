using EVN.Api.Jwt;
using EVN.Api.Model;
using EVN.Api.Model.Request;
using EVN.Core.Domain;
using EVN.Core.IServices;
using FX.Core;
using log4net;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Http;

namespace EVN.Api.Controllers
{
    [RoutePrefix("api/thongbao")]
    public class ThongBaoController : ApiController
    {
        private ILog log = LogManager.GetLogger(typeof(ThongBaoController));

        [JwtAuthentication]
        [HttpPost]
        [Route("filter")]
        public IHttpActionResult Filter(ThongBaoRequest request)
        {
            ResponseResult result = new ResponseResult();
            try
            {
                int pageindex = 0;
                int total = 0, pagesize = request.Paginator.pageSize > 0 ? request.Paginator.pageSize : 20;

                IUserdataService userservice = IoC.Resolve<IUserdataService>();
                IThongBaoService service = IoC.Resolve<IThongBaoService>();
                var user = userservice.GetbyName(HttpContext.Current.User.Identity.Name);

                string maNVien = user.maNVien;
                if (user.Roles.Any(p => p.isSysadmin)) maNVien = string.Empty;
                var list = service.GetbyFilter(request.Filter.maDViQLy, maNVien, request.Filter.maYCau, request.Filter.status, pageindex, pagesize, out total);
                log.Error("listTbao:" + list.Count());
                var data = new List<ThongBaoData>();
                foreach (var item in list)
                    data.Add(new ThongBaoData(item));
                result.total = total;
                result.data = data;
                result.success = true;
                return Ok(result);
            }
            catch (Exception ex)
            {
                log.Error(ex);
                result.success = false;
                result.message = "Có lỗi xảy ra, vui lòng thực hiện lại.";
                return Ok(result);
            }
        }

        [JwtAuthentication]
        [HttpPost]
        [Route("getnotifies")]
        public async Task<IHttpActionResult> GetNotifies(ThongBaoFilter filter)
        {
            ResponseResult result = new ResponseResult();
            try
            {
                int pageindex = 0;
                int total = 0;

                IUserdataService userservice = IoC.Resolve<IUserdataService>();
                IThongBaoService service = IoC.Resolve<IThongBaoService>();
                IXacNhanTroNgaiService xacMinhTroNgaiService = IoC.Resolve<IXacNhanTroNgaiService>();
                var user = userservice.GetbyName(HttpContext.Current.User.Identity.Name);

                string maNVien = user.maNVien;
                if (user.Roles.Any(p => p.isSysadmin)) maNVien = string.Empty;
                var list = service.GetbyFilter(user.maDViQLy, maNVien, filter.maYCau, filter.status, pageindex, 10, out total);
                var listkhaosat = xacMinhTroNgaiService.Getnotikhaosat(user.maDViQLy, filter.maYCau);
                var data = new List<ThongBaoData>();


                    data.AddRange(list.Select(x=> new ThongBaoData()
                    {
                        BPhanNhan = x.BPhanNhan,
                        CongViec = x.CongViec,
                        DuAnDien = x.DuAnDien,
                        ID = x.ID,
                        KhachHang = x.KhachHang,
                        Loai = x.Loai.ToString(),
                        MaCViec = x.MaCViec,
                        MaDViQLy = x.MaDViQLy,
                        MaYeuCau = x.MaYeuCau,
                        NgayHen = x.NgayHen.ToString(),
                        NgayTao = x.NgayTao.ToString(),
                        NguoiNhan = x.NguoiNhan,
                        NguoiTao = x.NguoiTao,
                        NoiDung = x.NoiDung,
                        TrangThai = x.TrangThai.GetHashCode(),
                        IsKhaoSat = false

                    }).ToList());

                //Map từng trường của KhaoSat => ThongBaoData
                data.AddRange(listkhaosat.Select(x => new ThongBaoData()
                {
                  //TrangThai = x.TRANGTHAI.HasValue ? x.TRANGTHAI.Value : 1 ,
                  TrangThai = 0,
                  NoiDung = "Khách hàng đã đánh giá 1*, 2*",
                  MaDViQLy = x.MA_DVI,
                  MaYeuCau = x.MA_YCAU,
                  IsKhaoSat = true,


                }).ToList());


                result.total = total;
                result.data = data;
                result.success = true;
                return Ok(result);
            }
            catch (Exception ex)
            {
                log.Error(ex);
                result.success = false;
                result.message = "Có lỗi xảy ra, vui lòng thực hiện lại.";
                return Ok(result);
            }
        }

        [JwtAuthentication]
        [HttpPost]
        [Route("updatestatus")]
        public IHttpActionResult UpdateStatus(ThongBaoUpdateStatus data)
        {
            ResponseResult result = new ResponseResult();
            try
            {
                if(data.tBaoIDs == null || data.tBaoIDs.Count == 0)
                {
                    result.success = false;
                    result.message = "Chưa chọn thông báo cần xử lý";
                    return Ok(result);
                }    
                IThongBaoService service = IoC.Resolve<IThongBaoService>();
                var thongbaos = service.Query.Where(p => data.tBaoIDs.Contains(p.ID)).ToList();
                foreach(var item in thongbaos)
                {
                    item.TrangThai = Core.Domain.TThaiThongBao.DaXuLy;
                    service.Save(item);
                }
                service.CommitChanges();
                result.success = true;
                return Ok(result);
            }
            catch(Exception ex)
            {
                log.Error(ex);
                result.success = false;
                result.message = "Chưa chọn thông báo đã xử lý";
                return Ok(result);
            }
        }
    }
}