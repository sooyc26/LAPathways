using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using LaPathways.Models.Domain;
using LaPathways.Models.Responses;
using LaPathways.Services;
using LaPathways.Web.Core.Enums;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Runtime.ExceptionServices;
using System.Threading.Tasks;
using System.Web;
using System.Web.Http;
using System.Web.Script.Serialization;
using LaPathways.Data.Providers;
using LaPathways.Models.Requests.EventBriteSearchCriteria;
using LaPathways.Models;


namespace LaPathways.Web.Controllers.Api
{

    public class EventBriteController : ApiController
    {
        readonly EventBriteService _eventBriteService;

        public EventBriteController(EventBriteService eventBriteService)
        {
            _eventBriteService = eventBriteService;
        }

        [HttpGet, Route("api/eventbrite")]
        public async Task<HttpResponseMessage> GetAllByIds()
        {
            List<EventBriteSearchCriteria> eventCriterias = _eventBriteService.GetAllByCriteria();

            List<dynamic> returnEventList = new List<dynamic>();

            foreach (EventBriteSearchCriteria criteria in eventCriterias)
            {
                if (criteria.TypeId == (int)EventBriteCriterias.UserId)
                {
                    var byUserId = await _eventBriteService.GetByEBuserId(criteria.CriteriaId);
                    foreach (dynamic user in byUserId)
                    {
                        returnEventList.Add(user);
                    }
                }
                else if (criteria.TypeId == (int)EventBriteCriterias.OrganizerId)
                {
                    var byOrgId = await _eventBriteService.GetEventByOrganizerId(criteria.CriteriaId);
                    foreach (dynamic org in byOrgId)
                    {
                        returnEventList.Add(org);
                    }
                }
            }

            return Request.CreateResponse(HttpStatusCode.OK, new ItemsResponse<object>
            {
                Items = returnEventList
            });
        }

        [HttpGet, Route("api/eventbrite/user-id/")]
        public async Task<HttpResponseMessage> GetByEBuserId(long id)
        {
            var eventsList = await _eventBriteService.GetByEBuserId(id);

            return Request.CreateResponse(HttpStatusCode.OK, new ItemsResponse<object>
            {
                Items = eventsList
            });
        }

        [HttpGet, Route("api/eventbrite/org-id")]
        public async Task<HttpResponseMessage> GetByOrganizerId(long id)
        {
            List<long> idList = new List<long>();
            List<dynamic> events = new List<dynamic>();

            List<dynamic> eventsList = await _eventBriteService.GetEventByOrganizerId(id);

            return Request.CreateResponse(HttpStatusCode.OK, new ItemsResponse<object>
            {
                Items = eventsList
            });
        }

        [HttpGet, Route("api/eventbrite/searchcriteria")]
        public HttpResponseMessage GetAllByCriteria()
        {
            List<EventBriteSearchCriteria> searchCriterias = _eventBriteService.GetAllByCriteria();

            if (!ModelState.IsValid)
            {
                return Request.CreateErrorResponse(HttpStatusCode.BadRequest, ModelState);
            }
            return Request.CreateResponse(HttpStatusCode.OK, new ItemsResponse<EventBriteSearchCriteria>
            {
                Items = searchCriterias
            });
        }

        [HttpGet, Route("api/eventbrite/searchcriteria/{id:int}")]
        public HttpResponseMessage ReadEventBriteSearchCriteriaById(int id)
        {
            EventBriteSearchCriteria answer = _eventBriteService.ReadEventBriteSearchCriteriaById(id);

            if (!ModelState.IsValid)
            {
                return Request.CreateErrorResponse(HttpStatusCode.BadRequest, ModelState);
            }
            return Request.CreateResponse(HttpStatusCode.OK, new ItemResponse<EventBriteSearchCriteria>
            {
                Item = answer
            });
        }


        [HttpPost, Route("api/eventbrite/searchcriteria")]
        public HttpResponseMessage Create(EventBriteSearchCriteriaCreate create)
        {
            if (create == null)
            {
                ModelState.AddModelError("empty object", "supply body");
            }
            if (!ModelState.IsValid)
            {
                return Request.CreateErrorResponse(HttpStatusCode.BadRequest, ModelState);
            }

            int newId = _eventBriteService.Create(create);

            return Request.CreateResponse(HttpStatusCode.OK, newId);
        }


        [HttpPut, Route("api/eventbrite/searchcriteria/{id:int}")]
        public HttpResponseMessage Update(EventBriteSearchCriteriaUpdate update, int id)
        {
            //EventBriteSearchCriteriaUpdate answer = _eventBriteService.Update(update, id);
            if (id == 0)
            {
                ModelState.AddModelError("Id or body missing", "enter valid id or body");
            }
            if (!ModelState.IsValid)
            {
                return Request.CreateErrorResponse(HttpStatusCode.BadRequest, ModelState);
            }
            _eventBriteService.Update(update, id);
            //return Request.CreateResponse(HttpStatusCode.OK, new ItemResponse<EventBriteSearchCriteriaUpdate> { Item = answer });
            return Request.CreateResponse(HttpStatusCode.OK, new SuccessResponse());
        }


        [HttpDelete, Route("api/eventbrite/searchcriteria/{Id:int}")]
        public object Del(int Id)
        {
            if (Id == 0)
            {
                ModelState.AddModelError("Id missing", "enter valid id");
            }
            if (!ModelState.IsValid)
            {
                return Request.CreateErrorResponse(HttpStatusCode.BadRequest, ModelState);
            }
            _eventBriteService.del(Id);
            return Request.CreateResponse(HttpStatusCode.OK, new SuccessResponse());
        }
    }
}
