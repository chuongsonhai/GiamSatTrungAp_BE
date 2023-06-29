using EVN.Api.Jwt;
using EVN.Api.Model;
using EVN.Core.Domain;
using EVN.Core.IServices;
using FX.Context.Security;
using FX.Core;
using log4net;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Http;

namespace EVN.Api.Controllers
{
    [RoutePrefix("api/cauhinhdongbo")]
    public class CauHinhDongBoController : ApiController
    {
        private ILog log = LogManager.GetLogger(typeof(CauHinhDongBoController));

        [JwtAuthentication]
        [HttpPost]
        [Route("filter")]
        public IHttpActionResult Filter()
        {
            ResponseResult result = new ResponseResult();
            try
            {
                ICauHinhDongBoService service = IoC.Resolve<ICauHinhDongBoService>();
                IList<CauHinhDongBo> list = service.Query.ToList();
                IList<CauHinhDongBoModel> listModel = new List<CauHinhDongBoModel>();
                foreach (CauHinhDongBo c in list)
                {
                    CauHinhDongBoModel cauHinhDongBoModel = new CauHinhDongBoModel(c);
                    listModel.Add(cauHinhDongBoModel);
                }
                result.data = listModel;
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
        [HttpGet]
        public IHttpActionResult Get(string id)
        {
            try
            {
                ICauHinhDongBoService service = IoC.Resolve<ICauHinhDongBoService>();
                var item = service.Getbykey(id);
                var model = new CauHinhDongBoModel(item);
                return Ok(model);
            }
            catch (Exception ex)
            {
                log.Error(ex);
                return NotFound();
            }
        }        

        [JwtAuthentication]
        [HttpPut]
        public IHttpActionResult Put([FromBody] CauHinhDongBoModel model)
        {
            try
            {
                ICauHinhDongBoService service = IoC.Resolve<ICauHinhDongBoService>();

                var entity = service.Getbykey(model.MA);
                entity=model.ToEntity(entity); 
                service.Save(entity);
                service.CommitChanges();
                return Ok(model);
            }
            catch (Exception ex)
            {
                log.Error(ex);
                return BadRequest();
            }
        }
    }
}
