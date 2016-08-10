using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Amirhome.Models;
using System.Drawing;
using System.Net.Mail;

namespace Amirhome.Controllers
{
    public class EstateController : Controller
    {
        UserManager _userManager = new UserManager();
        EstateManager _estateManager = new EstateManager();
        AgentManager _agentManager = new AgentManager();

        protected override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            base.OnActionExecuting(filterContext);
            if (Session["UserID"] != null)
            {
                if (Session["user_name"] == null)
                {
                    try
                    {
                        int uid = int.Parse(Session["UserID"].ToString());
                        UserAccouunt user = _userManager.getUserByID(uid);
                        string imageBase64 = Convert.ToBase64String(user.ProfileImage);
                        string imageSrc = string.Format("data:image/jpg;base64,{0}", imageBase64);
                        Session["user_name"] = user.Name;
                        Session["user_image_src"] = imageSrc;
                        Session["user_role_id"] = user.UserAccouuntsRole.ID;
                        Session["user_role_access"] = user.UserAccouuntsRole.Username;
                    }
                    catch
                    {
                        Session["user_name"] = null;
                        Session["user_image_src"] = null;
                    }
                }
                ViewData["user_name"] = Session["user_name"];
                ViewData["user_image_src"] = Session["user_image_src"];
                ViewData["user_role_id"] = Session["user_role_id"];
            }
        }

        public ActionResult ShowOccasions()
        {
            var model = _estateManager.getOccasions();
            return View(model);
        }

        public ActionResult EstateDetails()
        {
            int ID = int.Parse(Request.QueryString["EstateID"].ToString());
            try
            {
                State estate_model = _estateManager.getStateByID(ID);
                if (Session["serach_ids_array"] != null)
                {
                    int[] ids = Session["serach_ids_array"] as int[];
                    if (ids.Contains(ID))
                    {
                        int index = Array.IndexOf(ids, ID);
                        if (index > 0)
                            ViewData["prev_estate"] = ids[index - 1];
                        if (index < ids.Length - 1)
                            ViewData["next_estate"] = ids[index + 1];
                    }
                }
                if (estate_model.GoogleMaps.Count > 0)
                {
                    ViewData["latitude"] = string.IsNullOrEmpty(estate_model.GoogleMaps.First().latitude) ? "35.688045" : estate_model.GoogleMaps.First().latitude;
                    ViewData["longitude"] = string.IsNullOrEmpty(estate_model.GoogleMaps.First().longitude) ? "51.392884" : estate_model.GoogleMaps.First().longitude;
                }
                else
                {
                    ViewData["latitude"] = "35.688045";
                    ViewData["longitude"] = "51.392884";
                }
                ViewData["agent"] = _agentManager.getAgentById(estate_model.AgentID.Value);
                return View(estate_model);
            }
            catch
            {
                return View();
            }
        }
        public ActionResult SearchPage()
        {
            return View();
        }

        [HttpPost]
        public JsonResult DoSearch(SearchParams _params, int page, string order = "date")
        {
            if (_params == null)
                _params = Session["search_params"] as SearchParams;
            else
                Session["search_params"] = _params;
            if (page == -1)
                page = int.Parse(Session["search_page"].ToString());
            else
                Session["search_page"] = page;
            List<State> states = _estateManager.doSearch(_params, page * 10, order);
            Session["serach_ids_array"] = states.Select(s => s.ID).ToArray();
            var data = states.Select(o => new
            {
                ID = o.ID,
                Area = o.Area,
                FirstPrice = (o.TotalPrice == null || o.TotalPrice == 0) ? o.PrepaymentPrice : o.TotalPrice,
                SecondPrice = (o.PricePerMeter == null || o.PricePerMeter == 0) ? "اجاره " + o.MortgagePrice.ToString() : "متری " + o.PricePerMeter.ToString(),
                Date = o.Date.ToString().Split(' ')[0],
                //Prepayment = o.PrepaymentPrice,
                //Loan = o.Loan,
                //Mortage = o.MortgagePrice,
                Address = o.Address,
                Floor = o.Floor,
                Age = o.Age,
                Condition = o.Condition,
                Dist = o.District1.name,
                Bedrooms = o.Bedrooms,
                Serial = o.Serial,
                Units = o.Units,
                ImageSrc = (o.Images.Count(i => i.Primary == true) == 0 ? "no-thumb.png" : o.Images.FirstOrDefault(i => i.Primary == true).url),
                ImageCount = o.Images.Count
            }).ToList();

            if (data.Count() % 10 == 0)
            {
                data = data.Skip(Math.Max(0, data.Count() - 10)).ToList();
            }
            else
            {
                data = data.Skip(Math.Max(0, data.Count() - (data.Count() % 10))).ToList();
            }

            return Json(data);
        }

        public ActionResult SubmitEstate()
        {
            ViewData["Features"] = _estateManager.getAllFeatures();
            ViewData["Province"] = _estateManager.getAllProvince();
            return View();
        }
        [HttpPost]
        public ActionResult SubmitEstate(State model, HttpPostedFileBase[] main_image, HttpPostedFileBase[] plan_image, HttpPostedFileBase[] street_image, int[] features, string map_latitude, string map_longitude)
        {
            try
            {
                model.Approved = false;
                model.Archived = false;
                model.Date = DateTime.Now;
                model.Serial = _estateManager.generateSerial();
                if (model.Parking == null || model.Parking == "0")
                    model.Parking = "ندارد";
                if (model.Tells == null || model.Tells == "0")
                    model.Parking = "ندارد";
                if (model.Bathrooms == null)
                    model.Bathrooms = "1";
                model.AgentID = 1;

                model.GoogleMaps.Add(new GoogleMap() { latitude = map_latitude, longitude = map_longitude });

                string filePath = "~/Content/estate_images/",
                       tempFilePath = "~/Content/temp_img_folder/";

                #region initializeImages
                List<Tuple<byte[], string>> posted_files_img = new List<Tuple<byte[], string>>();
                int index = 0;
                foreach (HttpPostedFileBase item in main_image)
                {
                    if (item == null)
                        break;
                    Models.Image img = new Models.Image();
                    img.Primary = (index == 0) ? true : false;
                    string file_name = "estate_" + model.Serial.ToString() + "_" + generetareIndexForImage().ToString() + ".jpg";
                    img.url = file_name;

                    int nFileLen = item.ContentLength;
                    byte[] myData = new Byte[nFileLen];
                    item.InputStream.Read(myData, 0, nFileLen);
                    item.InputStream.Dispose();
                    posted_files_img.Add(new Tuple<byte[], string>(myData, file_name));

                    model.Images.Add(img);
                    index++;
                }
                index = 0;
                foreach (HttpPostedFileBase item in plan_image)
                {
                    if (item == null)
                        break;
                    Models.Plan plan = new Models.Plan();
                    string file_name = "estate_plan_" + model.Serial.ToString() + "_" + generetareIndexForImage().ToString() + ".jpg";
                    plan.url = file_name;

                    int nFileLen = item.ContentLength;
                    byte[] myData = new Byte[nFileLen];
                    item.InputStream.Read(myData, 0, nFileLen);
                    item.InputStream.Dispose();
                    posted_files_img.Add(new Tuple<byte[], string>(myData, file_name));

                    model.Plans.Add(plan);
                    index++;
                }
                index = 0;
                foreach (HttpPostedFileBase item in street_image)
                {
                    if (item == null)
                        break;
                    Models.StreetView street = new Models.StreetView();
                    string file_name = "estate_street_" + model.Serial.ToString() + "_" + generetareIndexForImage().ToString() + ".jpg";
                    street.url = file_name;

                    int nFileLen = item.ContentLength;
                    byte[] myData = new Byte[nFileLen];
                    item.InputStream.Read(myData, 0, nFileLen);
                    item.InputStream.Dispose();
                    posted_files_img.Add(new Tuple<byte[], string>(myData, file_name));

                    model.StreetViews.Add(street);
                    index++;
                }
                #endregion

                bool insert_success = _estateManager.addNewEstate(model, features);
                if (insert_success)
                {
                    foreach (var item in posted_files_img)
                    {
                        System.IO.FileStream newFile
                                    = new System.IO.FileStream(Server.MapPath(tempFilePath + "_temp.jpg"),
                                                               System.IO.FileMode.Create);
                        newFile.Write(item.Item1, 0, item.Item1.Length);
                        bool success = ResizeImageAndUpload(newFile, filePath + (item.Item2), 500, 700);//Save image your normal image path
                        //delete the temp file.
                        newFile.Close();
                        System.IO.File.Delete(Server.MapPath(tempFilePath + "_temp.jpg"));
                    }
                }
                else
                {
                    ViewBag.ErrMsg = "خطایی در حین ثبت ملک رخ داده است. لطفا مجددا تلاش نمایید";
                    ViewData["Features"] = _estateManager.getAllFeatures();
                    ViewData["Province"] = _estateManager.getAllProvince();
                    return View(model);
                }
                ViewBag.SucsMsg = "ملک شما با موفقیت ثبت گردید";
                return RedirectToAction("SubmitEstateSuccessfull", "Estate", new { serial = model.Serial });
            }
            catch
            {
                ViewBag.ErrMsg = "خطایی در حین ثبت ملک رخ داده است. لطفا مجددا تلاش نمایید";
                ViewData["Features"] = _estateManager.getAllFeatures();
                ViewData["Province"] = _estateManager.getAllProvince();
                return View(model);
            }
        }

        public ActionResult SubmitEstateSuccessfull(string serial)
        {
            ViewData["serial"] = serial;
            return View();
        }

        private bool ResizeImageAndUpload(System.IO.FileStream newFile, string folderPathAndFilename, double maxHeight, double maxWidth)
        {
            try
            {
                // Declare variable for the conversion
                float ratio;
                // Create variable to hold the image 
                System.Drawing.Image thisImage = System.Drawing.Image.FromStream(newFile);
                System.Drawing.Image waterMark = System.Drawing.Image.FromFile(System.Web.Hosting.HostingEnvironment.MapPath("~/Content/shared_images/logo2.png"));
                // Get height and width of current image
                int width = (int)thisImage.Width;
                int height = (int)thisImage.Height;
                // Ratio and conversion for new size
                if (width > maxWidth)
                {
                    ratio = (float)width / (float)maxWidth;
                    width = (int)(width / ratio);
                    height = (int)(height / ratio);
                }
                // Ratio and conversion for new size
                if (height > maxHeight)
                {
                    ratio = (float)height / (float)maxHeight;
                    height = (int)(height / ratio);
                    width = (int)(width / ratio);
                }
                // Create "blank" image for drawing new image
                Bitmap outImage = new Bitmap(width, height);
                Graphics outGraphics = Graphics.FromImage(outImage);
                SolidBrush sb = new SolidBrush(System.Drawing.Color.White);
                // Fill "blank" with new sized image
                outGraphics.FillRectangle(sb, 0, 0, outImage.Width, outImage.Height);
                outGraphics.DrawImage(thisImage, 0, 0, outImage.Width, outImage.Height);
                outGraphics.DrawImage(waterMark, (outImage.Width - waterMark.Width) / 2, (outImage.Height - waterMark.Height) / 2, waterMark.Width, waterMark.Height);
                sb.Dispose();
                outGraphics.Dispose();
                thisImage.Dispose();
                // Save new image as jpg
                outImage.Save(Server.MapPath(folderPathAndFilename), System.Drawing.Imaging.ImageFormat.Jpeg);
                outImage.Dispose();
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        [HttpPost]
        public JsonResult fetchCities(int p_id)
        {
            List<tbl_cities> cities = _estateManager.getCityByProvince(p_id);
            var data = cities.Select(c => new
            {
                id = c.id,
                name = c.name
            });
            return Json(data);
        }
        [HttpPost]
        public JsonResult fetchDistricts()
        {
            List<District> dists = _estateManager.getAllDistricts();
            var data = dists.Select(c => new
            {
                id = c.ID,
                name = c.name
            });
            return Json(data);
        }
        public ActionResult ManagementPanel(string message = null)
        {
            if (Session["user_role_id"] == null)
                return RedirectToAction("Index", "Home");
            if (!string.IsNullOrEmpty(message))
            {
                ViewBag.Message = message;
            }
            return View();
        }
        [HttpPost]
        public ActionResult ManagementCommand(string command, int page = 1, string serial = "")
        {
            if (Session["user_role_id"].ToString().Equals("5") || Session["user_role_id"].ToString().Equals("6"))
                return Json("Stop Trying Please!");
            List<State> all_estates = _estateManager.getAllStates();
            ViewData["command"] = command;
            switch (command)
            {
                case "mNg_dshbd":
                    {
                        if (Session["user_role_id"].ToString() != "1")
                            break;
                        int uid = int.Parse(Session["UserID"].ToString());
                        var usr = _userManager.getUserByID(uid);
                        var last_online_date = usr.LastTimeOnline.Value;
                        DashboardViewModel model = new DashboardViewModel();
                        model.newUserCount = _userManager.getNumOfUsersRegisteredAfter(last_online_date);
                        model.newEstateCount = _estateManager.getNumOfEstateSubmitedAfter(last_online_date);
                        model.totalFeedbacks = _estateManager.getAllFedbacks();
                        model.totalUsers = _userManager.getAllUser();
                        model.totalAgents = _agentManager.getAllAgent();
                        ViewData["title"] = "داشبورد مدیریت";
                        return PartialView("_DashboardPartialView", model);
                    }
                case "saLe_est":
                    {
                        ViewData["title"] = "املاک فروش";
                        if (!string.IsNullOrEmpty(serial))
                            all_estates = all_estates.Where(E => E.Condition == "فروش" && E.Archived.Value == false && E.Serial.ToString().Equals(serial)).ToList();
                        else
                            all_estates = all_estates.Where(E => E.Condition == "فروش" && E.Archived.Value == false).ToList();
                        return PartialView("_ManagementPartialView", all_estates.Take(Math.Min(all_estates.Count, page * 10)).ToList());
                    }
                case "mRtG_est":
                    {
                        ViewData["title"] = "املاک رهن و اجاره";
                        if (!string.IsNullOrEmpty(serial))
                            all_estates = all_estates.Where(E => E.Condition != "فروش" && E.Archived.Value == false && E.Serial.ToString().Equals(serial)).ToList();
                        else
                            all_estates = all_estates.Where(E => E.Condition != "فروش" && E.Archived.Value == false).ToList();
                        return PartialView("_ManagementPartialView", all_estates.Take(Math.Min(all_estates.Count, page * 10)).ToList());
                    }
                case "all_The_est":
                    {
                        ViewData["title"] = "همه املاک";
                        if (!string.IsNullOrEmpty(serial))
                            all_estates = all_estates.Where(E => E.Archived.Value == false && E.Serial.ToString().Equals(serial)).ToList();
                        else
                            all_estates = all_estates.Where(E => E.Archived.Value == false).ToList();
                        return PartialView("_ManagementPartialView", all_estates.Take(Math.Min(all_estates.Count, page * 10)).ToList());
                    }
                case "arsH_est":
                    {
                        ViewData["title"] = "املاک آرشیو شده";
                        if (!string.IsNullOrEmpty(serial))
                            all_estates = all_estates.Where(E => E.Archived == true && E.Serial.ToString().Equals(serial)).ToList();
                        else
                            all_estates = all_estates.Where(E => E.Archived == true).ToList();
                        return PartialView("_ManagementPartialView", all_estates.Take(Math.Min(all_estates.Count, page * 10)).ToList());
                    }
                case "seE_AdverT":
                    {
                        break;
                    }
                default:
                    {
                        break;
                    }
            }
            return Json("Invalid Command");
        }
        public JsonResult EstateApprovement(int id, bool flag)
        {
            if (Session["user_role_id"].ToString().Equals("5") || Session["user_role_id"].ToString().Equals("6"))
                return Json("Don't you have a job??");
            bool res = _estateManager.approveEstate(id, flag);
            if (res)
                return Json("Success", JsonRequestBehavior.AllowGet);
            else
                return Json("Error", JsonRequestBehavior.AllowGet);
        }
        public JsonResult EstateArchive(int id, bool flag)
        {
            if (Session["user_role_id"].ToString().Equals("5") || Session["user_role_id"].ToString().Equals("6"))
                return Json("Don't you have a job??");
            bool res = _estateManager.archiveEstate(id, flag);
            if (res)
                return Json("Success", JsonRequestBehavior.AllowGet);
            else
                return Json("Error", JsonRequestBehavior.AllowGet);
        }
        [HttpPost]
        public ActionResult EditEstate(int id)
        {
            if (Session["user_role_id"].ToString().Equals("5") || Session["user_role_id"].ToString().Equals("6"))
                return Json("We are not stupid you know!!");
            State _estate = _estateManager.getStateByID(id);
            ViewData["Features"] = _estateManager.getAllFeatures();
            ViewBag.Province = new SelectList(_estateManager.getAllProvince(), "id", "name", _estate.Province);
            ViewBag.City = new SelectList(_estateManager.getCityByProvince((int)_estate.Province), "id", "name", _estate.City);
            ViewBag.AgentID = new SelectList(_agentManager.getAllAgent(), "UserID", "DisplayName", _estate.AgentID);
            #region InitializeSelectLists
            ViewBag.District = new SelectList(_estateManager.getAllDistricts(), "ID", "name", _estate.District);
            List<object> StateTypeList = new List<object>()
            {
                new { Text = "آپارتمان", Value = "1" },
                new { Text = "ویلا", Value = "2" },
                new { Text = "سوئیت", Value = "8" },
                new { Text = "سایر", Value = "9" },

            };
            ViewBag.StateType = new SelectList(StateTypeList,"Value", "Text",  _estate.StateType);

            List<object> StatePositionList = new List<object>()
            {
                new { Text = "شمالی", Value = "شمالی" },
                new { Text = "جنوبی", Value = "جنوبی" },
                new { Text = "شرقی", Value = "شرقی" },
                new { Text = "غربی", Value = "غربی" },
				new { Text = "شمالی شرقی", Value = "شمالی شرقی" },
				new { Text = "شمالی غربی", Value = "شمالی غربی" },
				new { Text = "جنوبی شرقی", Value = "جنوبی شرقی" },
				new { Text = "جنوبی غربی", Value = "جنوبی غربی" },
				new { Text = "دونبش", Value = "دونبش" },
				new { Text = "دوبر", Value = "دوبر" },
				new { Text = "سایر", Value = "سایر" },

            };
            ViewBag.StatePosition = new SelectList(StatePositionList, "Value", "Text", _estate.StatePosition);

            List<object> UsageList = new List<object>()
            {
                new { Text = "مسکونی", Value = "مسکونی" },
                new { Text = "کلنگی و مشارکتی", Value = "کلنگی و مشارکتی" },
                new { Text = "تجاری و اداری", Value = "تجاری و اداری" },
                new { Text = "سایر", Value = "سایر" },


            };
            ViewBag.Usage = new SelectList(UsageList, "Value", "Text", _estate.Usage);

            List<object> ConditionList = new List<object>()
            {
                new { Text = "فروش", Value = "فروش" },
                new { Text = "رهن", Value = "رهن" },
                new { Text = "اجاره", Value = "اجاره" },
            };
            ViewBag.Condition = new SelectList(ConditionList, "Value", "Text", _estate.Condition);

            List<object> FacingList = new List<object>()
            {
                new { Text = "کنیتکس", Value = "کنیتکس" },
                new { Text = "سفال", Value = "سفال" },
                new { Text = "سنگ", Value = "سنگ" },
				new { Text = "رومی", Value = "رومی" },
				new { Text = "تراورتن", Value = "تراورتن" },
				new { Text = "سنگ و سفال", Value = "سنگ و سفال" },
				new { Text = "رومی و سفال", Value = "رومی و سفال" },
				new { Text = "سرامیک", Value = "سرامیک" },
				new { Text = "سفال سرامیک", Value = "سفال سرامیک" },
				new { Text = "گرانیت", Value = "گرانیت" },
				new { Text = "رومی گرانیت", Value = "رومی گرانیت" },
				new { Text = "آجرگری", Value = "آجرگری" },
				new { Text = "سیمان سفید", Value = "سیمان سفید" },
				new { Text = "آجر و سنگ", Value = "آجر و سنگ" },
				new { Text = "آلومینیوم", Value = "آلومینیوم" },
				new { Text = "ترکیبی", Value = "ترکیبی" },
				new { Text = "چوب و سنگ", Value = "چوب و سنگ" },
				new { Text = "آجر سه سانت", Value = "آجر سه سانت" },
				new { Text = "کامپوزیت", Value = "کامپوزیت" },
				new { Text = "شیشه", Value = "شیشه" },
				new { Text = "شیشه و سنگ", Value = "شیشه و سنگ" },
            };
            ViewBag.Facing = new SelectList(FacingList, "Value", "Text", _estate.Facing);

            List<object> CabinetList = new List<object>()
            {
                new { Text = "فلزی", Value = "فلزی" },
				new { Text = "چوبی", Value = "چوبی" },
				new { Text = "چوب و فلز", Value = "چوب و فلز" },
				new { Text = "های گلس", Value = "های گلس" },
				new { Text = "MDF", Value = "MDF" },
				new { Text = "سایر", Value = "سایر" },
            };
            ViewBag.Cabinet = new SelectList(CabinetList, "Value", "Text", _estate.Cabinet);

            List<object> CurrentStatusList = new List<object>()
            {
                new { Text = "تخلیه", Value = "تخلیه" },
				new { Text = "مسکونی مالک", Value = "مسکونی مالک" },
				new { Text = "اجاره", Value = "اجاره" },
            };
            ViewBag.CurrentStatus = new SelectList(CurrentStatusList, "Value", "Text", _estate.CurrentStatus);

            List<object> FloorList = new List<object>()
            {
                new { Text = "زیرزمین", Value = "زیرزمین" },
				new { Text = "پیلوت", Value = "پیلوت" },
				new { Text = "همکف", Value = "همکف" },
				new { Text = "طبقه اول", Value = "طبقه اول" },
				new { Text = "طبقه دوم", Value = "طبقه دوم" },
				new { Text = "طبقه سوم", Value = "طبقه سوم" },
				new { Text = "طبقه چهارم", Value = "طبقه چهارم" },
				new { Text = "طبقه پنجم", Value = "طبقه پنجم" },
				new { Text = "طبقه ششم", Value = "طبقه ششم" },
				new { Text = "طبقه هفتم تا دهم", Value = "طبقه هفتم تا دهم" },
				new { Text = "طبقه دهم به بالا", Value = "طبقه دهم به بالا" },
            };
            ViewBag.Floor = new SelectList(FloorList, "Value", "Text", _estate.Floor);

            List<object> FloorsList = new List<object>()
            {
                new { Text = "یک طبقه", Value = "یک طبقه" },
				new { Text = "دو طبقه", Value = "دو طبقه" },
				new { Text = "سه طبقه", Value = "سه طبقه" },
				new { Text = "چهار طبقه", Value = "چهار طبقه" },
				new { Text = "پنج طبقه", Value = "پنج طبقه" },
				new { Text = "شش طبقه", Value = "شش طبقه" },
				new { Text = "هفت تا ده طبقه", Value = "هفت تا ده طبقه" },
				new { Text = "ده تا بیست طبقه", Value = "ده تا بیست طبقه" },
				new { Text = "بیش از بیست طبقه", Value = "بیش از بیست طبقه" },
				
            };
            ViewBag.Floors = new SelectList(FloorsList, "Value", "Text", _estate.Floors);

            List<object> UnitsList = new List<object>()
            {
                new { Text = "تک واحدی", Value = "تک واحدی" },
				new { Text = "دو واحدی", Value = "دو واحدی" },
				new { Text = "سه واحدی", Value = "سه واحدی" },
				new { Text = "چهار واحدی", Value = "چهار واحدی" },
				new { Text = "بیش از چهار واحد در هر طبقه", Value = "بیش از چهار واحد در هر طبقه" },
				new { Text = "شش طبقه", Value = "شش طبقه" },
            };
            ViewBag.Units = new SelectList(UnitsList, "Value", "Text", _estate.Units);

            #endregion
            return PartialView("_EditEstatePartialView", _estate);
        }
        private int generetareIndexForImage()
        {
            Random rand = new Random();
            int res = rand.Next(1000, 10000);
            return res;
        }
        [HttpPost]
        public ActionResult SubmitEditEstate(State model, int[] img_ids, HttpPostedFileBase[] added_image)
        {
            if (Session["user_role_id"] == null)
                return Json("هویت شما مورد تأیید نیست");
            List<string> urls_to_delete;
            List<Amirhome.Models.Image> images_to_create = new List<Models.Image>();

            //Add objects of new uploaded images to estate model
            List<Tuple<byte[], string>> posted_files_img = new List<Tuple<byte[], string>>();
            int index = _estateManager.getImageCountOfEstate(model.ID);
            foreach (HttpPostedFileBase item in added_image)
            {
                if (item == null)
                    break;
                //Create Image object and add it to model
                Models.Image img = new Models.Image();
                img.Primary = (index == 0) ? true : false;
                string file_name = "estate_" + model.Serial.ToString() + "_" + generetareIndexForImage().ToString() + ".jpg";
                img.url = file_name;
                img.StateID = model.ID;

                //Create image InputStream for saving it a little after
                int nFileLen = item.ContentLength;
                byte[] myData = new Byte[nFileLen];
                item.InputStream.Read(myData, 0, nFileLen);
                item.InputStream.Dispose();
                posted_files_img.Add(new Tuple<byte[], string>(myData, file_name));

                images_to_create.Add(img);
                index++;
            }

            //Update the model and get the result
            bool success = _estateManager.updateEstate(model, img_ids, images_to_create, out urls_to_delete);
            if (success)
            {
                //Delete the actual image file from server
                foreach (var url in urls_to_delete)
                    System.IO.File.Delete(Server.MapPath("~/Content/estate_images/" + url));

                //Save new uploaded files to server
                string filePath = "~/Content/estate_images/",
                       tempFilePath = "~/Content/temp_img_folder/";
                foreach (var item in posted_files_img)
                {
                    System.IO.FileStream newFile
                                = new System.IO.FileStream(Server.MapPath(tempFilePath + "_temp.jpg"),
                                                           System.IO.FileMode.Create);
                    newFile.Write(item.Item1, 0, item.Item1.Length);
                    bool save_img_success = ResizeImageAndUpload(newFile, filePath + (item.Item2), 460, 700);//Save image your normal image path
                    //delete the temp file.
                    newFile.Close();
                    System.IO.File.Delete(Server.MapPath(tempFilePath + "_temp.jpg"));
                }

                return RedirectToAction("ManagementPanel", "Estate", new { message = "تغییرات با موفقیت ثبت گردید" });
            }
            else
                return RedirectToAction("ManagementPanel", "Estate", new { message = "خطا در ثبت تغییرات" });
        }

        [HttpPost]
        public PartialViewResult SearchOwner(string field, string value)
        {
            var estates = _estateManager.searchForOwner(field, value);
            List<OwnerSearchViewModel> model = new List<OwnerSearchViewModel>();
            foreach (var e in estates)
            {
                model.Add(new OwnerSearchViewModel()
                {
                    EstateID = e.ID,
                    EstateSerial = e.Serial,
                    OwnerAddress = e.Owner.Address,
                    OwnerMobile = e.Owner.Mobile,
                    OwnerName = e.Owner.Name,
                    OwnerPhone = e.Owner.Telephone
                });
            }
            return PartialView("SearchOwnerPartialView", model);
        }

        [HttpPost]
        public string SendMail(string reciever, string name, int id)
        {
            string sender = "email@amirhome.ir";
            string pass = "emailpass123";
            string mail_subject = "مسکن امیر - دعوت به مشاهده ملک";
            string mail_body = "از طرف " + name + "\n";
            mail_body += "سلام دوست عزیز! پیشنهاد میکنم اطلاعات این ملک را بر روی وبسایت 'مسکن امیر' مشاهده کنید. \n";
            mail_body += "http://www.amirhome.ir/Estate/EstateDetails?EstateID=" + id.ToString();
            var smtp = new SmtpClient()
            {
                Host = "amirhome.ir",
                Port = 25,
                DeliveryMethod = SmtpDeliveryMethod.Network,
                Credentials = new System.Net.NetworkCredential(sender, pass),
                Timeout = 20000,
            };
            try
            {
                smtp.Send(sender, reciever, mail_subject, mail_body);
                return "true";
            }
            catch
            {
                return "false";
            }
        }

        [HttpPost]
        public string SubmitFeedback(string name, string phone, string email, string body, int id)
        {
            Feedback fb = new Feedback()
            {
                Name = name,
                Tell = phone,
                Email = email,
                Text = body,
                StateID = id,
            };
            bool res = _estateManager.addFeedbackForEstate(fb);
            return res ? "true" : "false";
        }
    }
}