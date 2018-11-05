using Sabio.Models.Domain;
using Sabio.Models.Requests.BusinessVentures;
using Sabio.Models.Responses;
using Sabio.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web;
using System.Web.Http;

namespace Sabio.Web.Controllers.Api
{
    public class BusinessVenturesController:ApiController
    {
        readonly BusinessVenturesService _businessVenturesService;

        public BusinessVenturesController(BusinessVenturesService businessVenturesService)
        {
            _businessVenturesService= businessVenturesService;
        }

        [HttpPost, Route("api/business-ventures")]
        public HttpResponseMessage Create(BusinessVenturesCreateRequest request)
        {
            if (request == null)
            {
                ModelState.AddModelError("User", "User cannot be null");
            }
            if (!ModelState.IsValid)
            {
                return Request.CreateErrorResponse(HttpStatusCode.BadRequest, ModelState);
            }
            int returnId = _businessVenturesService.Create(request);

            return Request.CreateResponse(HttpStatusCode.OK, new ItemResponse<int> { Item = returnId });
        }

        [HttpDelete, Route("api/business-ventures/{Id:int}")]
        public HttpResponseMessage Delete(int Id)
        {
            _businessVenturesService.Delete(Id);
            return Request.CreateResponse(HttpStatusCode.OK, new SuccessResponse());
        }

        [HttpGet, Route("api/business-ventures/{Id:int}")]
        public HttpResponseMessage ReadById(int Id)
        {
            if (!ModelState.IsValid)
            {
                return Request.CreateErrorResponse(HttpStatusCode.BadRequest, ModelState);
            }
            var busiVent = _businessVenturesService.ReadById(Id);

            return Request.CreateResponse(HttpStatusCode.OK, new ItemResponse<BusinessVentures>
            {
                Item = busiVent
            });
        }

        [HttpGet, Route("api/business-ventures/user-id/{Id:int}")]
        public HttpResponseMessage GetByUserId(int Id)
        {
            var busiVents = _businessVenturesService.GetByUserId(Id);
            return Request.CreateResponse(HttpStatusCode.OK, new ItemsResponse<BusinessVentures>
            {
                Items = busiVents
            });
        }

        [HttpGet, Route("api/business-ventures")]
        public HttpResponseMessage ReadAll()
        {
            var busiVents = _businessVenturesService.ReadAll();
            return Request.CreateResponse(HttpStatusCode.OK, new ItemsResponse<BusinessVentures>
            {
                Items = busiVents
            });
        }

        [HttpPut, Route("api/business-ventures/{Id:int}")]
        public HttpResponseMessage Update(BusinessVenturesUpdateRequest request, int Id)
        {
            if (request == null)
            {
                ModelState.AddModelError("User", "User cannot be null");
            }
            else if (Id != request.Id)
            {
                ModelState.AddModelError("User", "Id does not match User.Id");
            }
            if (!ModelState.IsValid)
            {
                return Request.CreateErrorResponse(HttpStatusCode.BadRequest, ModelState);
            }
            var returnId=_businessVenturesService.Update(request, Id);

            return Request.CreateResponse(HttpStatusCode.OK, new ItemResponse<int> {Item=returnId});
        }

    }
}