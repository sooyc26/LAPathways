using Sabio.Models.Domain;
using Sabio.Models.Requests.Milestones;
using Sabio.Models.Requests.UserMilestones;
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
    public class UsersMilestonesController:ApiController
    {

        readonly UsersMilestonesService _usersMilestonesService;
        public UsersMilestonesController(UsersMilestonesService usersMilestonesService)
        {
            _usersMilestonesService = usersMilestonesService;
        }

        [HttpDelete, Route("api/milestone/user/{Id:int}")]
        public HttpResponseMessage Delete(int Id)
        {
            _usersMilestonesService.Delete(Id);
            return Request.CreateResponse(HttpStatusCode.OK, new SuccessResponse());
        }

        [HttpPost, Route("api/user-milestones")]
        public HttpResponseMessage Create(UserMilestoneCreateRequest request)
        {
            if (request == null)
            {
                ModelState.AddModelError("empty object", "supply body");
            }
            if (!ModelState.IsValid)
            {
                return Request.CreateErrorResponse(HttpStatusCode.BadRequest, ModelState);
            }
            var id = _usersMilestonesService.Create(request);

            return Request.CreateResponse(HttpStatusCode.OK, new ItemResponse<int> { Item = id });
        }

        [HttpGet, Route("api/milestone/user")]
        public HttpResponseMessage ReadAll()
        {

            List<UsersMilestone> milestones = _usersMilestonesService.ReadAll();

            return Request.CreateResponse(HttpStatusCode.OK, new ItemsResponse<UsersMilestone> { Items = milestones });
        }

        [HttpGet, Route("api/milestone/mentor/{Id:int}")]
        public HttpResponseMessage MentorReadAllByMentorId(int Id)
        {

            List<UsersMilestone> milestones = _usersMilestonesService.MentorReadAllByMentorId(Id);

            return Request.CreateResponse(HttpStatusCode.OK, new ItemsResponse<UsersMilestone> { Items = milestones });
        }

        [HttpGet, Route("api/milestone/user-id/{UserId:int}")]
        public HttpResponseMessage ReadByUserId(int UserId)
        {
            var milestones = _usersMilestonesService.ReadByUserId(UserId);

            return Request.CreateResponse(HttpStatusCode.OK, new ItemsResponse<UsersMilestone> { Items = milestones });
        }

        [HttpGet, Route("api/milestone/user/{UserId:int}")]
        public HttpResponseMessage ReadById(int UserId)
        {
            var milestone = _usersMilestonesService.ReadById(UserId);

            return Request.CreateResponse(HttpStatusCode.OK, new ItemResponse<UsersMilestone> { Item = milestone });
        }

        [HttpPut, Route("api/milestone/user/{Id:int}")]
        public HttpResponseMessage UpdateById(int Id, UserMilestoneUpdateRequest request)
        {
            if (request == null)
            {
                ModelState.AddModelError("empty object", "supply body");
            }
            if (!ModelState.IsValid)
            {
                return Request.CreateErrorResponse(HttpStatusCode.BadRequest, ModelState);
            }
            var id = _usersMilestonesService.UpdateById(Id, request);

            return Request.CreateResponse(HttpStatusCode.OK, new ItemResponse<int> { Item = id });
        }

        [HttpPatch, Route("api/milestone/user/{Id:int}")]
        public HttpResponseMessage UpdateIsCompletedById(int Id, UserMilestoneUpdateRequest request)
        {
            if (request == null)
            {
                ModelState.AddModelError("empty object", "supply body");
            }
            if (!ModelState.IsValid)
            {
                return Request.CreateErrorResponse(HttpStatusCode.BadRequest, ModelState);
            }
            var id = _usersMilestonesService.UpdateIsCompletedById(Id, request);

            return Request.CreateResponse(HttpStatusCode.OK, new ItemResponse<int> { Item = id });
        }
    }
}
