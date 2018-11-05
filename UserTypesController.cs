
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web;
using System.Web.Http;
using Sabio.Models.Domain;
using Sabio.Models.Requests.UserTypes;
using Sabio.Models.Responses;
using Sabio.Services;

namespace Sabio.Web.Controllers.Api
{
    public class UserTypesController:ApiController
    {
        readonly UserTypesService userTypeService;

        public UserTypesController(UserTypesService dataProvider)
        {
            userTypeService = dataProvider;
        }

        [HttpGet, Route("api/user-types/")]
        public HttpResponseMessage GetAll()
        {
            List<UserTypes> usersType = userTypeService.GetAll();

            return Request.CreateResponse(HttpStatusCode.OK, new ItemsResponse<UserTypes>
            {
                Items = usersType
            });
        }

        [HttpGet, Route("api/user-types/{Id:int}")]
        public HttpResponseMessage GetById(int Id)
        {
            if (Id == 0)
            {
                ModelState.AddModelError("invalid Id", "Please enter valid Id");
            }
            UserTypes userType = userTypeService.GetById(Id);

            return Request.CreateResponse(HttpStatusCode.OK, new ItemResponse<UserTypes>
            {
                Item = userType
            });
        }

        [HttpPost, Route("api/user-types/")]
        public HttpResponseMessage Insert(UserTypesInsertRequest request)
        {
            if(request==null)
            {
                ModelState.AddModelError(" empty object", "Please enter valid name");
            }

            if (!ModelState.IsValid)
            {
                return Request.CreateErrorResponse(HttpStatusCode.BadRequest, ModelState);
            }

            int NewId = userTypeService.Insert(request);

            return Request.CreateResponse(HttpStatusCode.OK, NewId);
        }

        [HttpPut, Route("api/user-types/{Id:int}")]
        public HttpResponseMessage UpdateById(int Id, UserTypesUpdateRequest update)
        {
            if (Id == 0)
            {
                ModelState.AddModelError("invalid Id", "Please enter valid Id");
            }
            if (!ModelState.IsValid)
            {
                return Request.CreateErrorResponse(HttpStatusCode.BadRequest, ModelState);
            }
            UserTypes userType = userTypeService.UpdateById(Id, update);
            return Request.CreateResponse(HttpStatusCode.OK, new ItemResponse<UserTypes>
            {
                Item = userType
            });
        }

        [HttpDelete, Route("api/user-types/{Id:int}")]
        public HttpResponseMessage Delete(int Id)
        {
            if (Id == 0)
            {
                ModelState.AddModelError("invalid Id", "Please enter valid Id");
            }
            if (!ModelState.IsValid)
            {
                return Request.CreateErrorResponse(HttpStatusCode.BadRequest, ModelState);
            }
            userTypeService.Delete(Id);
            return Request.CreateResponse(HttpStatusCode.OK, new SuccessResponse());

        }

    }
}