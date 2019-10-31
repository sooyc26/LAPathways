using LaPathways.Data.Providers;
using LaPathways.Models.Domain;
using LaPathways.Models.Domain.CoachResourceRecommendations;
using LaPathways.Models.Requests.CoachRecommendation;
using LaPathways.Models.Responses;
using LaPathways.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace LaPathways.Web.Controllers.Api
{
    [RoutePrefix("api/recommendations")]
    public class CoachRecommendationsController : ApiController
    {
        readonly CoachRecommendationService _coachRecommendationService;

        public CoachRecommendationsController(CoachRecommendationService coachRecommendationService)
        {
            _coachRecommendationService = coachRecommendationService;
        }

        [HttpGet, Route("resource-groups/{id:int}"), AllowAnonymous]
        public HttpResponseMessage GetCoachesAndResourcesRecommendation(int id)
        {
            var recommendations = _coachRecommendationService.GetResourceGroupsCoachIdByInstanceId(id);
            return Request.CreateResponse(HttpStatusCode.OK, new ItemResponse<ResourceCoachRecommendations>
            {
                Item = recommendations
            });
        }

        [HttpGet, Route]
        public HttpResponseMessage ReadById(int id)
        {
            List<UserCoachRecommendation> recommendation = _coachRecommendationService.GetRecommendation(id);
            return Request.CreateResponse(HttpStatusCode.OK, new ItemsResponse<UserCoachRecommendation>
            {
                Items = recommendation
            });
        }
        [HttpGet, Route("instanceId/{Id:int}"), AllowAnonymous]
        public HttpResponseMessage GetRecommendationsByInstanceId(int Id)
        {
            var recommendations = _coachRecommendationService.GetRecommendationsByInstanceId(Id);

            return Request.CreateResponse(HttpStatusCode.OK,
                new ItemResponse<CoachExpertiseResourceProvider> { Item = recommendations });
        }


        [HttpPost, Route("coach-expertise/list")]
        public HttpResponseMessage GetCoachesProfile(CoachExpertiseTypeIdList request)
        {
            var recommendations = _coachRecommendationService.ReadCoachProfiles(request);

            return Request.CreateResponse(HttpStatusCode.OK,
                new ItemsResponse<UserCoachProfile> { Items = recommendations });
        }

        [HttpGet, Route("userId/{id:int}"), AllowAnonymous]
        public HttpResponseMessage GetInstanceIdByUserId(int id)
        {
            int instanceId = _coachRecommendationService.GetAssessmentInstanceIdByUserId(id);
            return Request.CreateResponse(HttpStatusCode.OK, new ItemResponse<int>
            {
                Item = instanceId
            });
        }
    }
}
