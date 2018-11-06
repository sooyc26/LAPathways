using Sabio.Data.Providers;
using Sabio.Models;
using Sabio.Models.Domain;
using Sabio.Models.Requests.UserMentorMatch;
using Sabio.Models.Requests.Users;
using Sabio.Services.Cryptography;
using Sabio.Services.Security;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Security.Claims;
using System.Security.Principal;
using System.Web;

namespace Sabio.Services
{
    public class UserService : IUserService
    {
        private IAuthenticationService _authenticationService;
        private ICryptographyService _cryptographyService;
        private IDataProvider _dataProvider;
        private IPrincipal _principal;
        private const int HASH_ITERATION_COUNT = 1;
        private const int RAND_LENGTH = 15;

        public UserService(IAuthenticationService authSerice, ICryptographyService cryptographyService, IDataProvider dataProvider, IPrincipal principal)
        {
            _authenticationService = authSerice;
            _dataProvider = dataProvider;
            _cryptographyService = cryptographyService;
            _principal = principal;
        }
        public List<User> GetByTypeId(int Id)
        {
            List<User> users = new List<User>();
            _dataProvider.ExecuteCmd(
                "Users_Select_ByTypeId",
                cmd =>
                {
                    cmd.AddWithValue("@UserTypeId", Id);
                },
                (reader, var) =>
                {
                    User user = new User()
                    {
                        Id = (int)reader["Id"],
                        FirstName = (string)reader["FirstName"],
                        LastName = (string)reader["LastName"],
                        UserTypeId = (int)reader["UserTypeId"]
                    };
                    users.Add(user);
                });
            return users;
        }

        public List<UserMentorMatch> UsersMentors_GetByUserId(int Id)
        {
            var users = new List<UserMentorMatch>();

            _dataProvider.ExecuteCmd("UsersMentors_Select_ByUserId", 
                parameter => {
                    parameter.AddWithValue("@UserId", Id);
                }, 
                (reader, var) => {
                    var user = new UserMentorMatch()
                    {
                        Id = (int)reader["Id"],
                        MentorId = (int)reader["MentorId"],
                        FirstName = (string)reader["FirstName"],
                        LastName = (string)reader["LastName"],
                        ImageUrl = (string)reader["ImageUrl"],
                        DateCreated = (DateTime)reader["DateCreated"],
                        YearsInBusiness = (int)reader["YearsInBusiness"]
                    };
                    users.Add(user);
                    });

            return users;
        }

        public List<int> UsersMentors_GetByMentorId(int Id)
        {
            var users = new List<int>();

            _dataProvider.ExecuteCmd("UsersMentors_Select_ByMentorId",
                parameter => {
                    parameter.AddWithValue("@MentorId", Id);
                },
                (reader, var) => {
                    var userId = (int)reader["UserId"];
                    users.Add(userId);
                });
            return users;
        }


        public int CreateUserMentorMatch(UsersMentorsCreateRequest request)
        {
            int id = 0;

            _dataProvider.ExecuteNonQuery("UsersMentors_Insert",
                parameter => {
                    parameter.AddWithValue("@UserId", _principal.Identity.GetCurrentUser().Id);
                    parameter.AddWithValue("@MentorId", request.MentorId);
                    parameter.AddWithValue("@Id", SqlDbType.Int).Direction = ParameterDirection.Output;
                },
                reader => {
                    id = (int)reader["@Id"].Value;
                });

            return id;
        }

        public UserLogin Login(UserEmailPass user)
        {
            string decodeEmail = HttpUtility.UrlDecode(user.Email);

            var userSalt = GetSalt(user.Email);
            string passwordHash = _cryptographyService.Hash(user.Password, userSalt.Salt, HASH_ITERATION_COUNT);

            UserLogin response = null;

            if (!String.IsNullOrEmpty(userSalt.Email) && userSalt.Password == passwordHash)
            {
                response = Get(user.Email, passwordHash);

                if(response != null)
                {
                    _authenticationService.LogIn(response);
                }

            }
            return response;

        }

        public List<string> CheckUsers (string user)
        {
            List<string> userList = new List<string>();
            _dataProvider.ExecuteCmd(
                "Users_CheckEmails",
                cmd =>
                {
                    cmd.AddWithValue("@Email", user);
                },
                (reader, read) =>
                {
                    string Email = (string)reader["Email"];
                    userList.Add(Email);
                });
                return userList;
        }
    }
}
