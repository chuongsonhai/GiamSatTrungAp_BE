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
    [RoutePrefix("api/bophan")]
    public class BoPhanController : ApiController
    {
        [HttpPost]
        [JwtAuthentication(Roles = "Admin")]
        [Route("filter")]
        public IHttpActionResult Filter(UserFilterRequest request)
        {
            ResponseResult result = new ResponseResult();
            try
            {
                int pageindex = request.Paginator.page > 0 ? request.Paginator.page - 1 : 0;
                int total = 0;
                IBoPhanService service = IoC.Resolve<IBoPhanService>();
                if (request.Filter.dongBoCmis)
                    service.Sync(request.Filter.maDViQLy);
                var list = service.GetbyFilter(request.Filter.maDViQLy, request.Filter.keyword, pageindex, request.Paginator.pageSize, out total);

                result.total = total;
                result.data = list;
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
        [Route("GetByMaDV/{maDVQL}")]
        public IHttpActionResult GetByMaDV(string maDVQL)
        {
            ResponseResult result = new ResponseResult();
            try
            {
             
                IBoPhanService service = IoC.Resolve<IBoPhanService>();
               
                var list = service.GetbyMaDVi(maDVQL);
                result.data = list;
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
        [Route("GetByMaBP/{maDVQL}/{maBP}")]
        public IHttpActionResult GetByMaBP(string maDVQL,string maBP)
        {
          
            try
            {

                IBoPhanService service = IoC.Resolve<IBoPhanService>();

                var bp = service.GetbyCode(maDVQL,maBP);
             
                return Ok(bp);
            }
            catch (Exception ex)
            {
                return NotFound();
            }
        }

        [HttpGet]
        [JwtAuthentication]
        public IHttpActionResult Get(int id)
        {
            try
            {
                IBoPhanService service = IoC.Resolve<IBoPhanService>();
                IBPhanCongViecService bphancvsrv = IoC.Resolve<IBPhanCongViecService>();

                var item = service.Getbykey(id);
                var cviecs = bphancvsrv.GetbyBPhan(item.MA_DVIQLY, item.MA_BPHAN);

                BoPhanModel model = new BoPhanModel();
                model.ID = item.ID;
                model.MA_BPHAN = item.MA_BPHAN;
                model.TEN_BPHAN = item.TEN_BPHAN;
                model.MA_DVIQLY = item.MA_DVIQLY;
                model.MO_TA = item.MO_TA;
                model.GHI_CHU = item.GHI_CHU;
                if (cviecs.Count > 0)
                    model.CongViecs = cviecs.Select(p => p.MA_CVIEC).ToList();
                return Ok(model);
            }
            catch (Exception ex)
            {
                return NotFound();
            }
        }

        [HttpPut]
        [JwtAuthentication]
        public IHttpActionResult Put(BoPhanModel model)
        {
            try
            {
                IBoPhanService service = IoC.Resolve<IBoPhanService>();
                IBPhanCongViecService bphancvsrv = IoC.Resolve<IBPhanCongViecService>();
                ICongViecService cvservice = IoC.Resolve<ICongViecService>();

                var item = service.Getbykey(model.ID);
                var cviecs = bphancvsrv.GetbyBPhan(item.MA_DVIQLY, item.MA_BPHAN);

                foreach (var cviec in cviecs)
                {
                    bphancvsrv.Delete(cviec);
                }
                var listCV = cvservice.Query.Where(p => model.CongViecs.Contains(p.MA_CVIEC)).ToList();
                foreach (var cviec in listCV)
                {
                    BPhanCongViec bpcv = new BPhanCongViec();
                    bpcv.MA_DVIQLY = item.MA_DVIQLY;
                    bpcv.MA_BPHAN = item.MA_BPHAN;
                    bpcv.MA_CVIEC = cviec.MA_CVIEC;
                    bphancvsrv.CreateNew(bpcv);
                }
                bphancvsrv.CommitChanges();
                return Ok(model);
            }
            catch (Exception ex)
            {
                return NotFound();
            }
        }
    }
}
