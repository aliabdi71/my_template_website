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
        public string SearchEstate(int _condition, int _usage, int _type = 0, 
                                   int district_id = 0, int estate_serial = 0, int max_age = 0,
                                   int min_area = 0, int max_area = 0, long min_total_p = 0,
                                   long max_total_p = 0, long min_permeter_p = 0, long max_permeter_p = 0,
                                   long min_prepayment_p = 0, long max_prepayment = 0, long min_mortgage_p = 0,
                                   long max_mortgage_p = 0, int page = 0, string order = "date")
        {
            JavaScriptSerializer js = new JavaScriptSerializer();
            SearchParams _params = new SearchParams();
            string condition = string.Empty, 
                   usage = string.Empty,
                   type = string.Empty;
            switch (_condition)
            {
                case 1: condition = "فروش"; break;
                case 2: condition = "رهن"; break;
                case 3: condition = "اجاره"; break;
                default: condition = "فروش"; break;
            }
            switch (_usage)
            {
                case 1: usage = "مسکونی"; break;
                case 2: usage = "کلنگی و مشارکتی"; break;
                case 3: usage = "تجاری و اداری"; break;
                default: usage = "مسکونی"; break;
            }

            if (_type != 0)
                type = _type.ToString();

            #region Create SearchParams Object
            if (!string.IsNullOrEmpty(condition))
            {
                _params.EstateCondition = true;
                _params.EstateConditionValue = condition;
            }
            if (!string.IsNullOrEmpty(usage))
            {
                _params.EstateUsage = true;
                _params.EstateUsageValue = usage;
            }
            if (!string.IsNullOrEmpty(type))
            {
                _params.EstateType = true;
                _params.EstateTypeValue = type;
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
                else
                    _params.AreaTo = 1000000;
            }
            if (min_total_p != 0 || max_total_p != 0)
            {
                _params.TotalPrice = true;
                if (min_total_p != 0)
                    _params.TotalPriceFrom = min_total_p;
                if (max_total_p != 0)
                    _params.TotalPriceTo = max_total_p;
                else
                    _params.TotalPriceTo = 100000000000;
            }
            if (min_permeter_p != 0 || max_permeter_p != 0)
            {
                _params.PricePerMeter = true;
                if (min_permeter_p != 0)
                    _params.PricePerMeterFrom = min_permeter_p;
                if (max_permeter_p != 0)
                    _params.PricePerMeterTo = max_permeter_p;
                else
                    _params.PricePerMeterTo = 1000000000;
            }
            if (min_prepayment_p != 0 || max_prepayment != 0)
            {
                _params.PrepaymentPrice = true;
                if (min_prepayment_p != 0)
                    _params.PrepaymentPriceFrom = min_prepayment_p;
                if (max_prepayment != 0)
                    _params.PrepaymentPriceTo = max_prepayment;
                else
                    _params.PrepaymentPriceTo = 1000000000;
            }
            if (min_mortgage_p != 0 || max_mortgage_p != 0)
            {
                _params.MortagePrice = true;
                if (min_mortgage_p != 0)
                    _params.MortagePriceFrom = min_mortgage_p;
                if (max_mortgage_p != 0)
                    _params.MortagePriceTo = max_mortgage_p;
                else
                    _params.MortagePriceTo = 1000000000;
            }
            #endregion

            EstateManager _estateManager = new EstateManager();
            List<State> states = _estateManager.doSearch(_params, page * 10, order);
            var data = states.Select(o => new
            {
                ID = o.ID,
                Area = o.Area,
                FirstPrice = (o.TotalPrice == null || o.TotalPrice == 0) ? o.PrepaymentPrice : o.TotalPrice,
                SecondPrice = (o.PricePerMeter == null || o.PricePerMeter == 0) ? o.MortgagePrice  : o.PricePerMeter,
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
            AgentManager _agentManager = new AgentManager();
            var agent = _agentManager.getAgentById(estate.AgentID.Value);
            var data = new
            {
                ID = estate.ID,
                Area = estate.Area,
                FirstPrice = (estate.TotalPrice == null || estate.TotalPrice == 0) ? estate.PrepaymentPrice : estate.TotalPrice,
                SecondPrice = (estate.PricePerMeter == null || estate.PricePerMeter == 0) ? estate.MortgagePrice : estate.PricePerMeter,
                Date = estate.Date.ToString().Split(' ')[0],
                Condition = estate.Condition,
                Dist = estate.District1.name,
                Serial = estate.Serial.ToString(),
                Address1 = estate.Address,
                Address2 = estate.DetailedAddress,
                Usage = estate.Usage,
                Bedrooms = estate.Bedrooms,
                Type = estate.StateType1.types,
                Age = estate.Age.ToString(),
                Floor = estate.Floor,
                AgentName = agent.Name,
                AgentPhone = agent.Phone,
                TotalFloors = estate.Floors,
                Bathroom = estate.Bathrooms,
                Units = estate.Units,
                Cabinet = estate.Cabinet,
                Parking = estate.Parking,
                PhoneLines = estate.Tells,
                Facing = estate.Facing,
                Status = estate.CurrentStatus,
                Position = estate.StatePosition,
                Flooring = estate.Floors,
                ExtraInfo = estate.Description + " " + estate.Occasion,
                ImgCount = estate.Images.Count,
                ImgUrls = estate.Images.Count > 0 ? (estate.Images.Select(i => i.url).ToArray()) : null,
                StreetViewImg = estate.StreetViews.Count > 0 ? estate.StreetViews.First().url : null,
                PlanImg = estate.Plans.Count > 0 ? estate.Plans.First().url : null,
                GoogleMapLat = estate.GoogleMaps.Count <= 0 ? "35.688045" : estate.GoogleMaps.First().latitude,
                GoogleMapLong = estate.GoogleMaps.Count <= 0 ? "51.392884" : estate.GoogleMaps.First().longitude,
                Features = estate.Features.Select(f => f.Item).ToArray()
            };
            JavaScriptSerializer js = new JavaScriptSerializer();
            return js.Serialize(data);
        }

        [WebMethod(Description = "Get Addvertise by its ID!")]
        [ScriptMethod(ResponseFormat = ResponseFormat.Json)]
        public string GetAdvertiseByID(int ID)
        {
            AdvertiseManager _adverManager = new AdvertiseManager();
            FreeAdvertise obj = _adverManager.getAdvertiseById(ID);
            var data = new
            {
                Title = obj.title,
                Title2 = obj.title2,
                City = obj.city,
                Condition = obj.condition,
                District = obj.district,
                Email = obj.email,
                Phone = obj.phone,
                Images = (string.IsNullOrEmpty(obj.image) ? null : obj.image.Split(';')),
                Area = obj.area,
                Date = getAdverDate(obj.create_date),
                FirstPrice = (obj.condition.Equals("فروش")) ? obj.price_total : obj.price_prepayment,
                SecondPrice = (obj.condition.Equals("فروش")) ? obj.price_per_meter : obj.price_mortage,
            };
            JavaScriptSerializer js = new JavaScriptSerializer();
            return js.Serialize(data);
        }

        [WebMethod(Description = "Search amonge Addvertises!")]
        [ScriptMethod(ResponseFormat = ResponseFormat.Json)]
        public string SearchAddvertise(int _condition, int _district,
                                       long min_total_price, long max_total_price,
                                       long min_price_per_meter, long max_price_per_meter,
                                       long min_price_prepayment, long max_price_prepayment,
                                       long min_price_mortgage, long max_price_mortgage,
                                       int min_area, int max_area)
        {
            string[] districts = new string[] {"", "17شهریور", "17شهریور-پل محلاتی", "آذربایجان",
                                                "آذری", "آزادی", "آهنگ", "آیت الله سعیدی", "اراج و ازگل", "استادمعین",
                                                "اشرفی اصفهانی", "افسریه", "اقدسیه", "الهیه", "امام حسین", "امام خمینی",
                                                "امامت", "امیرآباد", "امیرآباد شمالی", "اندیشه", "بازار", "بریانک", "بلوار بسیج",
                                                "بلوار فردوس","بنی هاشم",  "بهار", "بهار شیراز", "بهارستان", "پاتریس", "پارک وی",
                                                "پاسداران", "پردیس", "پونک", "پیچ شمران", "پیروزی", "تلفنخانه", "تهران نو", "تهران ویلا",
                                                "تهرانپارس", "تهرانسر", "تولیددارو", "جاده ساوه", "جردن", "جلال ال احمد", "جمالزاده", "جمهوری",
                                                "جنت آباد", "جوادیه", "جی", "جیحون", "چهارراه سیروس", "حافظ", "حکیمیه", "خراسان", "خردمند", "خزانه",
                                                "خلیج فارس", "خواجه عبدالله", "خواجه نظام", "خوش", "دامپزشکی", "دروس", "دریان نو", "دوراهی قپان",
                                                "رسالت", "رودکی", "ری", "زعفرانیه", "سبلان", "ستارخان", "سراج", "سعادت آباد", "سی متری", "شاد آباد",
                                                "شریعتی", "شمس آباد", "شهدا", "شهرآرا", "شهران", "شهرزیبا", "شهرک امید", "شهرک اکباتان",
                                                "شهرک راه آهن", "شهرک صنعتی سپهر", "شهرک غرب", "شهرک ولی عصر", "شوش", "شیخ بهایی", "صادقیه",
                                                "طرشت", "ظفر", "عباس آباد", "علی آباد", "فرشته", "فرمانیه", "فلکه اول صادقیه", "قزوین", "قلهک",
                                                "قیطریه", "کاشانی", "کامرانیه", "کریم خان", "کیان شهر", "گاندی", "گیانی", "گیشا", "لویزان",
                                                "مجاهدین اسلام", "مجیدیه", "مختاری", "مرزداران", "مشیریه", "ملاصدرا", "مطهری", "مهراباد", "مولوی",
                                                "میدان 100", "میدان سپاه", "میدان میرداماد", "مینی سیتی", "نازی آباد", "نامجو", "نبرد",
                                                "نواب", "نیاوران", "نیرو هوایی", "وحدت اسلامی", "ولنجک", "ولی عصر", "هاشم آباد", "هاشمی", "هلال احمر", 
                                                "همت", "یوسف آباد", "هفت تیر"};

            string[] conditions = new string[] { "فروش", "رهن", "اجاره" };
            AdverSearchParams _params = new AdverSearchParams();
            _params.adverCondition = conditions[_condition];
            _params.adverDistrict = districts[_district];
            _params.minTotalPrice = min_total_price;
            _params.maxTotalPrice = max_total_price;
            _params.minPricePerMeter = min_price_per_meter;
            _params.maxPricePerMeter = max_price_per_meter;
            _params.minPricePrepayment = min_price_prepayment;
            _params.maxPricePrepayment = max_price_prepayment;
            _params.minPriceMortage = min_price_mortgage;
            _params.maxPriceMortage = max_price_mortgage;
            _params.minArea = min_area;
            _params.maxArea = max_area;
            AdvertiseManager _adverManager = new AdvertiseManager();
            List<FreeAdvertise> advers = _adverManager.AdverSearch(_params, 0);
            var data = advers.Select(A => new
            {
                Title = A.title,
                Condition = A.condition,
                FirstPrice = (A.condition.Equals("فروش")) ? A.price_total : A.price_prepayment,
                SecondPrice = (A.condition.Equals("فروش")) ? A.price_per_meter : A.price_mortage,
                Address = A.district,
                Area = A.area,
                ID = A.ID,
                ImageUrl = (string.IsNullOrEmpty(A.image)) ? ("no-thumb.png") : (A.image.Split(';')[0]),
                Date = getAdverDate(A.create_date)
            }).ToList();

            JavaScriptSerializer js = new JavaScriptSerializer();
            return js.Serialize(data);
        }

        private string getAdverDate(DateTime? date)
        {
            string final_date = "";
            int difference = (int)(DateTime.Now - date.Value).TotalDays;
            if (difference == 0)
            {
                final_date = "امروز";
            }
            else if (difference == 1)
            {
                final_date = "دیروز";
            }
            else if (difference == 2)
            {
                final_date = "دو روز پیش";
            }
            else if (difference == 3)
            {
                final_date = "سه روز پیش";
            }
            else if (difference == 4)
            {
                final_date = "چهار روز پیش";
            }
            else if (difference >= 5 && difference <= 7)
            {
                final_date = "هفته گذشته";
            }
            else if (difference > 7 && difference <= 14)
            {
                final_date = "دو هفته پیش";
            }
            else
            {
                final_date = "بیش از دو هفته پیش";
            }
            return final_date;
        }


        #region ManagementSection
        [WebMethod(Description = "Management Commands!")]
        [ScriptMethod(ResponseFormat = ResponseFormat.Json)]
        public void ManagementCommands(string email, string passkey, string command, int id = 0)
        {
            JavaScriptSerializer js = new JavaScriptSerializer();
            Dictionary<string, int[]> dict = new Dictionary<string, int[]>();
            dict.Add("get_users", new int[] { 1 });
            dict.Add("del_user", new int[] { 1 });
            dict.Add("get_sales", new int[] { 1,2,3 });
            dict.Add("get_rents", new int[] { 1, 2, 4 });
            dict.Add("get_estates", new int[] { 1, 2 });
            dict.Add("review_estate", new int[] { 1 });
            dict.Add("approve_estate", new int[] { 1, 2, 3 });
            dict.Add("del_estate", new int[] { 1, 2, 3 });
            dict.Add("see_users", new int[] { 1 });
            UserManager _userManaget = new UserManager();
            int uid = _userManaget.authenticateUser(email, passkey);
            if (uid < 0)
                Context.Response.Write("Authentication Failed");
            else
            {
                UserAccouunt u = _userManaget.getUserByID(uid);
                int RoleId = u.RoleID.Value;
            }

            //Context.Response.Write();
        }

        private object getEstateByID(int id)
        {
            EstateManager _estateManager = new EstateManager();
            var estate = _estateManager.getStateByID(id);
            AgentManager _agentManager = new AgentManager();
            var agent = _agentManager.getAgentById(estate.AgentID.Value);
            var data = new
            {
                ID = estate.ID,
                Area = estate.Area,
                FirstPrice = (estate.TotalPrice == null || estate.TotalPrice == 0) ? estate.PrepaymentPrice : estate.TotalPrice,
                SecondPrice = (estate.PricePerMeter == null || estate.PricePerMeter == 0) ? estate.MortgagePrice : estate.PricePerMeter,
                Date = estate.Date.ToString().Split(' ')[0],
                Condition = estate.Condition,
                Dist = estate.District1.name,
                Serial = estate.Serial.ToString(),
                Address1 = estate.Address,
                Address2 = estate.DetailedAddress,
                Usage = estate.Usage,
                Bedrooms = estate.Bedrooms,
                Type = estate.StateType1.types,
                Age = estate.Age.ToString(),
                Floor = estate.Floor,
                AgentName = agent.Name,
                AgentPhone = agent.Phone,
                TotalFloors = estate.Floors,
                Bathroom = estate.Bathrooms,
                Units = estate.Units,
                Cabinet = estate.Cabinet,
                Parking = estate.Parking,
                PhoneLines = estate.Tells,
                Facing = estate.Facing,
                Status = estate.CurrentStatus,
                Position = estate.StatePosition,
                Flooring = estate.Floors,
                ExtraInfo = estate.Description + " " + estate.Occasion,
                ImgCount = estate.Images.Count,
                ImgUrls = estate.Images.Count > 0 ? (estate.Images.Select(i => i.url).ToArray()) : null,
                StreetViewImg = estate.StreetViews.Count > 0 ? estate.StreetViews.First().url : null,
                PlanImg = estate.Plans.Count > 0 ? estate.Plans.First().url : null,
                GoogleMapLat = estate.GoogleMaps.Count <= 0 ? "35.688045" : estate.GoogleMaps.First().latitude,
                GoogleMapLong = estate.GoogleMaps.Count <= 0 ? "51.392884" : estate.GoogleMaps.First().longitude,
                Features = estate.Features.Select(f => f.Item).ToArray()
            };
            return data;
        }
        #endregion

        [WebMethod(Description = "Authenticate user")]
        [ScriptMethod(ResponseFormat = ResponseFormat.Json)]
        public string Login(string email, string password)
        {
            UserManager _userManaget = new UserManager();
            int id = _userManaget.authenticateUser(email, password);
            if (id < 0)
            {
                return "Authentication Failed";
            }
            else
            {
                UserAccouunt u = _userManaget.getUserByID(id);
                var data = new
                {
                    Name = u.Name,
                    Email = u.Email,
                    RoleId = u.RoleID
                };
                JavaScriptSerializer js = new JavaScriptSerializer();
                return js.Serialize(data);
            }
        }

       /* [WebMethod]
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
         * */
    }
}
