using Sabio.Data.Providers;
using Sabio.Models.Requests.UserTypes;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sabio.Services
{
    public class UserTypesService
    {
        private IDataProvider _dataProvider;

        public UserTypesService(IDataProvider dataProvider)
        {
            _dataProvider = dataProvider;
        }

        
        public List<UserTypes> GetAll()
        {
            List<UserTypes> usersType = new List<UserTypes>();

            _dataProvider.ExecuteCmd("UserTypes_SelectAll",
                parameter => { },
                (reader, var)=>{
                    UserTypes userType = new UserTypes()
                    {
                        Id = (int)reader["Id"],
                        TypeName = (string)reader["TypeName"]
                    };
                    usersType.Add(userType);
            });
            return usersType;
        }

        public UserTypes GetById(int Id)
        {
            UserTypes userType = new UserTypes();
            _dataProvider.ExecuteCmd("UserTypes_Select_ById",
                parameter =>
                {
                    parameter.AddWithValue("@Id", Id);
                },
                (reader, var) => 
                {
                    userType.Id = (int)reader["Id"];
                    userType.TypeName = (string)reader["TypeName"];
                });

            return userType;
        }

        public int Insert(UserTypesInsertRequest request)
        {
            int id=0;
            _dataProvider.ExecuteNonQuery("UserTypes_Insert",
                parameter =>
                {
                    parameter.AddWithValue("@TypeName", request.TypeName);

                    SqlParameter idParameter = new SqlParameter("@Id", SqlDbType.Int)
                    {
                        Direction = ParameterDirection.Output
                    };
                    parameter.Add(idParameter);
                },
                (reader) =>
                {
                    id = (int)reader["@Id"].Value;
                });

            return id;
        }

        public UserTypes UpdateById(int Id, UserTypesUpdateRequest update)
        {
            UserTypes userTypes = new UserTypes();
            _dataProvider.ExecuteCmd("UserTypes_Update_ById",
                parameter =>
                {
                    parameter.AddWithValue("@Id", Id);
                    parameter.AddWithValue("@TypeName", update.TypeName);
                },
                (reader, var) =>
                {
                    userTypes.Id = Id;
                    userTypes.TypeName = update.TypeName;
                    userTypes.DateModified = (DateTime)reader["DateModified"];
                });
            return userTypes;
        }

        public string Delete(int Id)
        {
            _dataProvider.ExecuteNonQuery("UserTypes_Delete",
                parameter =>
                {
                    parameter.AddWithValue("@Id", Id);
                },
                reader => { }
                );
            return "deleted";
        }
    }
}
