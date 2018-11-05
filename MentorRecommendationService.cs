using Sabio.Data.Providers;
using Sabio.Models.Domain;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sabio.Services
{
    public class MentorRecommendationService
    {
        IDataProvider _dataProvider;
        public MentorRecommendationService(IDataProvider dataProvider)
        {
            _dataProvider = dataProvider;
        }

        public List<UserMentorProfile> ReadCoachProfiles(int userId)
        {
            List<int> mentors = new List<int>();
            _dataProvider.ExecuteCmd(
                "MentorRecommendation_Select_ByUserId",
                cmd =>
                {
                    cmd.AddWithValue("@UserId", userId);
                },
                (reader, read) =>
                {
                    int id = (int)reader["MentorId"];
                    mentors.Add(id);
                });

            DataTable table = new DataTable();
            table.Columns.Add("Id");
            DataRow row = null;

            for (int i = 0; i < mentors.Count; i++)
            {
                row = table.NewRow();
                row["Id"] = mentors[i];
                table.Rows.Add(row);
            }
            List<UserMentorProfile> mentorProfileList = new List<UserMentorProfile>();

            _dataProvider.ExecuteCmd(
                "UserProfileInfos_Select_ById_Join",
                cmd =>
                {
                    SqlParameter param = cmd.AddWithValue("UserIdTable", table);
                    param.SqlDbType = SqlDbType.Structured;
                },
                (reader, read) =>
                {
                    UserMentorProfile mentor = new UserMentorProfile()
                    {
                        UserId = (int)reader["UserId"],
                        Bio = (string)reader["Bio"],
                        ImageUrl = (string)reader["ImageUrl"],
                        YearsInBusiness = (int)reader["YearsInBusiness"],
                        Email = (string)reader["Email"],
                        FirstName = (string)reader["FirstName"],
                        LastName = (string)reader["LastName"]
                    };
                    mentorProfileList.Add(mentor);
                });
            return mentorProfileList;
        }
    }
}
