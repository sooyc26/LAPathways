using Sabio.Data.Providers;
using Sabio.Models;
using Sabio.Models.Domain;
using Sabio.Models.Requests;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Web;


namespace Sabio.Services
{
    public class SurveyService
    {
        private IDataProvider _dataProvider;

        public SurveyService(IDataProvider dataProvider)
        {   
            _dataProvider = dataProvider;
        }

        public List<Survey> GetAll()
        {
            List<Survey> users = new List<Survey>();

            _dataProvider.ExecuteCmd("Survey_SelectAll",
                (parameters) => { },
                (reader, var) =>
                {
                    Survey user = new Survey()
                    {
                        Id = (int)reader["Id"],
                        Name = (string)reader["Name"],
                        Description = (string)reader["Description"],
                        StatusId = (int)reader["StatusId"],
                        OwnerId = (int)reader["OwnerId"],
                        TypeId = (int)reader["TypeId"],
                        DateCreated = (DateTime)reader["DateCreated"],
                        DateModified = (DateTime)reader["DateModified"]
                };
                    object version = reader["Version"];
                    if (version != DBNull.Value)
                    {
                        user.Version = (int)version;
                    }
                    object surveyParentId = reader["SurveyParentId"];
                    if (surveyParentId != DBNull.Value)
                    {
                        user.SurveyParentId = (int)surveyParentId;
                    }
                    users.Add(user);
                });
            return users;
        }

        public List<SurveyPaginationRequest> GetAllPaged(int Index, int Size)
        {
            List<SurveyPaginationRequest> users = new List<SurveyPaginationRequest>();

            _dataProvider.ExecuteCmd("Survey_SelectAll_Paged",
                (parameters) => {
                    parameters.AddWithValue("@PageIndex", Index);
                    parameters.AddWithValue("@PageSize", Size);

                    },
                (reader,var)=>
                {
                    SurveyPaginationRequest user = new SurveyPaginationRequest()
                    {
                        Id = (int)reader["Id"],
                        Name = (string)reader["Name"],
                        Description = (string)reader["Description"],
                        StatusId = (int)reader["StatusId"],
                        OwnerId = (int)reader["OwnerId"],
                        TypeId = (int)reader["TypeId"],
                        DateCreated = (DateTime)reader["DateCreated"],
                        DateModified = (DateTime)reader["DateModified"],
                        TotalCount = (int)reader["TotalCount"]
                    };
                    object version = reader["Version"];
                    if(version != DBNull.Value)
                    {
                        user.Version = (int)version;
                    }
                    object surveyParentId = reader["SurveyParentId"];
                    if( surveyParentId != DBNull.Value)
                    {
                        user.SurveyParentId = (int)surveyParentId;
                    }
                    
                    users.Add(user);
                });
            return users;

        }


        public int Create(SurveyCreateRequest request)
        {
            int idNumber = 0;

            if (request.SurveyParentId != null)
            {
                _dataProvider.ExecuteNonQuery(
                     "Survey_InsertVersion",
                     (parameters) =>
                     {
                         parameters.AddWithValue("@Name", request.Name);
                         parameters.AddWithValue("@Description", request.Description);
                         parameters.AddWithValue("@StatusId", request.StatusId);
                         parameters.AddWithValue("@OwnerId", request.OwnerId);
                         parameters.AddWithValue("@TypeId", request.TypeId);
                         parameters.AddWithValue("@Version", request.Version ?? (object)DBNull.Value);
                         parameters.AddWithValue("@SurveyParentId", request.SurveyParentId ?? (object)DBNull.Value);
                        

                         SqlParameter idParameter = new SqlParameter("@Id", SqlDbType.Int)
                         {
                             Direction = ParameterDirection.Output
                         };
                         parameters.Add(idParameter);

                     },
                     (reader) => {
                         idNumber = (int)reader["@Id"].Value;
                     }
                );
                return idNumber;
            } else { 
            _dataProvider.ExecuteNonQuery(
                 "Survey_Insert",
                 (parameters) =>
                 {
                     parameters.AddWithValue("@Name", request.Name);
                     parameters.AddWithValue("@Description", request.Description);
                     parameters.AddWithValue("@StatusId", request.StatusId);
                     parameters.AddWithValue("@OwnerId", request.OwnerId);
                     parameters.AddWithValue("@TypeId", request.TypeId);
                     parameters.AddWithValue("@Version", request.Version ?? (object)DBNull.Value);
                     parameters.AddWithValue("@SurveyParentId", request.SurveyParentId ?? (object)DBNull.Value); 

                     SqlParameter idParameter = new SqlParameter("@Id", SqlDbType.Int)
                     {
                         Direction = ParameterDirection.Output
                     };
                    parameters.Add(idParameter);
                     
                 },
                 (reader) => {
                     idNumber = (int)reader["@Id"].Value;
                 }
            );
                return idNumber;
            }
        }


        public Survey GetUserByid(int id)
        {
            Survey user = new Survey();
            //DanDataProvider dataProvider = new DanDataProvider();
            _dataProvider.ExecuteCmd("Survey_GetById",
                parameters =>
                {
                    parameters.AddWithValue("@Id", id);
                },
                (reader, vari) => {

                    user.Id = (int)reader["Id"];
                    user.Name = (string)reader["Name"];
                    user.Description = (string)reader["Description"];
                    user.StatusId = (int)reader["StatusId"];
                    user.OwnerId = (int)reader["OwnerId"];
                    user.TypeId = (int)reader["TypeId"];
                    user.DateCreated = (DateTime)reader["DateCreated"];
                    user.DateModified = (DateTime)reader["DateModified"];

                    object version = reader["Version"];
                    if (version != DBNull.Value)
                    {
                        user.Version = (int)version;
                    }
                    object surveyParentId = reader["SurveyParentId"];
                    if (surveyParentId != DBNull.Value)
                    {
                        user.SurveyParentId = (int)surveyParentId;
                    }

                }
            );
            return user;
        }

        public Survey UpdateById(int id, SurveyUpdateRequest update)
        {
            Survey user = new Survey();
            _dataProvider.ExecuteCmd("Survey_Update",
                (parameters) =>
                {
                    parameters.AddWithValue("@Id", id);
                    parameters.AddWithValue("@Name", update.Name);
                    parameters.AddWithValue("@Description", update.Description);
                    parameters.AddWithValue("@StatusId", update.StatusId);
                    parameters.AddWithValue("@OwnerId", update.OwnerId);
                    parameters.AddWithValue("@TypeId", update.TypeId);

                },
                (reader,vari) =>
                {
                    user.Id = id;
                    user.Name = update.Name;
                    user.Description = update.Description;
                    user.StatusId = update.StatusId;
                    user.OwnerId = update.OwnerId;
                    user.TypeId = update.TypeId;
                    user.DateCreated = (DateTime)reader["DateCreated"];
                    user.DateModified = (DateTime)reader["DateModified"];
                }
                );
            return user;
        }


        public string Delete(int id)
        {
            _dataProvider.ExecuteNonQuery("Survey_Delete",
            (parameters) =>
            {
                parameters.AddWithValue("@Id", id);
            },
            (reader) => { });
            return "deleted";
        }






    }
}