using Sabio.Models.Domain;
using Sabio.Models.Requests.Milestones;
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
    public class MilestoneController:ApiController
    {
        readonly MilestoneService _milestoneService;

        public MilestoneController(MilestoneService milestoneService)
        {
            _milestoneService = milestoneService;
        }

        [HttpDelete,Route("api/milestone/{Id:int}")]
        public HttpResponseMessage Delete(int Id)
        {
            _milestoneService.Delete(Id);
            return Request.CreateResponse(HttpStatusCode.OK, new SuccessResponse());
        }

        [HttpPost, Route("api/milestone")]
        public HttpResponseMessage Create(MilestoneCreateRequest request)
        {
            if (request == null)
            {
                ModelState.AddModelError("empty object", "supply body");
            }
            if (!ModelState.IsValid)
            {
                return Request.CreateErrorResponse(HttpStatusCode.BadRequest, ModelState);
            }
            var id = _milestoneService.Create(request);

            return Request.CreateResponse(HttpStatusCode.OK, new ItemResponse<int> { Item = id });
        }

        [HttpGet, Route("api/milestone")]
        public HttpResponseMessage ReadAll()
        {

            List<Milestone> milestones = _milestoneService.ReadAll();

            return Request.CreateResponse(HttpStatusCode.OK, new ItemsResponse<Milestone> { Items = milestones });
        }

        [HttpGet, Route("api/milestone/{Id:int}")]
        public HttpResponseMessage ReadById(int Id)
        {
            var milestone = _milestoneService.ReadById(Id);

            return Request.CreateResponse(HttpStatusCode.OK, new ItemResponse<Milestone> { Item = milestone });
        }

        //[HttpGet, Route("api/milestone/{Id:int}")]
        //public HttpResponseMessage ReadByMilestoneId(int Id)
        //{
        //    var milestone = _milestoneService.ReadByMilestoneId(Id);

        //    return Request.CreateResponse(HttpStatusCode.OK, new ItemResponse<Milestone> { Item = milestone });
        //}

        [HttpPut, Route("api/milestone/{Id:int}")]
        public HttpResponseMessage Update(int Id, MilestoneUpdateRequest request)
        {
            if (request == null)
            {
                ModelState.AddModelError("empty object", "supply body");
            }
            if (!ModelState.IsValid)
            {
                return Request.CreateErrorResponse(HttpStatusCode.BadRequest, ModelState);
            }
            var id = _milestoneService.Update(Id, request);

            return Request.CreateResponse(HttpStatusCode.OK, new ItemResponse<int> { Item = id });
        }

    }
}