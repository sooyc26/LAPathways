using LaPathways.Models.Domain;
using LaPathways.Models.Requests.Recommendation;
using LaPathways.Models.Responses;
using LaPathways.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web;
using System.Web.Http;

namespace LaPathways.Web.Controllers.Api
{
    public class RecommendationsController:ApiController
    {
        RecommendationService _recommendationService;
        public RecommendationsController(RecommendationService recommendationService)
        {
            _recommendationService = recommendationService;
        }

        [HttpDelete, Route("api/coach-resource/recommendation/{Id:int}")]
        public HttpResponseMessage Delete (int Id)
        {
            _recommendationService.Delete(Id);
            return Request.CreateResponse(HttpStatusCode.OK, new SuccessResponse());
        }

        [HttpPost, Route("api/coach-resource/recommendation")]
        public HttpResponseMessage Create(RecommendationCreateRequest request)
        {
            if (request == null)
            {
                ModelState.AddModelError("empty object", "supply body");
            }
            if (!ModelState.IsValid)
            {
                return Request.CreateErrorResponse(HttpStatusCode.BadRequest, ModelState);
            }
            var id = _recommendationService.Create(request);

            return Request.CreateResponse(HttpStatusCode.OK, new ItemResponse<int> { Item = id });
        }

        [HttpGet, Route("api/coach-resource/recommendation/{Id:int}")]
        public HttpResponseMessage ReadById(int Id)
        {
            var recommendation = _recommendationService.ReadById(Id);

            return Request.CreateResponse(HttpStatusCode.OK, new ItemResponse<RecommendationWhereString> { Item = recommendation });
        }

        [HttpGet, Route("api/coach-resource/recommendation")]
        public HttpResponseMessage ReadAll()
        {

           var recommendations = _recommendationService.ReadAll();

            return Request.CreateResponse(HttpStatusCode.OK, new ItemsResponse<RecommendationWhereString> { Items = recommendations });
        }

        [HttpPut, Route("api/coach-resource/recommendation/{Id:int}")]
        public HttpResponseMessage Update(int Id, RecommendationCreateRequest request)
        {
            if (request == null)
            {
                ModelState.AddModelError("empty object", "supply body");
            }
            if (!ModelState.IsValid)
            {
                return Request.CreateErrorResponse(HttpStatusCode.BadRequest, ModelState);
            }
            var recommendation = _recommendationService.UpdateById(Id, request);

            return Request.CreateResponse(HttpStatusCode.OK, new ItemResponse<RecommendationWhereString> { Item = recommendation });
        }
    }
}
