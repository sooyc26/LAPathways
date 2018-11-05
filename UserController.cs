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


        [HttpGet, Route]
        public HttpResponseMessage ReadAll()
        {
            List<User> users = _userService.ReadAll();
            return Request.CreateResponse(HttpStatusCode.OK, new ItemsResponse<User>
            {
                Items = users
            });
        }

        [HttpGet, Route("{userId:int}")]
        public HttpResponseMessage ReadById(int userId)
        {
            User user = _userService.ReadById(userId);
            return Request.CreateResponse(HttpStatusCode.OK, new ItemResponse<User>
            {
                Item = user
            });
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

        [HttpPost, Route]
        public HttpResponseMessage Create(UserCreate user)
        {
            if (user == null)
            {
                ModelState.AddModelError("User", "User cannot be null");
            }
            if (!ModelState.IsValid)
            {
                return Request.CreateErrorResponse(HttpStatusCode.BadRequest, ModelState);
            }
            int Id = _userService.Create(user);
            return Request.CreateResponse(HttpStatusCode.OK, new ItemResponse<int>
            {
                Item = Id
            });
        }
        
        [HttpPut, Route("{id:int}"), AllowAnonymous]
        public HttpResponseMessage Update(int id, UserUpdate user)
        {
            if (user == null)
            {
                ModelState.AddModelError("User", "User cannot be null");
            }
            else if (id != user.Id)
            {
                ModelState.AddModelError("User", "Id does not match User.Id");
            }
            if (!ModelState.IsValid)
            {
                return Request.CreateErrorResponse(HttpStatusCode.BadRequest, ModelState);
            }
            _userService.UpdateById(id, user);
            
            return Request.CreateResponse(HttpStatusCode.OK, new SuccessResponse());

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

        [HttpDelete, Route("{id:int}")]
        public HttpResponseMessage Delete(int id)
        {
            _userService.DeleteById(id);
            return Request.CreateResponse(HttpStatusCode.OK, new SuccessResponse());
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

        [HttpGet, Route("logout")]
        public HttpResponseMessage Logout()
        {
            _auth.LogOut();
            return Request.CreateResponse(HttpStatusCode.OK, new SuccessResponse());
        }


        [HttpGet, Route("{pageIndex:int}/{pageSize:int}")]
        public HttpResponseMessage ReadAllPaged(int pageIndex, int pageSize)
        {
            List<UserPaged> userList = _userService.ReadAllPaged(pageIndex, pageSize);
            return Request.CreateResponse(HttpStatusCode.OK, new ItemsResponse<UserPaged>
            {
                Items = userList
            });
        }

        [HttpGet, Route("{id:int}/resources")]
        public HttpResponseMessage resourcesUser(int id)
        {
            List<ResourcesUser> resourcesList = _resourcesList.ReadAll();
            User user = _userService.ReadById(id);

            if (user.UserTypeId == (int)UserTypes.Coach_Mentor && (bool)resourcesList.Any(i => i.UserId == id))
            {                    
                return Request.CreateResponse(HttpStatusCode.OK, new SuccessResponse());
            }
            else if (user.UserTypeId == (int)UserTypes.Coach_Mentor)
            {
                string errMsg = "User (Mentor/Coach) Id (" + id + ") does not exist in the ResourcesUser table.";
                return Request.CreateResponse(HttpStatusCode.BadRequest, new ErrorResponse(errMsg));                
            }
                
            else
            {
                string errMsg = "User is not a mentor/coach " + id;
                return Request.CreateResponse(HttpStatusCode.BadRequest, new ErrorResponse(errMsg));
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
