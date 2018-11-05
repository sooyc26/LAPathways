using Sabio.Data.Providers;
using Sabio.Models.Requests.Users;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sabio.Services
{
    public class UsersUserTypeService
    {
        private IDataProvider _dataProvider;

        public UsersUserTypeService(IDataProvider dataProvider)
        {
            _dataProvider = dataProvider;
        }

        public UsersUserType UserTypeReadById(int Id)
        {

            UsersUserType user = null;
            _dataProvider.ExecuteCmd(
                "UsersUserType_Select_ById",
                cmd =>
                {
                    cmd.AddWithValue("@Id", Id);
                },
                (reader, var) =>
                {
                    user = new UsersUserType()
                    {
                        Id = (int)reader["Id"],
                        FirstName = (string)reader["FirstName"],
                        LastName = (string)reader["LastName"],
                        Email = (string)reader["Email"],
                        Password = (string)reader["Password"],
                        UserTypeId = (string)reader["TypeName"],
                        IsConfirmed = (bool)reader["IsConfirmed"],
                        ReferralSource = (string)reader["ReferralSource"]
                    };
                });
            return user;
        }

        public List<UsersUserType> UserTypesReadAll_Paged(int index, int size)
        {
            List<UsersUserType> users = new List<UsersUserType>();

            _dataProvider.ExecuteCmd(
               "UsersUserType_SelectAll_Paged",
               parameter =>
               {
                   parameter.AddWithValue("@PageIndex", index);
                   parameter.AddWithValue("@PageSize", size);
               },
               (reader, var) =>
               {
                   UsersUserType user = new UsersUserType()
                   {
                       Id = (int)reader["Id"],
                       FirstName = (string)reader["FirstName"],
                       LastName = (string)reader["LastName"],
                       Email = (string)reader["Email"],
                       Password = (string)reader["Password"],
                       UserTypeId = (string)reader["TypeName"],
                       IsConfirmed = (bool)reader["IsConfirmed"],
                       ReferralSource = (string)reader["ReferralSource"]
                   };
                   users.Add(user);
               });
            return users;
        }
    }



}
