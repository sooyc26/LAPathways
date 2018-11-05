using Sabio.Data.Providers;
using Sabio.Models.Domain;
using Sabio.Models.Requests.Milestones;
using Sabio.Models.Requests.UserMilestones;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sabio.Services
{
    public class UsersMilestonesService
    {
        IDataProvider _dataProvider;
        MilestoneService _milestoneService;

        public UsersMilestonesService(IDataProvider dataProvider, MilestoneService milestone)
        {
            _dataProvider = dataProvider;
            _milestoneService = milestone;
        }

        public void Delete(int Id)
        {
            _dataProvider.ExecuteNonQuery("UsersMilestones_Delete",
                send => { send.AddWithValue("@Id", Id); },
                read => { });
        }

        public int Create(UserMilestoneCreateRequest Request)
        {
            int id = 0;

            _dataProvider.ExecuteNonQuery(
                "UsersMilestones_Insert",
                send =>
                {
                    send.AddWithValue("@UserId", Request.UserId);
                    send.AddWithValue("@MentorId", Request.MentorId);
                    send.AddWithValue("@MilestoneId", Request.MilestoneId);
                    send.AddWithValue("@IsCompleted", Request.IsCompleted);
                    send.AddWithValue("@Id", SqlDbType.Int).Direction = ParameterDirection.Output;

                },
                reader =>
                { id = (int)reader["@Id"].Value; }
             );

            return id;
        }

        public List<UsersMilestone> ReadAll()
        {
            var milestones = new List<UsersMilestone>();

            _dataProvider.ExecuteCmd("UsersMilestones_Select_All",
                send => { },
                (read, var) =>
                {
                    var milestone = new UsersMilestone()
                    {
                        Id = (int)read["Id"],
                        UserId = (int)read["UserId"],
                        MentorId = (int)read["MentorId"],
                        MilestoneId = (int)read["MilestoneId"],
                        IsCompleted = (bool)read["IsCompleted"],
                        DateCreated = (DateTime)read["DateCreated"],
                        DateModified = (DateTime)read["DateModified"],
                    };
                    milestone.Milestone = _milestoneService.ReadById((int)read["MilestoneId"]);
                    milestones.Add(milestone);
                });
            return milestones;
        }

        public List<UsersMilestone> MentorReadAllByMentorId(int Id)
        {
            var milestones = new List<UsersMilestone>();

            _dataProvider.ExecuteCmd("UsersMilestones_Select_ByMentorId",
                send => { send.AddWithValue("@MentorId", Id); },
                (read, var) =>
                {
                    var milestone = new UsersMilestone()
                    {
                        Id = (int)read["Id"],
                        UserId = (int)read["UserId"],
                        MentorId = (int)read["MentorId"],
                        FirstName = (string)read["FirstName"],
                        LastName = (string)read["LastName"],
                        MilestoneId = (int)read["MilestoneId"],
                        IsCompleted = (bool)read["IsCompleted"],
                        DateCreated = (DateTime)read["DateCreated"],
                        DateModified = (DateTime)read["DateModified"],
                    };
                    milestone.Milestone = _milestoneService.ReadById((int)read["MilestoneId"]);
                    milestones.Add(milestone);
                });
            return milestones;
        }


        public UsersMilestone ReadById(int Id)
        {
            var milestone = new UsersMilestone();

            _dataProvider.ExecuteCmd("UsersMilestones_Select_ById",
                send => { send.AddWithValue("@Id", Id); },
                (read, var) =>
                {
                    var readObject = new UsersMilestone()
                    {
                        Id = (int)read["Id"],
                        UserId = (int)read["UserId"],
                        MentorId = (int)read["MentorId"],
                        MilestoneId = (int)read["MilestoneId"],
                        Milestone = _milestoneService.ReadById((int)read["MilestoneId"]),
                        IsCompleted = (bool)read["IsCompleted"],
                        DateCreated = (DateTime)read["DateCreated"],
                        DateModified = (DateTime)read["DateModified"],
                    };
                    milestone = readObject;
                });

            return milestone;
        }

        public List<UsersMilestone> ReadByUserId(int Id)
        {
            var milestone = new List<UsersMilestone>();

            _dataProvider.ExecuteCmd("UsersMilestones_Select_ByUserId",
                send => { send.AddWithValue("@UserId", Id); },
                (read, var) =>
                {
                    var readObject = new UsersMilestone()
                    {
                        Id = (int)read["Id"],
                        UserId = (int)read["UserId"],
                        MentorId = (int)read["MentorId"],
                        MilestoneId = (int)read["MilestoneId"],
                        FirstName = (string)read["FirstName"],
                        LastName = (string)read["LastName"],
                        Description = (string)read["Description"],
                        Milestone = _milestoneService.ReadById((int)read["MilestoneId"]),
                        IsCompleted = (bool)read["IsCompleted"],
                        DateCreated = (DateTime)read["DateCreated"],
                        DateModified = (DateTime)read["DateModified"],
                    };
                    milestone.Add(readObject);
                });

            return milestone;
        }

        public int UpdateById(int Id, UserMilestoneUpdateRequest Request)
        {

            _dataProvider.ExecuteNonQuery(
                "UsersMilestones_Update_ById",
                send =>
                {
                    send.AddWithValue("@Id", Id);
                    send.AddWithValue("@UserId", Request.UserId);
                    send.AddWithValue("@MentorId", Request.MentorId);
                    send.AddWithValue("@MilestoneId", Request.MilestoneId);
                    send.AddWithValue("@IsCompleted", Request.IsCompleted);

                },
                reader =>
                { }
             );

            return Id;
        }

        public int UpdateIsCompletedById(int Id, UserMilestoneUpdateRequest Request)
        {
            _dataProvider.ExecuteNonQuery(
                "UserMilestone_Update_IsCompleted_ById",
                send =>
                {
                    send.AddWithValue("@Id", Id);
                    send.AddWithValue("@IsCompleted", Request.IsCompleted);
                },
                reader =>
                { }
             );

            return Id;
        }
    }
}
