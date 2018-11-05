using Newtonsoft.Json;
using Sabio.Data.Providers;
using Sabio.Models.Domain;
using Sabio.Models.Domain.CoachResourceRecommendations;
using Sabio.Models.Requests.CoachRecommendation;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using static Sabio.Models.Domain.Recommendation;

namespace Sabio.Services
{
    public class CoachRecommendationService
    {
        IDataProvider _dataProvider;

        public CoachRecommendationService(IDataProvider dataProvider)
        {
            _dataProvider = dataProvider;
        }

        public List<UserCoachRecommendation> GetRecommendation(int id)
        {
            List<int> answers = new List<int>();
            _dataProvider.ExecuteCmd(
                "SurveyAnswers_Select_ByInstance",
                cmd =>
                {
                    cmd.AddWithValue("@InstanceId", id);
                },
                (reader, read) =>
                {
                    int answer = (int)reader["AnswerOptionId"];
                    answers.Add(answer);
                });

            Recommendation recommendation = new Recommendation();
            for (int i = 0; i < answers.Count; i++)
            {
                Type type = recommendation.GetType();
                string answer = "Answer" + answers[i].ToString();
                try
                {
                    PropertyInfo prop = type.GetProperty(answer);
                    prop.SetValue(recommendation, true, null);
                }
                catch
                {
                    continue;
                }
            }

            DataTable dataTable = new DataTable();
            dataTable.Columns.Add("ExpertiseId");
            DataRow dataRow = null;

            foreach (string x in recommendation.Expertise)
            {
                if (!x.Contains("Answer"))
                {
                    string expertise = x.ToString();
                    int expertiseEnum = (int)Enum.Parse(typeof(EXPERTISE_ID), expertise);

                    dataRow = dataTable.NewRow();
                    dataRow["ExpertiseId"] = expertiseEnum;
                    dataTable.Rows.Add(dataRow);
                }
            }

            List<UserCoachRecommendation> coaches = new List<UserCoachRecommendation>();
            _dataProvider.ExecuteCmd(
                "UserCoachExpertise_Select_ByExpertiseId",
                cmd =>
                {
                    SqlParameter param = cmd.AddWithValue("ExpertiseId", dataTable);
                    param.SqlDbType = SqlDbType.Structured;
                },
                (reader, read) =>
                {
                    UserCoachRecommendation coach = new UserCoachRecommendation()
                    {
                        CoachExpertiseId = (int)reader["Matched Expertises"],
                        UserId = (int)reader["UserId"]
                    };
                    coaches.Add(coach);
                });

            return coaches;
        }

        public List<UserCoachProfile> ReadCoachProfiles(CoachExpertiseTypeIdList coaches)
        {
            DataTable table = new DataTable();
            table.Columns.Add("ExpertiseTypeId");
            DataRow row = null;

            foreach (int id in coaches.ExpertiseList)
            {
                row = table.NewRow();
                row["ExpertiseTypeId"] = id;
                table.Rows.Add(row);
            }

            var coachProfileList = new List<UserCoachProfile>();
            _dataProvider.ExecuteCmd(
                "CoachRecommendation_GetProfileInfo_ByTableType",
                cmd =>
                {
                    SqlParameter param = cmd.AddWithValue("@ExpertiseTable", table);
                    param.SqlDbType = SqlDbType.Structured;
                },
                (reader, read) =>
                {
                    UserCoachProfile coach = new UserCoachProfile()
                    {
                        UserId = (int)reader["UserId"],
                        Bio = (string)reader["Bio"],
                        ImageUrl = (string)reader["ImageUrl"],
                        YearsInBusiness = (int)reader["YearsInBusiness"],
                        Email = (string)reader["Email"],
                        FirstName = (string)reader["FirstName"],
                        LastName = (string)reader["LastName"],
                        ExpertiseTypeId = (int)reader["ExpertiseTypeId"],
                        ExpertiseTitle = (string)reader["Expertise"]
                    };
                    coachProfileList.Add(coach);
                });

            return coachProfileList;
        }
        
        public int GetAssessmentInstanceIdByUserId(int id)
        {
            int instanceId = 0;
            _dataProvider.ExecuteCmd(
                "Users_Select_AssessmentInstanceId_ByUserId",
                cmd =>
                {
                    cmd.AddWithValue("Id", id);
                },
                (reader, read) =>
                {
                    instanceId = (int)reader["SurveyInstanceId"];
                });
            return instanceId;
        }

        public ResourceCoachRecommendations GetResourceGroupsCoachIdByInstanceId(int id)
        {

            ResourceCoachRecommendations resourceCoachRecommendations = new ResourceCoachRecommendations();
            List<CoachesTypeIds> Coaches = new List<CoachesTypeIds>();
            List<ResourceGroups> Resources = new List<ResourceGroups>();


            _dataProvider.ExecuteCmd("CoachExpertise_ResourceProvider_Recommendation_V2",
                cmd =>
                {
                    cmd.AddWithValue("@InstId", id);
                },
                (reader, read) =>
                {
                    if(reader["CoachExpertiseTypeId"] != DBNull.Value)
                    {
                        CoachesTypeIds coach = new CoachesTypeIds()
                        {
                            Description = reader["Description"] as string,
                            TypeIds = (int)reader["CoachExpertiseTypeId"]
                        };
                        Coaches.Add(coach);
                    }
                    else if (reader["ResourceProvider"] != DBNull.Value)
                    {
                        string resourceGroup = (string)reader["ResourceProvider"];
                        List<int> result = JsonConvert.DeserializeObject<List<int>>(resourceGroup);
                        ResourceGroups resource = new ResourceGroups()
                        {
                            Description = (string)reader["Description"],
                            Groups = result
                        };
                        Resources.Add(resource);
                    }
                });
            resourceCoachRecommendations.Coaches = Coaches;
            resourceCoachRecommendations.Resources = Resources;
            return resourceCoachRecommendations;

        }

        public CoachExpertiseResourceProvider GetRecommendationsByInstanceId(int Id)
        {
            var recommendations = new CoachExpertiseResourceProvider();
            recommendations.CoachExpertiseTypeId = new List<int>();
            recommendations.ResourceProviderId = new List<int>();

            _dataProvider.ExecuteCmd("CoachExpertise_ResourceProvider_Recommendation",
                send => { send.AddWithValue("@InstId", Id); },
                (read, val) =>
                {
                    if (read["CoachExpertiseTypeId"] != DBNull.Value)
                    {
                        recommendations.CoachExpertiseTypeId.Add((int)read["CoachExpertiseTypeId"]);
                    }
                    if (read["ResourceProviderId"] != DBNull.Value)
                    {
                        recommendations.ResourceProviderId.Add((int)read["ResourceProviderId"]);
                    }
                });

            return recommendations;
        }
    }
}
