using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net.Http;
using Sabio.Data.Providers;
using Sabio.Models.Domain;
using System.Configuration;
using System.Net;
using Newtonsoft.Json;
using Sabio.Models.Requests.EventBriteSearchCriteria;
using Sabio.Models;
using Sabio.Services.Cryptography;
using System.Security.Claims;
using System.Data;
using System.Data.SqlClient;
using System.Web;


namespace Sabio.Services
{
    public class EventBriteService
    {
        private IDataProvider _dataProvider;

        private HttpClient _httpClient;

        private string apiKey = ConfigurationManager.AppSettings["EventBriteKey"];

        public EventBriteService(IDataProvider dataProvider)
        {
            System.Net.ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12 | SecurityProtocolType.Tls11 | SecurityProtocolType.Tls;
            _httpClient = new HttpClient();
            _dataProvider = dataProvider;
        }

        public List<EventBriteSearchCriteria> GetAllByCriteria()
        {
            List<EventBriteSearchCriteria> eventCriterias = new List<EventBriteSearchCriteria>();

            _dataProvider.ExecuteCmd("EventBriteSearchCriteria_SelectAll",
                parameter => { },
            (reader, var) =>
            {
                EventBriteSearchCriteria eventCriteria = new EventBriteSearchCriteria()
                {
                    Id = (int)reader["Id"],
                    CriteriaId = (long)reader["CriteriaId"],
                    CriteriaText = (string)reader["CriteriaText"],
                    TypeId = (int)reader["TypeId"],
                    TypeTitle = (string)reader["TypeTitle"]
                };
                object resourceCheck = reader["ResourceId"];

                if (resourceCheck != DBNull.Value)
                {
                    eventCriteria.ResourceId = (int)reader["ResourceId"];
                }

                eventCriterias.Add(eventCriteria);

            });
            return eventCriterias;
        }

        public EventBriteSearchCriteria ReadEventBriteSearchCriteriaById(int Id)
        {

            EventBriteSearchCriteria EventBriteSearchCriteria = new EventBriteSearchCriteria();

            _dataProvider.ExecuteCmd(
                "EventBriteSearchCriteria_Select_ById",
                cmd =>
                {
                    cmd.AddWithValue("@Id", Id);
                },
                (reader, var) =>
                {
                    EventBriteSearchCriteria.Id = (int)reader["Id"];
                    EventBriteSearchCriteria.CriteriaId = (long)reader["CriteriaId"];
                    EventBriteSearchCriteria.CriteriaText = (string)reader["CriteriaText"];
                    EventBriteSearchCriteria.TypeId = (int)reader["TypeId"];
                    EventBriteSearchCriteria.TypeTitle = (string)reader["Type"];

                });

            return EventBriteSearchCriteria;
        }

        public int Create(EventBriteSearchCriteriaCreate create)
        {
            int Id = 0;
            _dataProvider.ExecuteNonQuery(
                "EventBriteSearchCriteria_Insert",
                cmd =>
                {
                    cmd.AddWithValue("@CriteriaId", create.CriteriaId);
                    cmd.AddWithValue("@CriteriaText", create.CriteriaText);
                    cmd.AddWithValue("@TypeId", create.TypeId);

                    //cmd.AddWithValue("@Id", SqlDbType.Int).Direction = ParameterDirection.Output;
                    SqlParameter idParameter = new SqlParameter("@Id", SqlDbType.Int)
                    {
                        Direction = ParameterDirection.Output
                    };
                    cmd.Add(idParameter);
                },
                reader =>
                {
                    Id = (int)reader["@Id"].Value;
                }
                );
            return Id;
        }

        public void Update(EventBriteSearchCriteriaUpdate update, int id)
        {
            //EventBriteSearchCriteriaUpdate answer = new EventBriteSearchCriteriaUpdate();
            _dataProvider.ExecuteNonQuery(
                "EventBriteSearchCriteria_Update_ById",
                cmd =>
                {
                    cmd.AddWithValue("@Id", id);
                    cmd.AddWithValue("@CriteriaId", update.CriteriaId);
                    cmd.AddWithValue("@CriteriaText", update.CriteriaText);
                    cmd.AddWithValue("@TypeId", update.TypeId);

                },
                reader =>{});
        }

        public string del(int id)
        {
            _dataProvider.ExecuteNonQuery("EventBriteSearchCriteria_Delete",
            (parameters) =>
            {
                parameters.AddWithValue("@Id", id);
            },
            (reader) => { });
            return "deleted";
        }

        public async Task<List<dynamic>> GetByEBuserId(long Id)
        {
            List<dynamic> returnEventList = new List<dynamic>();

            var result = await _httpClient.GetStringAsync("https://www.eventbriteapi.com/v3/users/" + Id + "/events/?token=LBPRSQ62QORDQCP45WDI");

            dynamic events = JsonConvert.DeserializeObject<dynamic>(result);

            foreach (dynamic e in events.events)
            {
                returnEventList.Add(e);
            }
            return returnEventList;
        }

        public async Task<List<dynamic>> GetEventByOrganizerId(long Id)
        {
            List<dynamic> returnEventList = new List<dynamic>();

            var result = await _httpClient.GetStringAsync("https://www.eventbriteapi.com/v3/organizers/" + Id + "/events/?token=" + apiKey);

            dynamic events = JsonConvert.DeserializeObject<dynamic>(result);

            foreach (dynamic e in events.events)
            {
                returnEventList.Add(e);
            }
            return returnEventList;
        }
    }
}
