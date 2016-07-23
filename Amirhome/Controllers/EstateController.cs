﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Amirhome.Models;
using System.Drawing;

namespace Amirhome.Controllers
{
    public class EstateController : Controller
    {
        UserManager _userManager = new UserManager();
        EstateManager _estateManager = new EstateManager();

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

        public ActionResult EstateDetails()
        {
            int ID = int.Parse(Request.QueryString["EstateID"].ToString());
            try
            {
                State estate_model = _estateManager.getStateByID(ID);
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
            List<State> states = _estateManager.doSearch(_params, page * 10, order);
            var data = states.Select(o => new
            {
                ID = o.ID,
                Area = o.Area,
                TotalPrice = o.TotalPrice,
                PricePerMeter = o.PricePerMeter,
                Date = o.Date.ToString().Split(' ')[0],
                Prepayment = o.PrepaymentPrice,
                Loan = o.Loan,
                Mortage = o.MortgagePrice,
                Address = o.Address,
                Floor = o.Floor,
                Age = o.Age,
                Condition = o.Condition,
                Dist = o.District1.name,
                Bedrooms = o.Bedrooms,
                Serial = o.Serial,
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
        public ActionResult SubmitEstate(State model, HttpPostedFileBase[] main_image, HttpPostedFileBase[] plan_image, HttpPostedFileBase[] street_image, int[] features)
        {
            try
            {
                model.Approved = false;
                model.OccasionFlag = false;
                model.Archived = false;
                model.Date = DateTime.Now;
                model.Serial = _estateManager.generateSerial();
                if (model.Parking == null || model.Parking == "0")
                    model.Parking = "ندارد";
                if (model.Tells == null || model.Tells == "0")
                    model.Parking = "ندارد";
                if (model.Bathrooms == null)
                    model.Bathrooms = "1";

                List<Feature> _features = _estateManager.fetchFeaturesById(features);
                foreach (var item in _features)
                    model.Features.Add(item);

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
                    string file_name = "estate_" + model.Serial.ToString() + "_" + index.ToString() + ".jpg";
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
                    string file_name = "estate_plan_" + model.Serial.ToString() + "_" + index.ToString() + ".jpg";
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
                    string file_name = "estate_street_" + model.Serial.ToString() + "_" + index.ToString() + ".jpg";
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

                bool insert_success = _estateManager.addNewEstate(model);
                if (insert_success)
                {
                    foreach (var item in posted_files_img)
                    {
                        System.IO.FileStream newFile
                                    = new System.IO.FileStream(Server.MapPath(tempFilePath + "_temp.jpg"),
                                                               System.IO.FileMode.Create);
                        newFile.Write(item.Item1, 0, item.Item1.Length);
                        bool success = ResizeImageAndUpload(newFile, filePath + (item.Item2), 460, 700);//Save image your normal image path
                        //delete the temp file.
                        newFile.Close();
                        System.IO.File.Delete(Server.MapPath(tempFilePath + "_temp.jpg"));
                    }
                }
                else
                    throw new Exception("Error while Inserting new esteta");
                return RedirectToAction("Index", "Home");
            }
            catch
            {
                ViewBag.ErrMsg = "خطایی در حین ثبت ملک رخ داده است. لطفا مجددا تلاش نمایید";
                ViewData["Features"] = _estateManager.getAllFeatures();
                ViewData["Province"] = _estateManager.getAllProvince();
                return View(model);
            }
        }

        private bool ResizeImageAndUpload(System.IO.FileStream newFile, string folderPathAndFilename, double maxHeight, double maxWidth)
        {
            try
            {
                // Declare variable for the conversion
                float ratio;
                // Create variable to hold the image
                System.Drawing.Image thisImage = System.Drawing.Image.FromStream(newFile);
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
        public ActionResult ManagementPanel()
        {
            if (Session["user_role_id"] == null)
                return RedirectToAction("Index", "Home");
            return View();
        }
        [HttpPost]
        public ActionResult ManagementCommand(string command)
        {
            object return_obj = null;
            List<State> all_estates = _estateManager.getAllStates();
            switch (command)
            {
                case "mNg_dshbd":
                    {
                        ViewData["title"] = "داشبور مدیریت";
                        break;
                    }
                case "saLe_est":
                    {
                        ViewData["title"] = "املاک فروش";
                        all_estates = all_estates.Where(E => E.Condition == "فروش").ToList();
                        return PartialView("_ManagementPartialView", all_estates);
                    }
                case "mRtG_est":
                    {
                        ViewData["title"] = "املاک رهن و اجاره";
                        all_estates = all_estates.Where(E => E.Condition != "فروش").ToList();
                        return PartialView("_ManagementPartialView", all_estates);
                    }
                case "all_The_est":
                    {
                        ViewData["title"] = "همه املاک";
                        all_estates = all_estates.ToList();
                        return PartialView("_ManagementPartialView", all_estates);
                    }
                case "arsH_est":
                    {
                        ViewData["title"] = "املاک آرشیو شده";
                        all_estates = all_estates.Where(E => E.Archived == true).ToList();
                        return PartialView("_ManagementPartialView", all_estates);
                    }
                case "seE_USers":
                    {
                        break;
                    }
                default:
                    {
                        break;
                    }
            }
            return Json(return_obj);
        }
        public JsonResult EstateApprovement(int id, bool flag)
        {
            bool res = _estateManager.approveEstate(id, flag);
            if (res)
                return Json("Success");
            else
                return Json("Error");
        }
        public JsonResult EstateArchive(int id, bool flag)
        {
            bool res = _estateManager.archiveEstate(id, flag);
            if (res)
                return Json("Success");
            else
                return Json("Error");
        }
        [HttpPost]
        public ActionResult EditEstate(int id)
        {
            State _estate = _estateManager.getStateByID(id);
            ViewData["Features"] = _estateManager.getAllFeatures();
            ViewBag.Province = new SelectList(_estateManager.getAllProvince(), "id", "name", _estate.Province);
            ViewBag.City = new SelectList(_estateManager.getCityByProvince((int)_estate.Province), "id", "name", _estate.City);
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

            #endregion
            return PartialView("_EditEstatePartialView", _estate);
        }
    }
}