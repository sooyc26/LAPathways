using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using Sabio.Models.Requests.Users;
using Sabio.Services;
using SendGrid;
using SendGrid.Helpers.Mail;
using System.Security.Cryptography;
using Sabio.Data.Providers;
using Sabio.Models.Domain;
using System.Net.Mail;

namespace Sabio.Services
{
    public class SendGridService 
    {
        private IDataProvider _dataProvider;

        public SendGridService(IDataProvider dataProvider)
        {
            _dataProvider = dataProvider;

        }


        public string GetUniqueKey(int maxSize)
        {
            char[] chars = new char[62];
            chars =
            "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ1234567890".ToCharArray();
            byte[] data = new byte[1];
            using (RNGCryptoServiceProvider crypto = new RNGCryptoServiceProvider())
            {
                crypto.GetNonZeroBytes(data);
                data = new byte[maxSize];
                crypto.GetNonZeroBytes(data);
            }
            StringBuilder result = new StringBuilder(maxSize);
            foreach (byte b in data)
            {
                result.Append(chars[b % (chars.Length)]);
            }
            return result.ToString();
        }

        public async Task<HttpStatusCode> SendAppointmentEmail(Appointment appointment)
        {
            User requester = null;
            User guest = null;

            _dataProvider.ExecuteCmd(
                "Users_Select_ById",
                cmd =>
                {
                    cmd.AddWithValue("@Id", appointment.RequesterId);
                },
                (reader, read) =>
                {
                    requester = new User()
                    {
                        Email = (string)reader["Email"],
                        FirstName = (string)reader["FirstName"],
                        LastName = (string)reader["LastName"]
                    };
                });

            _dataProvider.ExecuteCmd(
                "Users_Select_ById",
                cmd =>
                {
                    cmd.AddWithValue("@Id", appointment.GuestId);
                },
                (reader, read) =>
                {
                    guest = new User()
                    {
                        Email = (string)reader["Email"],
                        FirstName = (string)reader["FirstName"],
                        LastName = (string)reader["LastName"]
                    };
                });
            string htmlContent = @"
                                <div>
                                    <h2>Pathways Appointment Requested</h2>
                                    <p>" + requester.FirstName + " has invited you to " + appointment.Description + " at " + appointment.Location + " on " + appointment.AppointmentDate + @".
                                    <br><br> <span style=""color: #000000"" >Please Click the link below to verify appointment.<span> <br>
                                    <a href=""http://localhost:3000/appointments/user/confirm/" + appointment.Id + @"""><h3>Confirm Appointment</h3></a>
                                        <a href=""https://calendar.yahoo.com/?v=60&view=d&type=20
                                        &title=" + appointment.Description + @"
                                        &st=" + appointment.AppointmentDate.ToString("yyyyMMdd"+"T"+"HHmmss") + @"
                                        & et=" + appointment.AppointmentEndDate.ToString("yyyyMMdd"+"T"+"HHmmss") + @"
                                        &desc=" + appointment.Description + @"
                                        &in_loc=" + appointment.Location + @"
                                        "">add to Yahoo calendar</a>
                                    <a href=""http://www.google.com/calendar/event
                                        ?action = TEMPLATE
                                        & text = " + appointment.Description + @"
                                        &dates=[start-custom format='" + appointment.AppointmentDate + "']/[end-custom format='" + appointment.AppointmentEndDate + @"']
                                        &details=" + appointment.AppointmentEndDate + @"
                                        &location=" + appointment.Location + @"
                                        &trp=false
                                        &sprop=
                                        &sprop=name:""
                                        target=""_blank"" rel=""nofollow""></a>
                                </div>";

            var apiKey = ConfigurationManager.AppSettings["SendGridApiKey"];
            var client = new SendGridClient(apiKey);
            var from = new EmailAddress(requester.Email, requester.FirstName + " " + requester.LastName);
            var to = new EmailAddress("raymond.y.xie@gmail.com", guest.FirstName);
            var subject = "Pathways Appointment";
            var plainTextContent = appointment.Description;

            var msg = MailHelper.CreateSingleEmail(from, to, subject, plainTextContent, htmlContent);
            string CalendarContent = MeetingRequestString("LA Pathways", new List<string>() { guest.FirstName }, appointment.Description + " at " + appointment.Location, appointment.Description, appointment.Location, appointment.AppointmentDate, appointment.AppointmentEndDate);
            byte[] calendarBytes = Encoding.UTF8.GetBytes(CalendarContent.ToString());
            SendGrid.Helpers.Mail.Attachment calendarAttachment = new SendGrid.Helpers.Mail.Attachment
            {
                Filename = "AddToCalendar.ics",
                Content = Convert.ToBase64String(calendarBytes),
                Type = "text/calendar"
            };

            msg.AddAttachment(calendarAttachment);
            Response response = await client.SendEmailAsync(msg);

            return response.StatusCode;
        }

        private static string MeetingRequestString(string from, List<string> toUsers, string subject, string desc, string location, DateTime startTime, DateTime endTime, int? eventID = null, bool isCancel = false)
        {
            StringBuilder str = new StringBuilder();

            str.AppendLine("BEGIN:VCALENDAR");
            str.AppendLine("PRODID:-//Microsoft Corporation//Outlook 12.0 MIMEDIR//EN");
            str.AppendLine("VERSION:2.0");
            str.AppendLine(string.Format("METHOD:{0}", (isCancel ? "CANCEL" : "REQUEST")));
            str.AppendLine("BEGIN:VEVENT");

            str.AppendLine(string.Format("DTSTART:{0:yyyyMMddTHHmmssZ}", startTime.ToUniversalTime()));
            str.AppendLine(string.Format("DTSTAMP:{0:yyyyMMddTHHmmss}", DateTime.Now));
            str.AppendLine(string.Format("DTEND:{0:yyyyMMddTHHmmssZ}", endTime.ToUniversalTime()));
            str.AppendLine(string.Format("LOCATION: {0}", location));
            str.AppendLine(string.Format("UID:{0}", (eventID.HasValue ? "blablabla" + eventID : Guid.NewGuid().ToString())));
            str.AppendLine(string.Format("DESCRIPTION:{0}", desc.Replace("\n", "<br>")));
            str.AppendLine(string.Format("X-ALT-DESC;FMTTYPE=text/html:{0}", desc.Replace("\n", "<br>")));
            str.AppendLine(string.Format("SUMMARY:{0}", subject));

            str.AppendLine(string.Format("ORGANIZER;CN=\"{0}\":MAILTO:{1}", from, from));
            str.AppendLine(string.Format("ATTENDEE;CN=\"{0}\";RSVP=TRUE:mailto:{1}", string.Join(",", toUsers), string.Join(",", toUsers)));

            str.AppendLine("BEGIN:VALARM");
            str.AppendLine("TRIGGER:-PT15M");
            str.AppendLine("ACTION:DISPLAY");
            str.AppendLine("DESCRIPTION:Reminder");
            str.AppendLine("END:VALARM");
            str.AppendLine("END:VEVENT");
            str.AppendLine("END:VCALENDAR");

            return str.ToString();
        }

        public async Task<HttpStatusCode> SendEmail(UserResetPassword request)
        {
          
            UserResetPassword user = new UserResetPassword();

            int expireTime = int.Parse(ConfigurationManager.AppSettings["PasswordResetExpirationDate"]);

            DateTime expireDate = DateTime.UtcNow.AddHours(expireTime);

            string SecretPasswordKey = GetUniqueKey(64);


            _dataProvider.ExecuteNonQuery("Users_IssuePasswordResetKey",
                (parameter) =>
                {
                    parameter.AddWithValue("@Email", request.Email);
                    parameter.AddWithValue("@PasswordResetKey", SecretPasswordKey);
                    parameter.AddWithValue("@PasswordResetKeyExpirationDate", expireDate);
                },
                (reader) =>
                {
                });

            var apiKey = ConfigurationManager.AppSettings["SendGridApiKey"];
            var client = new SendGridClient(apiKey);
            var from = new EmailAddress("admin@gmail.com", "LA Pathway");
            var subject = "LA PATHWAY Reset Password";
            var to = new EmailAddress(request.Email, "");
            var plainTextContent = "Please click on the link below to reset your account password!";

            var resetAddress = ConfigurationManager.AppSettings["PasswordResetAddress"];
            string htmlContent = string.Format("<a href=\"{0}{1}\"> Click here to reset password</a>", resetAddress, SecretPasswordKey);
            var msg = MailHelper.CreateSingleEmail(from, to, subject, plainTextContent, htmlContent);
            Response response = await client.SendEmailAsync(msg);

            return response.StatusCode;

        }

        public UserKeyExpireCheck CheckExpireDate(string check)
        {
            UserKeyExpireCheck user = new UserKeyExpireCheck();

            DateTime key = new DateTime();
            
            _dataProvider.ExecuteCmd("Users_Select_ByPasswordKey",
                parameter =>
                {
                    parameter.AddWithValue("@PasswordResetKey", check);
                },
                (reader, var) =>
                {
                    key = (DateTime)reader["PasswordResetKeyExpirationDate"];
                    user.Id = (int)reader["Id"];
                });

            DateTime today = DateTime.UtcNow;
            DateTime expireDate = key;
            int result = DateTime.Compare(today, expireDate);

            if (result <= 0 )
            {
                user.ExpireBoolean = true;
                user.ReturnMessage = "key not expired ";
            }
            else
            {
                user.ExpireBoolean = false;
                user.ReturnMessage = "key expired ";
            }
            return user;
        }
    }
}
