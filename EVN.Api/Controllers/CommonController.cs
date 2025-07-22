using FX.Core;
using EVN.Api.Model;
using EVN.Core.IServices;
using System;
using System.Linq;
using System.Web.Http;
using EVN.Api.Jwt;
using EVN.Core.Domain;
using System.Collections.Generic;
using EVN.Api.Model.Response;
using EVN.Api.Model.Request;
using System.Web;

namespace EVN.Api.Controllers
{
    [RoutePrefix("api/common")]
    public class CommonController : ApiController
    {
        [HttpGet]
        [JwtAuthentication]
        [Route("modules")]
        public IHttpActionResult Modules()
        {
            ResponseResult result = new ResponseResult();
            try
            {
                IModuleService service = IoC.Resolve<IModuleService>();
                var list = service.GetModules(true);
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

        [HttpGet]
        [JwtAuthentication]
        [Route("lydos/{nhom}")]
        public IHttpActionResult LyDos(int nhom = 1)
        {
            ResponseResult result = new ResponseResult();
            try
            {
                ILyDoService service = IoC.Resolve<ILyDoService>();
                var list = service.Query.Where(p => p.NHOM == nhom).OrderBy(p => p.STT_HTHI).ToList();
                List<Select2DataResult> listModel = list.Select(p => new Select2DataResult { id = p.MA_LDO, text = (p.MA_LDO + "-" + p.TEN_LDO) }).ToList();
                result.data = listModel;
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

        [JwtAuthentication]
        [HttpPost]
        [Route("listnhanvien")]
        public IHttpActionResult ListNhanVien(NhanVienFilter filter)
        {
            ResponseResult result = new ResponseResult();
            try
            {
                INhanVienService nviensrv = IoC.Resolve<INhanVienService>();
                var nhanviens = nviensrv.GetbyMaBPhan(filter.maDonVi, filter.maBPhan);
                if (filter.truongBPhan && nhanviens.Any(p => p.TRUONG_BPHAN))
                    nhanviens = nhanviens.Where(p => p.TRUONG_BPHAN).ToList();
                result.data = nhanviens;
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
        [Route("congviec")]
        public IHttpActionResult CongViec()
        {
            ResponseResult result = new ResponseResult();
            try
            {
                ICongViecService service = IoC.Resolve<ICongViecService>();
                var list = service.Query.OrderBy(p => p.STT).ToList();
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

        [HttpGet]
        [JwtAuthentication]
        [Route("organizations")]
        public IHttpActionResult Organizations()
        {
            ResponseResult result = new ResponseResult();
            try
            {
                IOrganizationService service = IoC.Resolve<IOrganizationService>();
                IUserdataService usersrv = IoC.Resolve<IUserdataService>();

                var data = new List<OrganizationModel>();
                var user = usersrv.GetbyName(HttpContext.Current.User.Identity.Name);
                var org = service.Getbykey(long.Parse(user.orgId));                
                if (org.orgCode == "X1")
                    org = service.GetbyCode("HN");

                if (org.orgCode == "X0206")
                    org = service.GetbyCode("HN");

                var list = service.GetbyParent(org.orgCode);
                data.Add(new OrganizationModel(org));
                foreach (var item in list)
                {
                    var model = new OrganizationModel(item);
                    data.Add(model);
                }
                result.data = data;
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
        [JwtAuthentication(Roles = "Admin")]
        [Route("bophans/{orgCode}")]
        public IHttpActionResult BoPhans(string orgCode)
        {
            ResponseResult result = new ResponseResult();
            try
            {
                IOrganizationService service = IoC.Resolve<IOrganizationService>();
                IBoPhanService deptservice = IoC.Resolve<IBoPhanService>();
                var list = deptservice.GetbyMaDVi(orgCode);

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

        [HttpGet]
        [JwtAuthentication]
        [Route("select2bophans/{orgCode}")]
        public IHttpActionResult Select2BoPhans(string orgCode)
        {
            ResponseResult result = new ResponseResult();
            try
            {
                IOrganizationService service = IoC.Resolve<IOrganizationService>();
                IBoPhanService deptservice = IoC.Resolve<IBoPhanService>();
                var list = deptservice.GetbyMaDVi(orgCode);
                List<Select2DataResult> listModel = new List<Select2DataResult>();
                listModel = list.Select(p => new Select2DataResult { id = p.MA_BPHAN, text = (p.MA_BPHAN + "-" + p.TEN_BPHAN) }).ToList();

                result.data = listModel;
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
        [Route("capdienaps")]
        public IHttpActionResult CapDienAps()
        {
            ResponseResult result = new ResponseResult();
            try
            {

                ICapDienApService service = IoC.Resolve<ICapDienApService>();
                var list = service.GetAll();

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

        [HttpGet]
        [JwtAuthentication]
        [Route("nhanviens/{maDVi}/{maBPhan}")]
        public IHttpActionResult NhanViens(string maDVi, string maBPhan)
        {
            ResponseResult result = new ResponseResult();
            try
            {
                INhanVienService deptservice = IoC.Resolve<INhanVienService>();
                var list = deptservice.GetbyMaBPhan(maDVi, maBPhan);

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

        [HttpGet]
        [JwtAuthentication]
        [Route("select2nhanviens/{maDVi}/{maBPhan}")]
        public IHttpActionResult Select2NhanViens(string maDVi, string maBPhan)
        {
            ResponseResult result = new ResponseResult();
            try
            {
                INhanVienService deptservice = IoC.Resolve<INhanVienService>();
                var list = deptservice.GetbyMaBPhan(maDVi, maBPhan);
                List<Select2DataResult> listModel = new List<Select2DataResult>();
                listModel = list.Select(p => new Select2DataResult { id = p.MA_NVIEN, text = (p.MA_NVIEN + "-" + p.TEN_NVIEN) }).ToList();
                result.data = listModel;
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
        [Route("select2nhanvientts/{maDVi}/{maBPhan}")]
        public IHttpActionResult Select2NhanVienTTs(string maDVi, string maBPhan)
        {
            ResponseResult result = new ResponseResult();
            try
            {
                INhanVienService deptservice = IoC.Resolve<INhanVienService>();
                var list = deptservice.GetbyMaBPhan(maDVi, maBPhan);
                List<Select2DataResult> listModel = new List<Select2DataResult>();
                listModel = list.Select(p => new Select2DataResult { id = (p.MA_NVIEN + "-" + p.TEN_NVIEN), text = (p.MA_NVIEN + "-" + p.TEN_NVIEN) }).ToList();
                result.data = listModel;
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


        [JwtAuthentication]
        [HttpGet]
        [Route("histories")]
        public IHttpActionResult Histories(int id = 0)
        {
            ResponseResult result = new ResponseResult();
            try
            {
                IThoaThuanDNChiTietService service = IoC.Resolve<IThoaThuanDNChiTietService>();
                ICongVanYeuCauService congvansrv = IoC.Resolve<ICongVanYeuCauService>();

                var congvan = congvansrv.Getbykey(id);
                var list = service.GetbyYCau(congvan.MaYeuCau);
                var data = new List<HistoryData>();
                foreach (var item in list)
                {
                    data.Add(new HistoryData(item));
                }
                result.data = data;
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
    }
}
