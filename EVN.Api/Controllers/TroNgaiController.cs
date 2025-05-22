using FX.Core;
using EVN.Api.Model;
using EVN.Core.Domain;
using EVN.Core.IServices;
using System;
using System.Linq;
using System.Web.Http;
using EVN.Api.Jwt;
using EVN.Core;

namespace EVN.Api.Controllers
{
    [RoutePrefix("api/trongai")]
    public class TroNgaiController : ApiController
    {
        [HttpPost]
        [JwtAuthentication(Roles = "Admin")]
        [Route("filter")]
        public IHttpActionResult Filter(BaseFilterRequest request)
        {
            ResponseResult result = new ResponseResult();
            try
            {
                int pageindex = request.Paginator.page > 0 ? request.Paginator.page - 1 : 0;
                int total = 0;
                ITroNgaiService service = IoC.Resolve<ITroNgaiService>();
                var list = service.GetbyFilter(request.Filter.keyword, pageindex, request.Paginator.pageSize, out total);

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
        public IHttpActionResult Get(string id)
        {
            try
            {
                ITroNgaiService service = IoC.Resolve<ITroNgaiService>();
                ITNgaiCongViecService tngaicvsrv = IoC.Resolve<ITNgaiCongViecService>();

                var item = service.Getbykey(id);
                var cviecs = tngaicvsrv.GetbyTroNgai(item.MA_TNGAI);

                TroNgaiModel model = new TroNgaiModel();
                model.MA_TNGAI = item.MA_TNGAI;
                model.TEN_TNGAI = item.TEN_TNGAI;
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
        public IHttpActionResult Put(TroNgaiModel model)
        {
            try
            {
                ITroNgaiService service = IoC.Resolve<ITroNgaiService>();
                ITNgaiCongViecService tngaicvsrv = IoC.Resolve<ITNgaiCongViecService>();
                ICongViecService cvservice = IoC.Resolve<ICongViecService>();

                var item = service.Getbykey(model.MA_TNGAI);
                var cviecs = tngaicvsrv.GetbyTroNgai(item.MA_TNGAI);

                foreach (var cviec in cviecs)
                {
                    tngaicvsrv.Delete(cviec);
                }
                var listCV = cvservice.Query.Where(p => model.CongViecs.Contains(p.MA_CVIEC)).ToList();
                foreach (var cviec in listCV)
                {
                    TNgaiCongViec bpcv = new TNgaiCongViec();
                    bpcv.MA_TNGAI = item.MA_TNGAI;
                    bpcv.MA_CVIEC = cviec.MA_CVIEC;
                    bpcv.MA_LOAI_YCAU = cviec.MA_LOAI_YCAU;
                    tngaicvsrv.CreateNew(bpcv);
                }
                tngaicvsrv.CommitChanges();
                return Ok(model);
            }
            catch (Exception ex)
            {
                return BadRequest();
            }
        }
    }
}
