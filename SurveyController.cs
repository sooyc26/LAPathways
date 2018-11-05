using Sabio.Models;
using Sabio.Services;
using Sabio.Models.Requests;
using Sabio.Models.Responses;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using Sabio.Data.Providers;
using Sabio.Models.Domain;
using System.Web;
using System.Security.Principal;
using Sabio.Services.Security;

namespace Sabio.Web.Controllers.Api
{
    
    public class SurveyController : ApiController
    {
        readonly IPrincipal _principal;
        readonly SurveyService _surveyService;
    
        public SurveyController(SurveyService surveyService, IPrincipal principal)
        {
            _surveyService = surveyService;
            _principal = principal;
        }
        //[AllowAnonymous]
        [HttpPost, Route("api/survey")]
        public object Create(SurveyCreateRequest request)
        {
            if (request == null)
            {
                ModelState.AddModelError("empty object", "supply body");
            }
            if (!ModelState.IsValid)
            {
                return Request.CreateErrorResponse(HttpStatusCode.BadRequest, ModelState);
            }
            //request.OwnerId = int.Parse(HttpContext.Current.User.Identity.Name);//using current user id
            request.OwnerId = _principal.Identity.GetCurrentUser().Id;
            int NewId = _surveyService.Create(request);

            return Request.CreateResponse(HttpStatusCode.OK, new ItemResponse<int>
            {
                Item = NewId
            });
        }
        [AllowAnonymous]
        [HttpGet, Route("api/survey")]
        public object GetAll()
        {
            List<Survey> users = new List<Survey>();

            if (!ModelState.IsValid)
            {
                return Request.CreateErrorResponse(HttpStatusCode.BadRequest, ModelState);
            }
            users = _surveyService.GetAll();
            return Request.CreateResponse(HttpStatusCode.OK, new ItemsResponse<Survey> { Items = users });
        }
        [AllowAnonymous]
        [HttpGet, Route("api/survey/{Id:int}")]
        public object GetUserByid(int Id)
        {
           Survey user = new Survey();
            if (Id == 0)
            {
                ModelState.AddModelError("Id missing", "enter valid id");
            }
            if (!ModelState.IsValid)
            {
                return Request.CreateErrorResponse(HttpStatusCode.BadRequest, ModelState);
            }
            user = _surveyService.GetUserByid(Id);
           return  Request.CreateResponse(HttpStatusCode.OK, new ItemResponse<Survey> { Item = user });
        }
        [AllowAnonymous]
        [HttpGet, Route("api/survey/{index:int}/{size:int}")]
        public object GetAllPaged(int index, int size)
        {
            List<SurveyPaginationRequest> users = new List<SurveyPaginationRequest>();

            if (size==0)
            {
                ModelState.AddModelError("size missing", "enter valid size");
            }
            
            if (!ModelState.IsValid)
            {
                return Request.CreateErrorResponse(HttpStatusCode.BadRequest, ModelState);
            }

            users = _surveyService.GetAllPaged(index,size);

            return Request.CreateResponse(HttpStatusCode.OK, new ItemsResponse<SurveyPaginationRequest> { Items = users });
        }

        //update by id
        [HttpPut, Route("api/survey/{Id:int}")]
        public object UpdateById(int Id, SurveyUpdateRequest request)
        {
            Survey user = new Survey();
            if (Id == 0 )

            {
                ModelState.AddModelError("Id or body missing", "enter valid id or body");
            }
            if (!ModelState.IsValid)
            {
                return Request.CreateErrorResponse(HttpStatusCode.BadRequest, ModelState);
            }
            user = _surveyService.UpdateById(Id, request);
            return Request.CreateResponse(HttpStatusCode.OK, new ItemResponse<Survey> { Item = user });
        }

        //delte by id
        [HttpDelete, Route("api/survey-data/{Id:int}")]
        public object Delete(int Id)
        {
            Survey user = new Survey();
            if (Id == 0)
            {
                ModelState.AddModelError("Id missing", "enter valid id");
            }
            if (!ModelState.IsValid)
            {
                return Request.CreateErrorResponse(HttpStatusCode.BadRequest, ModelState);
            }
            _surveyService.Delete(Id);
            return Request.CreateResponse(HttpStatusCode.OK, new SuccessResponse());
        }
    }
}
