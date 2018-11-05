using Sabio.Models.Domain;
using Sabio.Models.Requests.Users;
using Sabio.Models.Responses;
using Sabio.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;

namespace Sabio.Web.Controllers.Api
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

        [HttpPut, Route("appointment")]
        public async Task<HttpResponseMessage> SendAppointment(Appointment appointment)
        {
            var result = await _sendGridService.SendAppointmentEmail(appointment);
            return Request.CreateResponse(HttpStatusCode.OK, new ItemResponse<object>
            {
                Item = result
            });
        }

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
