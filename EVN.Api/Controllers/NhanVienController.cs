using FX.Core;
using EVN.Api.Model;
using EVN.Core.Domain;
using EVN.Core.IServices;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Http;
using EVN.Api.Jwt;
using EVN.Api.Model.Request;

namespace EVN.Api.Controllers
{
    [RoutePrefix("api/nhanvien")]
    public class NhanVienController : ApiController
    {

        [JwtAuthentication]
        [HttpPost]
        [Route("listnhanvien")]
        public IHttpActionResult ListNhanVien(NhanVienFilter filter)
        {
            ResponseResult result = new ResponseResult();
            try
            {
                INhanVienService service = IoC.Resolve<INhanVienService>();
                if (filter.dongBoCmis)
                    service.Sync(filter.maDonVi);
                var list = service.GetbyMaBPhan(filter.maDonVi, filter.maBPhan);
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
        [Route("filter")]
        public IHttpActionResult Filter(UserFilterRequest request)
        {
            ResponseResult result = new ResponseResult();
            try
            {
                int pageindex = request.Paginator.page > 0 ? request.Paginator.page - 1 : 0;
                int total = 0;
                INhanVienService service = IoC.Resolve<INhanVienService>();
                if (request.Filter.dongBoCmis)
                    service.Sync(request.Filter.maDViQLy);
                var list = service.GetbyFilter(request.Filter.maDViQLy, request.Filter.maBPhan, request.Filter.keyword, pageindex, request.Paginator.pageSize, out total);
                result.data = list;
                result.total = total;
                result.success = true;
                return Ok(result);
            }
            catch (Exception ex)
            {
                result.success = false;
                result.message = "Có lỗi xảy ra, vui lòng thực hiện lại";
                return Ok(result);
            }
        }
        
        [HttpGet]
        [JwtAuthentication]
        public IHttpActionResult Get(int id)
        {
            try
            {
                INhanVienService service = IoC.Resolve<INhanVienService>();
                var item = service.Getbykey(id);
                if (string.IsNullOrEmpty(item.MA_BPHAN))
                {
                    item.MA_BPHAN = "";
                }
                else
                {
                    var maBPhans = item.MA_BPHAN.Split(new char[] { '(', ')' }, StringSplitOptions.RemoveEmptyEntries);
                    item.MA_BPHAN = string.Empty;
                    foreach (var maBPhan in maBPhans)
                    {
                        if (!item.MA_BPHAN.Contains($"{maBPhan}"))
                        {
                            if (maBPhan == maBPhans.Last())
                            {
                                item.MA_BPHAN += $"{maBPhan}";
                            }
                            else
                            {
                                item.MA_BPHAN += $"{maBPhan},";
                            }

                        }

                    }
                }
                return Ok(item);
            }
            catch (Exception ex)
            {
                return NotFound();
            }
        }

        [HttpPut]
        [JwtAuthentication]
        public IHttpActionResult Put(NhanVien model)
        {
            try
            {
                INhanVienService service = IoC.Resolve<INhanVienService>();
                var item = service.Getbykey(model.ID);
                item.EMAIL = model.EMAIL;
                item.DIEN_THOAI = model.DIEN_THOAI;
                item.DIA_CHI = model.DIA_CHI;
                item.TRUONG_BPHAN = model.TRUONG_BPHAN;
                item.CVIEC = model.CVIEC;
                var maBPhans = model.MA_BPHAN.Split(',', (char)StringSplitOptions.RemoveEmptyEntries);
                item.MA_BPHAN = string.Empty;
                foreach (var maBPhan in maBPhans)
                {
                    if (!item.MA_BPHAN.Contains($"({maBPhan})"))
                        item.MA_BPHAN += $"({maBPhan})";
                }
                service.Update(item);
                service.CommitChanges();
                return Ok(model);
            }
            catch (Exception ex)
            {
                return NotFound();
            }
        }
    }
}
