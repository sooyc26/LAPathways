using Sabio.Data.Providers;
using Sabio.Models.Domain;
using Sabio.Models.Requests.Milestones;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sabio.Services
{
    public class MilestoneService
    {
        IDataProvider _dataProvider;

        public MilestoneService(IDataProvider dataProvider)
        {
            _dataProvider = dataProvider;
        }

        public void Delete(int Id)
        {
            _dataProvider.ExecuteNonQuery("Milestones_Delete",
                send => { send.AddWithValue("@Id", Id); },
                read => { });
        }

        public int Create(MilestoneCreateRequest Request)
        {
            int id = 0;

            _dataProvider.ExecuteNonQuery(
                "Milestones_Insert",
                send =>
                    {
                        send.AddWithValue("@Description", Request.Description);
                        send.AddWithValue("@CustomMentorId", Request.CustomMentorId);
                        send.AddWithValue("@IsCustomCreateByMentor", Request.IsCustomCreateByMentor);
                        send.AddWithValue("@Id", SqlDbType.Int).Direction = ParameterDirection.Output;

                    },
                reader =>
                    { id = (int)reader["@Id"].Value; }
             );

            return id;
        }

        public List<Milestone> ReadAll()
        {
            var milestones = new List<Milestone>();

            _dataProvider.ExecuteCmd("Milestones_Select_All",
                send => { },
                (read,var) => 
                {
                    var milestone = new Milestone()
                    {
                        Id = (int)read["Id"],
                        Description = (string)read["Description"],
                        IsCustomCreateByMentor = (bool)read["IsCustomCreateByMentor"],
                        CustomMentorId = (int)read["CustomMentorId"],
                        DateCreated = (DateTime)read["DateCreated"],
                        DateModified = (DateTime)read["DateModified"],
                    };
                    milestones.Add(milestone);
                });

            return milestones;
        }

        public Milestone ReadById(int Id)
        {
            var milestone = new Milestone();

            _dataProvider.ExecuteCmd("Milestones_Select_ById",
                send => { send.AddWithValue("@Id", Id); },
                (read, var) =>
                {
                    var readObject = new Milestone()
                    {
                        Id = (int)read["Id"],
                        CustomMentorId = (int)read["CustomMentorId"],
                        Description = (string)read["Description"],
                        IsCustomCreateByMentor = (bool)read["IsCustomCreateByMentor"],
                        DateCreated = (DateTime)read["DateCreated"],
                        DateModified = (DateTime)read["DateModified"],
                    };
                    milestone = readObject;
                });

            return milestone;
        }

        public Milestone ReadByMilestoneId(int Id)
        {
            var milestone = new Milestone();

            _dataProvider.ExecuteCmd("Milestones_Select_ByMilestoneId",
                send => { send.AddWithValue("@MilestoneId", Id); },
                (read, var) =>
                {
                    var readObject = new Milestone()
                    {
                        Id = (int)read["Id"],
                        CustomMentorId = (int)read["CustomMentorId"],
                        Description = (string)read["Description"],
                        IsCustomCreateByMentor = (bool)read["IsCustomCreateByMentor"],
                        DateCreated = (DateTime)read["DateCreated"],
                        DateModified = (DateTime)read["DateModified"],
                    };
                    milestone = readObject;
                });

            return milestone;
        }

        public int Update(int Id, MilestoneUpdateRequest Request)
        {

            _dataProvider.ExecuteNonQuery(
                "Milestones_Update",
                send =>
                {
                    send.AddWithValue("@Id", Id);
                    send.AddWithValue("@CustomMentorId", Request.CustomMentorId);
                    send.AddWithValue("@Description", Request.Description);
                    send.AddWithValue("@IsCustomCreateByMentor", Request.IsCustomCreateByMentor);
                },
                reader => { }
             );

            return Id;
        }
    }
}
