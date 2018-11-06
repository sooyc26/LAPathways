using Sabio.Models.Requests.Users;
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
    public class UsersUserTypeController : ApiController
    {
        readonly UsersUserTypeService UserService;

        public UsersUserTypeController(UsersUserTypeService usersUserTypesService)
        {
            UserService = usersUserTypesService;
        }


        [HttpGet, Route("api/usersUserTypes/{Index:int}/{Size:int}")]
        public HttpResponseMessage UserTypesReadAll_Paged(int Index,int Size)
        {

            if (Size == 0)
            {
                ModelState.AddModelError("empty object", "supply body");
            }
            List<UsersUserType> users = UserService.UserTypesReadAll_Paged(Index, Size);

            return Request.CreateResponse(HttpStatusCode.OK, new ItemsResponse<UsersUserType>
            {
                Items = users
            });
        }

        [HttpGet, Route("api/usersUserTypes/{id:int}")]
        public HttpResponseMessage UserTypeGetById(int id)
        {
            if (id == 0)
            {
                ModelState.AddModelError("invalid id", "please input a valid id");
            }
            UsersUserType user = UserService.UserTypeReadById(id);
            return Request.CreateResponse(HttpStatusCode.OK, new ItemResponse<UsersUserType>
            {
                Item = user
            });
        }

    }
}
