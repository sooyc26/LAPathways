using Sabio.Models.Domain;
using Sabio.Models.Responses;
using Sabio.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace Sabio.Web.Controllers.Api
{
    [RoutePrefix("api/mentors")]
    public class MentorRecommendationController : ApiController
    {
        MentorRecommendationService _mentorRecommendationService;
        public MentorRecommendationController(MentorRecommendationService mentorRecommendationService)
        {
            _mentorRecommendationService = mentorRecommendationService;
        }

        [HttpGet, Route("recommendations/{userId:int}")]
        public HttpResponseMessage GetMentors(int userId)
        {
            List<UserMentorProfile> mentors = _mentorRecommendationService.ReadCoachProfiles(userId);
            return Request.CreateResponse(HttpStatusCode.OK, new ItemsResponse<UserMentorProfile>
            {
                Items = mentors
            });
        }
    }
}
