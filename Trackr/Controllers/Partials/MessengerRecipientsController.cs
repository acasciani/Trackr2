using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web;
using System.Web.Http;
using Trackr.Controllers.Models;
using Trackr.Controllers.Security;
using Trackr.Utils;
using TrackrModels;

namespace Trackr
{
    public class GetMatchingMessengerRecipients_DTO
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }
        public int UserID { get; set; }
    }

    public partial class MessengerRecipientsController : OpenAccessBaseApiController<GetMatchingMessengerRecipients_DTO, TrackrModels.ClubManagement>
    {

        public List<GetMatchingMessengerRecipients_DTO> GetMatchingEmailRecipients(string query, int clubID)
        {
            try
            {
                using (SqlConnection conn = new SqlConnection(ConfigurationManager.ConnectionStrings["UserManagementConnection"].ConnectionString))
                using (SqlCommand cmd = new SqlCommand("GetMatchingEmailRecipients", conn))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@ClubID", clubID);
                    cmd.Parameters.AddWithValue("@Query", query);

                    conn.Open();

                    using (IDataReader dr = cmd.ExecuteReader())
                    {
                        if (dr != null)
                        {
                            List<GetMatchingMessengerRecipients_DTO> results = new List<GetMatchingMessengerRecipients_DTO>();

                            while (dr.Read())
                            {
                                results.Add(new GetMatchingMessengerRecipients_DTO()
                                {
                                    UserID = Convert.ToInt32(dr["UserID"]),
                                    Email = dr["Email"].ToString(),
                                    FirstName = dr["FName"].ToString(),
                                    LastName = dr["LName"].ToString(),
                                });
                            }

                            return results;
                        }
                        else
                        {
                            return Enumerable.Empty<GetMatchingMessengerRecipients_DTO>().ToList();
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception("An Error Occurred getting matching email recipients.", ex);
            }
        }



        protected override HttpResponseMessage CreateResponse(HttpStatusCode httpStatusCode, GetMatchingMessengerRecipients_DTO entityToEmbed)
        {
            throw new NotImplementedException();
        }
    }
}