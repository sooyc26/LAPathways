using Sabio.Data.Providers;
using Sabio.Models.Domain;
using Sabio.Models.Requests.BusinessVentures;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sabio.Services
{
    public class BusinessVenturesService
    {
        private IDataProvider _dataProvider;

        public BusinessVenturesService (IDataProvider dataProvider)
        {
            _dataProvider = dataProvider;
        }

        public int Create (BusinessVenturesCreateRequest request)
        {
            int id=0;

            _dataProvider.ExecuteNonQuery("BusinessVentures_Insert",
                send =>
            {
                send.AddWithValue("@UserId", request.UserId);
                send.AddWithValue("@StatusId", request.StatusId);
                send.AddWithValue("@Name", request.Name);
                send.AddWithValue("@IsMentored", request.IsMentored);
                send.AddWithValue("@AnnualBusinessIncome", request.AnnualBusinessIncome);
                send.AddWithValue("@YearsInBusiness", request.YearsInBusiness);
                send.AddWithValue("@Industry", request.Industry);

                SqlParameter idParameter = new SqlParameter("@Id", SqlDbType.Int)
                {
                    Direction = ParameterDirection.Output
                };
                send.Add(idParameter);
            },
                reader =>
                {
                    id = (int)reader["@Id"].Value;
                });
            return id;
        }

        public void Delete(int Id)
        {
            _dataProvider.ExecuteNonQuery("BusinessVentures_Delete",
                send => { send.AddWithValue("@Id", Id); },
                read => { });
        }


        public BusinessVentures ReadById(int Id)
        {
            var busiVent = new BusinessVentures();
            _dataProvider.ExecuteCmd("BusinessVentures_Select_ById",
                send =>
                {
                    send.AddWithValue("@Id", Id);
                },
                (read, var) =>
                {
                    busiVent.Id = (int)read["Id"];
                    busiVent.UserId = (int)read["UserId"];
                    busiVent.StatusId = (int)read["StatusId"];
                    busiVent.Name = (string)read["Name"];
                    busiVent.IsMentored = (bool)read["IsMentored"];
                    busiVent.AnnualBusinessIncome = (int)read["AnnualBusinessIncome"];
                    busiVent.YearsInBusiness = (int)read["YearsInBusiness"];
                    busiVent.Industry = (string)read["Industry"];

                });
            return busiVent;
        }

        public List<BusinessVentures> GetByUserId(int Id)
        {
            List<BusinessVentures> busiVentures = new List<BusinessVentures>();
            _dataProvider.ExecuteCmd("BusinessVentures_Select_ByUserId",
                send =>
                {
                    send.AddWithValue("@UserId", Id);
                },
                (read, var) =>
                {
                    BusinessVentures busiVent = new BusinessVentures();
                    busiVent.Id = (int)read["Id"];
                    busiVent.UserId = (int)read["UserId"];
                    busiVent.StatusId = (int)read["StatusId"];
                    busiVent.Name = (string)read["Name"];
                    busiVent.IsMentored = (bool)read["IsMentored"];
                    busiVent.AnnualBusinessIncome = (int)read["AnnualBusinessIncome"];
                    busiVent.YearsInBusiness = (int)read["YearsInBusiness"];
                    busiVent.Industry = (string)read["Industry"];

                    busiVentures.Add(busiVent);

                });
            return busiVentures;
        }


        public List<BusinessVentures> ReadAll()
        {
            var busiVents = new List<BusinessVentures>();

            _dataProvider.ExecuteCmd("BusinessVentures_SelectAll", 
                send=> { }, 
                (read, var) =>
                {
                    var busiVent = new BusinessVentures()
                    {
                        Id = (int)read["Id"],
                        UserId = (int)read["UserId"],
                        StatusId = (int)read["StatusId"],
                        Name = (string)read["Name"],
                        IsMentored = (bool)read["IsMentored"],
                        AnnualBusinessIncome = (int)read["AnnualBusinessIncome"],
                        YearsInBusiness = (int)read["YearsInBusiness"],
                        Industry = (string)read["Industry"]
                    };
                    busiVents.Add(busiVent);
                });
            return busiVents;
        }

        public int Update(BusinessVenturesUpdateRequest request, int Id)
        {
            _dataProvider.ExecuteNonQuery("BusinessVentures_Update",
                send =>
                {
                    send.AddWithValue("@Id", Id);
                    send.AddWithValue("@UserId", request.UserId);
                    send.AddWithValue("@StatusId", request.StatusId);
                    send.AddWithValue("@Name", request.Name);
                    send.AddWithValue("@IsMentored", request.IsMentored);
                    send.AddWithValue("@AnnualBusinessIncome", request.AnnualBusinessIncome);
                    send.AddWithValue("@YearsInBusiness", request.YearsInBusiness);
                    send.AddWithValue("@Industry", request.Industry);
                },
                reader => { }
                );
            return Id;
        }
    }
}
