using Sabio.Data.Providers;
using Sabio.Models.Requests.Recommendation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Sabio.Models.Domain;
using System.Data;
using System.Data.SqlClient;

namespace Sabio.Services
{
    public class RecommendationService
    {
        IDataProvider _dataProvider;

        public RecommendationService (IDataProvider dataProvider)
        {
            _dataProvider = dataProvider;
        }

        public string Delete(int id)
        {
            _dataProvider.ExecuteNonQuery("Recommendations_Delete_ById",
            (parameters) =>
            {
                parameters.AddWithValue("@Id", id);
            },
            (reader) => { });
            return "deleted";
        }

        public int Create(RecommendationCreateRequest request)
        {
            int returnId = 0;

            _dataProvider.ExecuteNonQuery("Recommendations_Insert",
                send => {
                    send.AddWithValue("@CoachExpertiseTypeId", request.CoachExpertiseTypeId);
                    send.AddWithValue("@ResourceProviderId", request.ResourceProviderId);
                    send.AddWithValue("@WhereString", request.WhereString);

                    SqlParameter idParameter = new SqlParameter("@Id", SqlDbType.Int)
                    {
                        Direction = ParameterDirection.Output
                    };
                    send.Add(idParameter);
                },
                read => 
                {
                    returnId = (int)read["@Id"].Value;
                });
            return returnId;
        }


        public RecommendationWhereString ReadById(int id)
        {
            var recommendation = new RecommendationWhereString();
            _dataProvider.ExecuteCmd("Recommendations_Select_ById",
                parameters =>
                {
                    parameters.AddWithValue("@Id", id);
                },
                (reader, vari) =>
                {
                    recommendation.Id = (int)reader["Id"];
                    recommendation.WhereString = (string)reader["WhereString"];

                    object Description = reader["Description"];
                    if (Description != DBNull.Value)
                    {
                        recommendation.Description = (string)Description;
                    }
                    object CoachExpertiseTypeId = reader["CoachExpertiseTypeId"];
                    if (CoachExpertiseTypeId != DBNull.Value)
                    {
                        recommendation.CoachExpertiseTypeId = (int)CoachExpertiseTypeId;
                    }

                    object ResourceProviderId = reader["ResourceProviderId"];
                    if (ResourceProviderId != DBNull.Value)
                    {
                        recommendation.ResourceProviderId = (int)ResourceProviderId;
                    }
                }
            );
            return recommendation;
        }

        public List<RecommendationWhereString> ReadAll()
        {
            var recommendations = new List<RecommendationWhereString>();

            _dataProvider.ExecuteCmd("Recommendations_SelectAll",
                send => { },
                (read, var) =>
                {
                    var recommendation = new RecommendationWhereString()
                    {
                        Id = (int)read["Id"],
                        WhereString = (string)read["WhereString"],
                        //ResourceProvider = (string)read["ResourceProvider"],
                    };
                    object ResourceProvider = read["ResourceProvider"];
                    if (ResourceProvider != DBNull.Value)
                    {
                        recommendation.ResourceProvider = (string)ResourceProvider;
                    }
                    object Description = read["Description"];
                    if (Description != DBNull.Value)
                    {
                        recommendation.Description = (string)Description;
                    }

                    object CoachExpertiseTypeId = read["CoachExpertiseTypeId"];
                    if (CoachExpertiseTypeId != DBNull.Value)
                    {
                        recommendation.CoachExpertiseTypeId = (int)CoachExpertiseTypeId;
                    }
                    object ResourceProviderId = read["ResourceProviderId"];
                    if (ResourceProviderId != DBNull.Value)
                    {
                        recommendation.ResourceProviderId = (int)ResourceProviderId;
                    }
                    object CoachExpertiseTitle = read["Expertise"];
                    if (CoachExpertiseTitle != DBNull.Value)
                    {
                        recommendation.CoachExpertiseTitle = (string)CoachExpertiseTitle;
                    }
                    object ResourceCategoryTitle = read["Type"];
                    if (ResourceCategoryTitle != DBNull.Value)
                    {
                        recommendation.ResourceCategoryTitle = (string)ResourceCategoryTitle;
                    }
                    recommendations.Add(recommendation);
                });
            return recommendations;
        }

        public RecommendationWhereString UpdateById(int id, RecommendationCreateRequest update)
        {
            var updatedReco = new RecommendationWhereString();

            _dataProvider.ExecuteCmd("Recommendations_Update_ById",
                send => {
                    send.AddWithValue("@Id", id);
                    send.AddWithValue("@CoachExpertiseTypeId", update.CoachExpertiseTypeId);
                    send.AddWithValue("@ResourceProviderId", update.ResourceProviderId);
                    send.AddWithValue("@WhereString", update.WhereString);
                    send.AddWithValue("@Description", update.Description);

                },
                (reader, vari) =>
                {
                    updatedReco.Id = (int)reader["Id"];
                    updatedReco.WhereString = (string)reader["WhereString"];

                    object CoachExpertiseTypeId = reader["CoachExpertiseTypeId"];
                    if (CoachExpertiseTypeId != DBNull.Value)
                    {
                        updatedReco.CoachExpertiseTypeId = (int)CoachExpertiseTypeId;
                    }
                    object ResourceProviderId = reader["ResourceProviderId"];
                    if (ResourceProviderId != DBNull.Value)
                    {
                        updatedReco.ResourceProviderId = (int)ResourceProviderId;
                    }
                });
            return updatedReco;
        }
    }
}
