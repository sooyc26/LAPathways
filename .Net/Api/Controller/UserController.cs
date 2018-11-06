using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using System.Web;
using System.Web.Http;
using Sabio.Data.Providers;
using Sabio.Models.Domain;
using Sabio.Models.Requests;
using Sabio.Models.Requests.Users;
using Sabio.Models.Responses;
using Sabio.Services;
using Sabio.Web.Core.Enums;
using Newtonsoft.Json;
using System.Web.Security;
using System.Web.Script.Serialization;
using System.IO;
using Sabio.Models.Requests.UserMentorMatch;
using System.Security.Principal;
using Sabio.Services.Security;

namespace Sabio.Web.Controllers.Api
{
    [RoutePrefix("api/user")]
    public class UserController : ApiController
    {
        readonly UserService _userService;
        readonly ResourcesUserService _resourcesList;
        readonly IPrincipal  _principal;
        readonly IAuthenticationService _auth;

        public UserController(UserService userService, ResourcesUserService resourcesList, IPrincipal principal, IAuthenticationService auth)
        {
            _userService = userService;
            _resourcesList = resourcesList;
            _principal = principal;
            _auth = auth;
        }

        [HttpGet, Route("type/{id:int}")]
        public HttpResponseMessage GetByTypeId(int id)
        {
            List<User> users = _userService.GetByTypeId(id);
            return Request.CreateResponse(HttpStatusCode.OK, new ItemsResponse<User>
            {
                Items = users
            });
        }

        [HttpPost, Route("user-mentor")]
        public HttpResponseMessage CreateUserMentorMatch(UsersMentorsCreateRequest request)
        {
            if (request == null)
            {
                ModelState.AddModelError("empty object", "supply body");
            }
            if (!ModelState.IsValid)
            {
                return Request.CreateErrorResponse(HttpStatusCode.BadRequest, ModelState);
            }
            var id = _userService.CreateUserMentorMatch(request);
            return Request.CreateResponse(HttpStatusCode.OK, id);
        }

        [HttpGet, Route("user-mentor/userid/{id:int}")]
        public HttpResponseMessage UsersMentors_GetByUserId(int id)
        {
            if (!ModelState.IsValid)
            {
                return Request.CreateErrorResponse(HttpStatusCode.BadRequest, ModelState);
            }
            var mentorIds = _userService.UsersMentors_GetByUserId(id);

            return Request.CreateResponse(HttpStatusCode.OK, new ItemsResponse<UserMentorMatch>
            {
                Items = mentorIds
            });
        }

        [HttpGet, Route("user-mentor/mentorid/{id:int}")]
        public HttpResponseMessage UsersMentors_GetByMentorId(int id)
        {
            if (!ModelState.IsValid)
            {
                return Request.CreateErrorResponse(HttpStatusCode.BadRequest, ModelState);
            }
            var userIds = _userService.UsersMentors_GetByMentorId(id);

            return Request.CreateResponse(HttpStatusCode.OK, new ItemsResponse<int>
            {
                Items = userIds
            });
        }

        [HttpPost, Route("login"), AllowAnonymous]
        public HttpResponseMessage Login(UserEmailPass user)
        {
            var loginUser = _userService.Login(user);

            if (loginUser != null)
            {
                if (user.Email == null || user.Password == null)
                {
                    string errMsg = "User email or password is not valid, please try again.";
                    return Request.CreateResponse(HttpStatusCode.BadRequest, new ErrorResponse(errMsg));
                }
                if (loginUser.UserTypeId == (int)UserTypes.Coach_Mentor && loginUser.IsMentorApproved == null)
                {
                    return Request.CreateErrorResponse(HttpStatusCode.Unauthorized, " STATUS: Pending Approval");
                }
                if (!loginUser.IsConfirmed)
                {
                    ModelState.AddModelError("User", "User is not confirmed!");
                    return Request.CreateErrorResponse(HttpStatusCode.BadRequest, ModelState);
                }

                var response = Request.CreateResponse(HttpStatusCode.OK, new SuccessResponse());

                var tempuserCookie = HttpContext.Current.Request.Cookies["tempuser"];
                if(tempuserCookie != null)
                {
                    var cookie = new CookieHeaderValue("tempuser", "0");
                    cookie.Expires = DateTimeOffset.Now.AddDays(-1);
                    cookie.Domain = Request.RequestUri.Host;
                    cookie.Path = "/";
                    response.Headers.AddCookies(new CookieHeaderValue[] { cookie });
                }
                return response;
            }
            else
            {
                return Request.CreateResponse(HttpStatusCode.BadRequest, ModelState);
            }
        }

        
        [HttpPost, Route("register"), AllowAnonymous]
        public HttpResponseMessage Register(UserRegister user)
        {

            List<string> email = _userService.CheckUsers(user.Email);
            if (!ModelState.IsValid)
            {
                return Request.CreateResponse(HttpStatusCode.BadRequest, ModelState);
            }
            else
            {
                int id = _userService.Register(user);
                var response = Request.CreateResponse(HttpStatusCode.OK, new ItemResponse<int>
                {
                    Item = id
                });
                var cookie = new CookieHeaderValue("tempuser", id.ToString());
                cookie.Expires = DateTimeOffset.Now.AddDays(3);
                cookie.Domain = Request.RequestUri.Host;
                cookie.Path = "/";
                response.Headers.AddCookies(new CookieHeaderValue[] { cookie });
                return response;
            }
        }
    }
}
