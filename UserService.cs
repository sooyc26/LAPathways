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


        //public bool LogIn(string email, string password)
        //{
        //    bool isSuccessful = false;

        //    var salt = GetSalt(email);

        //    if (!String.IsNullOrEmpty(salt.Email))
        //    {
        //        string passwordHash = _cryptographyService.Hash(password, salt.Salt, HASH_ITERATION_COUNT);

        //        IUserAuthData response = Get(email, passwordHash);

        //        if (response != null)
        //        {
        //            _authenticationService.LogIn(response);
        //            isSuccessful = true;
        //        }

        //    }

        //    return isSuccessful;

        //}


        //public bool LogInTest(string email, string password, int id, string[] roles = null)
        //{
        //    bool isSuccessful = false;
        //    var testRoles = new[] { "User", "Super", "Content Manager" };

        //    var allRoles = roles == null ? testRoles : testRoles.Concat(roles);

        //    IUserAuthData response = new UserBase
        //    {
        //        Id = id
        //        ,
        //        Name = "FakeUser" + id.ToString()
        //        ,
        //        Roles = allRoles
        //    };

        //    Claim tenant = new Claim("Tenant", "Acme Corp");
        //    Claim fullName = new Claim("CustomClaim", "Sabio Bootcamp");

        //    //Login Supports multiple claims
        //    //and multiple roles a good an quick example to set up for 1 to many relationship
        //    _authenticationService.LogIn(response, new Claim[] { tenant, fullName });

        //    return isSuccessful;

        //}


        //public int Create(object userModel)
        //{
        //    int userId = 0;
        //    string salt;
        //    string passwordHash;

        //    string password = "Get from user model when you have a concreate class";

        //    salt = _cryptographyService.GenerateRandomString(RAND_LENGTH);
        //    passwordHash = _cryptographyService.Hash(password, salt, HASH_ITERATION_COUNT);

        //    //DB provider call to create user and get us a user id

        //    //be sure to store both salt and passwordHash
        //    //DO NOT STORE the original password value that the user passed us


        //    return userId;
        //}

        /// <summary>
        /// Gets the Data call to get a give user
        /// </summary>
        /// <param name="email"></param>
        /// <param name="passwordHash"></param>
        /// <returns></returns>
        private UserLogin Get(string email, string passwordHash)
        {
            UserLogin user = null;
            _dataProvider.ExecuteCmd(
                "Users_Login",
                cmd =>
                {
                    cmd.AddWithValue("@Email", email);
                    cmd.AddWithValue("@Password", passwordHash);
                },
                (reader, read) =>
                {
                    user = new UserLogin()
                    {
                        Id = (int)reader["Id"],
                        IsConfirmed = (bool)reader["IsConfirmed"],
                        UserTypeId = (int)reader["UserTypeId"]
                    };
                    object IsMentorApproved = reader["IsMentorApproved"];
                    if (IsMentorApproved != DBNull.Value)
                    {
                        user.IsMentorApproved = (bool)IsMentorApproved;
                    }
                });
            return user;
        }

        /// <summary>
        /// The Dataprovider call to get the Salt for User with the given UserName/Email
        /// </summary>
        /// <param name="email"></param>
        /// <returns></returns>
        private UserSalt GetSalt(string email)
        {

            UserSalt saltLogin = new UserSalt();
            _dataProvider.ExecuteCmd(
               "User_EmailValidation",
               cmd =>
               {
                   cmd.AddWithValue("@Email", email);
               },
               (reader, val) =>
               {
                   saltLogin.Password = (string)reader["Password"];
                   saltLogin.Email = (string)reader["Email"];
                   saltLogin.Salt = (string)reader["Salt"];
               }

            );
            return saltLogin;

            //DataProvider Call to get Salt
            //throw new NotImplementedException();
        }

        public List<User> ReadAll()
        {
            List<User> userList = new List<User>();
            _dataProvider.ExecuteCmd(
                "Users_SelectAll",
                cmd => { },
                (reader, map) =>
                {
                    User user = new User()
                    {
                        Id = (int)reader["Id"],
                        FirstName = (string)reader["FirstName"],
                        LastName = (string)reader["LastName"],
                        Email = (string)reader["Email"],
                        Password = (string)reader["Password"],
                        UserTypeId = (int)reader["UserTypeId"],
                        IsConfirmed = (bool)reader["IsConfirmed"],
                        ReferralSource = (string)reader["ReferralSource"], 
                        TypeName = (string)reader["TypeName"]
                    };
                    userList.Add(user);
                });
            return userList;
        }

        public User ReadById(int userId)
        {
            User user = null;
            _dataProvider.ExecuteCmd(
                "Users_Select_ById",
                cmd =>
                {
                    cmd.AddWithValue("@Id", userId);
                },
                (reader, var) =>
                {
                    user = new User()
                    {
                        Id = (int)reader["Id"],
                        FirstName = (string)reader["FirstName"],
                        LastName = (string)reader["LastName"],
                        Email = (string)reader["Email"],
                        Password = (string)reader["Password"],
                        UserTypeId = (int)reader["UserTypeId"],
                        IsConfirmed = (bool)reader["IsConfirmed"],
                        ReferralSource = (string)reader["ReferralSource"],
                        TypeName = (string)reader["TypeName"]
                    };
                });
            return user;
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

        public int Create(UserCreate user)
        {
            int Id = 0;
            _dataProvider.ExecuteNonQuery(
                "Users_Insert",
                cmd =>
                {
                    cmd.AddWithValue("@FirstName", user.FirstName);
                    cmd.AddWithValue("@LastName", user.LastName);
                    cmd.AddWithValue("@Email", user.Email);
                    cmd.AddWithValue("@Password", user.Password);
                    cmd.AddWithValue("@UserTypeId", user.UserTypeId);
                    cmd.AddWithValue("@IsConfirmed", user.IsConfirmed);
                    cmd.AddWithValue("@ReferralSource", user.ReferralSource);
                    cmd.AddWithValue("@IsMentorApproved", user.IsMentorApproved ?? (object)DBNull.Value);
                    cmd.AddWithValue("@Id", SqlDbType.Int).Direction = ParameterDirection.Output;
                    
                },
                reader => 
                    {
                    Id = (int)reader["@Id"].Value;
                    }
                );
            return Id;
        }

        public void UpdateById(int id, UserUpdate user)
        {
            string salt;
            string passwordHash;
            string password = user.Password;

            salt = _cryptographyService.GenerateRandomString(RAND_LENGTH);
            passwordHash = _cryptographyService.Hash(password, salt, HASH_ITERATION_COUNT);

            _dataProvider.ExecuteNonQuery(
                "Users_Update_ById",
                cmd =>
                {
                    cmd.AddWithValue("@Id", id);
                    cmd.AddWithValue("@FirstName", user.FirstName);
                    cmd.AddWithValue("@LastName", user.LastName);
                    cmd.AddWithValue("@Email", user.Email);
                    cmd.AddWithValue("@Password", passwordHash);
                    cmd.AddWithValue("@Salt", salt);
                    cmd.AddWithValue("@IsConfirmed", 1);
                    cmd.AddWithValue("@UserTypeId", 1);
                    cmd.AddWithValue("@ReferralSource", user.ReferralSource);
                },
                reader => { }
                );

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

        public void DeleteById(int id)
        {
            _dataProvider.ExecuteNonQuery(
                "Users_Delete",
                cmd =>
                {
                    cmd.AddWithValue("@Id", id);
                },
                reader => { }
                );
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

        public List<UserPaged> ReadAllPaged(int pageIndex, int pageSize)
        {
            List<UserPaged> userList = new List<UserPaged>();
            _dataProvider.ExecuteCmd(
                "Users_SelectAll_Paged",
                cmd =>
                {
                    cmd.AddWithValue("@PageIndex", pageIndex);
                    cmd.AddWithValue("@PageSize", pageSize);
                },
                (reader, val) =>
                {
                    UserPaged user = new UserPaged()
                    {
                        Id = (int)reader["Id"],
                        FirstName = (string)reader["FirstName"],
                        LastName = (string)reader["LastName"],
                        Email = (string)reader["Email"],
                        Password = (string)reader["Password"],
                        UserTypeId = (int)reader["UserTypeId"],
                        IsConfirmed = (bool)reader["IsConfirmed"],
                        ReferralSource = (string)reader["ReferralSource"],
                        TotalCount = (int)reader["TotalCount"]
                    };
                    userList.Add(user);
                });
            return userList;
        }

        public int Register(UserRegister user)
        {
            int id = 0;

            _dataProvider.ExecuteNonQuery(
                "Users_Register",
                cmd =>
                {
                    cmd.AddWithValue("@FirstName", user.FirstName);
                    cmd.AddWithValue("@LastName", user.LastName);
                    cmd.AddWithValue("@Email", user.Email);
                    cmd.AddWithValue("@Password", user.Password);
                    cmd.AddWithValue("@ReferralSource", user.ReferralSource);
                    cmd.AddWithValue("@UserTypeId", 1);
                    cmd.AddWithValue("@IsConfirmed", false);
                    cmd.AddWithValue("@IsMentorApproved", false);
                    cmd.AddWithValue("@Id", SqlDbType.Int).Direction = ParameterDirection.Output;
                },
                reader => 
                {
                    id = (int)reader["@Id"].Value;
                });
            return id;
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
