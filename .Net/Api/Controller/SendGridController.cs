using LaPathways.Models.Domain;
using LaPathways.Models.Requests.Users;
using LaPathways.Models.Responses;
using LaPathways.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;

namespace LaPathways.Web.Controllers.Api
{
    [AllowAnonymous]
    [RoutePrefix("api/sendgrid")]
    public class SendGridController : ApiController
    {
        SendGridService _sendGridService;
        public SendGridController(SendGridService sendGridService)
        {
            _sendGridService = sendGridService;
        }

                {/* ...removed for brevity */}

        [HttpPut, Route("resetPassword")]
        public async Task<HttpResponseMessage> SendEmail(UserResetPassword request)
        {
            var result = await _sendGridService.SendEmail(request);

            return Request.CreateResponse(HttpStatusCode.OK, new ItemResponse<object>
            {
                Item = result
            });
        }

        [HttpGet, Route("expire-check/{key}")]
        public HttpResponseMessage CheckExpireDate(string key)
        {
            UserKeyExpireCheck result = _sendGridService.CheckExpireDate(key);

            if (result.ExpireBoolean == false)
            {
                return Request.CreateResponse(HttpStatusCode.BadRequest, result);
            }
            else

                return Request.CreateResponse(HttpStatusCode.OK, new ItemResponse<UserKeyExpireCheck>
                {
                    Item = result
                });
        }
    }
}
