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

                       {/* ...removed for brevity */}


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
