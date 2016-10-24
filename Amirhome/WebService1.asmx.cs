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

        [WebMethod(Description = "Search among estates!")]
        [ScriptMethod(ResponseFormat = ResponseFormat.Json)]
        public string SearchEstate(string condition, string usage, string type = "", 
                                   int district_id = 0, int estate_serial = 0, int max_age = 0,
                                   int min_area = 0, int max_area = 0, long min_total_p = 0,
                                   long max_total_p = 0, long min_permeter_p = 0, long max_permeter_p = 0,
                                   long min_prepayment_p = 0, long max_prepayment = 0, long min_mortgage_p = 0,
                                   long max_mortgage_p = 0, int page = 0, string order = "date")
        {
            JavaScriptSerializer js = new JavaScriptSerializer();
            SearchParams _params = new SearchParams();

            #region Create SearchParams Object
            if (!string.IsNullOrEmpty(usage))
            {
                _params.EstateCondition = true;
                _params.EstateConditionValue = condition;
            }
            if (!string.IsNullOrEmpty(condition))
            {
                _params.EstateUsage = true;
                _params.EstateUsageValue = condition;
            }
            if (!string.IsNullOrEmpty(type))
            {
                _params.EstateType = true;
                _params.EstateTypeValue = condition;
            }
            if (!string.IsNullOrEmpty(type))
            {
                _params.EstateType = true;
                _params.EstateTypeValue = condition;
            }
            if (district_id != 0)
            {
                _params.EstateDistrict = true;
                _params.EstateDistrictValue = district_id;
            }
            if (estate_serial != 0)
            {
                _params.EstateSerial = true;
                _params.EstateSerialValue = estate_serial;
            }
            if (max_age != 0)
            {
                _params.EstateAge = true;
                _params.EstateAgeValue = max_age;
            }
            if (min_area != 0 || max_area != 0)
            {
                _params.Area = true;
                if (min_area != 0)
                    _params.AreaFrom = min_area;
                if (max_area != 0)
                    _params.AreaTo = max_area;
            }
            if (min_total_p != 0 || max_total_p != 0)
            {
                _params.TotalPrice = true;
                if (min_total_p != 0)
                    _params.TotalPriceFrom = min_total_p;
                if (max_total_p != 0)
                    _params.TotalPriceTo = max_total_p;
            }
            if (min_permeter_p != 0 || max_permeter_p != 0)
            {
                _params.PricePerMeter = true;
                if (min_permeter_p != 0)
                    _params.PricePerMeterFrom = min_permeter_p;
                if (max_permeter_p != 0)
                    _params.PricePerMeterTo = max_permeter_p;
            }
            if (min_prepayment_p != 0 || max_prepayment != 0)
            {
                _params.PrepaymentPrice = true;
                if (min_prepayment_p != 0)
                    _params.PrepaymentPriceFrom = min_prepayment_p;
                if (max_prepayment != 0)
                    _params.PrepaymentPriceTo = max_prepayment;
            }
            if (min_mortgage_p != 0 || max_mortgage_p != 0)
            {
                _params.MortagePrice = true;
                if (min_mortgage_p != 0)
                    _params.MortagePriceFrom = min_mortgage_p;
                if (max_mortgage_p != 0)
                    _params.MortagePriceTo = max_mortgage_p;
            }
            #endregion

            EstateManager _estateManager = new EstateManager();
            List<State> states = _estateManager.doSearch(_params, page * 10, order);
            var data = states.Select(o => new
            {
                ID = o.ID,
                Area = o.Area,
                FirstPrice = (o.TotalPrice == null || o.TotalPrice == 0) ? o.PrepaymentPrice : o.TotalPrice,
                SecondPrice = (o.PricePerMeter == null || o.PricePerMeter == 0) ? "اجاره " + (o.MortgagePrice > 0 ? o.MortgagePrice.ToString() : "ندارد") : "متری " + o.PricePerMeter.ToString(),
                Date = o.Date.ToString().Split(' ')[0],
                Condition = o.Condition,
                Dist = o.District1.name,
                Serial = o.Serial,
                ImageUrl = (o.Images.Count == 0 ? "no-thumb.png" : o.Images.FirstOrDefault().url),
                ImageCount = o.Images.Count
            });
            return js.Serialize(data);
        }

        [WebMethod(Description = "Get Estate by its ID!")]
        [ScriptMethod(ResponseFormat = ResponseFormat.Json)]
        public string GetEstateById(int ID)
        {
            EstateManager _estateManager = new EstateManager();
            var estate = _estateManager.getStateByID(ID);
            var date = new Object 
            {
                ID = estate.ID,
                Area = estate.Area,

            };
            return "";
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
       /* [WebMethod]
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
        }*/

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
