using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Services;
using System.Data;
using System.Data.SqlClient;
using System.Configuration;
using System.Web.Script.Serialization;
using System.Web.Script.Services;
using Amirhome.Models;

namespace Amirhome
{
    /// <summary>
    /// Summary description for WebService1
    /// </summary>
    [WebService(Namespace = "http://tempuri.org/")]
    [WebServiceBinding(ConformsTo = WsiProfiles.BasicProfile1_1)]
    [System.ComponentModel.ToolboxItem(false)]
    // To allow this Web Service to be called from script, using ASP.NET AJAX, uncomment the following line. 
    // [System.Web.Script.Services.ScriptService]
    public class WebService1 : System.Web.Services.WebService
    {

        [WebMethod]
        public string HelloWorld()
        {
            return "Hello World";
        }

        [WebMethod(Description="Returns occasion estates!")]
        [ScriptMethod(ResponseFormat = ResponseFormat.Json)]
        public string GetOccasions()
        {
            JavaScriptSerializer js = new JavaScriptSerializer();
            EstateManager _estateManager = new EstateManager();
            var occasions = _estateManager.getOccasions();
            var data = occasions.Select(E => new
            {
                ID = E.ID,
                Condition = E.Condition,
                Area = E.Area.ToString(),
                FirstPrice = E.Condition == "فروش" ? E.TotalPrice : E.PrepaymentPrice,
                SecondPrice = E.Condition == "فروش" ? E.PricePerMeter : E.MortgagePrice,
                Date = E.Date.Value.ToString(),
                ImageUrl = (E.Images.Count() == 0 ? "no-thumb.png" : E.Images.FirstOrDefault().url),
            });
            return js.Serialize(data);
        }

        [WebMethod]
        public DataSet GetOccasionEstate()
        {
            try
            {
                string constr = System.Configuration.ConfigurationManager.ConnectionStrings["WebServiceCnn"].ConnectionString;
                string query = "SELECT TOP (12) StateTypes.types AS Type, tbl_cities.name AS City, Districts.name AS " +
                          " District, States.ID, States.Region, States.Address, States.DetailedAddress, States.Date, " +
                          " States.Condition, States.Floors, States.Floor, " +
                          " States.Units, States.TotalPrice, States.MortgagePrice, States.PrepaymentPrice,  " +
                          " States.Loan, States.PricePerMeter, States.Age, States.Facing, States.Infosource, " +
                          " States.Description, States.Bedrooms, " +
                          " States.Bathrooms, States.Parking, States.Tells, " +
                          " States.Flooring, States.Width, States.Area, " +
                          " States.Usage, States.Changeable, States.CurrentStatus, Owners.Name, Owners.Mobile, " +
                          " Owners.Telephone, States.Cabinet, province.name AS Province, Images.url AS Image " +
                          " FROM States INNER JOIN " +
                          " tbl_cities ON States.City = tbl_cities.id INNER JOIN " +
                          " province ON States.Province = province.id INNER JOIN " +
                          " Districts ON States.District = Districts.ID INNER JOIN " +
                          " StateTypes ON States.StateType = StateTypes.ID INNER JOIN " +
                          " Owners ON States.OwnerID = Owners.ID LEFT JOIN " +
                          " Images ON States.ID = Images.StateID " +
                          " WHERE (States.OccasionFlag = 'true') AND (States.Approved = 'true') " +
                          " AND (States.Archived = 'false') AND images.url is null OR images.[Primary] = 1";

                using (SqlConnection con = new SqlConnection(constr))
                {
                    using (SqlCommand cmd = new SqlCommand(query, con))
                    using (SqlDataAdapter da = new SqlDataAdapter(cmd))
                    {
                        con.Open();
                        DataSet ds = new DataSet();
                        da.Fill(ds);
                        con.Close();
                        return ds;
                    }
                }
            }
            catch
            {
                return null;
            }
        }
        [WebMethod]
        public DataSet GetEstateById(int ID)
        {
            try
            {
                string constr = System.Configuration.ConfigurationManager.ConnectionStrings["WebServiceCnn"].ConnectionString;
                string query = "SELECT StateTypes.types AS Type, tbl_cities.name AS City, Districts.name AS " +
                            " District, States.ID, States.Region, States.Address, States.DetailedAddress, States.Date, " +
                            " States.Condition, States.Floors, States.Floor, " +
                            " States.Units, States.TotalPrice, States.MortgagePrice, States.PrepaymentPrice,  " +
                            " States.Loan, States.PricePerMeter, States.Age, States.Facing, States.Infosource, " +
                            " States.Description, States.Bedrooms, States.AgentID, " +
                            " States.Bathrooms, States.Parking, States.Tells, " +
                            " States.Flooring, States.Width, States.Area, " +
                            " States.Usage, States.Changeable, States.CurrentStatus, Owners.Name, Owners.Mobile, " +
                            " Owners.Telephone, States.Cabinet, province.name AS Province, Images.url AS Image" +
                            " FROM States INNER JOIN " +
                            " tbl_cities ON States.City = tbl_cities.id INNER JOIN " +
                            " province ON States.Province = province.id INNER JOIN " +
                            " Districts ON States.District = Districts.ID INNER JOIN " +
                            " StateTypes ON States.StateType = StateTypes.ID INNER JOIN " +
                            " Owners ON States.OwnerID = Owners.ID LEFT JOIN " +
                            " Images ON States.ID = Images.StateID " +
                            " WHERE (States.ID = @ID) AND images.url is null OR images.[Primary] = 1";


                using (SqlConnection con = new SqlConnection(constr))
                {
                    using (SqlCommand cmd = new SqlCommand(query, con))
                    using (SqlDataAdapter da = new SqlDataAdapter(cmd))
                    {
                        con.Open();
                        cmd.Parameters.Add("@ID", System.Data.SqlDbType.Int).Value = ID;
                        DataSet ds = new DataSet();
                        da.Fill(ds);
                        con.Close();
                        return ds;
                    }
                }
            }
            catch
            {
                return null;
            }
        }

        [WebMethod]
        public DataSet GetImage(int ID)
        {
            try
            {
                string constr = System.Configuration.ConfigurationManager.ConnectionStrings["WebServiceCnn"].ConnectionString;
                string query = "SELECT * FROM Images WHERE (Images.StateID = @ID)";


                using (SqlConnection con = new SqlConnection(constr))
                {
                    using (SqlCommand cmd = new SqlCommand(query, con))
                    using (SqlDataAdapter da = new SqlDataAdapter(cmd))
                    {
                        con.Open();
                        cmd.Parameters.Add("@ID", System.Data.SqlDbType.Int).Value = ID;
                        DataSet ds = new DataSet();
                        da.Fill(ds);
                        con.Close();
                        return ds;
                    }
                }
            }
            catch
            {
                return null;
            }
        }

        [WebMethod]
        public DataSet GetAgent(int ID)
        {
            try
            {
                string constr = System.Configuration.ConfigurationManager.ConnectionStrings["WebServiceCnn"].ConnectionString;
                string query = "SELECT Users.UserID, Users.FirstName, Users.LastName, Users.Email, UserProfile.PropertyValue " +
                               "FROM Users INNER JOIN UserProfile ON Users.UserID = UserProfile.UserID INNER JOIN " +
                               "ProfilePropertyDefinition ON UserProfile.PropertyDefinitionID = ProfilePropertyDefinition.PropertyDefinitionID " +
                               "WHERE (Users.UserID = @uid) AND (ProfilePropertyDefinition.PropertyName = 'Telephone')";

                using (SqlConnection con = new SqlConnection(constr))
                {
                    using (SqlCommand cmd = new SqlCommand(query, con))
                    using (SqlDataAdapter da = new SqlDataAdapter(cmd))
                    {
                        con.Open();
                        cmd.Parameters.Add("@uid", System.Data.SqlDbType.Int).Value = ID;
                        DataSet ds = new DataSet();
                        da.Fill(ds);
                        con.Close();
                        return ds;
                    }
                }
            }
            catch
            {
                return null;
            }
        }
    }
}
